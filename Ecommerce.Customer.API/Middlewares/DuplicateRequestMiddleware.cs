namespace Ecommerce.Customer.API.Middlewares
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Ecommerce.Core.Enums.Auth;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class DuplicateRequestMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DuplicateRequestMiddleware(RequestDelegate next, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var user = context.User;
            var c = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Assuming the user is authenticated and we can get a unique identifier for the user
            // var userId = _httpContextAccessor.HttpContext.User?.Identity?.Name;
            var userId = user.FindFirstValue(TokenClaimTypeEnum.Id.ToString());

            if (string.IsNullOrEmpty(userId))
            {
                await _next(context);
                return;
            }

            // Create a unique key based on the user ID and the request path
            var requestKey = $"{userId}-{context.Request.Path}-{context.Request.QueryString}";

            // Check if the request already exists in the cache
            if (_cache.TryGetValue(requestKey, out _))
            {
                // If it exists, block the request
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Duplicate request detected. Please wait before retrying.");
                return;
            }

            // Otherwise, cache the request with a short expiration time (e.g., 1 second)
            _cache.Set(requestKey, true, TimeSpan.FromSeconds(1));

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
