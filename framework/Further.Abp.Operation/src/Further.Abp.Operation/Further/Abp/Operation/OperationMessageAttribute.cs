using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class OperationMessageAttribute: OperationInfoAttributeBase
    {
        public string Message { get; set; }

        public override void UpdateOperationInfo(OperationInfo operationInfo, object methodResult)
        {
            var success = new Success(Message);

            SetMetadata(success);

            operationInfo?.Result.WithSuccess(success);
        }

        public OperationMessageAttribute(string message)
        {
            this.Message = message;
        }

        public OperationMessageAttribute(string message, Dictionary<string, object> metadata)
        {
            this.Message = message;
            this.Metadata = metadata;
        }
    }
}
