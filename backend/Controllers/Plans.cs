using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Data;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PlansController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Show all plans
    [HttpGet]
    public async Task<IActionResult> GetAllPlans()
    {
        // Step 1: fetch from database
        var plansFromDb = await _context.Plans.ToListAsync();

        // Step 2: project in memory
        var plans = plansFromDb.Select(p => new
        {
            p.Id,
            p.Name,
            p.Price,
            p.VideoQuality,
            p.Resolution,
            p.SpatialAudio,
            p.MaxDevices,
            p.MaxDownloadDevices,
            AllowedDeviceTypes = p.AllowedDeviceTypes.Split(',').ToList() ?? new List<string>(),
            p.Description
        });

        return Ok(plans);
    }

    // Show single plan
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlanById(int id)
    {
        // Step 1: fetch from database
        var planFromDb = await _context.Plans.FirstOrDefaultAsync(p => p.Id == id);

        if (planFromDb == null) return NotFound("Plan not found");

        // Step 2: project in memory
        var plan = new
        {
            planFromDb.Id,
            planFromDb.Name,
            planFromDb.Price,
            planFromDb.VideoQuality,
            planFromDb.Resolution,
            planFromDb.SpatialAudio,
            planFromDb.MaxDevices,
            planFromDb.MaxDownloadDevices,
            AllowedDeviceTypes = planFromDb.AllowedDeviceTypes.Split(',').ToList(),
            planFromDb.Description
        };

        return Ok(plan);
    }
}