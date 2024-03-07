using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Further.Abp.Data
{
    [DependsOn(
        typeof(Volo.Abp.Data.AbpDataModule)
        )]
    public class AbpDataModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //ontext.Services.AddAutoMapperObjectMapper<AbpDataModule>();
        }
    }
}
