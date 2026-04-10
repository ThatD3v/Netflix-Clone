using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;
using NetflixClone.Services;


namespace NetflixClone.Controllers;


[ApiController]
[Route("api/[Controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JWTService _jwtService;

    public AuthController(ApplicationDbContext context, JWTService jwtService)
    {   
        _jwtService = jwtService;
        _context = context;
       
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
        var user = new User
        {
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            PasswordHash = hashedPassword
        };
        if (await _context.Users.AnyAsync(u => u.Email == model.Email || u.PhoneNumber == model.PhoneNumber))
            return BadRequest("User with this email or phone already exists");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var token = _jwtService.GenerateToken(user.Id.ToString());
        return Ok(new { token });   
    }

    //[Authorize]
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


        var token = _jwtService.GenerateToken(user.Id.ToString());
        return Ok(new { token });
    }
}