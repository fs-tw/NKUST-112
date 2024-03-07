using Further.Operation.Operations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Further.Abp.Operation
{
    public class CacheOperationStore : IOperationStore, ITransientDependency
    {
        /// <summary>
        /// 待後續調整jsonSerialization使用，進一步移除CacheOperationInfo這轉換物件
        /// </summary>
        public class CacheOperationInfo : OperationInfo
        {
            public new OperationResult Result { get; set; }

            public new List<OperationOwnerInfo> Owners { get; set; } = new();

            [JsonConstructor]
            public CacheOperationInfo(Guid id) : base(id)
            {
            }

            public CacheOperationInfo(OperationInfo operationInfo) : base(operationInfo.Id)
            {
                this.OperationId = operationInfo.OperationId;
                this.OperationName = operationInfo.OperationName;
                this.ExecutionDuration = operationInfo.ExecutionDuration;
                this.Result = new OperationResult(operationInfo.Result);
                this.Owners.AddRange(operationInfo.Owners);
            }

            public OperationInfo GetOperationInfo()
            {
                var operation = new OperationInfo(
                    id: this.Id,
                    operationId: this.OperationId,
                    operationName: this.OperationName,
                    result: this.Result.CopyToResult(),
                    owners: this.Owners,
                    executionDuration: this.ExecutionDuration);

                return operation;
            }
        }

        private readonly IDistributedCache<CacheOperationInfo, string> distributedCache;

        public CacheOperationStore(
            IDistributedCache<CacheOperationInfo, string> distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public virtual OperationInfo? Get(Guid id)
        {
            var operation = distributedCache.Get(id.GetCacheKey());

            //return operation;

            return operation?.GetOperationInfo();
        }

        public virtual async Task SaveAsync(OperationInfo? operationInfo, OperationScopeOptions options, CancellationToken cancellationToken = default)
        {
            if (operationInfo == null) return;

            //await distributedCache.SetAsync(operationInfo.GetCacheKey(), operationInfo, new DistributedCacheEntryOptions
            //{
            //    SlidingExpiration = TimeSpan.FromSeconds(options.MaxSurvivalTime)
            //});

            await distributedCache.SetAsync(operationInfo.GetCacheKey(), new CacheOperationInfo(operationInfo), new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromSeconds(options.MaxSurvivalTime)
            });
        }
    }
}
