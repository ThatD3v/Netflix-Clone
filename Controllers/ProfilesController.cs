using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;


namespace NetflixClone.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class ProfilesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfilesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProfile(CreateProfileDto dto)
    {

        // Get all profiles for this user
        var profiles = await _context.Profiles
            .Where(p => p.UserId == LoggedInUser.UserId)
            .ToListAsync();

        //  Max 5 profiles
        if (profiles.Count >= 5)
            return BadRequest("Maximum 5 profiles allowed");

        //Only 1 kids profile
        if (dto.IsKidProfile && profiles.Any(p => p.IsKidProfile))
            return BadRequest("Only one kids profile allowed");

        var profile = new Profile
        {
            UserId = LoggedInUser.UserId,
            Name = dto.Name,
            IsKidProfile = dto.IsKidProfile,
            AvatarUrl = dto.AvatarUrl
        };

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        return Ok(profile);
    }
    [Authorize]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetProfiles()
    {
        var profiles = await _context.Profiles
            .Where(p => p.UserId == LoggedInUser.UserId)
            .ToListAsync();

        return Ok(profiles);
    }
}