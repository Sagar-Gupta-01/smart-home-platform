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
        user.Id = 0;             // never trust an id from the body
        user.RefreshTokens = new();

        // A Client may only create Members inside their own tenant — they can't
        // mint SuperAdmins or users for another client. A SuperAdmin (no ClientId
        // claim) may set the role/tenant explicitly, defaulting to Member.
        var clientIdClaim = User.FindFirst("ClientId")?.Value;
        if (int.TryParse(clientIdClaim, out var clientId))
        {
            user.ClientId = clientId;
            user.Role = "Member";
        }
        else if (string.IsNullOrWhiteSpace(user.Role))
        {
            user.Role = "Member";
        }

        user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);
        _context.Users.Add(user);
        _context.SaveChanges();

        // Return a DTO — never echo PasswordHash or the refresh-token collection back
        return Ok(new { user.Id, user.Email, user.Role, user.ClientId });
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Scope the listing to the caller's tenant; SuperAdmin (no ClientId claim) sees everyone.
        var clientIdClaim = User.FindFirst("ClientId")?.Value;
        int? clientId = int.TryParse(clientIdClaim, out var parsed) ? parsed : null;

        var users = _context.Users
            .Where(u => clientId == null || u.ClientId == clientId)
            .Select(u => new { u.Id, u.Email, u.Role, u.ClientId })
            .ToList();

        return Ok(users);
    }
}   