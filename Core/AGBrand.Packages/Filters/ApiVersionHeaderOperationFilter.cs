////namespace AGBrand.Packages.Filters
////{
////    using System.Collections.Generic;

////    using Models;

////    using Swashbuckle.AspNetCore.Swagger;
////    using Swashbuckle.AspNetCore.SwaggerGen;

////    public class ApiVersionHeaderOperationFilter : IOperationFilter
////    {
////        public void Apply(Operation operation, OperationFilterContext context)
////        {
////            if (operation.Parameters == null)
////            {
////                operation.Parameters = new List<IParameter>();
////            }

////            operation.Parameters.Add(new Parameter
////            {
////                Name = "x-api-version",
////                In = "header",
////                Required = true
////            });
////        }
////    }
////}
