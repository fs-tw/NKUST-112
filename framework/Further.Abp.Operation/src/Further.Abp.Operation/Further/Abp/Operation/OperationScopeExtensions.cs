using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Uow;
using Volo.Abp;

namespace Further.Abp.Operation
{
    public static class OperationScopeExtensions
    {
        public static bool IsReservedFor([NotNull] this IOperationScope operationScope, string reservationName)
        {
            Check.NotNull(operationScope, nameof(operationScope));

            return operationScope.IsReserved && operationScope.ReservationName == reservationName;
        }
    }
}
