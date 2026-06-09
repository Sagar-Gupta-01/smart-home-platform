using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SmartHomeAPI.Data;
using System.Security.Claims;

namespace SmartHomeAPI.Hubs;

// Real-time device updates are scoped per location. On connect, each client is
// added to a "location-{id}" group for every location they have access to, so a
// device toggle is only pushed to users who can actually see that location —
// instead of being broadcast to every connected user across all tenants.
[Authorize]
public class DeviceHub : Hub
{
    private readonly AppDbContext _context;

    public DeviceHub(AppDbContext context)
    {
        _context = context;
    }

    public static string LocationGroup(int locationId) => $"location-{locationId}";

    public override async Task OnConnectedAsync()
    {
        var idClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idClaim, out var userId))
        {
            var locationIds = _context.UserLocationAccesses
                .Where(x => x.UserId == userId)
                .Select(x => x.LocationId)
                .ToList();

            foreach (var locationId in locationIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, LocationGroup(locationId));
            }
        }

        await base.OnConnectedAsync();
    }
}
