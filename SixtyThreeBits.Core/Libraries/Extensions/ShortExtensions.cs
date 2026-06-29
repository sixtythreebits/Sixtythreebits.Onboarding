using System;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class ShortExtensions
    {
        #region Methods        
        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this short inputNumber)
        {
            return Convert.ToInt32(inputNumber).ConvertToOrdinal();
        }

        /// <summary>
        /// Return the int's ordinal extension. 1 => 1st, 2 => 2nd, 3 => 3rd ...
        /// </summary>
        /// <param name="inputNumber">Number</param>
        /// <returns>Number as ordinal string</returns>
        public static string ConvertToOrdinal(this short? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.Value.ConvertToOrdinal() : null;
        }

        /// <summary>
        /// Converts integer to roman. (MinValue = 0 and MaxValue = 3999)
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToRoman(this short inputNumber)
        {
            return Convert.ToInt32(inputNumber).ConvertToRoman();
        }

        /// <summary>
        /// Converts integer to roman. (MinValue = 0 and MaxValue = 3999)
        /// </summary>
        /// <param name="inputNumber"></param>
        /// <returns></returns>
        public static string ConvertToRoman(this short? inputNumber)
        {
            return inputNumber.HasValue ? inputNumber.Value.ConvertToRoman() : null;
        }
        #endregion Methods        
    }    
}