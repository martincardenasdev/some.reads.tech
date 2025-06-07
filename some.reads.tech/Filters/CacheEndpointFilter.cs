using Microsoft.Extensions.Caching.Memory;

namespace some.reads.tech.Filters
{
    public class CacheEndpointFilter(IMemoryCache memoryCache) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var cacheKey = $"{httpContext.Request.Method}:{httpContext.Request.Path}{httpContext.Request.QueryString}";

            if (memoryCache.TryGetValue(cacheKey, out var cachedResponse))
                return cachedResponse;

            var result = await next(context);

            if (result is IResult)
                memoryCache.Set(cacheKey, result, TimeSpan.FromMinutes(5));

            return result;
        }
    }
}
