using Discord;
using Discord.WebSocket;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Hourai {

[Table("temp_actions")]
public class AbstractTempAction {

  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public ulong Id { get; set; }

  [Required]
  public ulong UserId { get; set; }
  [Required]
  public ulong GuildId { get; set; }
  [Required]
  public DateTimeOffset Start { get; set; }
  [Required]
  public DateTimeOffset End { get; set; }

  [Required]
  [ForeignKey("UserId")]
  public User User;

  [Required]
  [ForeignKey("GuildId")]
  public Guild Guild;

  public virtual async Task Apply(DiscordSocketClient client) {
    await Task.CompletedTask;
    throw new NotImplementedException();
  }

  public virtual async Task Unapply(DiscordSocketClient client) {
    await Task.CompletedTask;
    throw new NotImplementedException();
  }

}

public class TempBan : AbstractTempAction {

  public override async Task Apply(DiscordSocketClient client) {
    var guild = client.GetGuild(GuildId);
    await guild.AddBanAsync(UserId);
  }

  public override async Task Unapply(DiscordSocketClient client) {
    var guild = client.GetGuild(GuildId);
    await guild.RemoveBanAsync(UserId);
    Log.Info($"{UserId}'s temp ban from {GuildId} has been lifted.");
  }

}

public class TempRole : AbstractTempAction {

  [Required]
  public ulong RoleId { get; set; }

  public override async Task Apply(DiscordSocketClient client) {
    var guild = client.GetGuild(GuildId);
    var user = guild.GetUser(UserId);
    var role = guild.GetRole(RoleId);
    if(role == null)
      return;
    await user.AddRolesAsync(role);
  }

  public override async Task Unapply(DiscordSocketClient client) {
    var guild = client.GetGuild(GuildId);
    var user = guild.GetUser(UserId);
    var role = guild.GetRole(RoleId);
    if(role == null)
      return;
    await user.RemoveRolesAsync(role);
  }

}

}
