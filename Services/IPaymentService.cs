using NetflixClone.DTOs;

namespace NetflixClone.Services
{
    public interface IPaymentService
    {
        Task<InitializePaymentResponse> InitializePaymentAsync(string userId, string planId, string email);
        Task<VerifyPaymentResponse> VerifyPaymentAsync(string reference);
        Task<WebhookResponse> HandleWebhookAsync(string jsonPayload, string signature); 
    }
}