using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;

namespace NetflixClone.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class FavoritesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FavoritesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Add to favorites
    //[Authorize]
    [HttpPost("add")]
    public async Task<IActionResult> AddToFavorites(int profileId,int contentId, string contentType)
    {
        //var profileId = int.Parse(User.FindFirst("profileId")!.Value);

        var exists = await _context.Favorites
            .AnyAsync(f => f.ProfileId == profileId && f.ContentId == contentId && f.ContentType == contentType);

        if (exists)
            return BadRequest("Already in favorites");

        var favorite = new Favorite
        {
            ProfileId = profileId,
            ContentId = contentId,
            ContentType = contentType
        };

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return Ok("Added to favorites");
    }

    // Remove from favorites
    //[Authorize]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveFromFavorites(int profileId,int contentId, string contentType)
    {
       // var profileId = int.Parse(User.FindFirst("profileId")!.Value);

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.ProfileId == profileId && f.ContentId == contentId && f.ContentType == contentType);

        if (favorite == null)
            return NotFound("Not in favorites");

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();

        return Ok("Removed from favorites");
    }

    // Get all favorites
    //[Authorize]
    [HttpGet("list")]
    public async Task<IActionResult> GetFavorites(int profileId)
    {
       // var profileId = int.Parse(User.FindFirst("profileId")!.Value);

        var favorites = await _context.Favorites
            .Where(f => f.ProfileId == profileId)
            .OrderByDescending(f => f.AddedAt)
            .ToListAsync();

        return Ok(favorites);
    }
}
