using NetflixClone.Models;

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.Now;
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; } = true;

    public Plan? Plan { get; set; }
    public User? User { get; set; }
}