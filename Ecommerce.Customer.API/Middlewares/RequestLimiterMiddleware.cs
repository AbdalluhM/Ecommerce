namespace Ecommerce.Customer.API.Middlewares
{
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class RequestLimiterMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ConcurrentDictionary<string, DateTime> _requestDictionary;
    private readonly int _requestLimit;
    private readonly TimeSpan _timeWindow;

    public RequestLimiterMiddleware(RequestDelegate next, int requestLimit, TimeSpan timeWindow)
    {
        _next = next;
        _requestLimit = requestLimit;
        _timeWindow = timeWindow;
        _requestDictionary = new ConcurrentDictionary<string, DateTime>();
    }

    public async Task Invoke(HttpContext context)
    {
        string ipAddress = context.Connection.RemoteIpAddress.ToString();
        DateTime now = DateTime.Now;
        DateTime cutoff = now.Subtract(_timeWindow);

        // Remove old entries from dictionary
        foreach (var entry in _requestDictionary)
        {
            if (entry.Value < cutoff)
            {
                _requestDictionary.TryRemove(entry.Key, out _);
            }
        }

        // Count requests from this IP address
        int requestCount = 0;
        foreach (var entry in _requestDictionary)
        {
            if (entry.Key == ipAddress)
            {
                requestCount++;
            }
        }

        // Check if request count exceeds limit
        if (requestCount > _requestLimit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests. Please try again later.");
            return;
        }

        // Add current request to dictionary
        _requestDictionary.TryAdd(ipAddress, now);

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}

// Extension method used to add the middleware to the HTTP request pipeline
public static class RequestLimiterMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLimiterMiddleware(this IApplicationBuilder builder, int requestLimit, TimeSpan timeWindow)
    {
        return builder.UseMiddleware<RequestLimiterMiddleware>(requestLimit, timeWindow);
    }
}
}

