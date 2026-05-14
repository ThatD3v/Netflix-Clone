using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class User: IdentityUser
    {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsPhoneNumber {  get; set; }
    }
    public class UserSubscription
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string PlanId { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? PaystackSubscriptionCode { get; set; }
        public string? PaystackCustomerCode { get; set; }
        public string? PaystackAuthorizationCode { get; set; }
        public string? PaystackEmailToken { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("PlanId")]
        public virtual SubscriptionPlan? Plan { get; set; }
    }
}