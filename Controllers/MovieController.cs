using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public class MovieController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public MovieController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetMovies()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!int.TryParse(userIdClaim, out int userId)) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null || !user.IsSubscribed)
            {
                return Forbid("You must Subscribe before you can watch movies.");
            }
            var movies = await _context.Movies.ToListAsync();
            return Ok(movies);
        }
    }
}
