using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public class OperationScopeOptions
    {
        public bool EnabledLogger { get; set; } = false;

        /// <summary>
        /// 最大保存時間，單位是秒
        /// </summary>
        public int MaxSurvivalTime { get; set; } = 30;
    }
}
