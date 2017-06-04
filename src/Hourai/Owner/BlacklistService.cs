using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Hourai.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hourai {

[Service]
public class BlacklistService {

  readonly ILogger _log;
  readonly IServiceProvider _services;

  public BlacklistService(DiscordShardedClient client,
                          ILoggerFactory loggerFactory,
                          IServiceProvider services) {
    _services = services;
    client.GuildAvailable += CheckBlacklist(false);
    client.JoinedGuild += CheckBlacklist(true);
    _log = loggerFactory.CreateLogger<BlacklistService>();
  }

  Func<SocketGuild, Task> CheckBlacklist(bool normalJoin) {
    return async guild => {
      using (var context = _services.GetService<BotDbContext>()) {
        var config = await context.Guilds.Get(guild);
        var defaultChannel = guild.DefaultChannel;
        if (defaultChannel == null)
          return;
        if(config.IsBlacklisted) {
          _log.LogInformation($"Added to blacklisted guild {guild.Name} ({guild.Id})");
          await defaultChannel.Respond("This server has been blacklisted by this bot. " +
              "Please do not add it again. Leaving...");
          await guild.LeaveAsync();
          return;
        }
        if(normalJoin) {
          var help = $"{config.Prefix}help".Code();
          await defaultChannel.Respond(
              $"Hello {guild.Name}! {guild.CurrentUser.Username} has been added to your server!\n" +
              $"To see available commands, run the command {help}\n" +
              "For more information, see https://github.com/james7132/Hourai");
        }
      }
    };
  }

}

}
