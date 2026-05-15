using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Services;
using System.Security.Claims;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.RegisterAdminAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAdmin([FromBody] AdminLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.LoginAdminAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
        [Authorize]
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyAdmin()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = await _adminService.IsAdminAsync(userId);

            return Ok(new
            {
                isAdmin,
                message = isAdmin ? "User is an administrator" : "User is not an administrator"
            });
        }
        [HttpGet("check-secret")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckSecret([FromQuery] string secret)
        {
            var isValid = await _adminService.VerifyAdminSecretAsync(secret);
            return Ok(new { isValid = isValid });
        }
    }
}