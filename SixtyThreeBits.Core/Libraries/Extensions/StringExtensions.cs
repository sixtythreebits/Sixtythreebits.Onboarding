using SixtyThreeBits.Libraries;
using SixtyThreeBits.Libraries.Extensions;
using System.Text.RegularExpressions;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static class StringExtensions
    {
        #region Methods
        public static int? DecryptID(this string inputString)
        {
            var base64DecodedString = inputString?.Base64Decode();
            var decryptedString = base64DecodedString.AesDecryptString();
            var ID = decryptedString.ToInt();
            return ID;
        }

        public static T Decrypt<T>(this string inputString)
        {
            var base64DecodedString = inputString?.Base64Decode();
            var decryptedString = base64DecodedString.AesDecryptString();

            if (typeof(T) == typeof(string))
            {
                return (T)(object)decryptedString;
            }
            else
            {
                var deserialized = decryptedString.DeserializeJsonTo<T>();
                return deserialized;
            }            
        }

        public static string Encrypt(this string inputString)
        {
            return inputString.AesEncryptString().Base64Encode();
        }

        public static string GetUserInitials(this string userFullname)
        {
            var initials = default(string);
            if (!string.IsNullOrWhiteSpace(userFullname))
            {
                var parts = userFullname.Split(" ");
                initials = $"{parts[0][0]}{(parts.Length > 1 ? parts[1][0] : null)}".ToUpper();
            }
            return initials;
        }

        public static string StripPhoneNumber(this string PhoneNumber)
        {
            return PhoneNumber?.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Trim();
        }

        public static string StripAndHidePhoneNumber(this string PhoneNumber)
        {
            return $"*** *** *{PhoneNumber.StripPhoneNumber()[^2..]}";
        }

        public static string StripAllExceptNumbers(this string input)
        {
            return input == null ? null : Regex.Replace(input, "[^0-9]", "");
        }

        public static string StripHtml(this string html, string[] exceptTags = null)
        {
            var result = html;
            if (!string.IsNullOrWhiteSpace(result))
            {
                // Strip <script> tags and all their contents
                result = Regex.Replace(result, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", string.Empty, RegexOptions.IgnoreCase);

                // Strip <style> tags and all their contents
                result = Regex.Replace(result, @"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", string.Empty, RegexOptions.IgnoreCase);

                string text = null;
                if (exceptTags != null && exceptTags.Length != 0)
                {
                    string text2 = string.Join("|", exceptTags);
                    text = "(?!</?" + text2 + ">)";
                }

                result = Regex.Replace(result, text + "<.*?>", string.Empty);

                result = result.Replace("&nbsp;", " ").Replace("&quot;", "\"");
                result = Regex.Replace(result, @"[ \t]{2,}", " ");
                result = Regex.Replace(result, @"\n\s+\n", "\n\t\n");
                result = Regex.Replace(result, @"^[ \t]+", "", RegexOptions.Multiline);
                result = result.Trim();
            }

            return result;
        } 
        #endregion
    }
}
