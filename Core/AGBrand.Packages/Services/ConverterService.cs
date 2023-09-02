////using System.IO;
////using DinkToPdf;
////using DinkToPdf.Contracts;
////using Microsoft.Extensions.Configuration;
////using Microsoft.Extensions.DependencyInjection;
////using AGBrand.Packages.Helpers;

////namespace AGBrand.Packages.Services
////{
////    public static class ConverterService
////    {
////        public static void AddPdfConverterService(this IServiceCollection services)
////        {
////            var context = new UnmanagedAssemblyLoader();

////            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "Assemblies\\PdfAssemblies\\64bit\\libwkhtmltox.dll"));

////            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
////        }
////    }
////}
