using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class Season
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ContentId { get; set; } = string.Empty;
        [Required]
        public int SeasonNumber { get; set; }
        [MaxLength(200)]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PosterUrl { get; set; }
        public int? EpisodeCount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        [ForeignKey("ContentId")]
        public virtual Content? Content { get; set; }
        public virtual ICollection<Episode> Episodes { get; set; } = new List<Episode>();
    }
}