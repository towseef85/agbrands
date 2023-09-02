namespace AGBrand.Packages.Filters
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Linq;

    public class TokenHeaderOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            if (!context.ApiDescription.TryGetMethodInfo(out var methodInfo))
            {
                return;
            }

            var skip = methodInfo.CustomAttributes
                .Any(c => typeof(AllowAnonymousAttribute) == c.AttributeType);

            if (!skip)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Required = true,
                    AllowEmptyValue = false,
                    Description = $"{JwtBearerDefaults.AuthenticationScheme} XXX",
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });
            }
        }
    }
}
