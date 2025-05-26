using TuneSpace.Core.DTOs.Requests.Chat;
using TuneSpace.Core.DTOs.Responses.Chat;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Exceptions;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class ChatService(
    IChatRepository chatRepository,
    IMessageRepository messageRepository,
    IUserRepository userRepository) : IChatService
{
    async Task<List<MessageResponse>> IChatService.GetChatMessagesAsync(Guid chatId)
    {
        var chat = await chatRepository.GetChatByIdAsync(chatId) ?? throw new NotFoundException("Chat not found");
        var messages = await messageRepository.GetMessagesByChatIdAsync(chat.Id);

        var response = messages
            .Select(m => new MessageResponse(
                m.Id,
                m.Content,
                m.Sender.Id,
                m.Sender.UserName ?? string.Empty,
                m.Sender.ProfilePicture ?? [],
                m.Recipient.Id,
                m.Recipient.UserName ?? string.Empty,
                m.IsRead,
                m.Timestamp
                )
            )
            .ToList();

        return response;
    }

    async Task<ChatResponse> IChatService.GetChatByIdAsync(Guid chatId)
    {
        var chat = await chatRepository.GetChatByIdAsync(chatId) ?? throw new NotFoundException("Chat not found");

        var latestMessage = await messageRepository.GetLatestMessageBetweenUsersAsync(
                chat.ParticipantA.Id,
                chat.ParticipantB.Id);

        var unreadCount = await messageRepository.GetUnreadMessageCountFromUserAsync(
            chat.ParticipantA.Id,
            chat.ParticipantB.Id,
            chat.Id);

        return new ChatResponse(
            chat.Id,
            chat.ParticipantA.Id,
            chat.ParticipantA.UserName ?? string.Empty,
            chat.ParticipantA.ProfilePicture ?? [],
            chat.ParticipantB.Id,
            chat.ParticipantB.UserName ?? string.Empty,
            chat.ParticipantB.ProfilePicture ?? [],
            latestMessage?.Content ?? string.Empty,
            latestMessage?.Timestamp,
            unreadCount
        );
    }

    async Task<List<ChatResponse>> IChatService.GetUserChatsAsync(string username)
    {
        var user = await userRepository.GetUserByNameAsync(username) ?? throw new NotFoundException("User not found");
        var chats = await chatRepository.GetChatsByUser1IdOrUser2IdAsync(user, user);

        var response = new List<ChatResponse>();

        foreach (var chat in chats)
        {
            var otherUserId = chat.ParticipantA.UserName == username ? chat.ParticipantB.Id : chat.ParticipantA.Id;
            var otherUsername = chat.ParticipantA.UserName == username ? chat.ParticipantB.UserName : chat.ParticipantA.UserName;

            var otherUser = await userRepository.GetUserByIdAsync(otherUserId.ToString()) ?? throw new NotFoundException("User not found");

            var latestMessage = await messageRepository.GetLatestMessageBetweenUsersAsync(
                chat.ParticipantA.Id,
                chat.ParticipantB.Id);

            var unreadCount = await messageRepository.GetUnreadMessageCountFromUserAsync(
                otherUserId,
                user.Id,
                chat.Id);

            response.Add(new ChatResponse(
                chat.Id,
                user.Id,
                user.UserName ?? string.Empty,
                user.ProfilePicture ?? [],
                otherUserId,
                otherUsername ?? string.Empty,
                otherUser.ProfilePicture ?? [],
                latestMessage?.Content ?? string.Empty,
                latestMessage?.Timestamp,
                unreadCount
            ));
        }

        return response;
    }

    async Task<ChatResponse> IChatService.CreateChatAsync(CreateChatRequest request)
    {
        var user1 = await userRepository.GetUserByNameAsync(request.User1Name);
        var user2 = await userRepository.GetUserByNameAsync(request.User2Name);

        if (user1 is null || user2 is null)
        {
            throw new NotFoundException("User not found");
        }

        var chat = new Chat
        {
            ParticipantA = user1,
            ParticipantB = user2,
            CreatedAt = DateTime.Now.ToUniversalTime()
        };

        await chatRepository.InsertChatAsync(chat);

        return new ChatResponse(
            chat.Id,
            user1.Id,
            user1.UserName ?? string.Empty,
            user1.ProfilePicture ?? [],
            user2.Id,
            user2.UserName ?? string.Empty,
            user2.ProfilePicture ?? [],
            "",
            null,
            0
        );
    }

    async Task<MessageResponse> IChatService.CreateMessageAsync(SendMessageRequest request)
    {
        var user1 = await userRepository.GetUserByNameAsync(request.Sender);
        var user2 = await userRepository.GetUserByNameAsync(request.Receiver);

        if (user1 is null || user2 is null)
        {
            throw new NotFoundException("User not found");
        }

        var chat = await chatRepository.GetChatByUser1AndUser2Async(user1, user2) ?? throw new NotFoundException("Chat not found");
        var message = new Message
        {
            Content = request.Content,
            Chat = chat,
            Sender = user1,
            Recipient = user2,
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        await messageRepository.InsertMessageAsync(message);

        return new MessageResponse(
            message.Id,
            message.Content,
            message.Sender.Id,
            message.Sender.UserName ?? string.Empty,
            message.Sender.ProfilePicture ?? [],
            message.Recipient.Id,
            message.Recipient.UserName ?? string.Empty,
            false,
            message.Timestamp
        );
    }

    async Task IChatService.MarkMessagesAsReadAsync(Guid chatId, string username)
    {
        var user = await userRepository.GetUserByNameAsync(username) ?? throw new NotFoundException("User not found");
        var chat = await chatRepository.GetChatByIdAsync(chatId) ?? throw new NotFoundException("Chat not found");

        var otherUser = chat.ParticipantA.UserName == username ? chat.ParticipantB : chat.ParticipantA;

        await messageRepository.MarkMessagesAsReadAsync(chatId, otherUser.Id.ToString(), user.Id.ToString());
    }
}
