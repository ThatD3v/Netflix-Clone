using System.Globalization;

namespace NetflixClone.Models
{
    public class Episode
    {
        public int id { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string VideoUrl { get; set; } = string.Empty;
        public int SeasonId { get; set; }
        public Season? Season { get; set; }
    }
}
