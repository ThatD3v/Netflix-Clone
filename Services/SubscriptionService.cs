using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetflixClone.Services;

public class SubscriptionService(ApplicationDbContext context, ILogger<SubscriptionService> logger) : ISubscriptionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<SubscriptionService> _logger = logger;

    public async Task<List<PlanResponse>> GetAllPlansAsync()
    {
        var plans = await _context.SubscriptionPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .ToListAsync();

        return plans.Select(p => new PlanResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            VideoQuality = p.VideoQuality,
            MaxScreens = p.MaxScreens,
            DownloadDevices = p.DownloadDevices,
            IsPopular = p.IsPopular
        }).ToList();
    }

    public async Task<SubscriptionResponse?> GetUserSubscriptionAsync(string userId)
    {
        var subscription = await _context.UserSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId && s.Status == "active")
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null || subscription.Plan == null)
            return null;

        var isActive = subscription.EndDate == null || subscription.EndDate > DateTime.UtcNow;

        return new SubscriptionResponse
        {
            Id = subscription.Id,
            PlanName = subscription.Plan.Name,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            IsActive = isActive,
            RemainingTrialDays = subscription.TrialEndDate.HasValue
                ? Math.Max(0, (subscription.TrialEndDate.Value - DateTime.UtcNow).Days)
                : 0
        };
    }

    public async Task<bool> CancelSubscriptionAsync(string userId)
    {
        var subscription = await _context.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active");

        if (subscription == null)
            return false;

        subscription.Status = "canceled";
        subscription.EndDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> HasActiveSubscriptionAsync(string userId)
    {
        var subscription = await GetUserSubscriptionAsync(userId);
        return subscription?.IsActive == true;
    }

    public async Task<PlanResponse?> GetPlanByIdAsync(string planId)
    {
        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == planId && p.IsActive);

        if (plan == null) return null;

        return new PlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            VideoQuality = plan.VideoQuality,
            MaxScreens = plan.MaxScreens,
            DownloadDevices = plan.DownloadDevices,
            IsPopular = plan.IsPopular
        };
    }

    public async Task<bool> ChangePlanAsync(string userId, string newPlanId)
    {
        var subscription = await _context.UserSubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active");

        if (subscription == null)
            throw new Exception("No active subscription found");

        var newPlan = await _context.SubscriptionPlans.FindAsync(newPlanId);
        if (newPlan == null)
            throw new Exception("New plan not found");

        subscription.PlanId = newPlanId;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Plan changed for user {UserId} to {PlanName}", userId, newPlan.Name);
        return true;
    }

    public async Task<List<PaymentHistoryResponse>> GetPaymentHistoryAsync(string userId)
    {
        var payments = await _context.PaymentHistory
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new PaymentHistoryResponse
            {
                Id = p.Id,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status,
                PaymentMethod = p.PaymentMethod,
                Description = p.Description,
                PaymentDate = p.PaymentDate
            })
            .ToListAsync();

        return payments;
    }

    public async Task<bool> CanAccessFeatureAsync(string userId, string videoQuality, int screens)
    {
        var subscription = await _context.UserSubscriptions
            .Include(s => s.Plan)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Status == "active");

        if (subscription?.Plan == null || subscription.Status != "active")
            return false;

        var isActive = subscription.EndDate == null || subscription.EndDate > DateTime.UtcNow;
        if (!isActive) return false;

        if (screens > subscription.Plan.MaxScreens)
            return false;

        Dictionary<string, int> qualityLevels = new(StringComparer.OrdinalIgnoreCase)
        {
            { "480p", 1 },
            { "720p", 2 },
            { "1080p", 3 },
            { "4K", 4 },
            { "4K+HDR", 4 }
        };

        var requestedLevel = qualityLevels.GetValueOrDefault(videoQuality, 1);
        var allowedLevel = qualityLevels.GetValueOrDefault(subscription.Plan.VideoQuality, 1);

        return requestedLevel <= allowedLevel;
    }

    public async Task<SubscriptionResponse> CreateSubscriptionAsync(string userId, string planId, bool useTrial = true)
    {
        var plan = await _context.SubscriptionPlans.FindAsync(planId);
        if (plan == null)
            throw new Exception("Plan not found");

        var existingSubscriptions = await _context.UserSubscriptions
            .Where(s => s.UserId == userId && s.Status == "active")
            .ToListAsync();

        foreach (var existing in existingSubscriptions)
        {
            existing.Status = "expired";
            existing.EndDate = DateTime.UtcNow;
        }

        var subscription = new UserSubscription
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            PlanId = planId,
            Status = "active",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            TrialEndDate = useTrial ? DateTime.UtcNow.AddDays(7) : null
        };

        _context.UserSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        var payment = new PaymentHistory
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            SubscriptionId = subscription.Id,
            Amount = useTrial ? 0 : plan.Price, 
            Currency = "NGN",
            Status = "success",
            PaymentMethod = useTrial ? "trial" : "card",
            Description = useTrial ? $"7-day trial for {plan.Name} plan" : $"Subscription to {plan.Name} plan"
        };

        _context.PaymentHistory.Add(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Subscription created for user {UserId}. Plan: {PlanName}, Trial: {UseTrial}",
            userId, plan.Name, useTrial);

        var result = await GetUserSubscriptionAsync(userId);
        return result ?? throw new Exception("Failed to create subscription");
    }
}