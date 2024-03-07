using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Further.Abp.Operation
{
    public class OperationAttributeTest : OperationTestBase
    {
        private readonly TestOperationAttributeManager testOperationAttributeManager;

        public OperationAttributeTest()
        {
            testOperationAttributeManager = GetRequiredService<TestOperationAttributeManager>();
        }

        [Fact]
        public async Task OperationScopeAttribute()
        {
            operationScopeProvider.Current.ShouldBeNull();

            try
            {
                await testOperationAttributeManager.OperationScopeTestMethodAsync();
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.OperationId.ShouldBe("OperationScopeTestMethod");

                return;
            }

            operationScopeProvider.Current.ShouldBeNull();
        }

        [Fact]
        public async Task OperationMessageAttribute()
        {
            try
            {
                await testOperationAttributeManager.OperationMessageAttributeAsync();
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.Result.Successes.First().Message.ShouldBe("測試OperationMessageAttribute");

                return;
            }

            throw new Exception("測試失敗");
        }

        [Fact]
        public async Task OperationOwnerAttribute()
        {
            var id = Guid.NewGuid();

            try
            {
                await testOperationAttributeManager.OperationOwnerAttributeAsync(id);
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.Owners.First().EntityId.ShouldBe(id);

                var test = ex.OperationInfo;

                return;
            }

            throw new Exception("測試失敗");
        }

        [Fact]
        public async Task OperationScopeTestMix()
        {
            try
            {
                await testOperationAttributeManager.OperationScopeTestMixAsync();
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.OperationId.ShouldBe("OperationScopeTestMix");
                ex.OperationInfo.Result.Successes.First().Message.ShouldBe("混和標籤");
                ex.OperationInfo.Owners.First().EntityId.ShouldNotBe(Guid.Empty);

                return;
            }

            throw new Exception("測試失敗");
        }

        [Fact]
        public async Task OperationMultiAttributeAttribute()
        {
            try
            {
                await testOperationAttributeManager.OperationMultiAttributeAsync();
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.Result.Successes.Count.ShouldBe(2);
                ex.OperationInfo.Result.Successes.First().Message.ShouldBe("訊息標籤1");
                ex.OperationInfo.Result.Successes.Last().Message.ShouldBe("訊息標籤2");

                return;
            }

            throw new Exception("測試失敗");
        }

        [Fact]
        public async Task OperationNestAttribute()
        {
            try
            {
                await testOperationAttributeManager.OperationNestAttributeAsync();
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.Result.Successes.Count.ShouldBe(2);
                ex.OperationInfo.Result.Successes.First().Message.ShouldBe("第二層");
                ex.OperationInfo.Result.Successes.Last().Message.ShouldBe("第一層");

                return;
            }

            throw new Exception("測試失敗");
        }

        [Fact]
        public async Task OperationFailAttribute()
        {
            using (var scope = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = "OperationFailAttribute" }))
            {
                try
                {
                    await testOperationAttributeManager.OperationFailAttributeAsync();
                }
                catch (Exception ex)
                {
                    var operationInfo = operationScopeProvider.Current;

                    operationInfo.Result.Errors.First().Message.ShouldBe("測試OperationFailAttribute");

                    return;
                }
            }

            throw new Exception("測試失敗");
        }
    }
}
