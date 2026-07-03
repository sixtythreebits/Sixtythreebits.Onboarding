using Newtonsoft.Json;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static class StringExtensions
    {
        #region Properties
        const string _encryptionKeyDefault = "FxE4*ECD|834e-47j348dkjas*cl#@iB";
        static readonly FrozenDictionary<string, string> _georgianEnglishChars =  new Dictionary<string,string>
        {
            { "ა","a" },
            { "ბ","b" },
            { "გ","g" },
            { "დ","d" },
            { "ე","e" },
            { "ვ","v" },
            { "ზ","z" },
            { "თ","t" },
            { "ი","i" },
            { "კ","k" },
            { "ლ","l" },
            { "მ","m" },
            { "ნ","n" },
            { "ო","o" },
            { "პ","p" },
            { "ჟ","zh"},
            { "რ","r" },
            { "ს","s" },
            { "ტ","t" },
            { "უ","u" },
            { "ფ","f" },
            { "ქ","k" },
            { "ღ","gh" },
            { "ყ","k" },
            { "შ","sh" },
            { "ჩ","ch" },
            { "ც","c" },
            { "ძ","dz" },
            { "წ","ts" },
            { "ჭ","ch" },
            { "ხ","kh" },
            { "ჯ","j" },
            { "ჰ","h" }
        }.ToFrozenDictionary();
        #endregion Properties

        #region Methods                                
        public static string AesEncryptString(this string inputString, string key = _encryptionKeyDefault, CipherMode mode = CipherMode.CBC, AesEncryptedOutputTextFormat format = AesEncryptedOutputTextFormat.Base64)
        {
            if (inputString == null)
            {
                return null;
            }
            else
            {
                if (key.Length != 16 && key.Length != 24 && key.Length != 32) { throw new Exception("Key length must be: 16 for 128 bits key size, 24 for 192 bits key size or 32 for 256 bits key size"); }

                var encryptedBytes = default(byte[]);

                using (var aes = Aes.Create())
                {
                    var keyBase64 = key.Base64Encode(Encoding.ASCII);
                    aes.Key = Convert.FromBase64String(keyBase64);
                    aes.IV = new byte[16];
                    aes.Mode = mode;

                    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream((Stream)ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter((Stream)cs))
                            {
                                sw.Write(inputString);
                            }

                            encryptedBytes = ms.ToArray();
                        }
                    }
                }

                if (format == AesEncryptedOutputTextFormat.Hex)
                {
                    return encryptedBytes.ConvertToHex().ToLower();
                }
                else
                {
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string AesDecryptString(this string inputStringEncrypted, string key = _encryptionKeyDefault, CipherMode mode = CipherMode.CBC, AesEncryptedOutputTextFormat format = AesEncryptedOutputTextFormat.Base64, bool shouldThrowExceptionIfAny = false)
        {
            if (inputStringEncrypted == null)
            {
                return null;
            }
            else
            {
                if (key.Length != 16 && key.Length != 24 && key.Length != 32) { throw new Exception("Key length must be: 16 for 128 bits key size, 24 for 192 bits key size or 32 for 256 bits key size"); }

                var result = default(string);

                try
                {
                    var encryptedBytes = format == AesEncryptedOutputTextFormat.Hex ? inputStringEncrypted.ConvertHexToBytes() : Convert.FromBase64String(inputStringEncrypted);

                    using (var aes = Aes.Create())
                    {
                        var keyBase64 = key.Base64Encode(Encoding.ASCII);
                        aes.Key = Convert.FromBase64String(keyBase64);
                        aes.IV = new byte[16];
                        aes.Mode = mode;
                        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                        using (var ms = new MemoryStream(encryptedBytes))
                        {
                            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (var sr = new StreamReader(cs))
                                {
                                    result = sr.ReadToEnd();
                                }
                            }
                        }
                    }
                    return result;
                }
                catch
                {
                    if (shouldThrowExceptionIfAny)
                    {
                        throw;
                    }
                    return default;
                }
            }
        }

        public static string Base64Encode(this string inputString, Encoding encodingType = null)
        {
            encodingType ??= Encoding.UTF8;
            var bytes = encodingType.GetBytes(inputString);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(this string inputString, Encoding encodingType = null)
        {
            if (inputString == null)
            {
                return null;
            }
            else
            {
                encodingType ??= Encoding.UTF8;
                var bytes = Convert.FromBase64String(inputString);
                return encodingType.GetString(bytes);
            }
        }

        public static string CapitalizeFirstCharsInWords(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return inputString;
            }
            else
            {
                return $"{inputString.First().ToString().ToUpper()}{inputString[1..]}";
            }
        }

        public static string ConvertHexToString(this string hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] ConvertHexToBytes(this string hexString)
        {
            var numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
            return bytes;
        }

        public static string ConvertToHex(this string plainText)
        {
            var bytes = Encoding.UTF8.GetBytes(plainText);
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

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

        public static T DeserializeJsonTo<T>(this string inputString, DateFormatHandling dateFormatHandlingOption = DateFormatHandling.IsoDateFormat, DateTimeZoneHandling dateTimeZoneHandlingOption = DateTimeZoneHandling.Unspecified, bool shouldThrowExceptionIfAny = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputString))
                {
                    return default;
                }
                else
                {
                    var settings = new JsonSerializerSettings { DateFormatHandling = dateFormatHandlingOption, DateTimeZoneHandling = dateTimeZoneHandlingOption };
                    return JsonConvert.DeserializeObject<T>(inputString, settings);
                }
            }
            catch
            {
                if (shouldThrowExceptionIfAny)
                {
                    throw;
                }
                return default;
            }
        }

        public static T DeserializeXmlTo<T>(this string inputString, bool shouldThrowExceptionIfAny = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(inputString))
                {
                    return default;
                }
                else
                {
                    var serializer = new XmlSerializer(typeof(T));
                    using (var reader = new StringReader(inputString))
                    {
                        var result = (T)serializer.Deserialize(reader);
                        return result;
                    }
                }
            }
            catch
            {
                if (shouldThrowExceptionIfAny)
                {
                    throw;
                }
                return default;
            }
        }

        public static string Encrypt(this string inputString)
        {
            return inputString.AesEncryptString().Base64Encode();
        }

        public static string FromGeorgianCharsToEnglishChars(this string inputString)
        {
            _georgianEnglishChars.ToList().ForEach(c =>
            {
                inputString = Regex.Replace(inputString, c.Key, c.Value);
            });
            return inputString;
        }

        public static string GetInitials(string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return inputString;
            }
            else
            {
                var initials = inputString.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(word => char.ToUpper(word[0]));
                return string.Concat(initials);
            }
        }

        public static string GZipCompress(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return inputString;
            }

            var Bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] compressedBytes = null;
            using (var ms = new MemoryStream())
            {
                using (var gZip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gZip.Write(Bytes, 0, Bytes.Length);
                }
                compressedBytes = ms.ToArray();
            }

            return Convert.ToBase64String(compressedBytes);
        }

        public static string GZipDecompress(this string inputString)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return inputString;
            }

            var compressedBytes = Convert.FromBase64String(inputString);
            var decompressedBytes = default(byte[]);
            using (var stream = new GZipStream(new MemoryStream(compressedBytes), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (var ms = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    decompressedBytes = ms.ToArray();
                }
            }

            return Encoding.UTF8.GetString(decompressedBytes);
        }
        
        public static int? HexToInt(this string inputString)
        {
            try
            {
                int counter, hexInt;
                char[] hexarr;
                hexInt = 0;
                inputString = inputString.ToUpper();
                hexarr = inputString.ToCharArray();
                for (counter = hexarr.Length - 1; counter >= 0; counter--)
                {
                    if ((hexarr[counter] >= '0') && (hexarr[counter] <= '9'))
                    {
                        hexInt += (hexarr[counter] - 48) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                    }
                    else
                    {
                        if ((hexarr[counter] >= 'A') && (hexarr[counter] <= 'F'))
                        {
                            hexInt += (hexarr[counter] - 55) * ((int)(Math.Pow(16, hexarr.Length - 1 - counter)));
                        }
                        else
                        {
                            hexInt = 0;
                            break;
                        }
                    }
                }
                return hexInt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Highlights SearchPhrase in Source string, also returns short version of source string, few words before SearchPhrase and few words after
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="searchPhrase"></param>
        /// <param name="previousWordsToFind"></param>
        /// <param name="nextWordsToFind"></param>
        /// <returns>String with text to highlight wrapped in highlight span</returns>
        public static string HightLightAndShortenFoundedSearchPhrase(this string inputString, string searchPhrase, int previousWordsToFind = 20, int nextWordsToFind = 20)
        {
            if (string.IsNullOrWhiteSpace(inputString))
            {
                return inputString;
            }
            else
            {
                var index = inputString.IndexOf(searchPhrase, StringComparison.InvariantCultureIgnoreCase);
                var length = inputString.Length;

                int startIndex = 0;
                int endIndex = 0;

                if (index > -1)
                {
                    for (var i = index; i > -1 && previousWordsToFind > 0; i--)
                    {
                        if (inputString[i] == ' ')
                        {
                            --previousWordsToFind;
                        }
                        startIndex = i;
                    }

                    nextWordsToFind += previousWordsToFind;

                    for (var i = index; i < length && nextWordsToFind > 0; i++)
                    {
                        if (inputString[i] == ' ')
                        {
                            --nextWordsToFind;
                        }
                        endIndex = i;
                    }
                    ++endIndex;
                    inputString = inputString.Substring(startIndex, endIndex - startIndex);
                    inputString = Regex.Replace(input: inputString, pattern: Regex.Escape(searchPhrase), replacement: @"<span class=""highlight"">$0</span>", options: RegexOptions.IgnoreCase);

                }

                return inputString;
            }
        }

        /// <summary>
        /// Writes string into ErrorLog.txt file
        /// </summary>
        /// <param name="inputString">Input string to write into file</param>
        /// <param name="logFile">File physical path (optional) default value is $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\ErrorLog.txt</param>
        public static void LogString(this string inputString, string logFile = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(logFile))
                {
                    var LogFileDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}App_Data\\";
                    logFile = $"{LogFileDirectory}ErrorLog.txt";

                    if (!File.Exists(logFile))
                    {
                        if (!Directory.Exists(LogFileDirectory))
                        {
                            Directory.CreateDirectory(LogFileDirectory);
                        }
                        using (var fs = File.Create(logFile)) { }
                    }
                }

                if (!string.IsNullOrEmpty(logFile) && File.Exists(logFile))
                {
                    File.AppendAllText(logFile, $"\r\n\r\n------------------------------------\r\n{DateTime.Now}\r\n{inputString}\r\n------------------------------------\r\n\r\n", Encoding.UTF8);
                }
            }
            catch { }
        }

        public static string MD5Encrypt(this string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return inputString;
            }
            else
            {
                var bytes = MD5.HashData(Encoding.UTF8.GetBytes(inputString));
                var sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Surround searchPhrase inside input string with left and right strings. For example surround "new york" with <span class="highlight">new york</span>.
        /// </summary>
        /// <param name="inputString">Input string</param>
        /// <param name="searchPhrase">String to surround</param>
        /// <param name="left">Surrounding part left</param>
        /// <param name="right">Surrounding part right</param>        
        public static string SearchAndLeftRightSurroundResult(this string inputString, string searchPhrase, string left = null, string right = null)
        {
            left ??= string.Empty;
            right ??= string.Empty;

            int i = inputString.IndexOf(searchPhrase, StringComparison.OrdinalIgnoreCase);
            while (i > -1)
            {
                inputString = inputString.Insert(i, left);
                inputString = inputString.Insert(i += left.Length + searchPhrase.Length, right);
                i = inputString.IndexOf(searchPhrase, i + 1, StringComparison.OrdinalIgnoreCase);
            }

            return inputString;
        }

        public static string SearchAndHightlightResult(string inputString, string searchPhrase)
        {
            if (inputString.Contains(searchPhrase))
            {
                int index = inputString.IndexOf(searchPhrase);
                int startIndex = index - 100 > 0 ? index - 100 : 0;
                if (startIndex > 100)
                {
                    int i = startIndex;
                    var doNotStop = true;
                    while (i > 0 && doNotStop)
                    {
                        if (inputString[--i] == ' ')
                        {
                            doNotStop = false;
                        }
                    }
                    startIndex = i;
                }



                int endIndex = (index + searchPhrase.Length + 100) > inputString.Length ? inputString.Length : (index + searchPhrase.Length + 100);
                if (endIndex < inputString.Length)
                {
                    int i = endIndex;
                    bool doNotStop = true;
                    while (i > (index + searchPhrase.Length) && doNotStop)
                    {
                        if (inputString[--i] == ' ')
                        {
                            doNotStop = false;
                        }
                    }
                    endIndex = i;
                }
                return inputString.Substring(startIndex, endIndex - startIndex).Replace(searchPhrase, "<label style=\"background-color:Yellow;\">" + searchPhrase + "</label>");
            }
            else
            {

                if (inputString.Length > 200)
                {
                    int i = 200;
                    bool doNotStop = true;
                    while (i > 0 && doNotStop)
                    {
                        if (inputString[--i] == ' ')
                        {
                            doNotStop = false;
                        }
                    }
                    return inputString.Substring(0, i);
                }
                else
                {
                    return inputString;
                }

            }
        }

        public static string SHA256Encrypt(this string inputString)
        {
            if (inputString == null)
            {
                return null;
            }
            else
            {
                var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(inputString));
                var sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        public static string Shorten(this string inputString, int symbolCount)
        {
            return (string.IsNullOrEmpty(inputString) || inputString.Length <= symbolCount) ? inputString : $"{inputString.Substring(0, symbolCount)} ...";
        }

        public static string StripPhoneNumber(this string phoneNumber)
        {
            return phoneNumber?.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Trim();
        }

        public static string StripAndHidePhoneNumber(this string phoneNumber)
        {
            return $"*** *** *{phoneNumber.StripPhoneNumber()[^2..]}";
        }

        public static string StripAllExceptNumbers(this string inputString)
        {
            return inputString == null ? null : Regex.Replace(inputString, "[^0-9]", "");
        }

        public static string StripHtml(this string inputString, string[] exceptTags = null)
        {
            var result = inputString;

            if (!string.IsNullOrWhiteSpace(inputString))
            {
                // Strip <script> tags and all their contents
                result = Regex.Replace(result, @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>", string.Empty, RegexOptions.IgnoreCase);

                // Strip <style> tags and all their contents
                result = Regex.Replace(result, @"<style\b[^<]*(?:(?!<\/style>)<[^<]*)*<\/style>", string.Empty, RegexOptions.IgnoreCase);

                // Strip remaining HTML tags (respecting exceptTags)
                if (exceptTags != null && exceptTags.Length > 0)
                {
                    var exceptTagsString = string.Join("|", exceptTags);
                    var exceptExpression = $"(?!</?({exceptTagsString})\\b)";
                    result = Regex.Replace(result, $"{exceptExpression}<[^>]*>", string.Empty);
                }
                else
                {
                    result = Regex.Replace(result, "<[^>]*>", string.Empty);
                }

                result = result.Replace("&nbsp;", " ");
                result = Regex.Replace(result, @"[ \t]{2,}", " ");
                result = Regex.Replace(result, @"\n\s+\n", "\n\t\n");
                result = Regex.Replace(result, @"^[ \t]+", "", RegexOptions.Multiline);
                result = result.Trim();
            }
            return result;
        }

        public static string ToAZ09Dash(this string inputString, bool shouldIncludeGuid = false, bool shorten = false)
        {
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(inputString);
            var extension = Path.GetExtension(inputString);
            var guidAddition = shouldIncludeGuid ? $"_{Guid.NewGuid().ToString().Substring(0, 8)}" : string.Empty;

            if (shorten)
            {
                var stringLength = filenameWithoutExtension.Length;
                if (stringLength > 15)
                {
                    filenameWithoutExtension = filenameWithoutExtension.Substring(0, 10);
                }
            }

            filenameWithoutExtension = filenameWithoutExtension.ToSlug();

            var result = $"{filenameWithoutExtension}{guidAddition}{extension}";

            return result;
        }

        public static bool? ToBoolean(this string inputString)
        {
            if (bool.TryParse(inputString, out var val)) return val;
            return null;
        }

        public static bool ToBooleanValue(this string inputString)
        {
            if (bool.TryParse(inputString, out var val)) return val;
            return false;
        }

        public static byte? ToByte(this string inputString)
        {
            if (byte.TryParse(inputString, out var value))
            {
                return value;
            }

            return null;
        }

        public static DateTime? ToDateTime(this string inputString, CultureInfo culture = null, DateTimeStyles styles = DateTimeStyles.None)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (DateTime.TryParse(inputString, culture, styles, out var value))
            {
                return value;
            }

            return null;
        }

        public static double? ToDouble(this string inputString, NumberStyles styles = NumberStyles.Any, CultureInfo culture = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (double.TryParse(inputString, styles, culture, out var value))
            {
                return value;
            }

            return null;
        }

        public static decimal? ToDecimal(this string inputString, NumberStyles styles = NumberStyles.Any, CultureInfo culture = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (decimal.TryParse(inputString, styles, culture, out var value))
            {
                return value;
            }

            return null;
        }

        public static float? ToFloat(this string inputString, NumberStyles styles = NumberStyles.Any, CultureInfo culture = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (float.TryParse(inputString, styles, culture, out var value))
            {
                return value;
            }

            return null;
        }

        public static int? ToInt(this string inputString)
        {
            if (int.TryParse(inputString, out var value))
            {
                return value;
            }

            return null;
        }

        public static long? ToLong(this string inputString)
        {
            if (long.TryParse(inputString, out var value))
            {
                return value;
            }

            return null;
        }

        public static string ToSlug(this string inputString)
        {
            //First to lower case
            inputString = inputString.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.UTF8.GetBytes(inputString);
            inputString = Encoding.UTF8.GetString(bytes);

            //Replace spaces
            inputString = Regex.Replace(inputString, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            inputString = Regex.Replace(inputString, @"[^a-z0-9ა-ჰа-я\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            inputString = inputString.Trim('-', '_');

            //Replace double occurences of - or _
            inputString = Regex.Replace(inputString, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return inputString;
        }

        public static string ToSlugUnicode(this string inputString)
        {
            //First to lower case
            inputString = inputString.ToLowerInvariant();

            //Remove all accents
            var bytes = Encoding.UTF8.GetBytes(inputString);
            inputString = Encoding.UTF8.GetString(bytes);

            //Replace spaces
            inputString = Regex.Replace(inputString, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            inputString = Regex.Replace(inputString, @"[^a-z0-9ა-ჰа-я\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            inputString = inputString.Trim('-', '_');

            //Replace double occurences of - or _
            inputString = Regex.Replace(inputString, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return inputString;
        }

        public static short? ToShort(this string inputString)
        {
            if (short.TryParse(inputString, out var value))
            {
                return value;
            }

            return null;
        }

        public static Stream ToStream(this string inputString)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(inputString);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static TimeSpan? ToTimeSpan(this string inputString, CultureInfo culture = null)
        {
            culture ??= CultureInfo.InvariantCulture;

            if (TimeSpan.TryParse(inputString, culture, out var value))
            {
                return value;
            }

            return null;
        }

        public static string UrlToHtmlAHref(string inputString)
        {
            var regex = @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\.[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";
            var r = new Regex(regex, RegexOptions.IgnoreCase);
            return r.Replace(inputString, "<a href=\"$1\" target=\"&#95;blank\">$1</a>").Replace("href=\"www", "href=\"http://www");
        }        
        #endregion Methods

        #region Nested CLasses
        public enum AesEncryptedOutputTextFormat
        {
            Base64,
            Hex
        }
        #endregion
    }
}