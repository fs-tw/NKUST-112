using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
    public class OperationScopeAttribute : Attribute
    {
        public bool IsEnabled { get; set; } = true;

        public bool EnabledLogger { get; set; } = false;

        /// <summary>
        /// 最大保存時間，單位是秒
        /// </summary>
        public int MaxSurvivalTime { get; set; } = 30;

        public string? OperationId { get; set; }

        public string? OperationName { get; set; }

        public OperationScopeAttribute()
        {
        }

        public OperationScopeAttribute(string? operationId = null, string? operationName = null)
        {
            this.OperationId = operationId;
            this.OperationName = operationName;
        }
    }
}
