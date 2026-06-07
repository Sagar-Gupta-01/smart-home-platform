namespace SmartHomeAPI.Models;

public class Device
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int DeviceTypeId { get; set; }
    public DeviceType? DeviceType { get; set; }

    public int RoomId { get; set; }

    public bool IsOn { get; set; }
}
