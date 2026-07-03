using System;
using System.Text;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class ByteExtensions
    {
        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this byte inputNumber)
        {
            return Convert.ToInt32(inputNumber).ConvertToOrdinal();
        }

        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this byte? inputNumber)
        {
            return inputNumber.HasValue ? Convert.ToInt32(inputNumber.Value).ConvertToOrdinal() : null;
        }

        /// <summary>
        /// Converts string to hex
        /// </summary>
        /// <param name="PlainText"></param>
        /// <returns></returns>
        public static string ConvertToHex(this byte[] inputBytes)
        {            
            var sb = new StringBuilder(inputBytes.Length * 2);
            foreach (byte b in inputBytes)
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }
    }
}