using ApiEcommerce.Application.Common;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ApiEcommerce.Application.Behaviors;

public class CacheBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IDistributedCache _cache;
    public CacheBehavior(IDistributedCache cache) => _cache = cache;
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        try
        {
            if (request is not ICacheable c)
                return await next();

            var cached = await _cache.GetStringAsync(c.CacheKey, ct);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<TResponse>(cached)!;

            var response = await next();

            await _cache.SetStringAsync(
                c.CacheKey,
                JsonSerializer.Serialize(response),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(c.ExpirationMinutes)
                },
                ct
            );

            return response;
        }
        catch
        {
            return await next();
        }
    }
}
