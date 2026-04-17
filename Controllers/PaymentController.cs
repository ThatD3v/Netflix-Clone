using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Services;
using System.Security.Claims;
using System.Text.Json;
namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PaystackService _paystackService;

        public PaymentController(ApplicationDbContext context, PaystackService paystackService)
        {
            _context = context;
            _paystackService = paystackService;
        }
        [HttpGet("Plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _context.Plans
                .ToListAsync();
            return Ok(plans);
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaystackWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["x-paystack-signature"].ToString();

            if(!_paystackService.VerifyWebhookSignature(signature, json))
            {
                return BadRequest("Invalid Siganture");
            }

            var eventData =JsonSerializer.Deserialize<JsonElement>(json);
            var eventType = eventData.GetProperty("event").GetString();

            if(eventType == "charge.success")
            {
                var data = eventData.GetProperty("data");
                var reference = data.GetProperty("reference").GetString();
                var email = data.GetProperty("customer").GetProperty("email").GetString();

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email ==email);
                if (user != null)
                {
                    user.IsSubscribed = true;
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }
        [HttpGet("verify/{reference}")]
        [Authorize]
        public async Task<IActionResult> VerifyPayment(string reference)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            try
            {
                var response = await _paystackService.VerifyTransactionAsync(reference);
                if (response.Status && response.Data.Status == "success")
                {
                    var user = await _context.Users.FindAsync(userId);
                    if (user != null)
                    {
                        user.IsSubscribed = true;
                        var amountPaid = response.Data.Amount / 100;
                        var plan = await _context.Plans.FirstOrDefaultAsync(p => p.Price == amountPaid);
                        if (plan != null)
                        {
                            user.PlanId = plan.Id;
                        }
                        await _context.SaveChangesAsync();
                    }
                    return Ok(new { message = "Payment verified successfully", data = response.Data });
                }
                return BadRequest(new { message = "Payment was not successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("initialize")]
        [Authorize]
        public async Task<IActionResult> InitializePayment([FromBody] InitializePaymentDto dto) 
        
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User not authenticated");
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if(string.IsNullOrEmpty(email))
            {
                return BadRequest("User email not found");
            }

            var plan = await _context.Plans.FindAsync(dto.PlanId);
            if(plan == null)
            {
                return BadRequest("Invalid plan selected");
            }

            var request = new InitializeTransactionRequest
        {
            Email = email,
            Amount = (int)(plan.Price * 100),          
            //Plan = plan.PaystackPlanCode,
            //Reference = $"NETFLIX_{userId}_{DateTime.UtcNow.Ticks}",
            //CallbackUrl = "https://yourfrontend.com/payment-success"   
        };

            try
            {
                var response = await _paystackService.InitializeTransactionAsync(request);

                var paymentRecord = new Payment
                {
                    InitializeOn = DateTime.Now,
                    Amount = request.Amount,
                    UserId = LoggedInUser.UserId,
                    Reference = response.Data.Reference,
                    AccessCode = response.Data.AccessCode,
                    CheckoutUrl = response.Data.AuthorizationUrl,

                };
                _context.Add(paymentRecord);
               await  _context.SaveChangesAsync();

                if (response.Status)
                {
                    return Ok(new
                    {
                        authorizationUrl = response.Data.AuthorizationUrl,
                        reference = response.Data.Reference,
                        message = "Payment initialized successfully"
                    });
                }

                return BadRequest(new {message = response.Message ?? "Failed to initialize payment"});
            }
            catch (Exception ex)
            {
            return BadRequest(new { message = ex.Message });
            }
        }
    }
}