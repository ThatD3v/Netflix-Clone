namespace NetflixClone.DTOs
{
    public class PlayResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? StreamingUrl { get; set; }
        public string? VideoQuality { get; set; }
        public int? CurrentProgress { get; set; }
        public int? DurationSeconds { get; set; }
        public string? Title { get; set; }
        public bool IsSeries { get; set; }
        public string? ContentId { get; set; }
        public string? EpisodeId { get; set; }
        public int? SeasonNumber { get; set; }
        public int? EpisodeNumber { get; set; }
        public List<QualityOption> AvailableQualities { get; set; } = new();
    }
    public class QualityOption
    {
        public string Quality { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public int Bitrate { get; set; } 
        public string? Label { get; set; } 
    }
    public class UpdateProgressRequest
    {
        public string ContentId { get; set; } = string.Empty;
        public string? EpisodeId { get; set; }
        public int ProgressSeconds { get; set; }
        public int DurationSeconds { get; set; }
    }
    public class StreamingManifestResponse
    {
        public bool Success { get; set; }
    public string? Message { get; set; }
    public string ContentId { get; set; } = string.Empty;
    public string? EpisodeId { get; set; }
    public string MasterManifestUrl { get; set; } = string.Empty;
    public List<QualityOption> Qualities { get; set; } = new();
    }
}