namespace SmartHomeAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;    
    public string Role { get; set; } = "Member"; // SuperAdmin / Client / Member
    public int? ClientId { get; set; } // belongs to which client
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
