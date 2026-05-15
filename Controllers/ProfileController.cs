using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.DTOs;
using NetflixClone.Services;
using System.Security.Claims;

namespace NetflixClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

        [HttpPost("create")]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileRequest request)
        {
            try
            {
                var profile = await _profileService.CreateProfileAsync(GetUserId(), request);
                return Ok(new { success = true, message = "Profile created successfully", profile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetProfiles()
        {
            var profiles = await _profileService.GetUserProfilesAsync(GetUserId());
            return Ok(new { success = true, profiles });
        }

        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetProfile(string profileId)
        {
            var profile = await _profileService.GetProfileByIdAsync(profileId, GetUserId());

            if (profile == null)
                return NotFound(new { success = false, message = "Profile not found" });

            return Ok(new { success = true, profile });
        }

        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(string profileId, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var profile = await _profileService.UpdateProfileAsync(profileId, GetUserId(), request);
                return Ok(new { success = true, message = "Profile updated successfully", profile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("{profileId}/avatar")]
        public async Task<IActionResult> UpdateAvatar(string profileId, [FromBody] UpdateAvatarRequest request)
        {
            try
            {
            var updateRequest = new UpdateProfileRequest()
                {
                    AvatarUrl = request.AvatarUrl
                };

                var profile = await _profileService.UpdateProfileAsync(profileId, GetUserId(), updateRequest);
                return Ok(new { success = true, message = "Avatar updated successfully", profile });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("{profileId}")]
        public async Task<IActionResult> DeleteProfile(string profileId)
        {
            try
            {
                var result = await _profileService.DeleteProfileAsync(profileId, GetUserId());
                return Ok(new { success = true, message = "Profile deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("can-create")]
        public async Task<IActionResult> CanCreateMoreProfiles()
        {
            var canCreate = await _profileService.CanCreateMoreProfilesAsync(GetUserId());
            var canCreateKids = await _profileService.CanCreateKidsProfileAsync(GetUserId());

            return Ok(new
            {
                success = true,
                canCreateMoreProfiles = canCreate,
                canCreateKidsProfile = canCreateKids,
                maxProfiles = 5
            });
        }
    }
}