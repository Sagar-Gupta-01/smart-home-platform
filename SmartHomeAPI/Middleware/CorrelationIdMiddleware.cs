using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Serilog;
using Serilog.Context;

namespace SmartHomeAPI.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // ✅ Correlation ID
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var route = context.Request.Path.ToString();
        var method = context.Request.Method;

        context.Items["UserId"] = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? GetUserIdFromCookie(context);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        using (LogContext.PushProperty("Route", route))
        using (LogContext.PushProperty("Method", method))
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred");
                throw;
            }
            finally
            {
                stopwatch.Stop();

                var statusCode = context.Response.StatusCode;
                var duration = stopwatch.ElapsedMilliseconds;

                Log.Information(
                    "Request completed with Status {StatusCode} in {Duration} ms",
                    statusCode,
                    duration
                );
            }
        }
    }

    // Reads the user id directly from the jwt cookie. Used for logging only, so
    // the token is read (not re-validated) — the auth middleware already validates
    // it on protected routes.
    private static string? GetUserIdFromCookie(HttpContext context)
    {
        var jwt = context.Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(jwt))
            return null;

        try
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
            return token.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid" ||
                c.Type == "sub")?.Value;
        }
        catch
        {
            // Malformed/expired token — fall back to Anonymous.
            return null;
        }
    }
}