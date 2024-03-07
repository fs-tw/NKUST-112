using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;

namespace Further.Abp.Operation
{
    public class OperationInterceptor : AbpInterceptor, ITransientDependency
    {
        private readonly IServiceScopeFactory serviceScopeFactory;

        public OperationInterceptor(
            IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!OperationHelper.IsOperationMethod(invocation.Method, out var operationScopeAttribute))
            {
                await invocation.ProceedAsync();
                return;
            }

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var operationScopeProvider = scope.ServiceProvider.GetRequiredService<IOperationScopeProvider>();

                var value = CreateValue(operationScopeAttribute);
                var options = CreateOptions(operationScopeAttribute);

                if (operationScopeProvider.TryBeginReserved(OperationScope.OperationScopeReservationName, options, value))
                {
                    await ProceedByOperationInfoAsync(invocation, operationScopeProvider);

                    return;
                }

                using (var operationScope = operationScopeProvider.Begin(options, value))
                {
                    try
                    {
                        await ProceedByOperationInfoAsync(invocation, operationScopeProvider);
                    }
                    finally
                    {
                        await operationScope.CompleteAsync();
                    }
                }
            }
        }

        private async Task ProceedByOperationInfoAsync(IAbpMethodInvocation invocation, IOperationScopeProvider operationScopeProvider)
        {
            try
            {
                var operationInfoAttrs = OperationHelper.GetOperationInfoAttributes(invocation.Method);

                await invocation.ProceedAsync();

                var result = invocation.ReturnValue;

                if (operationScopeProvider.Current != null)
                {
                    foreach (var operationInfoAttr in operationInfoAttrs)
                    {
                        operationInfoAttr.UpdateOperationInfo(operationScopeProvider.Current, result);
                    }
                }
            }
            catch (Exception ex)
            {

                if (operationScopeProvider.Current == null) throw;

                var failedOperationInfoAttrs = OperationHelper.GetOperationFailAttributes(invocation.Method);

                if (failedOperationInfoAttrs != null)
                {
                    failedOperationInfoAttrs.UpdateOperationInfo(operationScopeProvider.Current, ex);
                }

                if (failedOperationInfoAttrs == null)
                {
                    operationScopeProvider.Current.Result.WithError(ex.Message);
                }

                throw;
            }
        }

        private OperationInfoInitializeValue CreateValue(OperationScopeAttribute? operationScopeAttribute)
        {
            var value = new OperationInfoInitializeValue();

            if (operationScopeAttribute != null)
            {
                value.OperationId = operationScopeAttribute.OperationId;
                value.OperationName = operationScopeAttribute.OperationName;
            }

            return value;
        }

        private OperationScopeOptions CreateOptions(OperationScopeAttribute? operationScopeAttribute)
        {
            var options = new OperationScopeOptions();

            if (operationScopeAttribute != null)
            {
                options.EnabledLogger = operationScopeAttribute.EnabledLogger;
                options.MaxSurvivalTime = operationScopeAttribute.MaxSurvivalTime;
            }

            return options;
        }
    }
}
