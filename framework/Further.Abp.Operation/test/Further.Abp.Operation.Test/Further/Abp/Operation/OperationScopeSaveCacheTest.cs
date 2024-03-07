using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Xunit;

namespace Further.Abp.Operation
{
    public class OperationScopeSaveCacheTest : OperationTestBase
    {
        private readonly IOperationStore operationStore;

        public OperationScopeSaveCacheTest()
        {
            this.operationStore = GetRequiredService<IOperationStore>();
        }

        [Fact]
        public async Task OperationScopeSaveCache()
        {
            var value = new OperationInfoInitializeValue
            {
                OperationId = "OperationScopeSaveCache",
                OperationName = "OperationScopeSaveCache"
            };

            var entityId = Guid.NewGuid();
            var entityType = "Test";

            try
            {

                using (var operationScope = operationScopeProvider.Begin(value: value))
                {
                    operationScopeProvider.Current.Result.WithSuccess("OperationRedisSaveSuccess");
                    operationScopeProvider.Current.Owners.Add(new OperationOwnerInfo
                    {
                        EntityId = entityId,
                        EntityType = entityType
                    });
                    await operationScope.CompleteAsync();
                }
            }
            catch (OperationScopeCompleteException ex)
            {
                var operationInfo = operationStore.Get(ex.OperationInfo.Id);

                Assert.NotNull(operationInfo);
                Assert.Equal(
                operationInfo.Result.Successes.First().Message,
                "OperationRedisSaveSuccess");
                Assert.Equal(
                    operationInfo.Owners.First().EntityId,
                    entityId);
                Assert.Equal(
                    operationInfo.Owners.First().EntityType,
                    entityType);
                Assert.Equal(operationInfo.Result.Reasons.Count, 1);
            }
        }
    }
}
