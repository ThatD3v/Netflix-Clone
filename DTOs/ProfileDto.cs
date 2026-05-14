using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class CreateProfileRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsKidsProfile { get; set; } = false;
    }
    public class UpdateProfileRequest
    {
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
    }
    public class ProfileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool IsKidsProfile { get; set; }
    }
}
public class UpdateAvatarRequest
{
    public string? AvatarUrl { get; set; }
}