using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;

namespace NetflixClone.Services;

public class StreamingService : IStreamingService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StreamingService> _logger;

    public StreamingService(
        ApplicationDbContext context,
        IConfiguration configuration,
        ILogger<StreamingService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<PlayResponse> GetPlayResponseAsync(string contentId, string profileId, string? episodeId = null, string? quality = null)
    {
        try
        {
            var content = await _context.Contents.FirstOrDefaultAsync(c => c.Id == contentId);
            if (content == null)
                return new PlayResponse { Success = false, Message = "Content not found" };

            string? videoUrl = null;
            string? title = content.Title;
            int? durationSeconds = null;
            int? currentProgress = null;
            bool isSeries = content.IsSeries;
            var availableQualities = new List<QualityOption>();

            if (!isSeries)
            {
                availableQualities = GetMovieQualities(content);
                videoUrl = GetVideoUrlByQuality(content, quality);
                durationSeconds = content.Duration * 60;

                var progress = await _context.ProfileWatchHistories
                    .FirstOrDefaultAsync(w => w.ProfileId == profileId && w.ContentId == contentId && w.EpisodeId == null);
                if (progress != null)
                    currentProgress = progress.ProgressSeconds;
            }
            else
            {
                if (string.IsNullOrEmpty(episodeId))
                    return new PlayResponse { Success = false, Message = "Episode ID required for series", IsSeries = true, ContentId = contentId };

                var episode = await _context.Episodes
                    .Include(e => e.Season)
                    .FirstOrDefaultAsync(e => e.Id == episodeId);

                if (episode == null)
                    return new PlayResponse { Success = false, Message = "Episode not found" };

                availableQualities = GetEpisodeQualities(episode);
                videoUrl = GetEpisodeVideoUrlByQuality(episode, quality);
                durationSeconds = episode.Duration * 60;
                title = $"{content.Title} - S{episode.Season?.SeasonNumber}:E{episode.EpisodeNumber} - {episode.Title}";

                var progress = await _context.ProfileWatchHistories
                    .FirstOrDefaultAsync(w => w.ProfileId == profileId && w.EpisodeId == episodeId);
                if (progress != null)
                    currentProgress = progress.ProgressSeconds;
            }

            if (string.IsNullOrEmpty(videoUrl))
            {
                var bestQuality = availableQualities.OrderByDescending(q => GetQualityRank(q.Quality)).FirstOrDefault();
                if (bestQuality != null)
                    videoUrl = bestQuality.Url;
            }

            return new PlayResponse
            {
                Success = true,
                StreamingUrl = videoUrl,
                VideoQuality = quality ?? GetDefaultQuality(availableQualities),
                CurrentProgress = currentProgress ?? 0,
                DurationSeconds = durationSeconds,
                Title = title,
                IsSeries = isSeries,
                ContentId = contentId,
                EpisodeId = episodeId,
                AvailableQualities = availableQualities
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting play response");
            return new PlayResponse { Success = false, Message = ex.Message };
        }
    }

    public async Task UpdateProgressAsync(string profileId, string contentId, int progressSeconds, int durationSeconds, string? episodeId = null)
    {
        try
        {
            var watchHistory = await _context.ProfileWatchHistories
                .FirstOrDefaultAsync(w => w.ProfileId == profileId &&
                                          w.ContentId == contentId &&
                                          w.EpisodeId == episodeId);

            if (watchHistory == null)
            {
                watchHistory = new ProfileWatchHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    ProfileId = profileId,
                    ContentId = contentId,
                    EpisodeId = episodeId,
                    ProgressSeconds = progressSeconds,
                    DurationSeconds = durationSeconds,
                    IsCompleted = progressSeconds >= durationSeconds - 30,
                    LastWatchedAt = DateTime.UtcNow
                };
                _context.ProfileWatchHistories.Add(watchHistory);
            }
            else
            {
                watchHistory.ProgressSeconds = progressSeconds;
                watchHistory.DurationSeconds = durationSeconds;
                watchHistory.IsCompleted = progressSeconds >= durationSeconds - 30;
                watchHistory.LastWatchedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Progress updated for profile {profileId}: {progressSeconds}/{durationSeconds}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating watch progress");
        }
    }

    public async Task<List<ContinueWatchingDto>> GetContinueWatchingAsync(string profileId)
    {
        var watchHistory = await _context.ProfileWatchHistories
            .Where(w => w.ProfileId == profileId && !w.IsCompleted)
            .OrderByDescending(w => w.LastWatchedAt)
            .Take(20)
            .ToListAsync();

        var result = new List<ContinueWatchingDto>();

        foreach (var history in watchHistory)
        {
            if (string.IsNullOrEmpty(history.EpisodeId))
            {
                var movie = await _context.Contents.FindAsync(history.ContentId);
                if (movie != null)
                {
                    result.Add(new ContinueWatchingDto
                    {
                        Id = history.Id,
                        ContentId = movie.Id,
                        Title = movie.Title ?? "Unknown",
                        PosterUrl = movie.PosterUrl,
                        ProgressSeconds = history.ProgressSeconds,
                        DurationSeconds = history.DurationSeconds,
                        ProgressPercentage = history.DurationSeconds > 0
                            ? (history.ProgressSeconds * 100 / history.DurationSeconds) : 0,
                        IsSeries = false
                    });
                }
            }
            else
            {
                var episode = await _context.Episodes
                    .Include(e => e.Season)
                    .ThenInclude(s => s!.Content)
                    .FirstOrDefaultAsync(e => e.Id == history.EpisodeId);

                if (episode?.Season?.Content != null)
                {
                    result.Add(new ContinueWatchingDto
                    {
                        Id = history.Id,
                        ContentId = episode.Season.Content.Id,
                        EpisodeId = episode.Id,
                        Title = episode.Season.Content.Title ?? "Unknown",

                        EpisodeTitle = episode.Season != null
                            ? $"S{episode.Season.SeasonNumber}:E{episode.EpisodeNumber} - {episode.Title}"
                            : episode.Title,
                        SeasonNumber = episode.Season?.SeasonNumber,
                        EpisodeNumber = episode.EpisodeNumber,
                        PosterUrl = episode.ThumbnailUrl ?? episode.Season?.Content.PosterUrl,
                        ProgressSeconds = history.ProgressSeconds,
                        DurationSeconds = history.DurationSeconds,
                        ProgressPercentage = history.DurationSeconds > 0
                            ? (history.ProgressSeconds * 100 / history.DurationSeconds) : 0,
                        IsSeries = true
                    });
                }
            }
        }

        return result;
    }

    public async Task<NetflixClone.DTOs.StreamingManifestResponse> GetManifestAsync(string contentId, string? episodeId = null)
    {
        var response = new NetflixClone.DTOs.StreamingManifestResponse();

        if (string.IsNullOrEmpty(episodeId))
        {
            var movie = await _context.Contents.FindAsync(contentId);
            if (movie == null)
            {
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.ContentId = movie.Id;
            response.MasterManifestUrl = movie.VideoUrlMaster ?? string.Empty;
            response.Qualities = GetMovieQualities(movie);
        }
        else
        {
            var episode = await _context.Episodes.FindAsync(episodeId);
            if (episode == null)
            {
                response.Success = false;
                return response;
            }

            response.Success = true;
            response.ContentId = contentId;
            response.EpisodeId = episodeId;
            response.MasterManifestUrl = episode.VideoUrlMaster ?? string.Empty;
            response.Qualities = GetEpisodeQualities(episode);
        }

        return response;
    }

    private List<QualityOption> GetMovieQualities(Content content)
    {
        var qualities = new List<QualityOption>();

        if (!string.IsNullOrEmpty(content.VideoUrl4K))
            qualities.Add(new QualityOption { Quality = "4K", Url = content.VideoUrl4K, Bitrate = 16000, Label = "4K Ultra HD" });
        if (!string.IsNullOrEmpty(content.VideoUrl1080p))
            qualities.Add(new QualityOption { Quality = "1080p", Url = content.VideoUrl1080p, Bitrate = 5000, Label = "Full HD" });
        if (!string.IsNullOrEmpty(content.VideoUrl720p))
            qualities.Add(new QualityOption { Quality = "720p", Url = content.VideoUrl720p, Bitrate = 2500, Label = "HD" });
        if (!string.IsNullOrEmpty(content.VideoUrl480p))
            qualities.Add(new QualityOption { Quality = "480p", Url = content.VideoUrl480p, Bitrate = 1000, Label = "SD" });

        return qualities;
    }

    private List<QualityOption> GetEpisodeQualities(Episode episode)
    {
        var qualities = new List<QualityOption>();

        if (!string.IsNullOrEmpty(episode.VideoUrl4K))
            qualities.Add(new QualityOption { Quality = "4K", Url = episode.VideoUrl4K, Bitrate = 16000, Label = "4K Ultra HD" });
        if (!string.IsNullOrEmpty(episode.VideoUrl1080p))
            qualities.Add(new QualityOption { Quality = "1080p", Url = episode.VideoUrl1080p, Bitrate = 5000, Label = "Full HD" });
        if (!string.IsNullOrEmpty(episode.VideoUrl720p))
            qualities.Add(new QualityOption { Quality = "720p", Url = episode.VideoUrl720p, Bitrate = 2500, Label = "HD" });
        if (!string.IsNullOrEmpty(episode.VideoUrl480p))
            qualities.Add(new QualityOption { Quality = "480p", Url = episode.VideoUrl480p, Bitrate = 1000, Label = "SD" });

        return qualities;
    }

    private string? GetVideoUrlByQuality(Content content, string? quality)
    {
        return quality?.ToLower() switch
        {
            "4k" => content.VideoUrl4K ?? content.VideoUrl1080p,
            "1080p" => content.VideoUrl1080p ?? content.VideoUrl720p,
            "720p" => content.VideoUrl720p ?? content.VideoUrl480p,
            "480p" => content.VideoUrl480p,
            "master" or "hls" => content.VideoUrlMaster,
            _ => content.VideoUrl1080p ?? content.VideoUrl720p ?? content.VideoUrl480p ?? content.VideoUrl4K
        };
    }

    private string? GetEpisodeVideoUrlByQuality(Episode episode, string? quality)
    {
        return quality?.ToLower() switch
        {
            "4k" => episode.VideoUrl4K ?? episode.VideoUrl1080p,
            "1080p" => episode.VideoUrl1080p ?? episode.VideoUrl720p,
            "720p" => episode.VideoUrl720p ?? episode.VideoUrl480p,
            "480p" => episode.VideoUrl480p,
            "master" or "hls" => episode.VideoUrlMaster,
            _ => episode.VideoUrl1080p ?? episode.VideoUrl720p ?? episode.VideoUrl480p ?? episode.VideoUrl4K
        };
    }

    private int GetQualityRank(string quality)
    {
        return quality?.ToLower() switch
        {
            "4k" => 4,
            "1080p" => 3,
            "720p" => 2,
            "480p" => 1,
            _ => 0
        };
    }

    private string GetDefaultQuality(List<QualityOption> qualities)
    {
        if (qualities.Any(q => q.Quality == "1080p"))
            return "1080p";
        if (qualities.Any(q => q.Quality == "720p"))
            return "720p";
        if (qualities.Any(q => q.Quality == "480p"))
            return "480p";
        return qualities.FirstOrDefault()?.Quality ?? "Auto";
    }
}