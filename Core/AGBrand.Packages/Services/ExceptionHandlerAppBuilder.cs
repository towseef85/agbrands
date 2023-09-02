////using Microsoft.AspNetCore.Builder;
////using Microsoft.AspNetCore.Diagnostics;
////using Microsoft.AspNetCore.Http;
////using Microsoft.Extensions.Logging;
////using AGBrand.Packages.Models.Configs.Services;
////using AGBrand.Packages.Util;

////using System;

////namespace AGBrand.Packages.Services
////{
////    public static class ExceptionHandlerAppBuilder
////    {
////        public static void MapExceptionHandler(this IApplicationBuilder app,
////            ILoggerFactory loggerFactory,
////            ExceptionHandlerAppBuilderConfig config,
////            Action<IApplicationBuilder> appBuilderAction = null)
////        {
////            //TOFIX
////            ////loggerFactory.AddApplicationInsights(app.ApplicationServices);
////            ////Microsoft.Extensions.Logging.ApplicationInsightsLoggingBuilderExtensions.AddApplicationInsights()

////            app.UseExceptionHandler(appBuilder =>
////            {
////                appBuilder.Run(async context =>
////                {
////                    var error = context.Features.Get<IExceptionHandlerFeature>();

////                    if (error != null)
////                    {
////                        var (message, id) = error.Error.GetExceptionMessage();

////                        var logger = loggerFactory.CreateLogger(config.LoggerName);
////                        logger.Log(LogLevel.Critical, message);

////                        context.Response.StatusCode = config.StatusCode;
////                        context.Response.ContentType = config.ContentType;

////                        await context.Response.WriteAsync(new
////                        {
////                            message = config.IsSecureEnvironment ? $"Exception Id: {id}" : message
////                        }.ToString()).ConfigureAwait(false);
////                    }
////                });

////                appBuilderAction?.Invoke(appBuilder);
////            });
////        }
////    }
////}
