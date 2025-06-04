using Microsoft.Extensions.Logging;
using TuneSpace.Core.DTOs.Responses.BandChat;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class BandChatService(
    IBandChatRepository bandChatRepository,
    IBandRepository bandRepository,
    IUserRepository userRepository,
    IBandMessageRepository bandMessageRepository,
    ILogger<BandChatService> logger) : IBandChatService
{
    private readonly IBandChatRepository _bandChatRepository = bandChatRepository;
    private readonly IBandRepository _bandRepository = bandRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IBandMessageRepository _bandMessageRepository = bandMessageRepository;
    private readonly ILogger<BandChatService> _logger = logger;

    async Task<BandChatResponse?> IBandChatService.GetBandChatAsync(Guid userId, Guid bandId)
    {
        try
        {
            var bandChat = await _bandChatRepository.GetBandChatAsync(userId, bandId);
            if (bandChat is null)
            {
                return null;
            }

            var unreadCount = await _bandMessageRepository.GetUnreadCountAsync(userId, bandChat.Id);
            var messages = await _bandMessageRepository.GetChatMessagesAsync(bandChat.Id, skip: 0, take: 1);

            var lastMessage = string.Empty;
            if (messages.Any())
            {
                lastMessage = messages.First().Content;
            }

            return new BandChatResponse(
                bandChat.Id,
                bandChat.UserId,
                bandChat.User?.UserName ?? string.Empty,
                bandChat.User?.ProfilePicture ?? [],
                bandChat.BandId,
                bandChat.Band?.Name ?? string.Empty,
                bandChat.Band?.CoverImage ?? [],
                lastMessage,
                bandChat.LastMessageAt,
                unreadCount,
                bandChat.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting band chat for user {UserId} and band {BandId}", userId, bandId);
            throw;
        }
    }

    async Task<BandChatResponse> IBandChatService.CreateOrGetBandChatAsync(Guid userId, Guid bandId)
    {
        try
        {
            var existingChat = await _bandChatRepository.GetBandChatAsync(userId, bandId);
            if (existingChat != null)
            {
                var unreadCount = await _bandMessageRepository.GetUnreadCountAsync(userId, existingChat.Id);
                var messages = await _bandMessageRepository.GetChatMessagesAsync(existingChat.Id, skip: 0, take: 1);

                var lastMessage = string.Empty;
                if (messages.Any())
                {
                    lastMessage = messages.First().Content;
                }

                return new BandChatResponse(
                    existingChat.Id,
                    existingChat.UserId,
                    existingChat.User?.UserName ?? string.Empty,
                    existingChat.User?.ProfilePicture ?? [],
                    existingChat.BandId,
                    existingChat.Band?.Name ?? string.Empty,
                    existingChat.Band?.CoverImage ?? [],
                    lastMessage,
                    existingChat.LastMessageAt,
                    unreadCount,
                    existingChat.CreatedAt
                );
            }

            var user = await _userRepository.GetUserByIdAsync(userId.ToString()) ?? throw new ArgumentException($"User with ID {userId} not found");
            var band = await _bandRepository.GetBandByIdAsync(bandId) ?? throw new ArgumentException($"Band with ID {bandId} not found");

            var bandChat = new BandChat
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BandId = bandId,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            var createdChat = await _bandChatRepository.InsertAsync(bandChat);

            return new BandChatResponse(
                createdChat.Id,
                createdChat.UserId,
                user.UserName ?? string.Empty,
                user.ProfilePicture ?? [],
                createdChat.BandId,
                band.Name ?? string.Empty,
                band.CoverImage ?? [],
                string.Empty,
                createdChat.LastMessageAt,
                0,
                createdChat.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating or getting band chat for user {UserId} and band {BandId}", userId, bandId);
            throw;
        }
    }

    async Task<IEnumerable<BandChatResponse>> IBandChatService.GetUserBandChatsAsync(Guid userId)
    {
        try
        {
            var bandChats = await _bandChatRepository.GetUserBandChatsAsync(userId);
            var responses = new List<BandChatResponse>();

            foreach (var bandChat in bandChats)
            {
                var unreadCount = await _bandMessageRepository.GetUnreadCountAsync(userId, bandChat.Id);
                var messages = await _bandMessageRepository.GetChatMessagesAsync(bandChat.Id, skip: 0, take: 1);

                var lastMessage = string.Empty;
                if (messages.Any())
                {
                    lastMessage = messages.First().Content;
                }

                responses.Add(new BandChatResponse(
                    bandChat.Id,
                    bandChat.UserId,
                    bandChat.User?.UserName ?? string.Empty,
                    bandChat.User?.ProfilePicture ?? [],
                    bandChat.BandId,
                    bandChat.Band?.Name ?? string.Empty,
                    bandChat.Band?.CoverImage ?? [],
                    lastMessage,
                    bandChat.LastMessageAt,
                    unreadCount,
                    bandChat.CreatedAt
                ));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting band chats for user {UserId}", userId);
            throw;
        }
    }

    async Task<IEnumerable<BandChatResponse>> IBandChatService.GetBandChatsAsync(Guid bandId)
    {
        try
        {
            var bandChats = await _bandChatRepository.GetBandChatsAsync(bandId);
            var responses = new List<BandChatResponse>();

            foreach (var bandChat in bandChats)
            {
                var unreadCount = await _bandMessageRepository.GetUnreadCountAsync(bandChat.UserId, bandChat.Id);
                var messages = await _bandMessageRepository.GetChatMessagesAsync(bandChat.Id, skip: 0, take: 1);

                var lastMessage = string.Empty;
                if (messages.Any())
                {
                    lastMessage = messages.First().Content;
                }

                responses.Add(new BandChatResponse(
                    bandChat.Id,
                    bandChat.UserId,
                    bandChat.User?.UserName ?? string.Empty,
                    bandChat.User?.ProfilePicture ?? [],
                    bandChat.BandId,
                    bandChat.Band?.Name ?? string.Empty,
                    bandChat.Band?.CoverImage ?? [],
                    lastMessage,
                    bandChat.LastMessageAt,
                    unreadCount,
                    bandChat.CreatedAt
                ));
            }

            return responses;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chats for band {BandId}", bandId);
            throw;
        }
    }

    async Task<BandChatResponse?> IBandChatService.GetBandChatByIdAsync(Guid chatId)
    {
        try
        {
            var bandChat = await _bandChatRepository.GetByIdAsync(chatId);
            if (bandChat is null)
            {
                return null;
            }

            var unreadCount = await _bandMessageRepository.GetUnreadCountAsync(bandChat.UserId, bandChat.Id);
            var messages = await _bandMessageRepository.GetChatMessagesAsync(bandChat.Id, skip: 0, take: 1);

            var lastMessage = string.Empty;
            if (messages.Any())
            {
                lastMessage = messages.First().Content;
            }

            return new BandChatResponse(
                bandChat.Id,
                bandChat.UserId,
                bandChat.User?.UserName ?? string.Empty,
                bandChat.User?.ProfilePicture ?? [],
                bandChat.BandId,
                bandChat.Band?.Name ?? string.Empty,
                bandChat.Band?.CoverImage ?? [],
                lastMessage,
                bandChat.LastMessageAt,
                unreadCount,
                bandChat.CreatedAt
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting band chat by ID {ChatId}", chatId);
            throw;
        }
    }

    async Task IBandChatService.DeleteBandChatAsync(Guid bandChatId, Guid requestingUserId)
    {
        try
        {
            var bandChat = await _bandChatRepository.GetByIdAsync(bandChatId) ?? throw new ArgumentException($"Band chat with ID {bandChatId} not found");

            if (bandChat.UserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own band chats");
            }

            await _bandChatRepository.DeleteAsync(bandChatId);
            _logger.LogInformation("Band chat {BandChatId} deleted by user {UserId}", bandChatId, requestingUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting band chat {BandChatId} by user {UserId}", bandChatId, requestingUserId);
            throw;
        }
    }
}
