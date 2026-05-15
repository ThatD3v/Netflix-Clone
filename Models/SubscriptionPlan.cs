using System.ComponentModel.DataAnnotations;

namespace NetflixClone.Models
{
    public class SubscriptionPlan
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "NGN";
        public string BillingPeriod { get; set; } = "monthly";
        public string VideoQuality { get; set; } = string.Empty;
        public int MaxScreens { get; set; } = 1;
        public int MaxProfiles { get; set; } = 1;
        public int DownloadDevices { get; set; } = 1;
        public bool IsPopular { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}