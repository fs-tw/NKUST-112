using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.TextTemplating.VirtualFiles;
using Volo.Abp.TextTemplating;
using Volo.Abp;

namespace Further.Abp.TextTemplate
{
    public static class FurtherTemplateDefinitionExtensions
    {
        public static TemplateDefinition WithRendererModelType(
            [NotNull] this TemplateDefinition templateDefinition,
            [NotNull] Type rendererModelType)
        {
            Check.NotNull(templateDefinition, nameof(templateDefinition));
            Check.NotNull(rendererModelType, nameof(rendererModelType));

            return templateDefinition.WithProperty(
                FurtherTemplateDefinitionConsts.RendererModelTypePropertyName,
                rendererModelType);
        }

        public static TemplateDefinition WithRendererModelType<T>(
            [NotNull] this TemplateDefinition templateDefinition)
        {
            return templateDefinition.WithRendererModelType(typeof(T));
        }
        public static Type? GetRendererModelTypeOrNull(
            [NotNull] this TemplateDefinition templateDefinition)
        {
            Check.NotNull(templateDefinition, nameof(templateDefinition));

            return templateDefinition
                .Properties
                .GetOrDefault(FurtherTemplateDefinitionConsts.RendererModelTypePropertyName) as Type;
        }
    }
}
