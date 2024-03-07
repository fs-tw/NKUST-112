using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public static class OperationCacheKeyExtensions
    {
        public static string GetCacheKey(this OperationInfo operationInfo)
        {
            return $"Operation:{operationInfo.Id}";
        }

        public static string GetCacheKey(this Guid operationId)
        {
            return $"Operation:{operationId}";
        }
    }
}
