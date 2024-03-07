using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace Further.Abp.Operation
{
    public class AmbientOperationScope : IAmbientOperationScope, ISingletonDependency
    {
        public IOperationScope? OperationScope => Current.Value;

        private readonly AsyncLocal<IOperationScope?> Current;

        public AmbientOperationScope()
        {
            Current = new AsyncLocal<IOperationScope?>();
        }

        public IOperationScope? GetCurrentOperationScope()
        {
            var operationScope = OperationScope;

            while (operationScope != null && (operationScope.IsReserved || operationScope.IsDisposed || operationScope.IsCompleted))
            {
                operationScope = operationScope.Outer;
            }

            return operationScope;
        }

        public void SetOperationScope(IOperationScope? operationScope)
        {
            Current.Value = operationScope;
        }

        public OperationInfo? GetOperationInfo()
        {
            return GetCurrentOperationScope()?.OperationInfo;
        }
    }
}
