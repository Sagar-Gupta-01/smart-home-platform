using System.Security.Claims;
using Serilog.Core;
using Serilog.Events;

namespace SmartHomeAPI.Logging;

// Resolves the current request's user id at the moment each log event is written.
// Reading from HttpContext.Items (seeded by the middleware, updated by controllers
// once the user is known) means a single log line can reflect the id even when it
// only becomes available mid-request (e.g. login).
public class UserIdEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _accessor;

    public UserIdEnricher(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var context = _accessor.HttpContext;
        if (context is null)
            return; // No request scope (e.g. startup logs) — leave UserId unset.

        var userId = context.Items["UserId"] as string
                     ?? context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? "Anonymous";

        logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UserId", userId));
    }
}
