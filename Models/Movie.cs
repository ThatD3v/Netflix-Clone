using System.ComponentModel.DataAnnotations;
namespace NetflixClone.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Genre { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int ReleaseYear { get; set; }
    }
}