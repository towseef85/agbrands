////using System;
////using System.IO;
////using System.Linq;

////using Microsoft.AspNetCore.Builder;
////using Microsoft.AspNetCore.Hosting;
////using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.DependencyInjection;
////using Microsoft.Extensions.PlatformAbstractions;

////using AGBrand.Packages.Extensions;
////using AGBrand.Packages.Filters;
////using AGBrand.Packages.Models.Configs.Services;
////using Swashbuckle.AspNetCore.Swagger;
////using Swashbuckle.AspNetCore.SwaggerGen;
////using Swashbuckle.AspNetCore.SwaggerUI;

////namespace AGBrand.Packages.Services
////{
////    public static class SwaggerService
////    {
////        public static void AddSwaggerGen(this IServiceCollection services, SwaggerGenConfig config, Action<SwaggerGenOptions> swaggerGenAction = null)
////        {
////            services.AddSwaggerGen(options =>
////            {
////                options.SwaggerDoc(
////                    config.AppVersion,
////                    new Info
////                    {
////                        Title = config.AppName,
////                        Version = config.Version,
////                        Description = config.Description,
////                        Contact = new Contact
////                        {
////                            Name = config.Name,
////                            Email = config.Contact
////                        }
////                    });

////                options.DocInclusionPredicate((docName, apiDesc) =>
////                {
////                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();

////                    if (actionApiVersionModel == null)
////                    {
////                        return true;
////                    }

////                    return actionApiVersionModel.DeclaredApiVersions.Count > 0 ?
////                        actionApiVersionModel.DeclaredApiVersions.Any(v => $"{v}" == docName) :
////                        actionApiVersionModel.ImplementedApiVersions.Any(v => $"{v}" == docName);
////                });

////                options.EnableAnnotations();

////                options.IgnoreObsoleteActions();

////                options.IgnoreObsoleteProperties();

////                options.DescribeAllEnumsAsStrings();

////                options.DescribeStringEnumsInCamelCase();

////                // options.TagActionsBy(api => api.HttpMethod);
////                options.SchemaFilter<AutoRestSchemaFilter>();

////                options.OperationFilter<FileUploadOperationFilter>();

////                options.OperationFilter<ApiVersionOperationFilter>();

////                options.OperationFilter<TokenHeaderOperationFilter>();

////                options.OperationFilter<AuthResponsesOperationFilter>();

////                options.OperationFilter<ApiVersionHeaderOperationFilter>();

////                options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, config.AppDocumentation));

////                swaggerGenAction?.Invoke(options);
////            });
////        }

////        public static void UseSwaggerUI(this IApplicationBuilder app, SwaggerUIConfig config, Action<SwaggerUIOptions> swaggerUiAction = null)
////        {
////            app.UseSwaggerUI(options =>
////            {
////                options.SwaggerEndpoint("/swagger/1.0/swagger.json", $"{config.AppName} API v{config.AppVersion}");

////                options.DocumentTitle = config.DocumentTitle;

////                options.DefaultModelExpandDepth(-1);
////                options.DefaultModelRendering(ModelRendering.Model);
////                options.DefaultModelsExpandDepth(-1);
////                options.DisplayOperationId();
////                options.DisplayRequestDuration();
////                options.DocExpansion(DocExpansion.None);
////                options.EnableDeepLinking();

////                // options.EnableFilter();
////                options.MaxDisplayedTags(5);
////                options.ShowExtensions();

////                // options.EnableValidator(); options.SupportedSubmitMethods(SubmitMethod.Get,
////                // SubmitMethod.Post);

////                swaggerUiAction?.Invoke(options);
////            });
////        }
////    }
////}
