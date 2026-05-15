using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Services;
using System.Security.Claims;
using System.Text;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly IPaymentService _paymentService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IConfiguration _configuration;


        public SubscriptionController(
            ISubscriptionService subscriptionService,
            IPaymentService paymentService,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<SubscriptionController> logger)
        {
            _subscriptionService = subscriptionService;
            _paymentService = paymentService;
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpGet("debug-subscription")]
        [Authorize]
        public async Task<IActionResult> DebugSubscription()
        {
            var userId = GetUserId();
            var results = new List<string>();

            try
            {
                results.Add($"User ID: {userId}");
                var subscriptions = _context.UserSubscriptions.ToList();
                results.Add($"Total subscriptions in DB: {subscriptions.Count}");

                var userSubscriptions = await _context.UserSubscriptions
                    .Where(s => s.UserId == userId)
                    .ToListAsync();
                results.Add($"User subscriptions: {userSubscriptions.Count}");

                foreach (var sub in userSubscriptions)
                {
                    results.Add($"  - ID: {sub.Id}, Status: {sub.Status}, PlanId: {sub.PlanId}");
                }
                var plans = await _context.SubscriptionPlans.ToListAsync();
                results.Add($"Total plans: {plans.Count}");

                foreach (var plan in plans)
                {
                    results.Add($"  - Plan: {plan.Name}, ID: {plan.Id}");
                }

                return Ok(new { success = true, debug = results });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message, stack = ex.StackTrace });
            }
        }


        [HttpGet("debug-paystack-api")]
        [AllowAnonymous]
        public async Task<IActionResult> DebugPaystackApi()
        {
            var results = new List<string>();

            try
            {
                var secretKey = _configuration["Paystack:SecretKey"];
                results.Add($"Secret Key loaded: {(string.IsNullOrEmpty(secretKey) ? "NO" : "YES")}");

                if (!string.IsNullOrEmpty(secretKey))
                {
                    results.Add($"Secret Key length: {secretKey.Length}");
                    results.Add($"Secret Key prefix: {secretKey.Substring(0, 10)}...");
                }

                results.Add("Making direct HTTP call to Paystack...");

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {secretKey}");

                var payload = new
                {
                    email = "test@example.com",
                    amount = 10000,
                    currency = "NGN",
                    callback_url = "https://localhost:5001/api/subscription/verify-payment"
                };

                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                results.Add($"Request payload: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://api.paystack.co/transaction/initialize", content);
                var responseString = await response.Content.ReadAsStringAsync();

                results.Add($"HTTP Status: {(int)response.StatusCode}");
                results.Add($"Response: {responseString}");

                return Ok(new { success = true, details = results });
            }
            catch (Exception ex)
            {
                results.Add($"ERROR: {ex.Message}");
                results.Add($"Stack: {ex.StackTrace}");
                return Ok(new { success = false, details = results });
            }
        }


        [HttpPost("test-save")]
        [Authorize]
        public async Task<IActionResult> TestSave()
        {
            try
            {
                var testPayment = new PaymentHistory
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = GetUserId(),
                    Amount = 100,
                    Currency = "NGN",
                    Status = "test",
                    Reference = $"TEST-{Guid.NewGuid()}",
                    Description = "Test payment",
                    PaymentDate = DateTime.UtcNow
                };

                await _context.PaymentHistory.AddAsync(testPayment);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Save successful!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("plans")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPlans()
        {
            var plans = await _subscriptionService.GetAllPlansAsync();
            return Ok(new { success = true, plans });
        }

        [HttpGet("plans/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlanById(string id)
        {
            var plan = await _subscriptionService.GetPlanByIdAsync(id);

            if (plan == null)
                return NotFound(new { success = false, message = "Plan not found" });

            return Ok(new { success = true, plan });
        }

        [HttpGet("my-subscription")]
        public async Task<IActionResult> GetMySubscription()
        {
            var subscription = await _subscriptionService.GetUserSubscriptionAsync(GetUserId());
            return Ok(new
            {
                hasSubscription = subscription != null,
                subscription,
                needsSubscription = subscription == null
            });
        }

        [HttpGet("status")]
        public async Task<IActionResult> CheckStatus()
        {
            var hasSubscription = await _subscriptionService.HasActiveSubscriptionAsync(GetUserId());
            var subscription = await _subscriptionService.GetUserSubscriptionAsync(GetUserId());

            return Ok(new
            {
                hasSubscription,
                needsSubscription = !hasSubscription,
                subscription,
                redirectTo = !hasSubscription ? "/plans" : "/profile"
            });
        }

        [HttpPost("create-free-trial")]
        public async Task<IActionResult> CreateFreeTrial([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var subscription = await _subscriptionService.CreateSubscriptionAsync(
                    GetUserId(), request.PlanId, useTrial: true);

                return Ok(new
                {
                    success = true,
                    message = "7-day free trial started!",
                    subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("create-paid")]
        public async Task<IActionResult> CreatePaidSubscription([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var subscription = await _subscriptionService.CreateSubscriptionAsync(
                    GetUserId(), request.PlanId, useTrial: false);

                return Ok(new
                {
                    success = true,
                    message = "Subscription activated!",
                    subscription
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelSubscription()
        {
            var result = await _subscriptionService.CancelSubscriptionAsync(GetUserId());

            if (result)
                return Ok(new { success = true, message = "Subscription canceled" });

            return BadRequest(new { success = false, message = "No active subscription found" });
        }

        [HttpPost("change-plan")]
        public async Task<IActionResult> ChangePlan([FromBody] ChangePlanRequest request)
        {
            try
            {
                var result = await _subscriptionService.ChangePlanAsync(GetUserId(), request.NewPlanId);

                if (result)
                {
                    var subscription = await _subscriptionService.GetUserSubscriptionAsync(GetUserId());
                    return Ok(new
                    {
                        success = true,
                        message = "Plan changed successfully",
                        subscription
                    });
                }

                return BadRequest(new { success = false, message = "Failed to change plan" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("initialize-payment")]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentRequest request)
        {
            var userId = GetUserId();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized(new { success = false, message = "User not found" });
            }

            var email = user.Email ?? request.Email;
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Email is required for payment" });
            }

            var result = await _paymentService.InitializePaymentAsync(userId, request.PlanId, email);

            if (!result.Status)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("paystack")]
        public async Task<IActionResult> PaystackWebhook()
        {
            
            var jsonPayload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["x-paystack-signature"].ToString();

            _logger.LogInformation("Webhook received");

            var result = await _paymentService.HandleWebhookAsync(jsonPayload, signature);

            if (result.Success)
                return Ok(new { status = "success" });

            return BadRequest(new { status = "failed", message = result.Message });
        }

        [HttpGet("verify-payment")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPayment([FromQuery] string reference)
        {
            var result = await _paymentService.VerifyPaymentAsync(reference);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("payment-history")]
        public async Task<IActionResult> GetPaymentHistory()
        {
            var payments = await _subscriptionService.GetPaymentHistoryAsync(GetUserId());
            return Ok(new { success = true, payments });
        }

        [HttpGet("can-access")]
        public async Task<IActionResult> CanAccessFeature([FromQuery] string videoQuality = "480p", [FromQuery] int screens = 1)
        {
            var canAccess = await _subscriptionService.CanAccessFeatureAsync(GetUserId(), videoQuality, screens);

            return Ok(new
            {
                canAccess,
                videoQuality,
                screens,
                message = canAccess ? "Access granted" : "Upgrade your plan to access this feature"
            });
        }
    }
}