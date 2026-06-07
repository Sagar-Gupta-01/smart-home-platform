using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using SmartHomeAPI.Helpers;

namespace SmartHomeAPI.Controllers;

[Authorize(Roles = "Client,SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);
        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_context.Users.ToList());
    }
}   