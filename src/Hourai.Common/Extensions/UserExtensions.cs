using System.Threading.Tasks;
using Discord;

namespace Hourai {

public static class UserExtensions {

  /// <summary> Self-explanatory helper functions to alter the state of a user</summary>
  public static Task SetMuted(this IGuildUser user, bool value) => user.ModifyAsync(p => p.Mute = value);
  public static Task SetDeafen(this IGuildUser user, bool value) => user.ModifyAsync(p => p.Deaf= value);
  public static Task MuteAsync(this IGuildUser user) => user.SetMuted(true);
  public static Task UnmuteAsync(this IGuildUser user) => user.SetMuted(false);
  public static Task DeafenAsync(this IGuildUser user) => user.SetDeafen(true);
  public static Task UndeafenAsync(this IGuildUser user) => user.SetDeafen(false);
  public static Task SetNickname(this IGuildUser user, string nickname) => user.ModifyAsync(p => p.Nickname = nickname);

  public static bool IsServerOwner(this IGuildUser user) => user.Guild.OwnerId == user.Id;

  public static async Task SendDMAsync(this IUser user, string message) {
    var channel = await user.CreateDMChannelAsync();
    await channel.SendMessageAsync(message);
  }

  public static Task BanAsync(this IGuildUser user, int pruneDays = 0) {
    return Check.NotNull(user).Guild.AddBanAsync(user, pruneDays);
  }

}

}
