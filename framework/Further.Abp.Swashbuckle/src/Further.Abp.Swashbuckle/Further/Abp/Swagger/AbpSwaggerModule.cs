using Microsoft.Extensions.DependencyInjection;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;
using Volo.Abp.Reflection;

namespace Further.Abp.Swashbuckle;

[DependsOn(typeof(Volo.Abp.Swashbuckle.AbpSwashbuckleModule))]
public class AbpSwashbuckleModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<SwashbuckleOptions>(options =>
        {
            var finder = context.Services.GetSingletonInstance<ITypeFinder>();

            var assemblies = finder.Types
                .Where(x => x.IsAssignableTo(typeof(IRemoteService)))
                .Where(x => x.IsAssignableTo(typeof(AbpControllerBase)) || x.IsAssignableTo(typeof(AbpController)))
                .Select(x => x.Assembly).Distinct();

            assemblies.ToList().ForEach(assembly =>
            {
                var name = assembly.GetName().Name;
                if (name == null) return;

                options.Configurators.TryAdd(name, new List<Assembly>() { assembly });
            });
        });
    }


    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var options = context.Services.ExecutePreConfiguredActions<SwashbuckleOptions>();

        Configure<Swashbuckle.SwashbuckleOptions>(o =>
        {
            options.Configurators.ForEach(x =>
            {
                o.Configurators.TryAdd(x.Key, x.Value);
            });
        });

    }
}
