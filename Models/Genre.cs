using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class Genre
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<ContentGenre> ContentGenres { get; set; } = new List<ContentGenre>();
    }
}