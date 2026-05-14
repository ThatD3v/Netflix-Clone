namespace NetflixClone.Models
{
   

    public class Favorite
    {
        public int Id { get; set; }

        public int ProfileId { get; set; } // Each favorite belongs to a profile

        public int ContentId { get; set; } // Movie or Episode

        public string? ContentType { get; set; } // "Movie" or "Episode"

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public Profile? Profile { get; set; }
    }
}
