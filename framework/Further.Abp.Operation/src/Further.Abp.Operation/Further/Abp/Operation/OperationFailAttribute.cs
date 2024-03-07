using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public class OperationFailAttribute : OperationMessageAttribute
    {
        public OperationFailAttribute(string message) : base(message)
        {
        }

        public override void UpdateOperationInfo(OperationInfo operationInfo, object methodResult)
        {
            var fail = new Error(Message);

            SetMetadata(fail);

            operationInfo?.Result.WithError(fail);
        }
    }
}
