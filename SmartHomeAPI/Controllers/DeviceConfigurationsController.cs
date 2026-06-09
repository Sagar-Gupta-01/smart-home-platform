using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using System.Security.Claims;

namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeviceConfigurationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeviceConfigurationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Add(DeviceConfiguration config)
    {
        if (!UserHasAccessToDevice(config.DeviceId)) return Forbid();

        _context.DeviceConfigurations.Add(config);
        _context.SaveChanges();
        return Ok(config);
    }

    [HttpGet("{deviceId}")]
    public IActionResult Get(int deviceId)
    {
        if (!UserHasAccessToDevice(deviceId)) return Forbid();

        var configs = _context.DeviceConfigurations
            .Where(x => x.DeviceId == deviceId)
            .ToList();

        return Ok(configs);
    }

    [HttpPut]
    public IActionResult Update(DeviceConfiguration config)
    {
        var existing = _context.DeviceConfigurations
            .FirstOrDefault(x => x.Id == config.Id);

        if (existing == null) return NotFound();

        if (!UserHasAccessToDevice(existing.DeviceId)) return Forbid();

        existing.Value = config.Value;
        _context.SaveChanges();

        return Ok(existing);
    }

    // Ensures the current user has access (via UserLocationAccess) to the
    // location that owns the device this configuration belongs to. Without
    // this check any authenticated user could read/modify configs for any
    // device in any tenant by guessing ids.
    private bool UserHasAccessToDevice(int deviceId)
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var device = _context.Devices.FirstOrDefault(d => d.Id == deviceId);
        if (device == null) return false;

        var room = _context.Rooms.FirstOrDefault(r => r.Id == device.RoomId);
        if (room == null) return false;

        return _context.UserLocationAccesses.Any(x =>
            x.UserId == userId && x.LocationId == room.LocationId);
    }
}
