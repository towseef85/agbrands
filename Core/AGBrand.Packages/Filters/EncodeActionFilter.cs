using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Web;

using AGBrand.Packages.Attributes;

namespace AGBrand.Packages.Filters
{
    public sealed class EncodeActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // For each of the items in the PUT/POST
            foreach (var item in context.ActionArguments.Values)
            {
                // Get the type of the object
                var type = item.GetType();

                // For each property of this object, html encode it if it is having the encode attribute
                foreach (var propertyInfo in type.GetProperties().Where(c => c.GetCustomAttributes(false).OfType<EncodeAttribute>().Any()))
                {
                    var propertyValue = propertyInfo.GetValue(item);

                    if (propertyValue is string value)
                    {
                        propertyInfo.SetValue(item, HttpUtility.HtmlEncode(value));
                    }
                }
            }
        }
    }
}
