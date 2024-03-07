using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace Further.Abp.MultiLingualObjects;

[DependsOn(
    typeof(AbpLocalizationModule))]
public class AbpMultiLingualObjectsModule : AbpModule
{
}
