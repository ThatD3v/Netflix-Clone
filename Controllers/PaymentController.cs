using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Services;
using System.Security.Claims;

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
            Plan = plan.PaystackPlanCode,
            Reference = $"NETFLIX_{userId}_{DateTime.UtcNow.Ticks}",
            CallbackUrl = "https://yourfrontend.com/payment-success"   
        };

            try
            {
                var response = await _paystackService.InitializeTransactionAsync(request);

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