using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;
using SmartHomeAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace SmartHomeAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AuthService _auth;

    public AuthController(AppDbContext context, AuthService auth)
    {
        _context = context;
        _auth = auth;
    }

    [HttpPost("register")]
    public IActionResult Register(User user)
    {
        user.PasswordHash = PasswordHelper.Hash(user.PasswordHash);
        
        // ✅ If user is Client → create Client entity
        if (user.Role == "Client")
        {
            var client = new Client
            {
                Name = user.Email // or any naming logic
            };

            _context.Clients.Add(client);
            _context.SaveChanges();

            user.ClientId = client.Id; // 🔗 link user to client
        }

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(user);

    }

    [HttpPost("login")]
    public IActionResult Login(User login)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == login.Email);

        if (user == null || !PasswordHelper.Verify(login.PasswordHash, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = _auth.GenerateToken(user);
        var refreshToken = _auth.GenerateRefreshToken();

        user.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        return Ok(new
        {
            token,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh(string refreshToken)
    {
        var user = _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
            return Unauthorized();

        var token = user.RefreshTokens.First(t => t.Token == refreshToken);

        if (token.IsExpired || token.IsRevoked)
            return Unauthorized();

        // Rotate token (IMPORTANT ✅)
        token.IsRevoked = true;

        var newAccessToken = _auth.GenerateToken(user);
        var newRefreshToken = _auth.GenerateRefreshToken();

        user.RefreshTokens.Add(newRefreshToken);
        _context.SaveChanges();

        return Ok(new
        {
            token = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }
}