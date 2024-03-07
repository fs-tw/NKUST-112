using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace Further.Abp.Npoi.Mapper
{
    [DependsOn(
        typeof(Volo.Abp.VirtualFileSystem.AbpVirtualFileSystemModule)
        )]
    public class AbpNpoiMapperModule : AbpModule
    {
    }
}
