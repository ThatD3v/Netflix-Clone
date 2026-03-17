using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;


namespace NetflixClone.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {

        if (await _context.Users.AnyAsync(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber))
            return BadRequest("User with this email or phone already exists");
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            PasswordHash = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == model.Identifier || u.PhoneNumber == model.Identifier);

        if (user == null)
            return Unauthorized("User not found");

        bool validPassword = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

        if (!validPassword)
            return Unauthorized("Invalid password");

        return Ok("Login successful");
    }
}