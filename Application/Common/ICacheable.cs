namespace ApiEcommerce.Application.Common;

public interface ICacheable 
{ 
    string CacheKey { get; } 
    int ExpirationMinutes { get; } 
}
