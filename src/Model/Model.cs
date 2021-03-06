using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Hourai {

public class BotDbContext : DbContext {

  // Discord Data
  public DbSet<Guild> Guilds { get; set; }
  public DbSet<Channel> Channels { get; set; }
  public DbSet<User> Users { get; set; }
  public DbSet<Username> Usernames { get; set; }
  public DbSet<GuildUser> GuildUsers { get; set; }
  public DbSet<CustomCommand> Commands { get; set; }

  // Temporary Action Data
  public DbSet<AbstractTempAction> TempActions { get; set; }
  public DbSet<TempBan> TempBans { get; set; }
  public DbSet<TempRole> TempRole { get; set; }

  // Service Data
  //public DbSet<Subreddit> Subreddits { get; set; }
  //public DbSet<SubredditChannel> SubredditChannels { get; set; }

  public bool AllowSave { get; set; } = true;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    Config.Load();
    optionsBuilder.UseSqlite($"Filename={Config.DbFilename}");
  }

  protected override void OnModelCreating(ModelBuilder builder) {
    builder.Entity<Guild>(b => {
        b.Property(g => g.Prefix)
          .HasDefaultValue("~");
      });
    builder.Entity<GuildUser>()
      .HasKey(u => new { u.Id, u.GuildId });
    builder.Entity<Channel>()
      .HasKey(c => new { c.Id, c.GuildId });
    builder.Entity<CustomCommand>()
      .HasKey(c => new { c.GuildId, c.Name });
    builder.Entity<Username>()
      .HasKey(c => new { c.UserId, c.Date });
    //builder.Entity<SubredditChannel>()
      //.HasKey(s => new { s.Name, s.ChannelId });
    builder.Entity<AbstractTempAction>()
      .HasIndex(t => t.End)
        .HasName("IX_temp_actions_End");
  }

  public async Task Save() {
    if(!AllowSave)
      return;
    var changes = await SaveChangesAsync();
    if(changes > 0)
      Log.Info($"Saved {changes} changes to the database.");
  }

  public Guild FindGuild(IGuild iguild) {
    Check.NotNull(iguild);
    return Guilds.Include(g => g.Commands)
      .Include(g => g.Channels)
      .FirstOrDefault(g => g.Id == iguild.Id);
  }

  //string SanitizeSubredditName(string name) {
    //return name.Trim().ToLower();
  //}

  public Guild GetGuild(IGuild iguild) {
    var guild = FindGuild(iguild);
    if(guild == null) {
      guild = new Guild(iguild);
      Guilds.Add(guild);
    }
    return guild;
  }

  public bool RemoveGuild(IGuild iguild) {
    var guild = FindGuild(iguild);
    if(guild == null)
      return false;
    Guilds.Remove(guild);
    return true;
  }

  public User FindUser(IUser iuser) {
    Check.NotNull(iuser);
    return Users.Include(u => u.Usernames)
      .FirstOrDefault(u => u.Id == iuser.Id);
  }

  public User GetUser(IUser iuser) {
    var user = FindUser(iuser);
    if(user == null) {
      user = new User(iuser);
      Users.Add(user);
    }
    return user;
  }

  public bool RemoveUser(IUser iuser) {
    var user = FindUser(iuser);
    if(user == null)
      return false;
    Users.Remove(user);
    return true;
  }

  public GuildUser FindGuildUser(IGuildUser iuser) {
    Check.NotNull(iuser);
    return GuildUsers.FirstOrDefault(u =>
        (u.Id == iuser.Id) && (u.GuildId == iuser.Guild.Id));
  }

  public GuildUser GetGuildUser(IGuildUser iuser) {
    var user = FindGuildUser(iuser);
    if(user == null) {
      user = new GuildUser(iuser);
      user.User = FindUser(iuser);
      if(user.User == null) {
        user.User = new User(iuser);
        Users.Add(user.User);
      }
      GuildUsers.Add(user);
    }
    return user;
  }

  public bool RemoveGuildUser(IGuildUser iuser) {
    var user = FindGuildUser(iuser);
    if(user == null)
      return false;
    GuildUsers.Remove(user);
    return true;
  }

  public Channel FindChannel(IGuildChannel ichannel) {
    Check.NotNull(ichannel);
    return Channels.FirstOrDefault(c =>
        (c.Id == ichannel.Id) && (c.GuildId == ichannel.Guild.Id));
  }

  public Channel GetChannel(IGuildChannel ichannel) {
    var channel = FindChannel(ichannel);
    if(channel == null) {
      channel = new Channel(ichannel);
      Channels.Add(channel);
    }
    return channel;
  }

  public bool RemoveChannel(IGuildChannel ichannel) {
    var channel = FindChannel(ichannel);
    if(channel == null)
      return false;
    Channels.Remove(channel);
    return true;
  }

  //public Subreddit FindSubreddit(string name) {
    //name = SanitizeSubredditName(name);
    //return Subreddits.Include(s => s.Channels).FirstOrDefault(s => s.Name == name);
  //}

  //public async Task<Subreddit> GetSubreddit(string name) {
    //var subreddit = FindSubreddit(name);
    //if(subreddit == null) {
      //subreddit = new Subreddit { Name = SanitizeSubredditName(name) };
      //Subreddits.Add(subreddit);
      //await Save();
    //}
    //return subreddit;
  //}

}


}
