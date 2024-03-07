using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace Further.Abp.TextTemplate
{
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    public class FurtherTemplateRenderer : AbpTemplateRenderer
    {
        public FurtherTemplateRenderer(
            IServiceScopeFactory serviceScopeFactory, 
            ITemplateDefinitionManager templateDefinitionManager, 
            IOptions<AbpTextTemplatingOptions> options) 
            : base(serviceScopeFactory, templateDefinitionManager, options)
        {
        }

        public async override Task<string> RenderAsync(string templateName, object? model = null, string? cultureName = null, Dictionary<string, object>? globalContext = null)
        {
            var templateDefinition = await TemplateDefinitionManager.GetAsync(templateName);

            var rendererModelType = templateDefinition.GetRendererModelTypeOrNull();

            if(rendererModelType != null)
            {
                if (!rendererModelType.IsAssignableFrom(model?.GetType()))
                {
                    throw new ArgumentException($"Invalid model type for template '{templateName}'! Expected type: {rendererModelType.FullName}");
                }
            }

            return await base.RenderAsync(templateName, model, cultureName, globalContext);
        }
    }
}
