using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;

namespace Further.Abp.Swashbuckle
{
    public class SwashbuckleOptions
    {
        public Dictionary<string, List<Assembly>> Configurators { get; } = new();
    }
}
