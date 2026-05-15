using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetflixClone.Models
{
    public class PaymentHistory
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string UserId { get; set; } = string.Empty;
        public string? SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "NGN";
        public string Status { get; set; } = "pending";
        public string? PaymentMethod { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        [ForeignKey("SubscriptionId")]
        public virtual UserSubscription? Subscription { get; set; }
    }
}