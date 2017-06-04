using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Hourai.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hourai {

[Service]
public class AnnounceService {

  readonly ILogger _log;
  readonly IServiceProvider _services;

  public AnnounceService(DiscordShardedClient client,
                         ILoggerFactory loggerFactory,
                         IServiceProvider services) {
    //TODO(james7132): make these messages configurable
    const string JoinMsg = "$mention has joined the server.";
    const string LeaveMsg = "**$user** has left the server.";
    const string BanMsg = "**$user** has been banned.";
    _services = services;
    client.UserJoined += u => GuildMessage(c => c.JoinMessage, JoinMsg)(u, u.Guild);
    client.UserLeft += u => GuildMessage(c => c.LeaveMessage, LeaveMsg)(u, u.Guild);
    client.UserBanned += GuildMessage(c => c.BanMessage, BanMsg);
    client.UserVoiceStateUpdated += VoiceStateChanged;
    client.UserPresenceUpdated += PresenceChanged;
    _log = loggerFactory.CreateLogger<AnnounceService>();
  }

  async Task PresenceChanged(Optional<SocketGuild> guild,
                             SocketUser user,
                             SocketPresence before,
                             SocketPresence after){
    if (!guild.IsSpecified)
      return;
    var bG = before.Game;
    var aG = after.Game;
    var wasStreaming = bG.HasValue && bG?.StreamType != StreamType.NotStreaming;
    var isStreaming = aG.HasValue && aG?.StreamType != StreamType.NotStreaming;
    string message = null;
    if (!wasStreaming && !isStreaming) {
      return;
    } else if (!wasStreaming && isStreaming) {
      var game = aG.Value;
      message = ProcessMessage($"**$user** is now streaming **{game.Name}**: <{game.StreamUrl}>.", user);
    } else if (wasStreaming && isStreaming && before.Game?.Name != after.Game?.Name) {
      var game = aG.Value;
      message = ProcessMessage($"**$user** is now streaming **{game.Name}**: <{game.StreamUrl}>.", user);
    }
    if (message != null)
      await ForEachChannel(guild.Value, c => c.StreamMessage, message);
  }

  async Task ForEachChannel(IGuild guild, Func<Channel, bool> validFunc, string message) {
    using (var context = _services.GetService<BotDbContext>()) {
      var guildConfig = await context.Guilds.Get(guild);
      await context.Entry(guildConfig).Collection(g => g.Channels).LoadAsync();
      var sent = false;
      foreach(var channel in guildConfig.Channels.ToArray()) {
        if(!validFunc(channel))
          continue;
        var dChannel = (await guild.GetChannelAsync(channel.Id)) as ITextChannel;
        if(dChannel == null) {
          guildConfig.Channels.Remove(channel);
          context.Channels.Remove(channel);
          continue;
        }
        try {
          await dChannel.Respond(message);
          sent = true;
        } catch(HttpException) {
          _log.LogError($"Announcement {message.DoubleQuote()} failed in {guild.ToIDString()}. Notifying server owner.");
          var owner = await dChannel.Guild.GetOwner();
          await owner.SendDMAsync($"There as an attempt to announce something in channel {dChannel.Mention} that failed. " +
              $"The announcement was {message.DoubleQuote()}. Please make sure the bot has the approriate permissions to do so or " +
              "or disable the feature in said channel. Check the help command for more information");
        }
      }
      if (sent)
        _log.LogInformation($"Announcement in {guild.ToIDString()}: \"{message}\"");
    }
  }

  string GetUserString(IUser user) {
    string nickname = (user as IGuildUser)?.Nickname;
    if(string.IsNullOrEmpty(nickname))
      return user.Username;
    return nickname;
  }

  async Task VoiceStateChanged(SocketUser user,
      SocketVoiceState before,
      SocketVoiceState after) {
    var guild = (user as IGuildUser)?.Guild;
    if (guild == null || before.VoiceChannel?.Id == after.VoiceChannel?.Id)
      return;
    var userString = GetUserString(user).Bold();
    var bv = before.VoiceChannel != null;
    var av = after.VoiceChannel != null;
    var bName = before.VoiceChannel?.Name?.Bold();
    var aName = after.VoiceChannel?.Name?.Bold();
    string changes = null;
    if(bv && av) {
      changes = $"{userString} moved from {bName} to {aName}";
    } else if (av) {
      changes = $"{userString} joined {aName}";
    } else if (bv) {
      changes = $"{userString} left {bName}";
    }

    if(string.IsNullOrEmpty(changes))
      return;

    await ForEachChannel(guild, c => c.VoiceMessage, changes);
  }

  string ProcessMessage(string message, IUser user) {
    return message.Replace("$user", user.Username)
      .Replace("$mention", user.Mention);
  }

  Func<IUser, IGuild, Task> GuildMessage(Func<Channel, bool> msg, string defaultMsg) {
    return (u, g) => ForEachChannel(g, msg, ProcessMessage(defaultMsg, u));
  }

}


}
