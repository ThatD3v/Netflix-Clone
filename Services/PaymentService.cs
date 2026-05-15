using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NetflixClone.Services;

public class PaymentService(
    ApplicationDbContext context,
    ISubscriptionService subscriptionService,
    IConfiguration configuration,
    ILogger<PaymentService> logger) : IPaymentService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<PaymentService> _logger = logger;

    private readonly string _secretKey = configuration["Paystack:SecretKey"] ?? "";
    private readonly HttpClient _httpClient = CreateConfiguredClient(configuration["Paystack:SecretKey"] ?? "");

    private static HttpClient CreateConfiguredClient(string secretKey)
    {
        HttpClient client = new();
        
        client.BaseAddress = new Uri("https://api.paystack.co/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secretKey);
        return client;
    }

    public async Task<InitializePaymentResponse> InitializePaymentAsync(string userId, string planId, string email)
    {
        try
        {
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            if (plan == null)
            {
                return new InitializePaymentResponse
                {
                    Status = false,
                    Message = "Invalid Plan Selected."
                };
            }

            var payload = new
            {
                email = email,
                amount = (int)(plan.Price * 100),
                currency = "NGN",
                callback_url = _configuration["Paystack:CallbackUrl"],
                metadata = new
                {
                    user_id = userId,
                    plan_id = planId,
                    plan_name = plan.Name
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

           
            var response = await _httpClient.PostAsync("transaction/initialize", content);
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;

            if (!root.GetProperty("status").GetBoolean())
            {
                return new InitializePaymentResponse
                {
                    Status = false,
                    Message = "Paystack initialization failed"
                };
            }

            var data = root.GetProperty("data");

            var reference = data.GetProperty("reference").GetString();

            var payment = new PaymentHistory
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Amount = plan.Price,
                Currency = "NGN",
                Status = "pending",
                Reference = reference,
                Description = $"Subscription to {plan.Name}"
            };

            _context.PaymentHistory.Add(payment);
            await _context.SaveChangesAsync();

            return new InitializePaymentResponse
            {
                Status = true,
                Message = "Payment Initialized Successfully",
                AuthorizationUrl = data.GetProperty("authorization_url").GetString(),
                Reference = reference,
                AccessCode = data.GetProperty("access_code").GetString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Initialize Payment Error");
            return new InitializePaymentResponse
            {
                Status = false,
                Message = ex.Message
            };
        }
    }

    public async Task<VerifyPaymentResponse> VerifyPaymentAsync(string reference)
    {
        try
        {
            var payment = await _context.PaymentHistory.FirstOrDefaultAsync(p => p.Reference == reference);
            if (payment == null)
                return new VerifyPaymentResponse { Success = false, Message = "Record Not Found." };

            // ✅ FIX: Use relative path with BaseAddress
            var response = await _httpClient.GetAsync($"transaction/verify/{reference}");
            var responseString = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;

            if (root.GetProperty("status").GetBoolean() &&
                root.GetProperty("data").GetProperty("status").GetString() == "success")
            {
                var data = root.GetProperty("data");
                payment.Status = "success";
                payment.PaymentMethod = data.GetProperty("channel").GetString();
                payment.PaymentDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var planId = data.GetProperty("metadata").GetProperty("plan_id").GetString() ?? "";
                var subscription = await _subscriptionService.CreateSubscriptionAsync(payment.UserId, planId, false);
                return new VerifyPaymentResponse
                {
                    Success = true,
                    Message = "Payment verified and subscription activated",
                    Subscription = subscription
                };
            }

            payment.Status = "failed";
            await _context.SaveChangesAsync();
            return new VerifyPaymentResponse { Success = false, Message = "Verification Failed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Verify Error");
            return new VerifyPaymentResponse { Success = false, Message = "An Error Occurred" };
        }
    }

    public async Task<WebhookResponse> HandleWebhookAsync(string jsonPayload, string signature)
    {
        try
        {
            if (string.IsNullOrEmpty(_secretKey))
                return new WebhookResponse { Success = false, Message = "Config Error" };

            var expectedSignature = ComputeSignature(jsonPayload, _secretKey);
            if (signature != expectedSignature)
            {
                _logger.LogWarning("Invalid Webhook Signature");
                return new WebhookResponse { Success = false, Message = "Invalid Signature" };
            }

            var payload = JsonSerializer.Deserialize<PaystackWebhookPayload>(jsonPayload);
            if (payload == null)
                return new WebhookResponse { Success = false, Message = "Invalid Payload" };

            _logger.LogInformation("Webhook Received: {Event}", payload.Event);

            switch (payload.Event)
            {
                case "charge.success":
                    await HandleChargeSuccessAsync(payload.Data);
                    break;
                case "subscription.create":
                    HandleSubscriptionCreate(payload.Data);
                    break;
                case "subscription.disable":
                    await HandleSubscriptionDisableAsync(payload.Data);
                    break;
                case "subscription.enable":
                    await HandleSubscriptionEnableAsync(payload.Data);
                    break;
            }
            return new WebhookResponse { Success = true, Message = "Webhook Processed" };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook Error");
            return new WebhookResponse { Success = false, Message = ex.Message };
        }
    }

    private static string ComputeSignature(string payload, string secretKey)
    {
        using HMACSHA512 hmac = new(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    private async Task HandleChargeSuccessAsync(WebhookData data)
    {
        _logger.LogInformation("Processing Charge Success: {Reference}", data.Reference);

        var payment = await _context.PaymentHistory.FirstOrDefaultAsync(p => p.Reference == data.Reference);
        if (payment == null)
        {
            _logger.LogWarning("Payment Not Found: {Reference}", data.Reference);
            return;
        }

        if (payment.Status == "success")
        {
            _logger.LogInformation("Payment Already Processed: {Reference}", data.Reference);
            return;
        }

        payment.Status = "success";
        payment.PaymentMethod = data.Channel;
        payment.PaymentDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var planId = data.Metadata?.PlanId;
        if (!string.IsNullOrEmpty(planId))
        {
            await _subscriptionService.CreateSubscriptionAsync(payment.UserId, planId, false);
            _logger.LogInformation("Subscription Created For User {UserId}", payment.UserId);
        }
    }

    private void HandleSubscriptionCreate(WebhookData data)
    {
        _logger.LogInformation("Subscription Create Webhook: {SubscriptionCode}", data.SubscriptionCode);
    }

    private async Task HandleSubscriptionDisableAsync(WebhookData data)
    {
        _logger.LogInformation("Subscription Disable Webhook: {SubscriptionCode}", data.SubscriptionCode);
        var subscription = await _context.UserSubscriptions.FirstOrDefaultAsync(s => s.PaystackSubscriptionCode == data.SubscriptionCode);

        if (subscription != null)
        {
            subscription.Status = "canceled";
            subscription.EndDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Subscription Canceled: {SubscriptionCode}", data.SubscriptionCode);
        }
    }

    private async Task HandleSubscriptionEnableAsync(WebhookData data)
    {
        _logger.LogInformation("Subscription Enable Webhook: {SubscriptionCode}", data.SubscriptionCode);
        var subscription = await _context.UserSubscriptions.FirstOrDefaultAsync(s => s.PaystackSubscriptionCode == data.SubscriptionCode);

        if (subscription != null)
        {
            subscription.Status = "active";
            subscription.CancelAtPeriodEnd = false;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Subscription reactivated: {SubscriptionCode}", data.SubscriptionCode);
        }
    }
}