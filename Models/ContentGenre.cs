using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class ContentGenre
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string ContentId { get; set; } = string.Empty;
        [Required]
        public string GenreId { get; set; } = string.Empty;
        [ForeignKey("ContentId")]
        public virtual Content? Content { get; set; }
        [ForeignKey("GenreId")]
        public virtual Genre? Genre { get; set; }
    }
}