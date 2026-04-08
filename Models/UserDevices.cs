public class UserDevices
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DeviceId { get; set; } = null!;
    public string DeviceType { get; set; } = null!; // mobile, tv, tablet, computer
    public bool IsActive { get; set; } = true;
    public DateTime LastActive { get; set; } = DateTime.Now;
}