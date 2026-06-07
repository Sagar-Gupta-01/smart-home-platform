using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using System.Security.Claims;

namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(Room room)
    {
        _context.Rooms.Add(room);
        _context.SaveChanges();

        return Ok(room);
    }

    [HttpGet]
    public IActionResult Get(int locationId)
    {
        var userId = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var hasAccess = _context.UserLocationAccesses.Any(x =>
            x.UserId == userId && x.LocationId == locationId);

        if (!hasAccess) return Forbid();

        var rooms = _context.Rooms
            .Where(r => r.LocationId == locationId)
            .ToList();

        return Ok(rooms);
    }
}