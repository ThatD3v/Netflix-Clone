using NetflixClone.Models;

public class WatchHistory
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public int ContentId { get; set; } 

    public string? ContentType { get; set; }

    public double Progress { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime WatchedAt { get; set; } = DateTime.Now;

    public Profile? Profile { get; set; }
}