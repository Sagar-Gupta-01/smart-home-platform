using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;

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
        _context.DeviceConfigurations.Add(config);
        _context.SaveChanges();
        return Ok(config);
    }

    [HttpGet("{deviceId}")]
    public IActionResult Get(int deviceId)
    {
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

        existing.Value = config.Value;
        _context.SaveChanges();

        return Ok(existing);
    }

}