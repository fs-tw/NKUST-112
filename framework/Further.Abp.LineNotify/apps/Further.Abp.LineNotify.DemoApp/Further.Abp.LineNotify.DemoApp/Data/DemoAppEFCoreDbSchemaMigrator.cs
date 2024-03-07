using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Further.Abp.LineNotify.DemoApp.Data;

public class DemoAppEFCoreDbSchemaMigrator : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public DemoAppEFCoreDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the DemoAppDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<DemoAppDbContext>()
            .Database
            .MigrateAsync();
    }
}
