using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using System.Security.Claims;

namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public LocationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(Location location)
    {
        _context.Locations.Add(location);
        _context.SaveChanges();

        return Ok(location);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var locations = _context.UserLocationAccesses
            .Where(x => x.UserId == userId)
            .Select(x => x.LocationId)
            .ToList();

        var result = _context.Locations
            .Where(l => locations.Contains(l.Id))
            .ToList();

        return Ok(result);
    }

    [HttpGet("full")]
    public IActionResult GetFull(int locationId)
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var hasAccess = _context.UserLocationAccesses.Any(x =>
            x.UserId == userId && x.LocationId == locationId);

        if (!hasAccess) return Forbid();

        var result = _context.Locations
            .Where(l => l.Id == locationId)
            .Select(l => new
            {
                l.Id,
                l.Name,
                Rooms = _context.Rooms
                    .Where(r => r.LocationId == l.Id)
                    .Select(r => new
                    {
                        r.Id,
                        r.Name,
                        Devices = _context.Devices
                            .Where(d => d.RoomId == r.Id)
                            .Select(d => new
                            {
                                d.Id,
                                d.Name,
                                d.IsOn,
                                d.DeviceTypeId,
                                Configs = _context.DeviceConfigurations
                                    .Where(c => c.DeviceId == d.Id)
                                    .ToList()
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .FirstOrDefault();

        return Ok(result);
    }

}