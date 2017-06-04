using Discord;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Hourai.Custom {

public class FakeMessage : IUserMessage {

  public FakeMessage(string content, IUser user, IMessageChannel channel) {
    Content = content;
    Author = user;
    Channel = channel;
    CreatedAt = DateTimeOffset.UtcNow;
  }
  public ulong Id => 0UL;
  public MessageSource Source => MessageSource.User;
  public string Content { get; }
  public IUser Author { get; }
  public IMessageChannel Channel { get; }
  public DateTimeOffset CreatedAt { get; }
  public DateTimeOffset Timestamp => CreatedAt;
  public ulong? WebhookId => null;
  public bool IsPinned => false;
  public bool IsTTS => false;
  public bool IsWebhook => false;
  public MessageType Type => MessageType.Default;
  public DateTimeOffset? EditedTimestamp => null;
  public IReadOnlyCollection<IAttachment> Attachments => new ReadOnlyCollection<IAttachment>(new IAttachment[0]);
  public IReadOnlyCollection<IEmbed> Embeds => new ReadOnlyCollection<IEmbed>(new IEmbed[0]);
  public IReadOnlyCollection<ITag> Tags => new ReadOnlyCollection<ITag>(new ITag[0]);
  public IReadOnlyDictionary<IEmote, ReactionMetadata> Reactions => new ReadOnlyDictionary<IEmote, ReactionMetadata>(new Dictionary<IEmote, ReactionMetadata>());
  public IReadOnlyCollection<ulong> MentionedUserIds => new ReadOnlyCollection<ulong>(new ulong[0]);
  public IReadOnlyCollection<ulong> MentionedRoleIds => new ReadOnlyCollection<ulong>(new ulong[0]);
  public IReadOnlyCollection<ulong> MentionedChannelIds => new ReadOnlyCollection<ulong>(new ulong[0]);

  public Task ModifyAsync(Action<MessageProperties> builder, RequestOptions options) => Task.CompletedTask;
  public Task PinAsync(RequestOptions options) => Task.CompletedTask;
  public Task UnpinAsync(RequestOptions options) => Task.CompletedTask;
  public Task AddReactionAsync(IEmote emoji, RequestOptions options) => Task.CompletedTask;
  public Task AddReactionAsync(string emoji, RequestOptions options) => Task.CompletedTask;
  public Task DeleteAsync(RequestOptions options) => Task.CompletedTask;
  public Task<IReadOnlyCollection<IUser>> GetReactionUsersAsync(string emoji, int limit, ulong? afterUserId, RequestOptions options) => Task.FromResult<IReadOnlyCollection<IUser>>(null);
  public Task RemoveReactionAsync(IEmote emoji, IUser user, RequestOptions options) => Task.CompletedTask;
  public Task RemoveReactionAsync(string  emoji, IUser user, RequestOptions options) => Task.CompletedTask;
  public Task RemoveAllReactionsAsync(RequestOptions options) => Task.CompletedTask;
  public string Resolve(TagHandling userHandling, TagHandling channelHandling, TagHandling roleHandling, TagHandling everyoneHandling, TagHandling emojiHandling) => Content;
}

}
