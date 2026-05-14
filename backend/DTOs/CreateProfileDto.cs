namespace NetflixClone.DTOs
{
    public class CreateProfileDto
    {
        public string? Name { get; set; }
        public bool IsKidProfile { get; set; }
        public string? AvatarUrl { get; set; }

    }
}
