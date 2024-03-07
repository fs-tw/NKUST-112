using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Reflection;

namespace Further.Abp.Swashbuckle
{
    public static class AbpSwashbuckleExtensions
    {
        public static IServiceCollection AddAbpSwaggerGenWithOAuth(
            this IServiceCollection services,
            [NotNull] string authority,
            [NotNull] Dictionary<string, string> scopes,
            Action<SwaggerGenOptions>? optionAction = null,
            string authorizationEndpoint = "/connect/authorize",
            string tokenEndpoint = "/connect/token")
        {
            return AbpSwaggerGenServiceCollectionExtensions.AddAbpSwaggerGenWithOAuth(
                services.AddAbpSwaggerGen(),
                authority,
                scopes,
                optionAction,
                authorizationEndpoint,
                tokenEndpoint);
        }

        public static IServiceCollection AddAbpSwaggerGenWithOidc(
        this IServiceCollection services,
        [NotNull] string authority,
        string[]? scopes = null,
        string[]? flows = null,
        string? discoveryEndpoint = null,
        Action<SwaggerGenOptions>? setupAction = null)
        {
            return AbpSwaggerGenServiceCollectionExtensions.AddAbpSwaggerGenWithOidc(
                services.AddAbpSwaggerGen(),
                authority,
                scopes,
                flows,
                discoveryEndpoint,
                setupAction);
        }

        public static IServiceCollection AddAbpSwaggerGen(
            this IServiceCollection services,
            Action<SwaggerGenOptions>? optionAction = null)
        {
            var serviceProvider = services.BuildServiceProvider();
            var swashbuckleOptions = serviceProvider.GetRequiredService<IOptions<SwashbuckleOptions>>().Value;

            var keys = swashbuckleOptions.Configurators.Select(x => x.Key).ToList();

            return services
                .AddSwaggerGen(options =>
                 {
                     options.SwaggerDoc("All", new OpenApiInfo { Title = "All", Version = "v1" });

                     foreach (var item in keys)
                     {
                         options.SwaggerDoc(item, new OpenApiInfo { Title = item, Version = "v1" });
                     }

                     options.DocInclusionPredicate((Func<string, Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription, bool>)((docName, description) =>
                     {
                         if (docName == "All") return true;

                         if (!swashbuckleOptions.Configurators.ContainsKey(docName))
                             return false;

                         if (description.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                         {
                             var configurator = swashbuckleOptions.Configurators[docName];

                             var target = controllerActionDescriptor.ControllerTypeInfo.Assembly;

                             if (configurator.Contains(target))
                             {
                                 return true;
                             }
                         }

                         return false;
                     }));
                     options.CustomSchemaIds(type => CalculateTypeName(type));

                     options.CustomOperationIds(apiDescription =>
                     {
                         return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? (apiDescription.GroupName + methodInfo.Name).Replace("Async", "") : null;
                     });

                     options.AddEnumsWithValuesFixFilters(o =>
                     {
                         // add schema filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema
                         o.ApplySchemaFilter = true;

                         // alias for replacing 'x-enumNames' in swagger document
                         o.XEnumNamesAlias = "x-enum-varnames";

                         // alias for replacing 'x-enumDescriptions' in swagger document
                         o.XEnumDescriptionsAlias = "x-enum-descriptions";

                         // add parameter filter to fix enums (add 'x-enumNames' for NSwag or its alias from XEnumNamesAlias) in schema parameters
                         o.ApplyParameterFilter = true;

                         // add document filter to fix enums displaying in swagger document
                         o.ApplyDocumentFilter = true;

                         // add descriptions from DescriptionAttribute or xml-comments to fix enums (add 'x-enumDescriptions' or its alias from XEnumDescriptionsAlias for schema extensions) for applied filters
                         o.IncludeDescriptions = true;

                         // add remarks for descriptions from xml-comments
                         o.IncludeXEnumRemarks = true;

                         // get descriptions from DescriptionAttribute then from xml-comments
                         o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;

                         // new line for enum values descriptions
                         // o.NewLine = Environment.NewLine;
                         o.NewLine = "\n";

                         // get descriptions from xml-file comments on the specified path
                         // should use "options.IncludeXmlComments(xmlFilePath);" before
                         //o.IncludeXmlCommentsFrom(xmlFilePath);
                         // the same for another xml-files...
                     });

                     options.UseAllOfForInheritance();
                     options.UseOneOfForPolymorphism();

                     optionAction?.Invoke(options);
                 }
            );

            static string CalculateTypeName(Type type)
            {
                if (!type.IsGenericTypeDefinition)
                {
                    return TypeHelper.GetFullNameHandlingNullableAndGenerics(type);
                }

                var i = 0;
                var argumentList = type
                    .GetGenericArguments()
                    .Select(_ => "T" + i++)
                    .JoinAsString(",");

                return $"{type.FullName!.Left(type.FullName!.IndexOf('`'))}<{argumentList}>";
            }
        }

        public static IApplicationBuilder UseAbpSwaggerUI(
            this IApplicationBuilder app,
            Action<SwaggerUIOptions>? optionAction = null)
        {
            var swashbuckleOptions = app.ApplicationServices.GetRequiredService<IOptions<SwashbuckleOptions>>().Value;

            var keys = swashbuckleOptions.Configurators.Select(x => x.Key).ToList();

            return AbpSwaggerUIBuilderExtensions.UseAbpSwaggerUI(app, options =>
            {
                options.SwaggerEndpoint($"/swagger/All/swagger.json", "All");

                foreach (var item in keys)
                {
                    options.SwaggerEndpoint($"/swagger/{item}/swagger.json", item);
                }

                optionAction?.Invoke(options);
            });
        }
    }
}
