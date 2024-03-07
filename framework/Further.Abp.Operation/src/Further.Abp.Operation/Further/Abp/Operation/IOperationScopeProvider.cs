using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Uow;

namespace Further.Abp.Operation
{
    public interface IOperationScopeProvider
    {
        IOperationScope? CurrentScope { get; }

        OperationInfo? Current { get; }

        IOperationScope Begin(OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null, bool requiresNew = false);

        IOperationScope Reserve(string reservationName, bool requiresNew = false);

        void BeginReserved(string reservationName, OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null);

        bool TryBeginReserved(string reservationName, OperationScopeOptions? options = null, OperationInfoInitializeValue? value = null);
    }
}
