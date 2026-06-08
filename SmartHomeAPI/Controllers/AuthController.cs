using Microsoft.AspNetCore.Mvc;
using SmartHomeAPI.Data;
using SmartHomeAPI.Models;
using SmartHomeAPI.Services;
using SmartHomeAPI.Helpers;
using Microsoft.EntityFrameworkCore;
using Serilog;

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

        Log.Information("API hit for new user registration");

        // ✅ If user is Client → create Client entity
        if (user.Role == "Client")
        {
            var client = new Client
            {
                Name = user.Email // or any naming logic
            };

            _context.Clients.Add(client);
            _context.SaveChanges();
            Log.Information("New Client registered with email id: {Email}", user.Email);
            user.ClientId = client.Id; // 🔗 link user to client
        }

        _context.Users.Add(user);
        _context.SaveChanges();
        Log.Information("New user added with email id: {Email}", user.Email);

        return Ok(user);

    }

    [HttpPost("login")]
    public IActionResult Login(User login)
    {
        var user = _context.Users.FirstOrDefault(x => x.Email == login.Email);
        Log.Information("User trying to login with username: {Email}", login.Email);

        if (user == null || !PasswordHelper.Verify(login.PasswordHash, user.PasswordHash))
        {
            Log.Warning("UnAuthorised login attempt by username: {Email}", login.Email);
            return Unauthorized();
        }

        // The login request carries no jwt cookie yet, so the middleware seeds the
        // user id as Anonymous. Now that we know the user, record it so the enricher
        // includes it on the remaining log lines (and the middleware's completion log).
        HttpContext.Items["UserId"] = user.Id.ToString();

        var token = _auth.GenerateToken(user);
        var refreshToken = _auth.GenerateRefreshToken();

        Log.Information("Auth and refresher token generated for {Email}", user.Email);

        user.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();


        // ✅ Access token cookie
        Response.Cookies.Append("jwt", token, new CookieOptions
        {

            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(15)

        });


        // ✅ Refresh token cookie
        Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = refreshToken.Expires
        });
        Log.Information("Login successful for user {Email}", user.Email);
        return Ok(new { message = "Login successful" });
    }

    [HttpPost("refresh")]
    public IActionResult Refresh()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized();

        var user = _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

        if (user == null)
            return Unauthorized();

        var tokenRecord = user.RefreshTokens.First(t => t.Token == refreshToken);

        if (tokenRecord.IsExpired || tokenRecord.IsRevoked)
            return Unauthorized();

        // ✅ Rotate refresh token
        tokenRecord.IsRevoked = true;

        var newAccessToken = _auth.GenerateToken(user);
        var newRefreshToken = _auth.GenerateRefreshToken();

        user.RefreshTokens.Add(newRefreshToken);
        _context.SaveChanges();

        // ✅ Set new cookies
        Response.Cookies.Append("jwt", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddMinutes(15)
        });

        Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = newRefreshToken.Expires
        });

        return Ok(new { message = "Token refreshed" });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // change to true in production
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(-1) // ✅ expire immediately
        };

        Response.Cookies.Append("jwt", "", cookieOptions);
        Log.Information("User logged out");
        return Ok(new { message = "Logged out successfully" });
    }
}