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
    async Task<List<MessageResponse>> IChatService.GetChatMessages(Guid chatId)
    {
        var chat = await chatRepository.GetChatById(chatId) ?? throw new NotFoundException("Chat not found");
        var messages = await messageRepository.GetMessagesByChatIdAsync(chat.Id);

        var response = messages
            .Select(m => new MessageResponse(
                m.Id,
                m.Content,
                m.Sender.Id,
                m.Sender.UserName ?? string.Empty,
                m.Recipient.Id,
                m.Recipient.UserName ?? string.Empty,
                m.IsRead,
                m.Timestamp
                )
            )
            .ToList();

        return response;
    }

    async Task<ChatResponse> IChatService.GetChatById(Guid chatId)
    {
        var chat = await chatRepository.GetChatById(chatId) ?? throw new NotFoundException("Chat not found");
        var otherUser = chat.ParticipantA.Id == chat.ParticipantB.Id ? chat.ParticipantB.Id : chat.ParticipantA.Id;
        var otherUsername = chat.ParticipantA.UserName == chat.ParticipantB.UserName ? chat.ParticipantB.UserName : chat.ParticipantA.UserName;

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
            chat.ParticipantB.Id,
            chat.ParticipantB.UserName ?? string.Empty,
            latestMessage?.Content ?? string.Empty,
            latestMessage?.Timestamp,
            unreadCount
        );
    }

    async Task<List<ChatResponse>> IChatService.GetUserChats(string username)
    {
        var user = await userRepository.GetUserByName(username) ?? throw new NotFoundException("User not found");
        var chats = await chatRepository.GetChatsByUser1IdOrUser2Id(user, user);

        var response = new List<ChatResponse>();

        foreach (var chat in chats)
        {
            var otherUser = chat.ParticipantA.UserName == username ? chat.ParticipantB.Id : chat.ParticipantA.Id;
            var otherUsername = chat.ParticipantA.UserName == username ? chat.ParticipantB.UserName : chat.ParticipantA.UserName;

            var latestMessage = await messageRepository.GetLatestMessageBetweenUsersAsync(
                chat.ParticipantA.Id,
                chat.ParticipantB.Id);

            var unreadCount = await messageRepository.GetUnreadMessageCountFromUserAsync(
                otherUser,
                user.Id,
                chat.Id);

            response.Add(new ChatResponse(
                chat.Id,
                user.Id,
                user.UserName ?? string.Empty,
                otherUser,
                otherUsername ?? string.Empty,
                latestMessage?.Content ?? string.Empty,
                latestMessage?.Timestamp,
                unreadCount
            ));
        }

        return response;
    }

    async Task<ChatResponse> IChatService.CreateChat(CreateChatRequest request)
    {
        var user1 = await userRepository.GetUserByName(request.User1Name);
        var user2 = await userRepository.GetUserByName(request.User2Name);

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

        await chatRepository.InsertChat(chat);

        return new ChatResponse(
            chat.Id,
            user1.Id,
            user1.UserName ?? string.Empty,
            user2.Id,
            user2.UserName ?? string.Empty,
            "",
            null,
            0
        );
    }

    async Task<MessageResponse> IChatService.CreateMessage(SendMessageRequest request)
    {
        var user1 = await userRepository.GetUserByName(request.Sender);
        var user2 = await userRepository.GetUserByName(request.Receiver);

        if (user1 is null || user2 is null)
        {
            throw new NotFoundException("User not found");
        }

        var chat = await chatRepository.GetChatByUser1AndUser2(user1, user2) ?? throw new NotFoundException("Chat not found");
        var message = new Message
        {
            Content = request.Content,
            Chat = chat,
            Sender = user1,
            Recipient = user2,
            Timestamp = DateTime.Now.ToUniversalTime()
        };

        await messageRepository.InsertMessage(message);

        return new MessageResponse(
            message.Id,
            message.Content,
            message.Sender.Id,
            message.Sender.UserName ?? string.Empty,
            message.Recipient.Id,
            message.Recipient.UserName ?? string.Empty,
            false,
            message.Timestamp
        );
    }

    async Task IChatService.MarkMessagesAsRead(Guid chatId, string username)
    {
        var user = await userRepository.GetUserByName(username) ?? throw new NotFoundException("User not found");
        var chat = await chatRepository.GetChatById(chatId) ?? throw new NotFoundException("Chat not found");

        var otherUser = chat.ParticipantA.UserName == username ? chat.ParticipantB : chat.ParticipantA;

        await messageRepository.MarkMessagesAsReadAsync(chatId, otherUser.Id.ToString(), user.Id.ToString());
    }
}
