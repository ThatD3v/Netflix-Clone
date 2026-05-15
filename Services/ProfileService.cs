using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;
using NetflixClone.DTOs;
using NetflixClone.Models;

namespace NetflixClone.Services;

public class ProfileService(ApplicationDbContext context, ILogger<ProfileService> logger) : IProfileService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ProfileService> _logger = logger;

    public async Task<ProfileResponse> CreateProfileAsync(string userId, CreateProfileRequest request)
    {
        var canCreate = await CanCreateMoreProfilesAsync(userId);
        if (!canCreate)
        {
            throw new Exception("Maximum 5 profiles per account");
        }

        if (request.IsKidsProfile)
        {
            var canCreateKids = await CanCreateKidsProfileAsync(userId);
            if (!canCreateKids)
            {
                throw new Exception("You already have a kids profile. Only one kids profile per account is allowed.");
            }
        }

        var nameExists = await _context.Profiles
            .AnyAsync(p => p.UserId == userId && p.Name == request.Name && p.IsActive);

        if (nameExists)
        {
            throw new Exception("A profile with this name already exists");
        }

        Profile profile = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Name = request.Name,
            AvatarUrl = request.AvatarUrl,
            IsKidsProfile = request.IsKidsProfile,
            IsActive = true
        };

        _context.Profiles.Add(profile);
        await _context.SaveChangesAsync();

        return new ProfileResponse
        {
            Id = profile.Id,
            Name = profile.Name,
            AvatarUrl = profile.AvatarUrl,
            IsKidsProfile = profile.IsKidsProfile
        };
    }

    public async Task<List<ProfileResponse>> GetUserProfilesAsync(string userId)
    {
        var profiles = await _context.Profiles
            .Where(p => p.UserId == userId && p.IsActive)
            .ToListAsync();

        return profiles.Select(p => new ProfileResponse
        {
            Id = p.Id,
            Name = p.Name,
            AvatarUrl = p.AvatarUrl,
            IsKidsProfile = p.IsKidsProfile
        }).ToList();
    }

    public async Task<ProfileResponse?> GetProfileByIdAsync(string profileId, string userId)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId && p.IsActive);

        if (profile == null) return null;

        return new ProfileResponse
        {
            Id = profile.Id,
            Name = profile.Name,
            AvatarUrl = profile.AvatarUrl,
            IsKidsProfile = profile.IsKidsProfile
        };
    }

    public async Task<ProfileResponse> UpdateProfileAsync(string profileId, string userId, UpdateProfileRequest request)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId && p.IsActive);

        if (profile == null)
            throw new Exception("Profile not found");

        if (!string.IsNullOrEmpty(request.Name) && request.Name != profile.Name)
        {
            var nameExists = await _context.Profiles
                .AnyAsync(p => p.UserId == userId && p.Name == request.Name && p.Id != profileId && p.IsActive);

            if (nameExists)
            {
                throw new Exception("A profile with this name already exists");
            }
            profile.Name = request.Name;
        }

        if (request.AvatarUrl != null)
            profile.AvatarUrl = request.AvatarUrl;

        profile.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new ProfileResponse
        {
            Id = profile.Id,
            Name = profile.Name,
            AvatarUrl = profile.AvatarUrl,
            IsKidsProfile = profile.IsKidsProfile
        };
    }

    public async Task<bool> DeleteProfileAsync(string profileId, string userId)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId && p.UserId == userId && p.IsActive);

        if (profile == null) return false;

        var profileCount = await _context.Profiles
            .CountAsync(p => p.UserId == userId && p.IsActive);

        if (profileCount <= 1)
        {
            throw new Exception("Cannot delete the last profile. You must have at least one profile.");
        }

        profile.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CanCreateMoreProfilesAsync(string userId)
    {
        var profileCount = await _context.Profiles
            .CountAsync(p => p.UserId == userId && p.IsActive);

        return profileCount < 5;
    }

    public async Task<bool> CanCreateKidsProfileAsync(string userId)
    {
        var kidsProfileExists = await _context.Profiles
            .AnyAsync(p => p.UserId == userId && p.IsKidsProfile && p.IsActive);

        return !kidsProfileExists;
    }
}