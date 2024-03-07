using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating;

namespace Further.Abp.TextTemplate;

[DependsOn(
       typeof(AbpTextTemplatingCoreModule)
    )]
public class FurtherAbpTextTemplateModule : AbpModule
{
}
