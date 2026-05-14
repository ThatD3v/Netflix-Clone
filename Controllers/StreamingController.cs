using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetflixClone.DTOs;
using NetflixClone.Services;
using System;
using System.Threading.Tasks;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StreamingController : ControllerBase
    {
        private readonly IStreamingService _streamingService;
        private readonly ILogger<StreamingController> _logger;

        public StreamingController(IStreamingService streamingService, ILogger<StreamingController> logger)
        {
            _streamingService = streamingService;
            _logger = logger;
        }

        private string GetProfileId() => Request.Headers["X-Profile-Id"].ToString();

        [HttpGet("play/{contentId}")]
        public async Task<IActionResult> Play(string contentId, [FromQuery] string? episodeId = null, [FromQuery] string? quality = null)
        {
            var profileId = GetProfileId();
            if (string.IsNullOrEmpty(profileId))
                return Unauthorized(new { message = "Profile ID required. Use header: X-Profile-Id" });

            var result = await _streamingService.GetPlayResponseAsync(contentId, profileId, episodeId, quality);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("progress")]
        public async Task<IActionResult> UpdateProgress([FromBody] UpdateProgressRequest request)
        {
            var profileId = GetProfileId();
            if (string.IsNullOrEmpty(profileId))
                return Unauthorized(new { message = "Profile ID required" });

            await _streamingService.UpdateProgressAsync(profileId, request.ContentId, request.ProgressSeconds, request.DurationSeconds, request.EpisodeId);
            return Ok(new { success = true });
        }

        [HttpGet("manifest/{contentId}")]
        public async Task<IActionResult> GetManifest(string contentId, [FromQuery] string? episodeId = null)
        {
            var result = await _streamingService.GetManifestAsync(contentId, episodeId);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("continue-watching")]
        public async Task<IActionResult> GetContinueWatching()
        {
            var profileId = GetProfileId();
            if (string.IsNullOrEmpty(profileId))
                return Unauthorized(new { message = "Profile ID required" });

            var items = await _streamingService.GetContinueWatchingAsync(profileId);
            return Ok(new { success = true, data = items });
        }
    }
}