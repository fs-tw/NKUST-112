using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace Further.Abp.LineNotify;

[DependsOn(
    typeof(AbpLineNotifyAbstractionsModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpCachingStackExchangeRedisModule)
    )]
public class AbpLineNotifyModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClient(LineNotifyConsts.HttpClientName);
    }
}
