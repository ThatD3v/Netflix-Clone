namespace NetflixClone.Models
{
    public class Series
    {
        public int Id { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Season> Seasons { get; set; } = new List<Season>();
    }
}
