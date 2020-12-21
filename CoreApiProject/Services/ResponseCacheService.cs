﻿using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApiProject.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _DistributedCache;

        public ResponseCacheService(IDistributedCache distributedCache)
        {
            _DistributedCache = distributedCache;
        }
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response is null) return;
            var serializedResponse = JsonConvert.SerializeObject(response);
            await _DistributedCache.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions {AbsoluteExpirationRelativeToNow = timeToLive });
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            return await _DistributedCache.GetStringAsync(cacheKey);
        }
    }
}
