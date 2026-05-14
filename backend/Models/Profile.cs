namespace NetflixClone.Models;

public class Profile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? Name { get; set; }

    public bool IsKidProfile { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public User? User { get; set; }
}