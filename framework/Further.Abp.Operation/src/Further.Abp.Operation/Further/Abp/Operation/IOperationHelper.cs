using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Volo.Abp.DynamicProxy;

namespace Further.Abp.Operation
{
    public interface IOperationHelper
    {
        bool ShouldIntercept(IAbpMethodInvocation invocation);

        bool IsOperationType(TypeInfo typeInfo);

        bool IsOperationMethod(MethodInfo methodInfo,out OperationScopeAttribute? operationAttribute);

        List<OperationInfoAttributeBase> GetListOperationInfoAttribute(MethodInfo methodInfo);
    }
}
