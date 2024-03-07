using FluentResults;
using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public abstract class OperationInfoAttributeBase : Attribute
    {
        public Dictionary<string, object> Metadata { get; set; } = new();
        public abstract void UpdateOperationInfo(OperationInfo operationInfo, object methodResult);

        protected void SetMetadata(IReason reason)
        {
            foreach (var metadata in Metadata)
            {
                reason.Metadata.Add(metadata.Key, metadata.Value);
            }
        }
    }
}
