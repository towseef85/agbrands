using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace AGBrand.Packages.Filters
{
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FileUploadOperationFilter : IOperationFilter
    {
        private const string FormDataMimeType = "multipart/form-data";
        private static readonly string[] FormFilePropertyNames = typeof(IFormFile).GetTypeInfo().DeclaredProperties.Select(p => p.Name).ToArray();

        ////public void Apply(Operation operation, OperationFilterContext context)
        ////{
        ////    var parameters = operation.Parameters;
        ////    if (parameters == null || parameters.Count == 0)
        ////    {
        ////        return;
        ////    }

        ////    var formFileParameterNames = new List<string>();
        ////    var formFileSubParameterNames = new List<string>();

        ////    foreach (var actionParameter in context.ApiDescription.ActionDescriptor.Parameters)
        ////    {
        ////        var properties =
        ////            actionParameter.ParameterType.GetProperties()
        ////                .Where(p => p.PropertyType == typeof(IFormFile))
        ////                .Select(p => p.Name)
        ////                .ToArray();

        ////        if (properties.Length != 0)
        ////        {
        ////            formFileParameterNames.AddRange(properties);
        ////            formFileSubParameterNames.AddRange(properties);
        ////            continue;
        ////        }

        ////        if (actionParameter.ParameterType != typeof(IFormFile))
        ////        {
        ////            continue;
        ////        }

        ////        formFileParameterNames.Add(actionParameter.Name);
        ////    }

        ////    if (!formFileParameterNames.Any())
        ////    {
        ////        return;
        ////    }

        ////    var consumes = operation.Consumes;
        ////    consumes.Clear();
        ////    consumes.Add(FormDataMimeType);

        ////    foreach (var parameter in parameters
        ////        .ToArray()
        ////        .Where(parameter => parameter is NonBodyParameter && parameter.In == "formData")
        ////        .Where(parameter => formFileSubParameterNames
        ////                                .Any(p => parameter.Name.StartsWith(p + ".", StringComparison.Ordinal)) ||
        ////                            FormFilePropertyNames.Contains(parameter.Name)))
        ////    {
        ////        parameters.Remove(parameter);
        ////    }

        ////    foreach (var formFileParameter in formFileParameterNames)
        ////    {
        ////        parameters.Add(new NonBodyParameter
        ////        {
        ////            Name = formFileParameter,
        ////            Type = "file",
        ////            In = "formData"
        ////        });
        ////    }
        ////}

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = operation.Parameters;

            if (parameters == null || parameters.Count == 0)
            {
                return;
            }

            var formFileParameterNames = new List<string>();
            var formFileSubParameterNames = new List<string>();

            foreach (var actionParameter in context.ApiDescription.ActionDescriptor.Parameters)
            {
                var properties =
                    actionParameter.ParameterType.GetProperties()
                        .Where(p => p.PropertyType == typeof(IFormFile))
                        .Select(p => p.Name)
                        .ToArray();

                if (properties.Length != 0)
                {
                    formFileParameterNames.AddRange(properties);
                    formFileSubParameterNames.AddRange(properties);
                    continue;
                }

                if (actionParameter.ParameterType != typeof(IFormFile))
                {
                    continue;
                }

                formFileParameterNames.Add(actionParameter.Name);
            }

            if (!formFileParameterNames.Any())
            {
                return;
            }

            ////var consumes = operation.Consumes;
            ////consumes.Clear();
            ////consumes.Add(FormDataMimeType);

            foreach (var parameter in parameters
                .Where(parameter => parameter.Style == ParameterStyle.Form)
                .Where(parameter => formFileSubParameterNames
                    .Any(p => parameter.Name.StartsWith(p + ".", StringComparison.Ordinal)) ||
                                                        FormFilePropertyNames.Contains(parameter.Name)))
            {
                parameters.Remove(parameter);
            }

            var param = new OpenApiParameter
            {
                Style = ParameterStyle.Form,
                Schema = new OpenApiSchema
                {
                    Type = "file"
                },
            };

            foreach (var formFileParameter in formFileParameterNames)
            {
                param.Name = formFileParameter;
                parameters.Add(param);
            }
        }
    }
}
