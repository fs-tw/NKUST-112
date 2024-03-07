using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Testing;

namespace Further.Abp.Operation
{
    public class OperationTestBase : AbpIntegratedTest<FurtherAbpOperationTestBaseModule>
    {
        protected readonly IOperationScopeProvider operationScopeProvider;

        public OperationTestBase()
        {
            operationScopeProvider = GetRequiredService<IOperationScopeProvider>();
        }

        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
