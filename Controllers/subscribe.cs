using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetflixClone.Data;
using Microsoft.EntityFrameworkCore;
using NetflixClone.Models;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaymentController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(int planId, string deviceId, string deviceType)
    {
        var userId = int.Parse(User.FindFirst("userId")!.Value);

        var plan = await _context.Plans.FindAsync(planId);
        if (plan == null) return NotFound("Plan not found");

        // Device type check
        if (!plan.AllowedDeviceTypes.Split(',').Contains(deviceType.ToLower()))
            return Forbid("Device not allowed for this plan");

        // Max devices check
        int activeDevices = await _context.UserDevices.CountAsync(d => d.UserId == userId && d.IsActive);
        if (activeDevices >= plan.MaxDevices)
            return Forbid("Maximum devices reached for this plan");

        // Simulate payment success
        var subscription = new Subscription
        {
            UserId = userId,
            PlanId = plan.Id,
            StartedAt = DateTime.Now,
            ExpiresAt = DateTime.Now.AddMonths(1),
            IsActive = true
        };
        _context.Subscriptions.Add(subscription);

        // Track device
        var userDevice = new UserDevices
        {
            UserId = userId,
            DeviceId = deviceId,
            DeviceType = deviceType,
            IsActive = true
        };
        _context.UserDevices.Add(userDevice);

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Subscription successful", SubscriptionId = subscription.Id });
    }
}
