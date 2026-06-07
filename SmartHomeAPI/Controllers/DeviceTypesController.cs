using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeviceTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public DeviceTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(DeviceType type)
    {
        _context.DeviceTypes.Add(type);
        _context.SaveChanges();
        return Ok(type);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_context.DeviceTypes.ToList());
    }
}