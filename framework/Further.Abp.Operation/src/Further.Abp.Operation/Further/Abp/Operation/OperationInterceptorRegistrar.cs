using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Further.Abp.Operation
{
    public static class OperationInterceptorRegistrar
    {
        public static void RegisterIfNeeded(IOnServiceRegistredContext context)
        {
            if (OperationHelper.ShouldIntercept(context.ImplementationType))
            {
                context.Interceptors.TryAdd<OperationInterceptor>();
            }
        }
    }
}
