using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Further.Abp.LineNotify
{
    public class DefaultAccessTokenProvider : IAccessTokenProvider, Volo.Abp.DependencyInjection.ITransientDependency
    {
        private readonly IDistributedCache<AccessTokenCacheItem> cache;
        private readonly DistributedCacheKeyPersistProvider distributedCacheKeyPersist;

        public DefaultAccessTokenProvider(
            IDistributedCache<AccessTokenCacheItem> cache,
            DistributedCacheKeyPersistProvider distributedCacheKeyPersist
            )
        {
            this.cache = cache;
            this.distributedCacheKeyPersist = distributedCacheKeyPersist;
        }

        public async Task<AccessTokenCacheItem?> GetAccessTokenAsync(string configuratorName, string subject)
        {

            var result = await cache.GetAsync(LineNotifyConsts.AccessTokenCacheName(configuratorName, subject));

            return result;
        }

        public async Task SetAccessTokenAsync(string configuratorName, string subject, string value)
        {
            var key = LineNotifyConsts.AccessTokenCacheName(configuratorName, subject);

            await cache.SetAsync(key, new AccessTokenCacheItem { AccessToken = value });

            await distributedCacheKeyPersist.KeyPersistAsync<AccessTokenCacheItem>(key);
        }

        public async Task RemoveAccessTokenAsync(string configuratorName, string subject)
        {
            await cache.RemoveAsync(LineNotifyConsts.AccessTokenCacheName(configuratorName, subject));
        }


    }
}
