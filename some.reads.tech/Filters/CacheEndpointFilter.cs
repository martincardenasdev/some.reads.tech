using System.Text.Json;
using StackExchange.Redis;

namespace some.reads.tech.Filters
{
    public class CacheEndpointFilter(IConnectionMultiplexer redis, ILogger<CacheEndpointFilter> logger) : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var redisDb = redis.GetDatabase();
            
            var httpContext = context.HttpContext;
            var cacheKey = $"{httpContext.Request.Method}:{httpContext.Request.Path}{httpContext.Request.QueryString}";
            
            if (redisDb.KeyExists(cacheKey))
            {
                var cachedValue = redisDb.StringGet(cacheKey);
                logger.LogInformation("Data cache HIT for key: {CacheKey}. Result returned from cache", cacheKey);
                return JsonSerializer.Deserialize<object>(cachedValue);
            }

            var result = await next(context);
            
            if (result is IValueHttpResult valueResult and IStatusCodeHttpResult { StatusCode: 200 })
            {
                logger.LogInformation("Data cache MISS for key: {CacheKey}. Adding result to cache", cacheKey);
                var value = valueResult.Value;
                if (value != null)
                    await redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(value));
            }
            
            return result;
        }
    }
}
