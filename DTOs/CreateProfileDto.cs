namespace NetflixClone.DTOs
{
    public class CreateProfileDto
    {
       public int UserId { get; set; }
        public string? Name { get; set; }
        public bool IskidProfile { get; set; }
        public string? AvaterUrl { get; set; }

    }
}
