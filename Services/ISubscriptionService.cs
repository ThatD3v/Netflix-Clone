using NetflixClone.DTOs;

namespace NetflixClone.Services;

public interface ISubscriptionService
{
    Task<List<PlanResponse>> GetAllPlansAsync();

    Task<SubscriptionResponse?> GetUserSubscriptionAsync(string userId);

    Task<bool> CancelSubscriptionAsync(string userId);

    Task<bool> HasActiveSubscriptionAsync(string userId);

    Task<PlanResponse?> GetPlanByIdAsync(string planId);

    Task<bool> ChangePlanAsync(string userId, string newPlanId);

    Task<List<PaymentHistoryResponse>> GetPaymentHistoryAsync(string userId);

    Task<bool> CanAccessFeatureAsync(string userId, string videoQuality, int screens);

    Task<SubscriptionResponse> CreateSubscriptionAsync(string userId, string planId, bool useTrial = true);
}