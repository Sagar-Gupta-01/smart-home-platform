namespace SmartHomeAPI.Models;

public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = string.Empty;

    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;

    public bool IsRevoked { get; set; }

    public int UserId { get; set; }
}