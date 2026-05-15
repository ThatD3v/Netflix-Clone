namespace NetflixClone.DTOs
{
    public class PlanResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string VideoQuality { get; set; } = string.Empty;
        public int MaxScreens { get; set; }
        public int DownloadDevices { get; set; }
        public bool IsPopular { get; set; }
        public string FormattedPrice => $"₦{Price:N0}/month";
    }
    public class CreateSubscriptionRequest
    {
        public string PlanId { get; set; } = string.Empty;
    }
    public class SubscriptionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public int RemainingTrialDays { get; set; }
    }
    public class PaymentHistoryResponse
    {
        public string Id { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? CardLast4 { get; set; }
        public string? CardBrand { get; set; }
        public string? Description { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class ChangePlanRequest
    {
        public string NewPlanId { get; set; } = string.Empty;
    }
    public class PaystackWebhookPayload
    {
        public string Event { get; set; } = string.Empty;
        public WebhookData Data { get; set; } = new();
    }

    public class WebhookData
    {
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SubscriptionCode { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string AuthorizationCode { get; set; } = string.Empty;
        public string CustomerCode { get; set; } = string.Empty;
        public PaystackMetadata? Metadata { get; set; }
    }
    public class WebhookResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}