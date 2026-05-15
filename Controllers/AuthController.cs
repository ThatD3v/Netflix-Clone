using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Services;
using System.Security.Claims;

namespace NetflixClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ApplicationDbContext _context;

    public AuthController(IAuthService authService, ApplicationDbContext context)
    {
        _authService = authService;
        _context = context;
    }
    [HttpGet("test-db")]
    public async Task<IActionResult> TestDatabase()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            return Ok(new
            {
                connected = canConnect,
                message = canConnect ? "Database connected!" : "Cannot connect to database"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, stack = ex.StackTrace });
        }
    }
    [HttpPost("check")]
    public async Task<IActionResult> CheckUser([FromBody] CheckUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.CheckUserExistsAsync(request);
        return Ok(result);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] NetflixClone.DTOs.LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] NetflixClone.DTOs.RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request);

        if (!result.Success)
            return Unauthorized(result);

        return Ok(result);
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _authService.LogoutAsync(userId);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _authService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user);
    }
}