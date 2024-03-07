using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Json;

namespace Further.Abp.Operation
{
    [Dependency(ReplaceServices = true)]
    public class TestOperationScope : OperationScope
    {
        public TestOperationScope(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            await base.CompleteAsync(cancellationToken);

            throw new OperationScopeCompleteException(this.OperationInfo);
        }
    }

    [Serializable]
    public class OperationScopeCompleteException : BusinessException
    {
        public OperationInfo? OperationInfo { get; }

        public OperationScopeCompleteException(OperationInfo? operationInfo)
        {
            this.OperationInfo = operationInfo;
        }
    }
}
