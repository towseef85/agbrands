using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AGBrand.Packages.Util
{
    public static class RegexUtilities
    {
        public static void DumpHRefs(string inputString)
        {
            Match m;
            string hRefPattern = @"href\s*=\s*(?:[""'](?<1>[^""']*)[""']|(?<1>\S+))";

            try
            {
                m = Regex.Match(inputString, hRefPattern,
                                RegexOptions.IgnoreCase | RegexOptions.Compiled,
                                TimeSpan.FromSeconds(1));

                while (m.Success)
                {
                    Console.WriteLine("Found href " + m.Groups[1] + " at " + m.Groups[1].Index);

                    m = m.NextMatch();
                }
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine("The matching operation timed out.");
            }
        }

        /// <summary>
        /// Match Phone Number as begin with, one of "+", ( capture (digit, once), never or more,
        /// one of "- .(", never or more, capture (digit, exactly 3 times), one of "- .)", never or
        /// more, digit, exactly 3 times, one of "- .", never or more, digit, exactly 4 times ),
        /// once or more, must end
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool IsMobileNumber(string mobile)
        {
            return Regex.IsMatch(mobile, @"^[+](?:([0-9]{1})*[- .(]*([0-9]{3})[- .)]*[0-9]{3}[- .]*[0-9]{4})+$");
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string StripInvalidCharacters(string text)
        {
            return Regex.Replace(text, @"[^\w\._@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1.5));
        }
    }
}
