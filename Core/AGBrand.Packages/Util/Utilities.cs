using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using AGBrand.Packages.Helpers;
using AGBrand.Packages.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AGBrand.Packages.Util
{
    public static class Utilities
    {
        ////public static IHttpClientFactory MockHttpClientFactory<T>(T value, HttpStatusCode statusCode)
        ////{
        ////    IHttpClientFactory httpClientFactory;

        ////    var clientHandlerStub = new HttpClientFactoryInterceptor((request, cancellationToken) =>
        ////    {
        ////        request.SetConfiguration(new HttpConfiguration
        ////        {
        ////        });

        ////        var response = request.CreateResponse(statusCode, value);

        ////        return Task.FromResult(response);
        ////    });

        ////    var httpClient = new HttpClient(clientHandlerStub)
        ////    {
        ////        BaseAddress = new Uri("http://localhost")
        ////    };

        ////    httpClientFactory = Substitute.For<IHttpClientFactory>();

        ////    httpClientFactory.CreateClient(Arg.Any<string>()).Returns(httpClient);

        ////    return httpClientFactory;
        ////}

        public static string CreateBasicAuthToken(string username, string password)
        {
            return $"Basic {$"{username}:{password}".Base64Encode()}";
        }

        ////    return settings;
        ////}
        public static string CreateHashKey(string data, string key)
        {
            var encoding = new ASCIIEncoding();

            using var hmacsha256Crypt = new HMACSHA256(encoding.GetBytes(key));

            var hash = hmacsha256Crypt.ComputeHash(encoding.GetBytes(data /*+ nonce*/));

            return Convert.ToBase64String(hash);
        }

        ////    settings.Converters.Add(new IsoDateTimeConverter
        ////    {
        ////        DateTimeFormat = serializerSettings.DateTimeFormat,
        ////        DateTimeStyles = DateTimeStyles.AdjustToUniversal
        ////    });
        public static string GenerateNonce(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randomObj = new Random();

            var nonceString = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                nonceString.Append(validChars[randomObj.Next(0, validChars.Length - 1)]);
            }

            return nonceString.ToString();
        }

        ////public static JsonSerializerSettings GetSerializerSettings(ISerializerSettings serializerSettings)
        ////{
        ////    var settings = new JsonSerializerSettings
        ////    {
        ////        NullValueHandling = NullValueHandling.Ignore
        ////    };
        public static bool GetBitStatus(long number, int bitNumber)
        {
            return (number & (1 << bitNumber)) != 0;
        }

        public static void GetDateRange(string range, out DateTime pastDate, out DateTime currentDate)
        {
            var today = DateTime.UtcNow;
            var lastDay = DateTime.UtcNow.AddDays(-1);
            var lastWeek = DateTime.UtcNow.AddDays(-7);
            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var lastYear = DateTime.UtcNow.AddYears(-1);
            var overall = new DateTime(1970, 1, 1);

            currentDate = new DateTime(today.Year, today.Month, today.Day, 23, 59, 59);

            switch (range.ToLower(CultureInfo.InvariantCulture))
            {
                case "week":
                    pastDate = new DateTime(lastWeek.Year, lastWeek.Month, lastWeek.Day, 0, 0, 0);
                    break;

                case "month":
                    pastDate = new DateTime(lastMonth.Year, lastMonth.Month, lastMonth.Day, 0, 0, 0);
                    break;

                case "year":
                    pastDate = new DateTime(lastYear.Year, lastYear.Month, lastYear.Day, 0, 0, 0);
                    break;

                case "overall":
                    pastDate = new DateTime(overall.Year, overall.Month, overall.Day, 0, 0, 0);
                    break;

                case "day":
                    pastDate = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 0, 0, 0);
                    currentDate = new DateTime(lastDay.Year, lastDay.Month, lastDay.Day, 23, 59, 59);
                    break;

                default:
                    pastDate = new DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
                    break;
            }
        }

        public async static Task<List<T>> GetDeserializedContent<T>(string fileName)
        {
            var fileContent = await File.ReadAllTextAsync(fileName).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<List<T>>(fileContent);
        }

        public static void GetEpochRange(string range, out double pastDate, out double currentDate)
        {
            GetDateRange(range, out var pastDateDt, out var currentDateDt);

            pastDate = pastDateDt.ToEpoch();
            currentDate = currentDateDt.ToEpoch();
        }

        public static GridModel<T> GetGridModel<T>(this PagerArgs args, string defaultSortKey)
        {
            var gm = new GridModel<T>
            {
                Pager = new GridPager
                {
                    Pages = new List<GridPage>()
                }
            };

            gm.Pager.CurrentPage = args.CurrentPage ?? 1;

            gm.Pager.PageSize = args.PageSize ?? 10;

            gm.SortKey = string.IsNullOrWhiteSpace(args.SortKey) ? defaultSortKey : args.SortKey;

            gm.SortDirection = args.SortDirection;

            return gm;
        }

        public static string GetQueryStringValue(this string relativePath, string queryItem)
        {
            const string UriString = "https://localhost/";

            Uri baseUri = new Uri(UriString);

            Uri myUri = new Uri(baseUri, relativePath);

            var query = HttpUtility.ParseQueryString(myUri.Query);

            if (query.HasKeys() && query.AllKeys.Any(c => c == queryItem))
            {
                return query.Get(queryItem);
            }

            return null;
        }

        public static int GetRandomNumber(int start, int end)
        {
            var rand = new Random();
            var number = rand.Next(start, end);

            return number;
        }

        public static string GetRandomString(int length)
        {
            var builder = new StringBuilder();
            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString().ToLower();
        }

        public static List<OListItem> GetSelectList(List<OListItem> items,
            bool takeValue = false,
            bool friendlyText = false,
            bool friendlyValue = false,
            string selectedValue = null,
            bool insertSelectOption = false,
            bool selectOptionSelected = true,
            string selectOptionLabel = "-- Select --",
            string selectOptionValue = null,
            bool isSelectOptionDisabled = true,
            bool translate = false,

            //string locale = "en-US",
            bool isTextTranslatable = false)
        {
            items = items.Select(x => new OListItem
            {
                Text = GetListItemText(x, isTextTranslatable, translate, /*locale,*/ friendlyText),
                Value = GetListItemValue(x, takeValue, friendlyValue),
                Selected = IsListItemSelected(x, takeValue, friendlyValue, selectedValue)
            }).ToList();

            if (insertSelectOption)
            {
                var item = new OListItem
                {
                    Selected = string.IsNullOrWhiteSpace(selectedValue) && selectOptionSelected,
                    Text = selectOptionLabel,
                    Value = selectOptionValue,
                    Disabled = isSelectOptionDisabled
                };
                items.Insert(0, item);
            }

            return items;
        }

        public static bool IsExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return true;
            }

            var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

            return DateTime.UtcNow > securityToken.ValidTo;
        }

        public static bool IsFileSizeOk(Stream content, int maxSizeInMb)
        {
            var maxFileSizeBytes = maxSizeInMb * 1024 * 1024;

            return content.Length <= maxFileSizeBytes;
        }

        public static string MaskEmail(string email)
        {
            const string pattern = @"(?<=[\w]{2})[\w-\._\+%]*(?=[\w]{2}.)";
            return Regex.Replace(email, pattern, m => new string('*', m.Length));
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private static string GetListItemText(OListItem x, bool isTextTranslatable, bool translate, /*string locale,*/ bool isFriendlyText)
        {
            var friendlyText = isFriendlyText ? x.Text.ToFriendlyCase() : x.Text;

            if (!isTextTranslatable)
            {
                return friendlyText;
            }

            return translate ? x.Text /*LanguageHelper.GetKey(x.Text, locale)*/ : friendlyText;
        }

        private static object GetListItemValue(OListItem x, bool takeValue, bool isFriendlyValue)
        {
            //if isFriendlyValue is true then the friendly value returned is from the text because value can be number also
            var friendlyValue = isFriendlyValue ? x.Text.ToFriendlyCase() : x.Text;

            //takeValue true will always return the value which can be a string or a number
            return takeValue ? x.Value : friendlyValue;
        }

        private static bool IsListItemSelected(OListItem item, bool takeValue, bool firendlyValue, string selectedValue)
        {
            if (item.Selected)
            {
                return item.Selected;
            }

            if (string.IsNullOrWhiteSpace(selectedValue))
            {
                return false;
            }

            if (takeValue)
            {
                return item.Value.ToString() == selectedValue;
            }

            //friendlyValue is taken from Text because Value can be a number
            return firendlyValue ? item.Text.ToFriendlyCase() == selectedValue : item.Text == selectedValue;
        }

        public static FileUploadResult UploadFile(IWebHostEnvironment iWebHostEnvironment, IFormFile content, FileUploadSettings fs, string customExtensions = "")
        {
            var fr = new FileUploadResult
            {
                IsSuccess = false,
                NoFileSelected = true,
                FileSizeInvalid = true,
                InvalidFileType = true,
                Message = fs.Messages.Failed
            };
            var maxFileSizeBytes = (fs.MaxSize * 1024) * 1024;

            if (content != null)
            {
                fr.NoFileSelected = false;

                if (content.Length > maxFileSizeBytes)
                {
                    fr.Message = fs.Messages.FileSizeInvalid;
                    return fr;
                }

                fr.FileSizeInvalid = false;

                var extension = Path.GetExtension(content.FileName);

                if (ValidateFileType(extension, fs.FileType, customExtensions))
                {
                    fr.InvalidFileType = false;

                    var fileName = $"{Guid.NewGuid().ToString().Replace("-", "").ToLower()}{extension}";
                    var relativePath = fs.StoragePath + fileName;
                    string webRootPath = iWebHostEnvironment.WebRootPath;
                    string absolutePath = Path.Combine(webRootPath, relativePath);

                    if (!Directory.Exists(Path.Combine(webRootPath, fs.StoragePath)))
                    {
                        Directory.CreateDirectory(Path.Combine(webRootPath, fs.StoragePath));
                    }


                    try
                    {
                        using (var stream = new FileStream(absolutePath, FileMode.Create))
                        {
                            content.CopyToAsync(stream);
                        }

                        fr.IsSuccess = true;
                        fr.Result = relativePath;
                        fr.Message = fs.Messages.Success;
                    }
                    catch (Exception ex)
                    {
                        fr.Message = ExceptionHelper.GetExceptionMessage(ex);
                    }
                }
                else
                {
                    fr.Message = fs.Messages.InvalidFileType;
                }
            }
            else
            {
                fr.Message = fs.Messages.NoFileSelected;
            }

            return fr;
        }

        public static bool ValidateFileType(string extension, FileType fileType, string customExtensions = "")
        {
            string[] imageExtensions = { ".png", ".bmp", ".jpeg", ".jpg", ".gif" };

            string[] docExtensions = { ".pdf", ".txt", ".doc", ".docx", ".xls", ".xlsx", ".pps", ".ppsx" };

            string[] pdfExtensions = { ".pdf" };

            string[] extensions = { };

            switch (fileType)
            {
                case FileType.All:
                    return true;

                case FileType.Document:
                    extensions = docExtensions;
                    break;

                case FileType.Image:
                    extensions = imageExtensions;
                    break;

                case FileType.PDF:
                    extensions = pdfExtensions;
                    break;

            }

            return extensions.Contains(extension.ToLower());
        }
    }
}
