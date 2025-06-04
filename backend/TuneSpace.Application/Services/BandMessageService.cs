using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Responses.BandChat;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class BandMessageService(
    IBandMessageRepository bandMessageRepository,
    IBandChatService bandChatService,
    IBandChatRepository bandChatRepository,
    IBandRepository bandRepository,
    ILogger<BandMessageService> logger) : IBandMessageService
{
    private readonly IBandMessageRepository _bandMessageRepository = bandMessageRepository;
    private readonly IBandChatService _bandChatService = bandChatService;
    private readonly IBandChatRepository _bandChatRepository = bandChatRepository;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly ILogger<BandMessageService> _logger = logger;

    async Task<BandMessageResponse> IBandMessageService.SendMessageToBandAsync(Guid userId, Guid bandId, string content)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Message content cannot be empty");
            }

            var bandChatResponse = await _bandChatService.CreateOrGetBandChatAsync(userId, bandId);

            var bandChat = await _bandChatRepository.GetByIdAsync(bandChatResponse.Id) ?? throw new InvalidOperationException("Band chat not found");
            var message = new BandMessage
            {
                Id = Guid.NewGuid(),
                BandChatId = bandChat.Id,
                SenderId = userId,
                BandId = null,
                Content = content.Trim(),
                Timestamp = DateTime.UtcNow,
                IsRead = false,
                IsFromBand = false
            };

            var createdMessage = await _bandMessageRepository.InsertAsync(message);

            bandChat.LastMessageAt = DateTime.UtcNow;
            await _bandChatRepository.UpdateAsync(bandChat);

            _logger.LogInformation("User {UserId} sent message to band {BandId}", userId, bandId);

            return new BandMessageResponse(
                createdMessage.Id,
                createdMessage.BandChatId,
                createdMessage.Content,
                createdMessage.Timestamp,
                createdMessage.IsRead,
                createdMessage.IsFromBand,
                createdMessage.SenderId,
                createdMessage.Sender?.UserName ?? string.Empty,
                createdMessage.Sender?.ProfilePicture ?? [],
                createdMessage.BandId,
                createdMessage.Band?.Name ?? string.Empty,
                createdMessage.Band?.CoverImage ?? []
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message from user {UserId} to band {BandId}", userId, bandId);
            throw;
        }
    }

    async Task<BandMessageResponse> IBandMessageService.SendMessageFromBandAsync(Guid bandId, Guid userId, string content, Guid bandMemberId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Message content cannot be empty");
            }

            var band = await _bandRepository.GetBandByIdAsync(bandId) ?? throw new ArgumentException($"Band with ID {bandId} not found");
            if (!band.Members.Any(m => m.Id == bandMemberId))
            {
                throw new UnauthorizedAccessException("User is not a member of this band");
            }

            var bandChatResponse = await _bandChatService.CreateOrGetBandChatAsync(userId, bandId);

            var bandChat = await _bandChatRepository.GetByIdAsync(bandChatResponse.Id) ?? throw new InvalidOperationException("Band chat not found");
            var message = new BandMessage
            {
                Id = Guid.NewGuid(),
                BandChatId = bandChat.Id,
                SenderId = null,
                BandId = bandId,
                Content = content.Trim(),
                Timestamp = DateTime.UtcNow,
                IsRead = false,
                IsFromBand = true
            };

            var createdMessage = await _bandMessageRepository.InsertAsync(message);

            bandChat.LastMessageAt = DateTime.UtcNow;
            await _bandChatRepository.UpdateAsync(bandChat);

            _logger.LogInformation("Band {BandId} sent message to user {UserId} via member {BandMemberId}", bandId, userId, bandMemberId);

            return new BandMessageResponse(
                createdMessage.Id,
                createdMessage.BandChatId,
                createdMessage.Content,
                createdMessage.Timestamp,
                createdMessage.IsRead,
                createdMessage.IsFromBand,
                createdMessage.SenderId,
                createdMessage.Sender?.UserName ?? string.Empty,
                createdMessage.Sender?.ProfilePicture ?? [],
                createdMessage.BandId,
                createdMessage.Band?.Name ?? string.Empty,
                createdMessage.Band?.CoverImage ?? []
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message from band {BandId} to user {UserId} via member {BandMemberId}", bandId, userId, bandMemberId);
            throw;
        }
    }

    async Task<IEnumerable<BandMessageResponse>> IBandMessageService.GetChatMessagesAsync(Guid bandChatId, int skip, int take)
    {
        try
        {
            if (take > 100)
            {
                take = 100;
            }

            var messages = await _bandMessageRepository.GetChatMessagesAsync(bandChatId, skip, take);

            return messages.Select(message => new BandMessageResponse(
                message.Id,
                message.BandChatId,
                message.Content,
                message.Timestamp,
                message.IsRead,
                message.IsFromBand,
                message.SenderId,
                message.Sender?.UserName ?? string.Empty,
                message.Sender?.ProfilePicture ?? [],
                message.BandId,
                message.Band?.Name ?? string.Empty,
                message.Band?.CoverImage ?? []
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages for band chat {BandChatId}", bandChatId);
            throw;
        }
    }

    async Task IBandMessageService.MarkMessageAsReadAsync(Guid messageId, Guid requestingUserId)
    {
        try
        {
            var message = await _bandMessageRepository.GetByIdAsync(messageId) ?? throw new ArgumentException($"Message with ID {messageId} not found");

            var bandChat = message.BandChat;
            if (bandChat.UserId != requestingUserId && (message.IsFromBand || message.SenderId != requestingUserId))
            {
                throw new UnauthorizedAccessException("You can only mark messages as read if you are the recipient");
            }

            await _bandMessageRepository.MarkAsReadAsync(messageId);
            _logger.LogInformation("Message {MessageId} marked as read by user {UserId}", messageId, requestingUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking message {MessageId} as read by user {UserId}", messageId, requestingUserId);
            throw;
        }
    }

    async Task<int> IBandMessageService.GetUnreadCountAsync(Guid userId, Guid bandChatId)
    {
        try
        {
            return await _bandMessageRepository.GetUnreadCountAsync(userId, bandChatId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user {UserId} in band chat {BandChatId}", userId, bandChatId);
            throw;
        }
    }

    async Task IBandMessageService.DeleteMessageAsync(Guid messageId, Guid requestingUserId)
    {
        try
        {
            var message = await _bandMessageRepository.GetByIdAsync(messageId) ?? throw new ArgumentException($"Message with ID {messageId} not found");

            if ((!message.IsFromBand && message.SenderId != requestingUserId) ||
                (message.IsFromBand && message.BandChat.UserId != requestingUserId))
            {
                throw new UnauthorizedAccessException("You can only delete your own messages");
            }

            await _bandMessageRepository.DeleteAsync(messageId);
            _logger.LogInformation("Message {MessageId} deleted by user {UserId}", messageId, requestingUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId} by user {UserId}", messageId, requestingUserId);
            throw;
        }
    }
}
