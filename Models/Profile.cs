using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class Profile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsKidsProfile { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}