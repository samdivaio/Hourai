using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using Hourai.Preconditions;

namespace Hourai.Modules {

public partial class Admin {

  [Group("config")]
  [RequireContext(ContextType.Guild)]
  public class ConfigGroup : HouraiModule {

    BotDbContext Database { get; }

    public ConfigGroup(BotDbContext db) {
      Database = db;
    }

    [Command("prefix")]
    [GuildRateLimit(1, 60)]
    [Remarks("Sets the bot's command prefix for this server.")]
    [RequirePermission(GuildPermission.ManageGuild, Require.BotOwnerOverride)]
    public async Task Prefix([Remainder] string prefix) {
      if(string.IsNullOrEmpty(prefix)) {
        await RespondAsync("Cannot set bot prefix to an empty result");
        return;
      }
      var guild = Database.GetGuild(Context.Guild);
      guild.Prefix = prefix.Substring(0, 1);
      await Database.Save();
      await Success($"Bot command prefix set to {guild.Prefix.ToString().Code()}");
    }

  }

}

}
