namespace SmartHomeAPI.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
      public int LocationId { get; set; }
}
