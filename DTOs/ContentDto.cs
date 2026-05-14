using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class CreateContentRequest
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? MaturityRating { get; set; }
        public string? MaturityLevel { get; set; }
        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public bool IsSeries { get; set; }
        public string? VideoUrl4K { get; set; }
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public List<string>? GenreIds { get; set; }
        public List<CastRequest>? Cast { get; set; }
    }
    public class UpdateContentRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? MaturityRating { get; set; }
        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public bool? IsActive { get; set; }
        public string? VideoUrl4K { get; set; }
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public List<string>? GenreIds { get; set; }
    }
    public class CastRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? CharacterName { get; set; }
        public string? ProfileUrl { get; set; }
    }
    public class CreateSeasonRequest
    {
        [Required]
        public int SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
    public class CreateEpisodeRequest
    {
        [Required]
        public int EpisodeNumber { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl4K { get; set; }
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
    public class ContentResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? Duration { get; set; }
        public string? MaturityRating { get; set; }
        public string? MaturityLevel { get; set; }
        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public bool IsSeries { get; set; }
        public bool IsActive { get; set; }
        public string? VideoUrl4K { get; set; }
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public double? AverageRating { get; set; }
        public List<string>? Genres { get; set; }
        public List<CastResponse>? Cast { get; set; }
        public List<SeasonResponse>? Seasons { get; set; }
    }

    public class CastResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? CharacterName { get; set; }
        public string? ProfileUrl { get; set; }
    }
    public class SeasonResponse
    {
        public string Id { get; set; } = string.Empty;
        public int SeasonNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public int? EpisodeCount { get; set; }
        public List<EpisodeResponse>? Episodes { get; set; }
    }

    public class EpisodeResponse
    {
        public string Id { get; set; } = string.Empty;
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Duration { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl4K { get; set; }
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
    public class GenreResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
    public class CreateGenreRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}