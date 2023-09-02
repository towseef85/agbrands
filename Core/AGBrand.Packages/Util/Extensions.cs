using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
////using System.Data.Entity.Validation;
using AGBrand.Packages.Attributes;

namespace AGBrand.Packages.Util
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Dynamic.Core;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using AGBrand.Packages.Contracts;

    ////using RestSharp;

    public static class Extensions
    {
        public static IQueryable<T> ApplyPagingFilter<T>(this IQueryable<T> entitySet, GridModel<T> gm)
        {
            gm.Count = entitySet.Count();

            gm.BuildPager();

            var sortDirection = gm.SortDirection;

            gm.SortDirection = sortDirection switch
            {
                SortDirection.DESC => SortDirection.ASC,
                SortDirection.ASC => SortDirection.DESC,
                _ => SortDirection.ASC,
            };

            return entitySet.OrderBy(gm.SortKey + Constants.Placeholder.Space + sortDirection).Skip((gm.Pager.CurrentPage - 1) * gm.Pager.PageSize).Take(gm.Pager.PageSize);
        }

        public static Dictionary<string, object> AsObjectDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public static Dictionary<string, string> AsStringDictionary<T>(this T obj)
        {
            return obj.GetType()
                .GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(obj).ToString());
        }

        public static string Base64Decode(this string encodedText)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));
        }

        public static string Base64Encode(this string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        public static TResponse Desrialize<TResponse>(this string content, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return (jsonSerializerSettings == null) ? JsonConvert.DeserializeObject<TResponse>(content) : JsonConvert.DeserializeObject<TResponse>(content, jsonSerializerSettings);
        }

        public static double DoubleParse(this string number, double nullReturn = 0)
        {
            if (!double.TryParse(number, out var value))
            {
                value = nullReturn;
            }

            return value;
        }

        public static List<OListItem> EnumToList(this Type type, string filter = "")
        {
            return (from fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField)
                    let rawConstantValue = fieldInfo.GetRawConstantValue()
                    where EnumFilter(fieldInfo, filter)
                    select new OListItem
                    {
                        Name = GetDisplayName(fieldInfo),
                        Text = fieldInfo.Name,
                        Value = rawConstantValue,
                        Description = GetMemberDescription(fieldInfo)
                    }).ToList();
        }

        public static IQueryable<T> FilterBySearchTerm<T>(this IQueryable<T> query, Expression<Func<T, bool>> expression, string searchTerm)
        {
            return !string.IsNullOrWhiteSpace(searchTerm) ? query.Where(expression) : query;
        }

        ////    return tcs.Task;
        ////}
        public static IEnumerable<T> FlattenHierarchy<T>(this T node, Func<T, IEnumerable<T>> getChildEnumerator)
        {
            yield return node;

            if (getChildEnumerator?.Invoke(node) == null)
            {
                yield break;
            }

            foreach (var childOrDescendant in getChildEnumerator(node).SelectMany(child => child.FlattenHierarchy(getChildEnumerator)))
            {
                yield return childOrDescendant;
            }
        }

        ////    client.ExecuteAsync(request, response =>
        ////    {
        ////        if (response.ErrorException != null)
        ////        { tcs.TrySetException(response.ErrorException); }
        ////        else
        ////        { tcs.TrySetResult(response); }
        ////    });
        public static string GetDisplayName(FieldInfo fieldInfo)
        {
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>(false);

            return string.IsNullOrWhiteSpace(displayAttribute?.GetName()) ? fieldInfo.Name : displayAttribute.GetName();
        }

        ////public static Task<IRestResponse> ExecuteTaskAsync(this RestClient client, RestRequest request)
        ////{
        ////    var tcs = new TaskCompletionSource<IRestResponse>();
        public static TAttribute GetEnumAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();

            var name = Enum.GetName(type, value);

            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .FirstOrDefault();
        }

        public static T GetEnumByDisplayName<T>(this string displayName)
        {
            var enumItems = EnumToList(typeof(T));

            foreach (var enumItem in enumItems)
            {
                var enumByText = (T)Enum.Parse(typeof(T), enumItem.Text, ignoreCase: true);

                Type enumType = enumByText.GetType();

                string enumItemByValue = Enum.GetName(enumType, enumItem.Value);

                var displayAttribute = enumType.GetField(enumItemByValue).GetCustomAttributes(inherit: false).OfType<DisplayNameAttribute>().FirstOrDefault();

                if (displayAttribute != null && displayAttribute.DisplayName == displayName)
                {
                    return enumByText;
                }
            }

            return default;
        }

        public static T GetEnumByName<T>(this string enumName)
        {
            return (T)Enum.Parse(typeof(T), enumName, true);
        }

        public static string GetEnumDescription(this Enum value)
        {
            var type = value.GetType();

            var fieldInfo = type.GetField(value.ToString());

            return GetMemberDescription(fieldInfo);
        }

        public static int GetEnumGrade(this Enum value)
        {
            var type = value.GetType();

            var fieldInfo = type.GetField(value.ToString());

            var gradeAttribute = ((EnumGradeAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumGradeAttribute), false)).FirstOrDefault();

            return gradeAttribute?.Grade ?? int.MinValue;
        }

        /// <summary>Gets the SelectListItem Collection from the enum.</summary>
        /// <param name="type">Enum Type</param>
        /// <param name="takeValue">
        /// if set to <c>true</c> the value of the SelectListItem will be the enum integer value.
        /// </param>
        /// <param name="friendlyText">convert text to friendly text</param>
        /// <param name="friendlyValue">
        /// if set to <c>true</c> the value of the SelectListItem will be space separated.
        /// </param>
        /// <param name="selectedValue">
        /// The default selected value in the SelectListItem collection
        /// </param>
        /// <param name="insertSelectOption">
        /// if set to <c>true</c> the first item will be the Select prompt.
        /// </param>
        /// <param name="selectOptionSelected">
        /// if set to <c>true</c> the Select prompt will be selected by default.
        /// </param>
        /// <param name="selectOptionLabel">The label of the Select prompt</param>
        /// <param name="selectOptionValue">The value of the Select prompt</param>
        /// <param name="isSelectOptionDisabled">
        /// if set to <c>true</c> the Select prompt will be disabled and unelectable.
        /// </param>
        /// <param name="translate">activates translation</param>
        /////// <param name="locale">locale to use while translating the text</param>
        public static List<OListItem> GetEnumList(this Type type,
               bool takeValue = false,
               bool friendlyText = true,
               bool friendlyValue = false,
               string selectedValue = null,
               bool insertSelectOption = false,
               bool selectOptionSelected = true,
               string selectOptionLabel = "-- Select --",
               string selectOptionValue = null,
               bool isSelectOptionDisabled = true,
               bool translate = false,
               ////string locale = "en-US",
               bool isTextTranslatable = true)
        {
            return Utilities.GetSelectList(EnumToList(type),
                takeValue,
                friendlyText,
                friendlyValue,
                selectedValue,
                insertSelectOption,
                selectOptionSelected,
                selectOptionLabel,
                selectOptionValue,
                isSelectOptionDisabled,
                translate,
                ////locale,
                isTextTranslatable);
        }

        public static string GetEnumName(this Enum e)
        {
            return e.ToString();
        }

        public static (string message, string id) GetExceptionMessage(this Exception ex, object obj = null)
        {
            var id = Guid.NewGuid();

            StringBuilder message = new StringBuilder();

            message.Append($"Correlation Id: {id}. \n");
            message.Append($"Error Message:  {ex.Message}. \n");
            message.Append($"{(ex.InnerException == null ? string.Empty : $"Error Details:  {GetInnerException(ex)}")}. \n");
            message.Append($"{(ex.TargetSite != null ? $"Method Name: {ex.TargetSite.Name}" : string.Empty)}. \n");
            message.Append($"Source:  {ex.Source}. \n");
            message.Append($"Stack Trace: {ex.StackTrace}. \n");

            if (obj != null)
            {
                message.Append($"\nObject: {JsonConvert.SerializeObject(obj)}. \n");
            }

            ////var exceptionType = ex.GetType();
            ////if (exceptionType == typeof(DbEntityValidationException))
            ////{
            ////    var errorMessages = ((DbEntityValidationException)ex).EntityValidationErrors
            ////        .SelectMany(x => x.ValidationErrors)
            ////        .Select(x => x.ErrorMessage);

            ////    // Join the list to a single string.
            ////    var fullErrorMessage = string.Join("; ", errorMessages);

            ////    // Combine the original exception message with the new one.
            ////    var exceptionMessage = $"The validation errors are: {fullErrorMessage}. \n";

            ////    // Throw a new DbEntityValidationException with the improved exception message.
            ////    message.Append(exceptionMessage);
            ////}

            return (message.ToString(), id.ToString());

            static string GetInnerException(Exception ex)
            {
                return ex.InnerException.InnerException == null ? ex.InnerException.ToString() : ex.InnerException.InnerException.ToString();
            }
        }

        public static List<T> GetIds<T>(this string ids, string separator = ";")
        {
            return string.IsNullOrWhiteSpace(ids) ? null : ids.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => (T)Convert.ChangeType(c, typeof(T))).ToList();
        }

        public static List<object> GetIds(this string ids, Type type, string separator = ";")
        {
            return string.IsNullOrWhiteSpace(ids) ? null : ids.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => Convert.ChangeType(c, type)).ToList();
        }

        public static string GetMemberDescription(this MemberInfo fieldInfo)
        {
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return !attributes.Any() ? fieldInfo.Name : attributes.FirstOrDefault()?.Description;
        }

        public static T GetObject<T>(this Dictionary<string, object> dict)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var (key, value) in dict)
            {
                var property = obj.GetType().GetProperty(key);

                if (property != null)
                {
                    var underlyningPropertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if (underlyningPropertyType.IsEnum && underlyningPropertyType.IsEnumDefined(value))
                    {
                        var safeValue = Enum.Parse(underlyningPropertyType, value.ToString(), true);

                        property.SetValue(obj, safeValue, null);
                    }
                    else
                    {
                        var safeValue = (value == null) ? null : Convert.ChangeType(value, underlyningPropertyType);

                        property.SetValue(obj, safeValue, null);
                    }
                }
            }

            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static StringContent GetStringHttpContent(this object obj, JsonSerializerSettings jsonSerializerSettings = null, string mediaType = "application/json")
        {
            return (jsonSerializerSettings == null) ? new StringContent(Serialize(obj), Encoding.UTF8, mediaType) : new StringContent(Serialize(obj, jsonSerializerSettings), Encoding.UTF8, mediaType);
        }

        public static int IntParse(this string number, int nullReturn = 0)
        {
            var dotPos = number.IndexOf('.');

            if (dotPos > 0)
            {
                number = number.Remove(dotPos);
            }

            if (!int.TryParse(number, out var value))
            {
                value = nullReturn;
            }

            return value;
        }

        public static bool Is(this object statusType, StatusType type)
        {
            var status = (StatusType)Enum.Parse(typeof(StatusType), statusType.ToString());

            return status == type;
        }

        public static string Join<T>(this IEnumerable<T> collection, string delimeter = ";")
        {
            return string.Join(delimeter, collection) + delimeter;
        }

        public static string Join<T>(this List<T> collection, string delimeter = ";")
        {
            return string.Join(delimeter, collection) + delimeter;
        }

        public static T MergeLeft<T, TKey, TValue>(this T me, params IDictionary<TKey, TValue>[] others) where T : IDictionary<TKey, TValue>, new()
        {
            var newMap = new T();

            foreach (var (key, value) in (new List<IDictionary<TKey, TValue>> { me }).Concat(others).SelectMany(src => src))
            {
                newMap[key] = value;
            }

            return newMap;
        }

        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri)
        {
            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("PATCH"),
                RequestUri = new Uri(client.BaseAddress + requestUri)
            };

            return client.SendAsync(request);
        }

        public static IActionResult Respond<T>(this ServiceStatus<T> serviceStatus, IApiRespondService apiRespondService)
        {
            apiRespondService.HttpContextAccessor.HttpContext.Response.Headers.Add("X-Frame-Options", "deny");
            apiRespondService.HttpContextAccessor.HttpContext.Response.Headers.Add("X-Xss-Protection", "1");
            apiRespondService.HttpContextAccessor.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            apiRespondService.HttpContextAccessor.HttpContext.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

            apiRespondService.HttpContextAccessor.HttpContext.Response.Headers.Add("message", serviceStatus.Message);

            apiRespondService.ContextLogger.Commit(EventLogEntryType.SuccessAudit, Guid.NewGuid().ToString());

            if (serviceStatus.Object == null)
            {
                return new ObjectResult(new
                {
                    message = serviceStatus.Message
                })
                {
                    StatusCode = (int)serviceStatus.Code
                };
            }
            else
            {
                return new ObjectResult(serviceStatus.Object)
                {
                    StatusCode = (int)serviceStatus.Code
                };
            }
        }

        public static async Task<IActionResult> RespondAsync<T>(this Task<ServiceStatus<T>> task, IApiRespondService apiRespondService)
        {
            try
            {
                var result = await task.ConfigureAwait(false);

                return Respond(result, apiRespondService);
            }
            catch (Exception ex)
            {
                var (message, id) = ex.GetExceptionMessage();

                apiRespondService.ContextLogger.Log(message);

                apiRespondService.ContextLogger.Commit(EventLogEntryType.Error, id);

                return new ObjectResult(new
                {
                    message = apiRespondService.IsSecureEnvironment ? $"Exception Logged With Correlation Id: {id}" : message
                })
                {
                    StatusCode = 500
                };
            }
        }

        public static string Serialize(this object obj, JsonSerializerSettings jsonSerializerSettings = null)
        {
            return (jsonSerializerSettings == null) ? JsonConvert.SerializeObject(obj) : JsonConvert.SerializeObject(obj, jsonSerializerSettings);
        }

        public static DateTime ToDateTimeUtc(this double epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
        }

        public static double ToEpoch(this DateTime date)
        {
            if (date == default)
            {
                return int.MinValue;
            }

            var epoch = new DateTime(1970, 1, 1, 0, 0, 0);
            var epochTimeSpan = date - epoch;
            return epochTimeSpan.TotalSeconds;
        }

        public static string ToFriendlyCase(this string pascalString, int trimEnd = 0, int trimStart = 0)
        {
            if (trimEnd > 0)
            {
                pascalString = pascalString.Remove(pascalString.Length - trimEnd);
            }

            if (trimStart > 0)
            {
                pascalString = pascalString.Substring(trimStart);
            }

            pascalString = Regex.Replace(pascalString, "(?!^)([A-Z])", " $1");

            return pascalString;
        }

        public static string ToQueryString(this NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                where !string.IsNullOrWhiteSpace(value)
                select string.Format("{0}={1}",
            HttpUtility.UrlEncode(key),
            HttpUtility.UrlEncode(value))).ToArray();

            return string.Join("&", array);
        }

        public static Task<T> ToTask<T>(this T value)
        {
            return Task.FromResult(value);
        }

        public static DateTime ToTimeZone(this DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, sourceTimeZoneId, destinationTimeZoneId);
        }

        public static IdUpdater<T> UpdatedIds<T>(this IdUpdater<T> updaterModel)
        {
            updaterModel.IdsToAdd = updaterModel.NewIds.Where(c => !updaterModel.ExistingIds.Contains(c)).ToList();
            updaterModel.IdsToRemove = updaterModel.ExistingIds.Where(c => !updaterModel.NewIds.Contains(c)).ToList();

            return updaterModel;
        }

        public static (bool IsValid, List<ValidationResult> ValidationResult) ValidateModel<T>(this T model)
        {
            var validationResult = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(model, new ValidationContext(model, null, null), validationResult);

            return (IsValid: isValid, ValidationResult: validationResult);
        }

        private static void BuildPager<T>(this GridModel<T> gm)
        {
            gm.Pager.TotalPages = (int)Math.Ceiling(decimal.Divide(gm.Count, gm.Pager.PageSize));

            if (gm.Pager.CurrentPage > gm.Pager.TotalPages)
            {
                gm.Pager.CurrentPage = gm.Pager.TotalPages;
            }

            if (gm.Pager.CurrentPage < 1)
            {
                gm.Pager.CurrentPage = 1;
            }

            var startPage = gm.Pager.CurrentPage - 5;
            var endPage = gm.Pager.CurrentPage + 4;

            if (startPage <= 0)
            {
                endPage -= startPage - 1;
                startPage = 1;
            }

            if (endPage > gm.Pager.TotalPages)
            {
                endPage = gm.Pager.TotalPages;
                if (endPage > 10)
                {
                    startPage = endPage - 9;
                }
            }

            gm.Pager.StartPage = startPage;
            gm.Pager.EndPage = endPage;

            for (var x = gm.Pager.StartPage; x <= gm.Pager.EndPage; x++)
            {
                gm.Pager.Pages.Add(new GridPage
                {
                    SortKey = gm.SortKey,
                    SortDirection = gm.SortDirection,
                    PageNumber = x,
                    Text = x.ToString(),
                    CssClass = x == gm.Pager.CurrentPage ? gm.Pager.LinkSelectedCssClass : string.Empty
                });
            }

            if (gm.Pager.TotalPages <= 1)
            {
                return;
            }

            if (gm.Pager.CurrentPage != 1)
            {
                gm.Pager.Pages.Insert(0, new GridPage
                {
                    SortKey = gm.SortKey,
                    SortDirection = gm.SortDirection,
                    PageNumber = 1,
                    Text = gm.Pager.FirstLinkText,
                    CssClass = gm.Pager.FirstLinkCssClass
                });
            }

            if (gm.Pager.CurrentPage != gm.Pager.TotalPages)
            {
                gm.Pager.Pages.Insert(gm.Pager.Pages.Count, new GridPage
                {
                    SortKey = gm.SortKey,
                    SortDirection = gm.SortDirection,
                    PageNumber = gm.Pager.TotalPages,
                    Text = gm.Pager.LastLinkText,
                    CssClass = gm.Pager.LastLinkCssClass
                });
            }

            if (gm.Pager.CurrentPage > 1)
            {
                gm.Pager.Pages.Insert(1, new GridPage
                {
                    SortKey = gm.SortKey,
                    SortDirection = gm.SortDirection,
                    PageNumber = gm.Pager.CurrentPage - 1,
                    Text = gm.Pager.PreviousLinkText,
                    CssClass = gm.Pager.PreviousLinkCssClass
                });
            }

            if (gm.Pager.CurrentPage < gm.Pager.TotalPages)
            {
                gm.Pager.Pages.Insert(gm.Pager.Pages.Count - 1, new GridPage
                {
                    SortKey = gm.SortKey,
                    SortDirection = gm.SortDirection,
                    PageNumber = gm.Pager.CurrentPage + 1,
                    Text = gm.Pager.NextLinkText,
                    CssClass = gm.Pager.NextLinkCssClass
                });
            }
        }

        private static bool EnumFilter(ICustomAttributeProvider fieldInfo, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return true;
            }

            var attributes = (EnumFilterAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumFilterAttribute), false);

            //checks if at least one instance of the EnumFilter attribute exists or not
            return !attributes.Any() || attributes.Any(c => c.Filter == filter);

            //returns true for those enum which does not have any EnumFilter attribute defined.
            //These enum return in every collection as default included.
        }
    }
}
