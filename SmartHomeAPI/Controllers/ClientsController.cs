using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;

namespace SmartHomeAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(Client client)
    {
        _context.Clients.Add(client);
        _context.SaveChanges();

        return Ok(client);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_context.Clients.ToList());
    }
}
