using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching;

namespace WeatherService.Extensions
{
    public static class CacheExtensions
    {
        public static TItem Set<TItem>(this IMemoryCache cache, object key, TItem value, DateTimeOffset absoluteExpiration, int size)
        {
            return cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = absoluteExpiration,
                Size = size
            });
        }
    }
}
