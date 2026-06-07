using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Controllers;

[Authorize(Roles = "Client,SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class UserLocationAccessController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserLocationAccessController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Assign(UserLocationAccess access)
    {
        _context.UserLocationAccesses.Add(access);
        _context.SaveChanges();

        return Ok(access);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_context.UserLocationAccesses.ToList());
    }
}