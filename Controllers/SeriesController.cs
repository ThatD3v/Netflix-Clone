using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SeriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Series>>> GetAllSeries()
        {
            var allSeries = await _context.Series.Include(s => s.Seasons).ThenInclude(s => s.Episodes).ToListAsync();
            return Ok(allSeries);
        }
    }
}
