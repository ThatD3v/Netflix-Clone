using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Services;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly IContentService _contentService;
        private readonly ILogger<ContentController> _logger;

        public ContentController(IContentService contentService, ILogger<ContentController> logger)
        {
            _contentService = contentService;
            _logger = logger;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(string id)
        {
            var content = await _contentService.GetContentByIdAsync(id);

            if (content == null)
                return NotFound(new { success = false, message = "Content not found" });

            return Ok(new { success = true, data = content });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllContent([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var content = await _contentService.GetAllContentAsync(page, pageSize);
            return Ok(new { success = true, data = content, page, pageSize });
        }
        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var movies = await _contentService.GetMoviesAsync(page, pageSize);
            return Ok(new { success = true, data = movies, page, pageSize });
        }
        [HttpGet("series")]
        public async Task<IActionResult> GetSeries([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var series = await _contentService.GetSeriesAsync(page, pageSize);
            return Ok(new { success = true, data = series, page, pageSize });
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest(new { success = false, message = "Search query required" });

            var results = await _contentService.SearchContentAsync(q, page, pageSize);
            return Ok(new { success = true, data = results, page, pageSize });
        }
        [HttpGet("genres")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await _contentService.GetAllGenresAsync();
            return Ok(new { success = true, data = genres });
        }
        [HttpPost("admin-genre")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateGenre([FromBody] CreateGenreRequest request)
        {
            try
            {
                var genre = await _contentService.CreateGenreAsync(request.Name, request.Description);
                return Ok(new { success = true, message = "Genre created successfully", data = genre });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("admin/content")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateContent([FromBody] CreateContentRequest request)
        {
            try
            {
                var content = await _contentService.CreateContentAsync(request);
                return Ok(new { success = true, message = "Content created successfully", data = content });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateContent(string id, [FromBody] UpdateContentRequest request)
        {
            try
            {
                var content = await _contentService.UpdateContentAsync(id, request);
                return Ok(new { success = true, message = "Content updated successfully", data = content });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpDelete("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteContent(string id)
        {
            var result = await _contentService.DeleteContentAsync(id);

            if (!result)
                return NotFound(new { success = false, message = "Content not found" });

            return Ok(new { success = true, message = "Content deleted successfully" });
        }
        [HttpPost("admin/{id}/season")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSeason(string id, [FromBody] CreateSeasonRequest request)
        {
            try
            {
                var season = await _contentService.AddSeasonAsync(id, request);
                return Ok(new { success = true, message = "Season added successfully", data = season });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
        [HttpPost("admin/season/{seasonId}/episode")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddEpisode(string seasonId, [FromBody] CreateEpisodeRequest request)
        {
            try
            {
                var episode = await _contentService.AddEpisodeAsync(seasonId, request);
                return Ok(new { success = true, message = "Episode added successfully", data = episode });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}