using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;

namespace Further.Abp.Operation
{
    public class TestOperationAttributeEntity : Entity<Guid>
    {
        public string Name { get; set; }

        public TestOperationAttributeEntity(Guid id)
        {
            Id = id;
            Name = "測試";
        }
    }

    [OperationScope("OperationAttributeTestClass", "測試OperationScope")]
    public class TestOperationAttributeManager : ITransientDependency
    {
        [OperationMessage("測試OperationMessageAttribute")]
        public virtual Task OperationMessageAttributeAsync()
        {
            return Task.CompletedTask;
        }

        [OperationOwner(typeof(TestOperationAttributeEntity))]
        public virtual Task<TestOperationAttributeEntity> OperationOwnerAttributeAsync(Guid id)
        {
            var testClass = new TestOperationAttributeEntity(id);

            return Task.FromResult(testClass);
        }

        [OperationScope("OperationScopeTestMethod", "測試OperationScope能否在方法開啟")]
        public virtual Task OperationScopeTestMethodAsync()
        {
            return Task.CompletedTask;
        }

        [OperationScope("OperationScopeTestMix", "測試OperationAttribute混合")]
        [OperationMessage("混和標籤")]
        [OperationOwner(typeof(TestOperationAttributeEntity))]
        public virtual Task<TestOperationAttributeEntity> OperationScopeTestMixAsync()
        {
            var testClass = new TestOperationAttributeEntity(Guid.NewGuid());
            return Task.FromResult(testClass);
        }

        [OperationMessage("訊息標籤1")]
        [OperationMessage("訊息標籤2")]
        public virtual Task OperationMultiAttributeAsync()
        {
            return Task.CompletedTask;
        }

        [OperationMessage("第一層")]
        public virtual async Task OperationNestAttributeAsync()
        {
            await OperationNestAttributeAsync2();
        }

        [OperationMessage("第二層")]
        protected virtual Task OperationNestAttributeAsync2()
        {
            return Task.CompletedTask;
        }

        [OperationFail("測試OperationFailAttribute")]
        public virtual Task<IOperationScopeProvider> OperationFailAttributeAsync()
        {
            throw new Exception("測試OperationFailAttribute");
        }
    }
}
