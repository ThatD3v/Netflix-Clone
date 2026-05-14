using NetflixClone.DTOs;

namespace NetflixClone.Services
{
    public interface IContentService
    {
        Task<ContentResponse> CreateContentAsync(CreateContentRequest request);
        Task<ContentResponse> UpdateContentAsync(string contentId, UpdateContentRequest request);
        Task<bool> DeleteContentAsync(string contentId);
        Task<ContentResponse?> GetContentByIdAsync(string contentId);
        Task<List<ContentResponse>> GetAllContentAsync(int page = 1, int pageSize = 20);
        Task<List<ContentResponse>> GetMoviesAsync(int page = 1, int pageSize = 20);
        Task<List<ContentResponse>> GetSeriesAsync(int page = 1, int pageSize = 20);
        Task<SeasonResponse> AddSeasonAsync(string contentId, CreateSeasonRequest request);
        Task<EpisodeResponse> AddEpisodeAsync(string seasonId, CreateEpisodeRequest request);
        Task<List<GenreResponse>> GetAllGenresAsync();
        Task<GenreResponse> CreateGenreAsync(string name, string? description = null);
        Task<List<ContentResponse>> SearchContentAsync(string query, int page = 1, int pageSize = 20);
        Task<HomePageResponse> GetHomePageAsync(string profileId);
        Task<List<ContentCardDto>> GetTrendingAsync(int limit = 10);
        Task<List<ContentCardDto>> GetPopularAsync(int limit = 10);
        Task<List<ContentCardDto>> GetNewReleasesAsync(int limit = 10);
        Task<List<ContentCardDto>> GetContinueWatchingAsync(string profileId, int limit = 10);
        Task<List<ContentCardDto>> GetMyListAsync(string profileId, int limit = 10);
        Task<List<ContentCardDto>> GetRecommendedAsync(string profileId, int limit = 10);
        Task<HeroBannerDto?> GetHeroBannerAsync();
    }
}