using System;
using System.Linq;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class IntExtensions
    {
        #region Methods
        /// <summary>
        /// Converts integer number to HEX string
        /// </summary>
        /// <param name="inputNumber">Input integer number</param>
        /// <returns>HEX string</returns>
        public static string ConvertToHex(this int inputNumber)
        {
            int counter, reminder;
            string hexStr;

            counter = 1;
            hexStr = "";
            while (inputNumber + 15 > Math.Pow(16, counter - 1))
            {
                reminder = (int)(inputNumber % Math.Pow(16, counter));
                reminder = (int)(reminder / Math.Pow(16, counter - 1));

                if (reminder <= 9)
                {
                    hexStr = hexStr + (char)(reminder + 48);
                }
                else
                {
                    hexStr = hexStr + (char)(reminder + 55);
                }

                inputNumber -= reminder;
                counter++;
            }

            return new string(hexStr.Reverse().ToArray());
        }

        /// <summary>
        /// Converts integer number to HEX string
        /// </summary>
        /// <param name="inputNumber">Input integer number</param>
        /// <returns>HEX string</returns>
        public static string ConvertToHex(this int? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.Value.ConvertToHex() : null;
        }

        /// <summary>
        /// Converts number to metrikc prefix string 1225 -> 1.22K
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToMetricPrefixed(this int inputNumber)
        {
            if (inputNumber < 1000)
            {
                return inputNumber.ToString();
            }
            else if (inputNumber < 1000000)
            {
                return $"{Math.Round(((double)inputNumber / 1000), 2)}K";
            }
            else if (inputNumber < 1000000000)
            {
                return $"{Math.Round(((double)inputNumber / 1000000), 2)}M";
            }
            else
            {
                return $"{Math.Round(((double)inputNumber / 1000000000), 2)}B";
            }
        }

        /// <summary>
        /// Converts number to metrikc prefix string 1225 -> 1.22K
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToMetricPrefixed(this int? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.Value.ConvertToMetricPrefixed() : null;
        }

        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this int inputNumber)
        {
            // Start with the most common extension.
            string extension = "th";

            // Examine the last 2 digits.
            int lastDigits = inputNumber % 100;

            // If the last digits are 11, 12, or 13, use th. Otherwise:
            if (lastDigits < 11 || lastDigits > 13)
            {
                // Check the last digit.
                switch (lastDigits % 10)
                {
                    case 1:
                        extension = $"{inputNumber}st";
                        break;
                    case 2:
                        extension = $"{inputNumber}nd";
                        break;
                    case 3:
                        extension = $"{inputNumber}rd";
                        break;
                }
            }

            return extension;
        }

        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this int? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.Value.ConvertToOrdinal() : null;            
        }        

        /// <summary>
        /// Converts integer to roman. (MinValue = 0 and MaxValue = 3999)
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToRoman(this int inputNumber)
        {
            if ((inputNumber < 0) || (inputNumber > 3999)) return null;
            if (inputNumber < 1) return string.Empty;
            if (inputNumber >= 1000) return "M" + ConvertToRoman(inputNumber - 1000);
            if (inputNumber >= 900) return "CM" + ConvertToRoman(inputNumber - 900); 
            if (inputNumber >= 500) return "D" + ConvertToRoman(inputNumber - 500);
            if (inputNumber >= 400) return "CD" + ConvertToRoman(inputNumber - 400);
            if (inputNumber >= 100) return "C" + ConvertToRoman(inputNumber - 100);
            if (inputNumber >= 90) return "XC" + ConvertToRoman(inputNumber - 90);
            if (inputNumber >= 50) return "L" + ConvertToRoman(inputNumber - 50);
            if (inputNumber >= 40) return "XL" + ConvertToRoman(inputNumber - 40);
            if (inputNumber >= 10) return "X" + ConvertToRoman(inputNumber - 10);
            if (inputNumber >= 9) return "IX" + ConvertToRoman(inputNumber - 9);
            if (inputNumber >= 5) return "V" + ConvertToRoman(inputNumber - 5);
            if (inputNumber >= 4) return "IV" + ConvertToRoman(inputNumber - 4);
            if (inputNumber >= 1) return "I" + ConvertToRoman(inputNumber - 1);
            else { return null; }
        }

        /// <summary>
        /// Converts integer to roman. (MinValue = 0 and MaxValue = 3999)
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToRoman(this int? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.ConvertToRoman() : null;
        }

        public static string EncryptID(this int? ID)
        {
            return ID?.ToString().AesEncryptString().Base64Encode();
        }

        public static string EncryptID(this int ID)
        {
            return new int?(ID).EncryptID();
        }
        #endregion
    }
}