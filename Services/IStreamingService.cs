using System.Collections.Generic;
using System.Threading.Tasks;
using NetflixClone.DTOs;

namespace NetflixClone.Services
{
    public interface IStreamingService
    {
        Task<PlayResponse> GetPlayResponseAsync(string contentId, string profileId, string? episodeId = null, string? quality = null);
        Task UpdateProgressAsync(string profileId, string contentId, int progressSeconds, int durationSeconds, string? episodeId = null);
        Task<List<ContinueWatchingDto>> GetContinueWatchingAsync(string profileId);
        Task<StreamingManifestResponse> GetManifestAsync(string contentId, string? episodeId = null);
    }
}