using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;

namespace Caching.Example.PipelineBehaviors
{
    public class CachingBehavior<TRequest, TResponse>(RedisService redisService) : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var redisDatabase = redisService.Database;

            var cacheKey = $"{request.GetType().FullName}";

            TResponse cachedResponse = null;

            var cachedData = await redisDatabase.StringGetAsync(cacheKey);
            if (cachedData.HasValue)
            {
                var cachedResponseJson = Encoding.UTF8.GetString(cachedData);
                cachedResponse = JsonSerializer.Deserialize<TResponse>(cachedResponseJson);
            }

            //Cache'de varsa o veriyi döndürüyoruz.
            if (cachedResponse != null)
                return cachedResponse;

            //Cache'de yoksa gerçek sorguyu yürütüyoruz.
            var response = await next();

            //Yeni veriyi cache'e alıyoruz.
            var responseJson = JsonSerializer.Serialize(response);
            var encodedData = Encoding.UTF8.GetBytes(responseJson);

            var result = await redisDatabase.StringSetAsync(cacheKey, encodedData, expiry: TimeSpan.FromSeconds(60));

            return response;
        }
    }
}
