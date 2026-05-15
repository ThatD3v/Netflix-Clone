using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Services;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BrowseController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ILogger<BrowseController> _logger;

        public BrowseController(IContentService contentService, ILogger<BrowseController> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }
        [HttpGet("home")]
        public async Task<IActionResult> GetHomePage([FromQuery] string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return BadRequest(new { success = false, message = "Profile ID is required. Use ?profileId=your-profile-id" });
            }
            var homePage = await _contentService.GetHomePageAsync(profileId);
            return Ok(new { success = true, data = homePage });
        }
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending([FromQuery] int limit = 10)
        {
            var trending = await _contentService.GetTrendingAsync(limit);
            return Ok(new { success = true, data = trending });
        }
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopular([FromQuery] int limit = 10)
        {
            var popular = await _contentService.GetPopularAsync(limit);
            return Ok(new { success = true, data = popular });
        }
        [HttpGet("new-releases")]
        public async Task<IActionResult> GetNewReleases([FromQuery] int limit = 10)
        {
            var newReleases = await _contentService.GetNewReleasesAsync(limit);
            return Ok(new { success = true, data = newReleases });
        }
        [HttpGet("continue-watching")]
        public async Task<IActionResult> GetContinueWatching([FromQuery] string profileId, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return BadRequest(new { success = false, message = "Profile ID is required. Use ?profileId=your-profile-id" });
            }
            var continueWatching = await _contentService.GetContinueWatchingAsync(profileId, limit);
            return Ok(new { success = true, data = continueWatching });
        }
        [HttpGet("my-list")]
        public async Task<IActionResult> GetMyList([FromQuery] string profileId, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return BadRequest(new { success = false, message = "Profile ID is required. Use ?profileId=your-profile-id" });
            }
            var myList = await _contentService.GetMyListAsync(profileId, limit);
            return Ok(new { success = true, data = myList });
        }
        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommended([FromQuery] string profileId, [FromQuery] int limit = 10)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return BadRequest(new { success = false, message = "Profile ID is required. Use ?profileId=your-profile-id" });
            }
            var recommended = await _contentService.GetRecommendedAsync(profileId, limit);
            return Ok(new { success = true, data = recommended });
        }
        [HttpGet("hero")]
        public async Task<IActionResult> GetHeroBanner()
        {
            var hero = await _contentService.GetHeroBannerAsync();
            return Ok(new { success = true, data = hero });
        }
    }
}