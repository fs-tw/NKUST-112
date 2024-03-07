using System;
using System.Collections.Generic;
using System.Text;

namespace Further.Abp.Operation
{
    public class OperationOwnerInfo
    {
        public string EntityType { get; set; } = null!;

        public Guid EntityId { get; set; }

        public Dictionary<string, object> MetaData { get; set; } = new Dictionary<string, object>();
    }
}
