using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class ProfileMyList
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ProfileId { get; set; } = string.Empty;
        [Required]
        public string ContentId { get; set; } = string.Empty;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey("ProfileId")]
        public virtual Profile? Profile { get; set; }
    }
}