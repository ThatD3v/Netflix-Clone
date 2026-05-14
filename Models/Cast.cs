using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class Cast
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ContentId { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Role { get; set; } 
        public string? CharacterName { get; set; } 
        public string? ProfileUrl { get; set; }
        [ForeignKey("ContentId")]
        public virtual Content? Content { get; set; }
    }
}