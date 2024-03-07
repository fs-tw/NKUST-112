using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public interface IAmbientOperationScope
    {
        IOperationScope? OperationScope { get; }

        void SetOperationScope(IOperationScope? operationScope);

        IOperationScope? GetCurrentOperationScope();

        OperationInfo? GetOperationInfo();
    }
}
