using TuneSpace.Core.DTOs.Requests.Forum;
using TuneSpace.Core.DTOs.Responses.Forum;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Enums;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Core.Interfaces.IServices;

namespace TuneSpace.Application.Services;

internal class ForumService(
    IForumRepository forumRepository,
    IUserRepository userRepository,
    IBandRepository bandRepository) : IForumService
{
    private readonly IForumRepository _forumRepository = forumRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IBandRepository _bandRepository = bandRepository;

    async Task<CategoryResponse> IForumService.CreateCategoryAsync(CreateCategoryRequest request)
    {
        var category = new ForumCategory
        {
            Name = request.Name,
            Description = request.Description,
            IconName = request.IconName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsPinned = false,
        };

        var createdCategory = await _forumRepository.CreateCategoryAsync(category);

        return new CategoryResponse(
            createdCategory.Id,
            createdCategory.Name,
            createdCategory.Description ?? string.Empty,
            createdCategory.IconName,
            0,
            0
        );
    }

    async Task<ThreadResponse> IForumService.CreateThreadAsync(CreateThreadRequest request, Guid userId)
    {
        var user = await _userRepository.GetUserById(userId.ToString()) ?? throw new ArgumentException("User not found");

        var now = DateTime.UtcNow;
        var thread = new ForumThread
        {
            Title = request.Title,
            CategoryId = request.CategoryId,
            AuthorId = userId,
            CreatedAt = now,
            LastActivityAt = now,
            Views = 0,
            IsPinned = false,
            IsLocked = false,
        };

        var createdThread = await _forumRepository.CreateThreadAsync(thread);

        var post = new ForumPost
        {
            Content = request.Content,
            ThreadId = createdThread.Id,
            AuthorId = userId,
            CreatedAt = now,
        };

        await _forumRepository.CreatePostAsync(post);

        return new ThreadResponse(
            createdThread.Id,
            createdThread.Title,
            "",
            user.Id,
            user.UserName ?? "Unknown",
            "",
            createdThread.CreatedAt,
            createdThread.LastActivityAt,
            0,
            0,
            createdThread.IsPinned,
            createdThread.IsLocked
        );
    }

    async Task<ForumPostResponse> IForumService.CreatePostAsync(CreatePostRequest request, Guid userId)
    {
        var user = await _userRepository.GetUserById(userId.ToString()) ?? throw new ArgumentException("User not found");

        var now = DateTime.UtcNow;
        var post = new ForumPost
        {
            Content = request.Content,
            ThreadId = request.ThreadId,
            AuthorId = userId,
            CreatedAt = now,
        };

        var createdPost = await _forumRepository.CreatePostAsync(post);

        await _forumRepository.UpdateThreadLastActivityAsync(request.ThreadId, now);

        return new ForumPostResponse(
            createdPost.Id,
            createdPost.Content,
            user.Id,
            user.UserName ?? "Unknown",
            //p.Author.ProfilePictureUrl,
            "",
            user.Role.ToString(),
            createdPost.CreatedAt,
            null,
            0,
            false
        );
    }

    async Task<List<CategoryResponse>> IForumService.GetAllCategoriesAsync()
    {
        var categories = await _forumRepository.GetAllCategoriesAsync();

        return [.. categories.Select(c => new CategoryResponse(
            c.Id,
            c.Name,
            c.Description ?? string.Empty,
            c.IconName,
            c.Threads.Count,
            c.Threads.SelectMany(t => t.Posts).Count()
        ))];
    }

    async Task<CategoryResponse?> IForumService.GetCategoryByIdAsync(Guid categoryId)
    {
        var category = await _forumRepository.GetCategoryByIdAsync(categoryId);
        if (category == null)
        {
            return null;
        }

        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description ?? string.Empty,
            category.IconName,
            category.Threads.Count,
            category.Threads.SelectMany(t => t.Posts).Count()
        );
    }

    async Task<List<ThreadResponse>> IForumService.GetThreadsByCategoryAsync(Guid categoryId)
    {
        var threads = await _forumRepository.GetThreadsByCategoryAsync(categoryId);

        return [.. threads.Select(t => new ThreadResponse(
            t.Id,
            t.Title,
            t.Category.Name,
            t.Author.Id,
            t.Author.UserName ?? "Unknown",
            "",
            t.CreatedAt,
            t.LastActivityAt,
            t.Posts.Count - 1,
            t.Views,
            t.IsPinned,
            t.IsLocked
        ))];
    }

    async Task<ThreadDetailResponse?> IForumService.GetThreadDetailAsync(Guid threadId, Guid userId)
    {
        var thread = await _forumRepository.GetThreadDetailWithPostsAsync(threadId);
        if (thread == null)
        {
            return null;
        }

        var postResponses = thread.Posts.OrderBy(p => p.CreatedAt).Select(p => new ForumPostResponse(
            p.Id,
            p.Content,
            p.Author.Id,
            p.Author.UserName ?? "Unknown",
            //p.Author.ProfilePictureUrl,
            "",
            p.Author.Role.ToString(),
            p.CreatedAt,
            p.UpdatedAt,
            p.Likes.Count,
            p.Likes.Any(l => l.UserId == userId)
        )).ToList();

        return new ThreadDetailResponse(
            thread.Id,
            thread.Title,
            new CategorySummaryResponse(
                thread.Category.Id,
                thread.Category.Name,
                thread.Category.Description ?? string.Empty
            ),
            thread.CategoryId,
            thread.Category.Name,
            thread.CreatedAt,
            thread.IsPinned,
            thread.IsLocked,
            postResponses
        );
    }

    async Task<List<ThreadResponse>?> IForumService.GetBandThreads(Guid bandId)
    {
        var band = await _bandRepository.GetBandById(bandId);

        if (band == null)
        {
            return null;
        }

        var bandAdmin = band.Members.FirstOrDefault(m => m.Role == Roles.BandAdmin);

        if (bandAdmin == null)
        {
            return null;
        }

        var threads = await _forumRepository.GetThreadsByAuthorId(bandAdmin?.Id ?? Guid.Empty);
        if (threads == null)
        {
            return null;
        }

        return [.. threads.Select(t => new ThreadResponse(
            t.Id,
            t.Title,
            t.Category.Name,
            t.Author.Id,
            t.Author.UserName ?? "Unknown",
            "",
            t.CreatedAt,
            t.LastActivityAt,
            t.Posts.Count - 1,
            t.Views,
            t.IsPinned,
            t.IsLocked
        ))];
    }

    async Task<ForumPostResponse?> IForumService.GetPostByIdAsync(Guid postId, Guid userId)
    {
        var post = await _forumRepository.GetPostByIdAsync(postId);
        if (post == null)
        {
            return null;
        }

        var hasLiked = await _forumRepository.HasUserLikedPostAsync(postId, userId);

        return new ForumPostResponse(
            post.Id,
            post.Content,
            post.Author.Id,
            post.Author.UserName ?? "Unknown",
            //p.Author.ProfilePictureUrl,
            "",
            post.Author.Role.ToString(),
            post.CreatedAt,
            post.UpdatedAt,
            post.Likes.Count,
            hasLiked
        );
    }

    async Task IForumService.IncrementThreadViewAsync(Guid threadId)
    {
        await _forumRepository.IncrementThreadViewAsync(threadId);
    }

    async Task<bool> IForumService.LikePostAsync(Guid postId, Guid userId)
    {
        var hasLiked = await _forumRepository.HasUserLikedPostAsync(postId, userId);
        if (hasLiked)
        {
            return false;
        }

        var like = new ForumPostLike
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _forumRepository.CreatePostLikeAsync(like);
        return true;
    }

    async Task<bool> IForumService.UnlikePostAsync(Guid postId, Guid userId)
    {
        var like = await _forumRepository.GetPostLikeAsync(postId, userId);
        if (like == null)
        {
            return false;
        }

        await _forumRepository.RemovePostLikeAsync(like);
        return true;
    }
}
