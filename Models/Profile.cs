namespace NetflixClone.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Name { get; set; }
        public bool IskidProfile { get; set; }
        public string? AvaterUrl { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public string? User User { get; set; }

    }
}
