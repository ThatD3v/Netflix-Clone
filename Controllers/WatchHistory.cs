using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.Models;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchHistoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WatchHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Add or Update Watch
       // [Authorize]
        [HttpPost("watch")]
        public async Task<IActionResult> AddorUpdateWatch(int profileId,int ContentId,double progress,string ContentType)
       // public async Task<IActionResult> AddOrUpdateWatch(int ContentId, double progress,string ContentType)
        {
            //var profileId = int.Parse(User.FindFirst("profileId")!.Value);

            var history = await _context.WatchHistories
                .FirstOrDefaultAsync(
                w => w.ProfileId == profileId &&
                w.ContentId == ContentId &&
                w.ContentType == ContentType
                );

           if (history == null)
            {
                history = new WatchHistory
                {
                    ProfileId = profileId,
                    ContentId = ContentId,
                    Progress = progress,
                    ContentType = ContentType,
                    IsCompleted = false
                };

                _context.WatchHistories.Add(history);
            }
            else
            {
                history.Progress = progress;
                history.WatchedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok("Progress saved");
        }

        // ✅ Continue Watching
        //[Authorize]
        [HttpGet("continue-watching")]
        public async Task<IActionResult> GetContinueWatching(int profileId)

       // public async Task<IActionResult> GetContinueWatching()
        {
           // var profileId = int.Parse(User.FindFirst("profileId")!.Value);

            var data = await _context.WatchHistories
                .Where(w => w.ProfileId == profileId && !w.IsCompleted)
                .OrderByDescending(w => w.WatchedAt)
                .ToListAsync();

            return Ok(data);
        }

        // ✅ Mark as Completed
       // [Authorize]
        [HttpPost("complete")]
        public async Task<IActionResult> postComplete(int profileId,int ContentId)

        // public async Task<IActionResult> MarkAsCompleted(int ContentId)
        {
            //var profileId = int.Parse(User.FindFirst("profileId")!.Value);

            var history = await _context.WatchHistories
                .FirstOrDefaultAsync(w => w.ProfileId == profileId && w.ContentId == ContentId);

            if (history == null)
                return NotFound();

            history.IsCompleted = true;
            history.Progress = 100;

            await _context.SaveChangesAsync();

            return Ok("Marked as completed");
        }
    }
}