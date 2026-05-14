using System;
using System.Collections.Generic;
namespace NetflixClone.DTOs
{
    public class HeroBannerDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BannerUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public bool IsSeries { get; set; }
        public int? ReleaseYear { get; set; }
        public string? MaturityRating { get; set; }
    }

    public class ContentRowDto
    {
        public string Title { get; set; } = string.Empty;
        public string? RowId { get; set; }
        public List<ContentCardDto> Items { get; set; } = new();
    }

    public class ContentCardDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? MaturityRating { get; set; }
        public bool IsSeries { get; set; }
        public double? AverageRating { get; set; }
        public int? ReleaseYear { get; set; }
        public int? ProgressPercentage { get; set; }
        public string? EpisodeTitle { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
    }

    public class HomePageResponse
    {
        public HeroBannerDto? HeroBanner { get; set; }
        public List<ContentRowDto> Rows { get; set; } = new();
    }

    public class ContinueWatchingDto
    {
        public string Id { get; set; } = string.Empty;
        public string ContentId { get; set; } = string.Empty;
        public string? EpisodeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? PosterUrl { get; set; }
        public int ProgressSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public int ProgressPercentage { get; set; }
        public bool IsSeries { get; set; }
        public string? EpisodeTitle { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
    }
}