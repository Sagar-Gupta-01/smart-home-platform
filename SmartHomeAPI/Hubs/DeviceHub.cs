using Microsoft.AspNetCore.SignalR;

namespace SmartHomeAPI.Hubs;

public class DeviceHub : Hub
{
    public async Task SendDeviceUpdate(object device)
    {
        await Clients.All.SendAsync("ReceiveDeviceUpdate", device);
    }
}