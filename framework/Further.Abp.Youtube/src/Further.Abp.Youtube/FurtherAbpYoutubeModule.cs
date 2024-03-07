using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Further.Abp.Youtube;

[DependsOn(typeof(FurtherAbpYoutubeAbstractionsModule))]
public class FurtherAbpYoutubeModule : AbpModule
{
}
