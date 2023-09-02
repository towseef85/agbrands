using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;

namespace AGBrand.Packages.Services
{
    public static class MapAllServicesRouteAppBuilder
    {
        public static void MapAllServicesRoute(this IApplicationBuilder app, IServiceCollection services)
        {
            app.Map(
                "/allservices",
                builder => builder.Run(
                    context =>
                    {
                        var sb = new StringBuilder();

                        sb.Append(
                            "<h1 style='font-weight:bold;font-family:verdana;line-height:35px;font-size:25px;text-align:center;'>All Services</h1>");

                        sb.Append("<table><thead>");

                        sb.Append(
                            "<tr style='background: #333; color:#fff;font-weight:bold;font-family:verdana;line-height:25px;font-size:15px;'>");

                        sb.Append(
                            "<th style='padding:0px 20px 0px 20px;'>Type</th><th style='padding:0px 20px 0px 20px;'>Lifetime</th><th style='padding:0px 20px 0px 20px;'>Instance</th></tr>");

                        sb.Append("</thead><tbody>");

                        foreach (var svc in services.OrderBy(c => c.Lifetime))
                        {
                            sb.Append("<tr>");

                            sb.Append("<td style='border-bottom:1px solid black;line-height:15px;font-family:verdana;font-size:12px;'>")
                                .Append(svc.ServiceType.FullName).Append("</td>");

                            sb.Append("<td style='border-bottom:1px solid black;line-height:15px;font-family:verdana;font-size:12px;'>")
                                .Append(svc.Lifetime).Append("</td>");

                            sb.Append("<td style='border-bottom:1px solid black;line-height:15px;font-family:verdana;font-size:12px;'>")
                                .Append(svc.ImplementationType?.FullName).Append("</td>");

                            sb.Append("</tr>");
                        }

                        sb.Append("</tbody></table>");

                        return context.Response.WriteAsync(sb.ToString());
                    }));
        }
    }
}
