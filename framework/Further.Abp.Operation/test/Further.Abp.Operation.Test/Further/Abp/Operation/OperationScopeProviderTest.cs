using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Testing;
using Volo.Abp.Threading;
using Xunit;

namespace Further.Abp.Operation
{
    public class OperationScopeProviderTest : OperationTestBase
    {
        [Fact]
        public async Task Should_Create_Nested_OperationScope()
        {
            operationScopeProvider.Current.ShouldBeNull();

            try
            {
                using (var scope1 = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = "Scope1" }))
                {
                    operationScopeProvider.Current.ShouldNotBeNull();
                    operationScopeProvider.Current.ShouldBe(scope1.OperationInfo);
                    operationScopeProvider.Current.OperationName.ShouldBe("Scope1");

                    using (var scope2 = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = "Scope2" }))
                    {
                        operationScopeProvider.Current.ShouldNotBeNull();
                        operationScopeProvider.Current.ShouldBe(scope2.OperationInfo);
                        operationScopeProvider.Current.OperationName.ShouldBe("Scope2");

                        await scope2.CompleteAsync();
                    }

                    operationScopeProvider.Current.ShouldNotBeNull();
                    operationScopeProvider.Current.ShouldBe(scope1.OperationInfo);

                    await scope1.CompleteAsync();
                }
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.OperationName.ShouldBe("Scope2");
            }

            operationScopeProvider.Current.ShouldBeNull();
        }

        [Fact]
        public async Task Should_Create_Nested_ReqiiredNew_OperationScope()
        {
            operationScopeProvider.Current.ShouldBeNull();

            try
            {
                using (var scope1 = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = "Scope1" }))
                {
                    operationScopeProvider.Current.ShouldNotBeNull();
                    operationScopeProvider.Current.ShouldBe(scope1.OperationInfo);
                    operationScopeProvider.Current.OperationName.ShouldBe("Scope1");

                    try
                    {
                        using (var scope2 = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = "Scope2" }, requiresNew: true))
                        {
                            operationScopeProvider.Current.ShouldNotBeNull();
                            operationScopeProvider.Current.ShouldBe(scope2.OperationInfo);
                            operationScopeProvider.Current.OperationName.ShouldBe("Scope2");

                            await scope2.CompleteAsync();
                        }
                    }
                    catch (OperationScopeCompleteException ex)
                    {
                        ex.OperationInfo.OperationName.ShouldBe("Scope2");
                    }

                    operationScopeProvider.Current.ShouldNotBeNull();
                    operationScopeProvider.Current.ShouldBe(scope1.OperationInfo);

                    await scope1.CompleteAsync();
                }
            }
            catch (OperationScopeCompleteException ex)
            {
                ex.OperationInfo.OperationName.ShouldBe("Scope1");
            }

            operationScopeProvider.Current.ShouldBeNull();
        }

        [Fact]
        public async Task EnsureConcurrentAccessToScopeProviderBehavesCorrectly()
        {
            var tasks = new List<Task>();

            int numberOfTasks = 20;
            for (int i = 0; i < numberOfTasks; i++)
            {
                var value = i.ToString();
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var scope = operationScopeProvider.Begin(value: new OperationInfoInitializeValue { OperationName = $"Scope{value}" }))
                        {
                            operationScopeProvider.Current.ShouldNotBeNull();
                            operationScopeProvider.Current.ShouldBe(scope.OperationInfo);
                            operationScopeProvider.Current.OperationName.ShouldBe($"Scope{value}");

                            var changed = $"changed{value}";

                            operationScopeProvider.Current.OperationName = changed;

                            Task.Delay(10).Wait();

                            operationScopeProvider.Current.OperationName.ShouldBe(changed);

                            await scope.CompleteAsync();
                        }
                    }
                    catch (OperationScopeCompleteException ex)
                    {
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
