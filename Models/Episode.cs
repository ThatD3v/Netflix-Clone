using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class Episode
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string SeasonId { get; set; } = string.Empty;
        [Required]
        public int EpisodeNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? Duration { get; set; } 
        public string? ThumbnailUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? VideoUrl4K { get; set; }  
        public string? VideoUrl1080p { get; set; }
        public string? VideoUrl720p { get; set; }
        public string? VideoUrl480p { get; set; }
        public string? VideoUrlMaster { get; set; }
        [ForeignKey("SeasonId")]
        public virtual Season? Season { get; set; }
    }
}