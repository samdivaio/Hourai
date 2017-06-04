using Discord;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hourai.Preconditions;

namespace Hourai.Admin {

public partial class Admin {

  [Group("channel")]
  public class Channel : HouraiModule {

    [Log]
    [Command("create")]
    [GuildRateLimit(1, 5)]
    [RequirePermission(GuildPermission.ManageChannels)]
    [Remarks("Creates a public channel with a specified name.")]
    public async Task Create(string name) {
      var channel = await Check.NotNull(Context.Guild).CreateTextChannelAsync(name);
      await Success($"{channel.Mention} created.");
    }

    [Log]
    [Command("delete")]
    [GuildRateLimit(1, 5)]
    [RequirePermission(GuildPermission.ManageChannels)]
    [Remarks("Deletes all provided channels.")]
    public Task Delete(params IGuildChannel[] channels) =>
      ForEvery(channels, Do((IGuildChannel c) => c.DeleteAsync()));

    //[Log]
    //[Command("ban")]
    //[RequirePermission(ChannelPermission.ManageChannel)]
    //[Remarks("Bans all provided users from using the current channel.")]
    //public Task Ban(params IGuildUser[] users) {
      //var channel = Context.Channel as ITextChannel;
      //return ForEvery(users, Do((IGuildUser user) =>  {
        //var overwrite = channel.GetPermissionOverwrite(user) ??
          //OverwritePermissions.InheritAll;
        //overwrite.ReadMessages = PermValue.Deny;
        //return channel.AddPermissionOverwriteAsync(user, overwrite);
      //}));
    //}

    [Command("list")]
    [UserRateLimit(1, 1)]
    [ChannelRateLimit(1, 15)]
    [Remarks("Responds with a list of all text channels that the bot can see on this server.")]
    public async Task List() {
      var channels = Check.NotNull(Context.Guild).Channels.OfType<ITextChannel>();
      await RespondAsync(channels.OrderBy(c => c.Position).Select(c => c.Mention).Join(", "));
    }

    [Command("permissions")]
    [UserRateLimit(1, 1)]
    [ChannelRateLimit(1, 15)]
    [Remarks("Shows the channel permissions for one user on the current channel.\nShows your permisisons if no other user is specified")]
    public async Task Permissions(IGuildUser user = null) {
      user = user ?? (Context.User as IGuildUser);
      var perms = user.GetPermissions(Check.InGuild(Context.Message));
      await Context.Message.Respond(perms.ToList()
          .Select(p => p.ToString())
          .OrderBy(s => s)
          .Join(", "));
    }

  }

}

}
