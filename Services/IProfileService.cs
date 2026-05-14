using System.Collections.Generic;
using System.Threading.Tasks;
using NetflixClone.DTOs;

namespace NetflixClone.Services
{
    public interface IProfileService
    {
        Task<ProfileResponse> CreateProfileAsync(string userId, CreateProfileRequest request);
        Task<List<ProfileResponse>> GetUserProfilesAsync(string userId);
        Task<ProfileResponse?> GetProfileByIdAsync(string profileId, string userId);
        Task<ProfileResponse> UpdateProfileAsync(string profileId, string userId, UpdateProfileRequest request);
        Task<bool> DeleteProfileAsync(string profileId, string userId);
        Task<bool> CanCreateMoreProfilesAsync(string userId);
        Task<bool> CanCreateKidsProfileAsync(string userId);
    }
}