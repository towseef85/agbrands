using System;

namespace AGBrand.Packages.Helpers
{
    public static class ExceptionHelper
    {
        public static string GetExceptionMessage(Exception ex)
        {
            var message = "<br /><strong>Error occurred while performing this operation</strong><br /><br />";
            message += $"<strong>Error Message:</strong> {ex.Message}<br /><br />";
            message +=
                $"<strong>Error Details:</strong> {((ex.InnerException == null) ? string.Empty : ((ex.InnerException.InnerException == null) ? ex.InnerException.ToString() : ex.InnerException.InnerException.ToString()))}<br /><br />";
            message += $"<strong>Stack Trace:</strong> {ex.StackTrace}<br /><br />";
            message +=
                $"<strong>Method Name:</strong> {(ex.TargetSite != null ? ex.TargetSite.Name : string.Empty)}<br /><br />";
            message += $"<strong>Source:</strong> {ex.Source}<br /><br />";
            return message;
        }
    }
}