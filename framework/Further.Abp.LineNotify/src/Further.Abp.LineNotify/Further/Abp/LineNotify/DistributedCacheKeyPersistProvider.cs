using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.MultiTenancy;

namespace Further.Abp.LineNotify
{
    public class DistributedCacheKeyPersistProvider : Volo.Abp.DependencyInjection.ISingletonDependency
    {
        private readonly IDistributedCache cache;
        private readonly IDistributedCacheKeyNormalizer keyNormalizer;

        public DistributedCacheKeyPersistProvider(IDistributedCache cache, IDistributedCacheKeyNormalizer keyNormalizer)
        {
            this.cache = cache;
            this.keyNormalizer = keyNormalizer;
        }
        public async Task KeyPersistAsync<TCacheItem>(string key)
        {
            var redisDatabaseProperty = typeof(AbpRedisCache).GetProperty("RedisDatabase", BindingFlags.Instance | BindingFlags.NonPublic);

            var redisDatabase = (redisDatabaseProperty!.GetValue(cache) as StackExchange.Redis.IDatabase);

            var cacheName = CacheNameAttribute.GetCacheName(typeof(TCacheItem));

            var ignoreMultiTenancy = typeof(TCacheItem).IsDefined(typeof(IgnoreMultiTenancyAttribute), true);

            var normalizeKey = keyNormalizer.NormalizeKey(
                new DistributedCacheKeyNormalizeArgs(
                    key.ToString()!,
                    cacheName,
                    ignoreMultiTenancy
                    )
                );

            await redisDatabase!.KeyPersistAsync(normalizeKey);
        }
    }
}
