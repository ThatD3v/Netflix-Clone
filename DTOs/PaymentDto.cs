using System.Text.Json.Serialization;

namespace NetflixClone.DTOs
{
    public class InitializePaymentRequest
    {
        public string PlanId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class InitializePaymentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AuthorizationUrl { get; set; }
        public string? Reference { get; set; }
        public string? AccessCode { get; set; }
    }
    public class PaystackVerifyResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
        [JsonPropertyName("data")]
        public PaystackVerifyData? Data { get; set; }
    }
    public class PaystackVerifyData
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;
        [JsonPropertyName("reference")]
        public string Reference { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("channel")]
        public string Channel { get; set; } = string.Empty;
        [JsonPropertyName("authorization")]
        public PaystackAuthorization? Authorization { get; set; }
        [JsonPropertyName("customer")]
        public PaystackCustomer? Customer { get; set; }
        [JsonPropertyName("paid_at")]
        public DateTime? PaidAt { get; set; }
        [JsonPropertyName("metadata")]
        public PaystackMetadata? Metadata { get; set; }
    }
    public class PaystackMetadata
    {
        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; } = string.Empty;
        [JsonPropertyName("user_id")]
        public string UserId { get; set; } = string.Empty;
        [JsonPropertyName("plan_name")]
        public string PlanName { get; set; } = string.Empty;
    }
    public class PaystackAuthorization
    {
        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; } = string.Empty;
        [JsonPropertyName("card_type")]
        public string CardType { get; set; } = string.Empty;
        [JsonPropertyName("last4")]
        public string Last4 { get; set; } = string.Empty;
        [JsonPropertyName("bank")]
        public string Bank { get; set; } = string.Empty;
    }
    public class PaystackCustomer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("customer_code")]
        public string CustomerCode { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
    public class VerifyPaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public SubscriptionResponse? Subscription { get; set; }
    }
}