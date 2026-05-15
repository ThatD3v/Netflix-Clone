using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;

namespace NetflixClone.Services;

public class ContentService : IContentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ContentService> _logger;

    public ContentService(ApplicationDbContext context, ILogger<ContentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ContentResponse> CreateContentAsync(CreateContentRequest request)
    {
        var content = new Content
        {
            Id = Guid.NewGuid().ToString(),
            Title = request.Title,
            Description = request.Description,
            ReleaseYear = request.ReleaseYear,
            Duration = request.Duration,
            MaturityRating = request.MaturityRating,
            MaturityLevel = request.MaturityLevel,
            PosterUrl = request.PosterUrl,
            BannerUrl = request.BannerUrl,
            TrailerUrl = request.TrailerUrl,
            IsSeries = request.IsSeries,
            IsActive = true,
            VideoUrl4K = request.VideoUrl4K,
            VideoUrl1080p = request.VideoUrl1080p,
            VideoUrl720p = request.VideoUrl720p,
            VideoUrl480p = request.VideoUrl480p,
            VideoUrlMaster = request.VideoUrlMaster
        };

        _context.Contents.Add(content);

        if (request.GenreIds != null && request.GenreIds.Any())
        {
            foreach (var genreId in request.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    _context.ContentGenres.Add(new ContentGenre
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContentId = content.Id,
                        GenreId = genreId
                    });
                }
            }
        }

        if (request.Cast != null && request.Cast.Any())
        {
            foreach (var castMember in request.Cast)
            {
                _context.Casts.Add(new Cast
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentId = content.Id,
                    Name = castMember.Name,
                    Role = castMember.Role,
                    CharacterName = castMember.CharacterName,
                    ProfileUrl = castMember.ProfileUrl
                });
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Content created: {content.Title} (ID: {content.Id})");

        return await GetContentByIdAsync(content.Id) ?? throw new Exception("Failed to create content");
    }

    public async Task<ContentResponse> UpdateContentAsync(string contentId, UpdateContentRequest request)
    {
        var content = await _context.Contents
            .Include(c => c.ContentGenres)
            .FirstOrDefaultAsync(c => c.Id == contentId);

        if (content == null)
            throw new Exception("Content not found");

        if (request.Title != null) content.Title = request.Title;
        if (request.Description != null) content.Description = request.Description;
        if (request.ReleaseYear.HasValue) content.ReleaseYear = request.ReleaseYear;
        if (request.Duration.HasValue) content.Duration = request.Duration;
        if (request.MaturityRating != null) content.MaturityRating = request.MaturityRating;
        if (request.PosterUrl != null) content.PosterUrl = request.PosterUrl;
        if (request.BannerUrl != null) content.BannerUrl = request.BannerUrl;
        if (request.TrailerUrl != null) content.TrailerUrl = request.TrailerUrl;
        if (request.IsActive.HasValue) content.IsActive = request.IsActive.Value;

        if (request.VideoUrl4K != null) content.VideoUrl4K = request.VideoUrl4K;
        if (request.VideoUrl1080p != null) content.VideoUrl1080p = request.VideoUrl1080p;
        if (request.VideoUrl720p != null) content.VideoUrl720p = request.VideoUrl720p;
        if (request.VideoUrl480p != null) content.VideoUrl480p = request.VideoUrl480p;
        if (request.VideoUrlMaster != null) content.VideoUrlMaster = request.VideoUrlMaster;

        content.UpdatedAt = DateTime.UtcNow;

        if (request.GenreIds != null)
        {
            var existingGenres = _context.ContentGenres.Where(cg => cg.ContentId == contentId);
            _context.ContentGenres.RemoveRange(existingGenres);

            foreach (var genreId in request.GenreIds)
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre != null)
                {
                    _context.ContentGenres.Add(new ContentGenre
                    {
                        Id = Guid.NewGuid().ToString(),
                        ContentId = contentId,
                        GenreId = genreId
                    });
                }
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Content updated: {content.Title}");

        return await GetContentByIdAsync(contentId) ?? throw new Exception("Failed to update content");
    }

    public async Task<bool> DeleteContentAsync(string contentId)
    {
        var content = await _context.Contents.FindAsync(contentId);
        if (content == null) return false;

        content.IsActive = false;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Content deleted: {content.Title}");

        return true;
    }

    public async Task<ContentResponse?> GetContentByIdAsync(string contentId)
    {
        var content = await _context.Contents
            .Include(c => c.ContentGenres)
                .ThenInclude(cg => cg.Genre)
            .Include(c => c.Cast)
            .Include(c => c.Seasons)
                .ThenInclude(s => s.Episodes)
            .FirstOrDefaultAsync(c => c.Id == contentId);

        if (content == null) return null;
        return MapToContentResponse(content);
    }
    public async Task<List<ContentResponse>> GetAllContentAsync(int page = 1, int pageSize = 20)
    {
        var contents = await _context.Contents
            .Include(c => c.ContentGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.Cast)
            .Include(c => c.Seasons).ThenInclude(s => s.Episodes)
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return contents.Select(MapToContentResponse).ToList();
    }
    public async Task<List<ContentResponse>> GetMoviesAsync(int page = 1, int pageSize = 20)
    {
        var contents = await _context.Contents
            .Include(c => c.ContentGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.Cast)
            .Include(c => c.Seasons).ThenInclude(s => s.Episodes)
            .Where(c => c.IsActive && !c.IsSeries)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return contents.Select(MapToContentResponse).ToList();
    }
    public async Task<List<ContentResponse>> GetSeriesAsync(int page = 1, int pageSize = 20)
    {
        var contents = await _context.Contents
            .Include(c => c.ContentGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.Cast)
            .Include(c => c.Seasons).ThenInclude(s => s.Episodes)
            .Where(c => c.IsActive && c.IsSeries)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return contents.Select(MapToContentResponse).ToList();
    }

    public async Task<SeasonResponse> AddSeasonAsync(string contentId, CreateSeasonRequest request)
    {
        var content = await _context.Contents.FindAsync(contentId);
        if (content == null)
            throw new Exception("Content not found");

        if (!content.IsSeries)
            throw new Exception("Cannot add season to a movie");

        var season = new Season
        {
            Id = Guid.NewGuid().ToString(),
            ContentId = contentId,
            SeasonNumber = request.SeasonNumber,
            Title = request.Title,
            Description = request.Description,
            PosterUrl = request.PosterUrl,
            ReleaseDate = request.ReleaseDate
        };

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Season {request.SeasonNumber} added to {content.Title}");

        return new SeasonResponse
        {
            Id = season.Id,
            SeasonNumber = season.SeasonNumber,
            Title = season.Title,
            Description = season.Description,
            PosterUrl = season.PosterUrl,
            EpisodeCount = 0,
            Episodes = new List<EpisodeResponse>()
        };
    }

    public async Task<EpisodeResponse> AddEpisodeAsync(string seasonId, CreateEpisodeRequest request)
    {
        var season = await _context.Seasons.FindAsync(seasonId);
        if (season == null)
            throw new Exception("Season not found");

        var episode = new Episode
        {
            Id = Guid.NewGuid().ToString(),
            SeasonId = seasonId,
            EpisodeNumber = request.EpisodeNumber,
            Title = request.Title,
            Description = request.Description,
            Duration = request.Duration,
            ThumbnailUrl = request.ThumbnailUrl,
            ReleaseDate = request.ReleaseDate,
            VideoUrl4K = request.VideoUrl4K,
            VideoUrl1080p = request.VideoUrl1080p,
            VideoUrl720p = request.VideoUrl720p,
            VideoUrl480p = request.VideoUrl480p,
            VideoUrlMaster = request.VideoUrlMaster
        };

        _context.Episodes.Add(episode);
        season.EpisodeCount = (season.EpisodeCount ?? 0) + 1;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Episode {request.EpisodeNumber} added to season {season.SeasonNumber}");

        return new EpisodeResponse
        {
            Id = episode.Id,
            EpisodeNumber = episode.EpisodeNumber,
            Title = episode.Title,
            Description = episode.Description,
            Duration = episode.Duration,
            ThumbnailUrl = episode.ThumbnailUrl,
            ReleaseDate = episode.ReleaseDate
        };
    }

    public async Task<List<GenreResponse>> GetAllGenresAsync()
    {
        var genres = await _context.Genres
            .Where(g => g.IsActive)
            .OrderBy(g => g.Name)
            .ToListAsync();

        return genres.Select(g => new GenreResponse
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description
        }).ToList();
    }

    public async Task<GenreResponse> CreateGenreAsync(string name, string? description = null)
    {
        var genre = new Genre
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            Description = description,
            IsActive = true
        };

        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Genre created: {name}");

        return new GenreResponse
        {
            Id = genre.Id,
            Name = genre.Name,
            Description = genre.Description
        };
    }
    public async Task<List<ContentResponse>> SearchContentAsync(string query, int page = 1, int pageSize = 20)
    {
        var contents = await _context.Contents
            .Include(c => c.ContentGenres).ThenInclude(cg => cg.Genre)
            .Include(c => c.Cast)
            .Include(c => c.Seasons).ThenInclude(s => s.Episodes)
            .Where(c => c.IsActive && (c.Title.Contains(query) || (c.Description != null && c.Description.Contains(query))))
            .OrderByDescending(c => c.Title.StartsWith(query) ? 1 : 0)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return contents.Select(MapToContentResponse).ToList();
    }

    public async Task<HomePageResponse> GetHomePageAsync(string profileId)
    {
        var response = new HomePageResponse
        {
            HeroBanner = await GetHeroBannerAsync(),
            Rows = new List<ContentRowDto>()
        };

        var trending = await GetTrendingAsync(10);
        if (trending.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "Trending Now",
                RowId = "trending",
                Items = trending
            });
        }

        var popular = await GetPopularAsync(10);
        if (popular.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "Popular on Netflix",
                RowId = "popular",
                Items = popular
            });
        }

        var continueWatching = await GetContinueWatchingAsync(profileId, 10);
        if (continueWatching.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "Continue Watching",
                RowId = "continue-watching",
                Items = continueWatching
            });
        }

        var myList = await GetMyListAsync(profileId, 10);
        if (myList.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "My List",
                RowId = "my-list",
                Items = myList
            });
        }

        var newReleases = await GetNewReleasesAsync(10);
        if (newReleases.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "New Releases",
                RowId = "new-releases",
                Items = newReleases
            });
        }

        var recommended = await GetRecommendedAsync(profileId, 10);
        if (recommended.Any())
        {
            response.Rows.Add(new ContentRowDto
            {
                Title = "Recommended for You",
                RowId = "recommended",
                Items = recommended
            });
        }

        return response;
    }

    public async Task<List<ContentCardDto>> GetTrendingAsync(int limit = 10)
    {
        var trending = await _context.Contents
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.ViewCount)
            .Take(limit)
            .ToListAsync();

        if (!trending.Any()) return new List<ContentCardDto>();

        return MapToCardDtoList(trending);
    }

    public async Task<List<ContentCardDto>> GetPopularAsync(int limit = 10)
    {
        var popular = await _context.Contents
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.AverageRating)
            .Take(limit)
            .ToListAsync();

        if (!popular.Any()) return new List<ContentCardDto>();

        return MapToCardDtoList(popular);
    }

    public async Task<List<ContentCardDto>> GetNewReleasesAsync(int limit = 10)
    {
        var newReleases = await _context.Contents
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.CreatedAt)
            .Take(limit)
            .ToListAsync();

        if (!newReleases.Any()) return new List<ContentCardDto>();

        return MapToCardDtoList(newReleases);
    }

    public async Task<List<ContentCardDto>> GetContinueWatchingAsync(string profileId, int limit = 10)
    {
        var watchHistory = await _context.ProfileWatchHistories
            .Where(w => w.ProfileId == profileId && !w.IsCompleted)
            .OrderByDescending(w => w.LastWatchedAt)
            .Take(limit)
            .ToListAsync();

        var result = new List<ContentCardDto>();

        foreach (var history in watchHistory)
        {
            if (string.IsNullOrEmpty(history.EpisodeId))
            {
                var movie = await _context.Contents.FindAsync(history.ContentId);
                if (movie != null)
                {
                    result.Add(new ContentCardDto
                    {
                        Id = movie.Id,
                        Title = movie.Title ?? "Unknown Title",
                        PosterUrl = movie.PosterUrl,
                        BannerUrl = movie.BannerUrl,
                        MaturityRating = movie.MaturityRating,
                        IsSeries = false,
                        AverageRating = movie.AverageRating,
                        ProgressPercentage = history.DurationSeconds > 0
                            ? (history.ProgressSeconds * 100 / history.DurationSeconds)
                            : 0
                    });
                }
            }
            else
            {
                var episode = await _context.Episodes
                    .Include(e => e.Season)
                    .ThenInclude(s => s.Content)
                    .FirstOrDefaultAsync(e => e.Id == history.EpisodeId);

                if (episode?.Season?.Content != null)
                {
                    result.Add(new ContentCardDto
                    {
                        Id = episode.Season.Content.Id,
                        Title = episode.Season.Content.Title ?? "Unknown Title",
                        PosterUrl = episode.ThumbnailUrl ?? episode.Season.Content.PosterUrl,
                        BannerUrl = episode.Season.Content.BannerUrl,
                        IsSeries = true,
                        ProgressPercentage = history.DurationSeconds > 0
                            ? (history.ProgressSeconds * 100 / history.DurationSeconds)
                            : 0,
                        EpisodeTitle = episode.Title,
                        SeasonNumber = episode.Season.SeasonNumber,
                        EpisodeNumber = episode.EpisodeNumber
                    });
                }
            }
        }

        return result;
    }

    public async Task<List<ContentCardDto>> GetMyListAsync(string profileId, int limit = 10)
    {
        var myList = await _context.ProfileMyLists
            .Where(m => m.ProfileId == profileId)
            .OrderByDescending(m => m.AddedAt)
            .Take(limit)
            .ToListAsync();

        if (!myList.Any()) return new List<ContentCardDto>();

        var contentIds = myList.Select(m => m.ContentId).ToList();
        var contents = await _context.Contents
            .Where(c => contentIds.Contains(c.Id))
            .ToListAsync();

        if (!contents.Any()) return new List<ContentCardDto>();

        return MapToCardDtoList(contents);
    }

    public async Task<List<ContentCardDto>> GetRecommendedAsync(string profileId, int limit = 10)
    {
        var watchHistory = await _context.ProfileWatchHistories
            .Where(w => w.ProfileId == profileId)
            .Take(20)
            .ToListAsync();

        if (!watchHistory.Any())
            return await GetTrendingAsync(limit);

        var contentIds = watchHistory.Select(w => w.ContentId).Distinct().ToList();

        var watchedContents = await _context.Contents
            .Include(c => c.ContentGenres)
            .Where(c => contentIds.Contains(c.Id))
            .ToListAsync();

        if (!watchedContents.Any())
            return await GetTrendingAsync(limit);

        var genreIds = watchedContents
            .SelectMany(c => c.ContentGenres.Select(cg => cg.GenreId))
            .Distinct()
            .Take(5)
            .ToList();

        if (!genreIds.Any())
            return await GetTrendingAsync(limit);

        var recommended = await _context.Contents
            .Include(c => c.ContentGenres)
            .Where(c => c.IsActive &&
                        c.ContentGenres.Any(cg => genreIds.Contains(cg.GenreId)) &&
                        !contentIds.Contains(c.Id))
            .OrderByDescending(c => c.AverageRating)
            .Take(limit)
            .ToListAsync();

        if (!recommended.Any())
            return await GetTrendingAsync(limit);

        return MapToCardDtoList(recommended);
    }

    public async Task<HeroBannerDto?> GetHeroBannerAsync()
    {
        var heroContent = await _context.Contents
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.ViewCount)
            .FirstOrDefaultAsync();

        if (heroContent == null) return null;

        return new HeroBannerDto
        {
            Id = heroContent.Id,
            Title = heroContent.Title ?? "Featured Content",
            Description = heroContent.Description,
            BannerUrl = heroContent.BannerUrl,
            TrailerUrl = heroContent.TrailerUrl,
            IsSeries = heroContent.IsSeries,
            ReleaseYear = heroContent.ReleaseYear,
            MaturityRating = heroContent.MaturityRating
        };
    }

    private List<ContentCardDto> MapToCardDtoList(List<Content> contents)
    {
        var result = new List<ContentCardDto>();

        foreach (var content in contents)
        {
            result.Add(new ContentCardDto
            {
                Id = content.Id,
                Title = content.Title ?? "Unknown Title",
                PosterUrl = content.PosterUrl,
                BannerUrl = content.BannerUrl,
                MaturityRating = content.MaturityRating,
                IsSeries = content.IsSeries,
                AverageRating = content.AverageRating,
                ReleaseYear = content.ReleaseYear
            });
        }

        return result;
    }
    private ContentResponse MapToContentResponse(Content content)
    {
        var genres = content.ContentGenres
            .Where(cg => cg.Genre != null)
            .Select(cg => cg.Genre!.Name)
            .ToList();

        var cast = content.Cast.Select(c => new CastResponse
        {
            Id = c.Id,
            Name = c.Name,
            Role = c.Role,
            CharacterName = c.CharacterName,
            ProfileUrl = c.ProfileUrl
        }).ToList();

        List<SeasonResponse>? seasons = null;
        if (content.IsSeries && content.Seasons != null && content.Seasons.Any())
        {
            seasons = new List<SeasonResponse>();
            foreach (var season in (content.Seasons ?? new List<Season>()).OrderBy(s => s.SeasonNumber))
            {
                var episodes = season.Episodes != null
                    ? season.Episodes.OrderBy(e => e.EpisodeNumber).Select(e => new EpisodeResponse
                    {
                        Id = e.Id,
                        EpisodeNumber = e.EpisodeNumber,
                        Title = e.Title,
                        Description = e.Description,
                        Duration = e.Duration,
                        ThumbnailUrl = e.ThumbnailUrl,
                        ReleaseDate = e.ReleaseDate
                    }).ToList()
                    : new List<EpisodeResponse>();

                seasons.Add(new SeasonResponse
                {
                    Id = season.Id,
                    SeasonNumber = season.SeasonNumber,
                    Title = season.Title,
                    Description = season.Description,
                    PosterUrl = season.PosterUrl,
                    EpisodeCount = episodes.Count,
                    Episodes = episodes
                });
            }
        }

        return new ContentResponse
        {
            Id = content.Id,
            Title = content.Title,
            Description = content.Description,
            ReleaseYear = content.ReleaseYear,
            Duration = content.Duration,
            MaturityRating = content.MaturityRating,
            MaturityLevel = content.MaturityLevel,
            PosterUrl = content.PosterUrl,
            BannerUrl = content.BannerUrl,
            TrailerUrl = content.TrailerUrl,
            IsSeries = content.IsSeries,
            IsActive = content.IsActive,
            AverageRating = content.AverageRating,
            Genres = genres,
            Cast = cast,
            Seasons = seasons
        };
    }
}