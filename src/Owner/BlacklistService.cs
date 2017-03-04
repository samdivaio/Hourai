using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hourai.Model;
using System;
using System.Threading.Tasks;

namespace Hourai {

public class BlacklistService : IService {

  DiscordShardedClient Client { get; }

  public BlacklistService(DiscordShardedClient client) {
    client.GuildAvailable += CheckBlacklist(false);
    client.JoinedGuild += CheckBlacklist(true);
    Client = client;
  }

  Func<IGuild, Task> CheckBlacklist(bool normalJoin) {
    return async guild => {
      using (var context = new BotDbContext()) {
        var config = await context.Guilds.Get(guild);
        var defaultChannel = (await guild.GetChannelAsync(guild.DefaultChannelId)) as ITextChannel;
        if (defaultChannel == null)
          return;
        if(config.IsBlacklisted) {
          Log.Info($"Added to blacklisted guild {guild.Name} ({guild.Id})");
          await defaultChannel.Respond("This server has been blacklisted by this bot. " +
              "Please do not add it again. Leaving...");
          await guild.LeaveAsync();
          return;
        }
        if(normalJoin) {
          var help = $"{config.Prefix}help".Code();
          await defaultChannel.Respond(
              $"Hello {guild.Name}! {Client.CurrentUser.Username} has been added to your server!\n" +
              "To see available commands, run the command {help}\n" +
              "For more information, see https://github.com/james7132/Hourai");
        }
      }
    };
  }

}

}
