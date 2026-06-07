using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartHomeAPI.Data;
using SmartHomeAPI.Hubs;
using SmartHomeAPI.Models;
using System.Security.Claims;


namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<DeviceHub> _hub;
    public DevicesController(AppDbContext context, IHubContext<DeviceHub> hub)
    {
        _context = context;
        _hub = hub;
    }

    
    [HttpPost]
    public IActionResult Create(Device device)
    {
        var room = _context.Rooms.FirstOrDefault(r => r.Id == device.RoomId);
        if (room == null) return BadRequest("Invalid Room");

        var deviceType = _context.DeviceTypes.FirstOrDefault(d => d.Id == device.DeviceTypeId);
        if (deviceType == null) return BadRequest("Invalid Device Type");

        _context.Devices.Add(device);
        _context.SaveChanges();

        return Ok(device);
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var device = _context.Devices.FirstOrDefault(d => d.Id == id);
        if (device == null) return NotFound();

        var room = _context.Rooms.FirstOrDefault(r => r.Id == device.RoomId);
        if (room == null) return NotFound();

        var hasAccess = _context.UserLocationAccesses.Any(x =>
            x.UserId == userId && x.LocationId == room.LocationId);

        if (!hasAccess) return Forbid();

        device.IsOn = !device.IsOn;
        _context.SaveChanges();

        await _hub.Clients.All.SendAsync("ReceiveDeviceUpdate", device);

        return Ok(device);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var locationIds = _context.UserLocationAccesses
            .Where(x => x.UserId == userId)
            .Select(x => x.LocationId)
            .ToList();

        var devices = _context.Devices
            .Where(d => _context.Rooms
                .Where(r => locationIds.Contains(r.LocationId))
                .Select(r => r.Id)
                .Contains(d.RoomId))
            .ToList();

        return Ok(devices);
    }

    [HttpGet("by-room")]
    public IActionResult GetByRoom(int roomId)
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
        if (room == null) return NotFound();

        var hasAccess = _context.UserLocationAccesses.Any(x =>
            x.UserId == userId && x.LocationId == room.LocationId);

        if (!hasAccess) return Forbid();

        var devices = _context.Devices
            .Where(d => d.RoomId == roomId)
            .ToList();

        return Ok(devices);
    }
}