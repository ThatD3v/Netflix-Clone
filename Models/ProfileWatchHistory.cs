using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class ProfileWatchHistory
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ProfileId { get; set; } = string.Empty;
        [Required]
        public string ContentId { get; set; } = string.Empty;
        public string? EpisodeId { get; set; }
        public int ProgressSeconds { get; set; } = 0;
        public int DurationSeconds { get; set; } = 0;
        public bool IsCompleted { get; set; } = false;
        public DateTime LastWatchedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("ProfileId")]
        public virtual Profile? Profile { get; set; }
    }
}