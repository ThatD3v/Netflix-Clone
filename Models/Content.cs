using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class Content
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ReleaseYear { get; set; }
        public int? Duration { get; set; } 
        public string? MaturityRating { get; set; } 
        public string? MaturityLevel { get; set; } 
        public string? PosterUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? TrailerUrl { get; set; }
        public bool IsSeries { get; set; } = false; 
        public bool IsActive { get; set; } = true;
        public long ViewCount { get; set; } = 0;
        public double? AverageRating { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? VideoUrl4K { get; set; } 
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        public virtual ICollection<ContentGenre> ContentGenres { get; set; } = new List<ContentGenre>();
        public virtual ICollection<Season> Seasons { get; set; } = new List<Season>();
        public virtual ICollection<Cast> Cast { get; set; } = new List<Cast>();
    }
}