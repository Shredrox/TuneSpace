using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using TuneSpace.Core.Entities;
using TuneSpace.Core.Interfaces.IRepositories;
using TuneSpace.Infrastructure.Data;

namespace TuneSpace.Infrastructure.Repositories;

internal class ArtistEmbeddingRepository(TuneSpaceDbContext context) : IArtistEmbeddingRepository
{
    private readonly TuneSpaceDbContext _context = context;

    async Task<ArtistEmbedding?> IArtistEmbeddingRepository.GetByArtistNameAsync(string artistName)
    {
        return await _context.ArtistEmbeddings
            .FirstOrDefaultAsync(a => EF.Functions.ILike(a.ArtistName, artistName));
    }

    async Task<ArtistEmbedding?> IArtistEmbeddingRepository.GetBySpotifyIdAsync(string spotifyId)
    {
        return await _context.ArtistEmbeddings
            .FirstOrDefaultAsync(a => a.SpotifyId == spotifyId);
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.GetByGenresAsync(List<string> genres, int limit)
    {
        var lowerGenres = genres.Select(g => g.ToLower()).ToList();

        return await _context.ArtistEmbeddings
            .Where(a => a.Genres.Any(g => lowerGenres.Contains(g.ToLower())))
            .Take(limit)
            .ToListAsync();
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.GetByLocationAsync(string location, int limit)
    {
        return await _context.ArtistEmbeddings
            .Where(a => a.Location != null && EF.Functions.ILike(a.Location, $"%{location}%"))
            .Take(limit)
            .ToListAsync();
    }
    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.FindSimilarArtistsAsync(Vector queryEmbedding, int limit, double threshold)
    {
        // Use pgvector similarity search
        return await _context.ArtistEmbeddings
            .Where(a => a.Embedding != null)
            .OrderBy(a => a.Embedding!.CosineDistance(queryEmbedding))
            .Take(limit)
            .ToListAsync();
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.FindSimilarToArtistAsync(string artistName, int limit)
    {
        var artist = await ((IArtistEmbeddingRepository)this).GetByArtistNameAsync(artistName);
        if (artist?.Embedding == null)
        {
            return [];
        }

        return await ((IArtistEmbeddingRepository)this).FindSimilarArtistsAsync(artist.Embedding, limit);
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.FindSimilarByGenresAndLocationAsync(List<string> genres, string? location, int limit)
    {
        var query = _context.ArtistEmbeddings.AsQueryable();

        if (genres.Count > 0)
        {
            var lowerGenres = genres.Select(g => g.ToLower()).ToList();
            query = query.Where(a => a.Genres.Any(g => lowerGenres.Contains(g.ToLower())));
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(a => a.Location != null && EF.Functions.ILike(a.Location, $"%{location}%"));
        }

        return await query
            .Where(a => a.Embedding != null)
            .Take(limit)
            .ToListAsync();
    }

    async Task<ArtistEmbedding> IArtistEmbeddingRepository.CreateAsync(ArtistEmbedding artistEmbedding)
    {
        artistEmbedding.CreatedAt = DateTime.UtcNow;
        artistEmbedding.UpdatedAt = DateTime.UtcNow;

        _context.ArtistEmbeddings.Add(artistEmbedding);
        await _context.SaveChangesAsync();

        return artistEmbedding;
    }

    async Task<ArtistEmbedding> IArtistEmbeddingRepository.UpdateAsync(ArtistEmbedding artistEmbedding)
    {
        artistEmbedding.UpdatedAt = DateTime.UtcNow;

        _context.ArtistEmbeddings.Update(artistEmbedding);
        await _context.SaveChangesAsync();

        return artistEmbedding;
    }

    async Task<bool> IArtistEmbeddingRepository.DeleteAsync(Guid id)
    {
        var artist = await _context.ArtistEmbeddings.FindAsync(id);
        if (artist == null)
        {
            return false;
        }

        _context.ArtistEmbeddings.Remove(artist);
        await _context.SaveChangesAsync();

        return true;
    }

    async Task<bool> IArtistEmbeddingRepository.ExistsAsync(string artistName)
    {
        return await _context.ArtistEmbeddings
            .AnyAsync(a => EF.Functions.ILike(a.ArtistName, artistName));
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.CreateBulkAsync(List<ArtistEmbedding> artistEmbeddings)
    {
        var now = DateTime.UtcNow;
        foreach (var embedding in artistEmbeddings)
        {
            embedding.CreatedAt = now;
            embedding.UpdatedAt = now;
        }

        _context.ArtistEmbeddings.AddRange(artistEmbeddings);
        await _context.SaveChangesAsync();

        return artistEmbeddings;
    }

    async Task<List<ArtistEmbedding>> IArtistEmbeddingRepository.GetAllAsync(int skip, int take)
    {
        return await _context.ArtistEmbeddings
            .OrderBy(a => a.ArtistName)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    async Task<int> IArtistEmbeddingRepository.GetCountAsync()
    {
        return await _context.ArtistEmbeddings.CountAsync();
    }
}
