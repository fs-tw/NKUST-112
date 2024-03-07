using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Further.Abp.Operation
{
    public interface IOperationStore
    {
        Task SaveAsync(OperationInfo? operationInfo, OperationScopeOptions options, CancellationToken cancellationToken = default);

        OperationInfo? Get(Guid id);
    }
}
