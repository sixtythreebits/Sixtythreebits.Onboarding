using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class XElementExtensions
    {
        #region Methods
        /// <summary>
        /// Convert xml element value to bool?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static bool? BooleanValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);

            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmedValue = value.Trim();

            if (trimmedValue == "1")
            {
                return true;
            }

            if (trimmedValue == "0")
            {
                return false;
            }

            return bool.Parse(trimmedValue);
        }

        /// <summary>
        /// Convert xml element value to byte?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static byte? ByteValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? (byte?)null : byte.Parse(value);
        }

        /// <summary>
        /// Searching for element by name, inside xml hierarchy and returns all child elements from it.
        /// <para>For Example. x.Children("node1","node1.1","node1.1.1") will find all child elements of node1.1.1, combining with all child elements of node1.1, combining with all child elements of node1</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="elementNames"></param>
        /// <returns></returns>
        public static IEnumerable<XElement> Children(this XElement x, params string[] elementNames)
        {
            return elementNames.Aggregate(new[] { x }.AsEnumerable(), (current, tag) => current.Elements(tag));
        }

        /// <summary>
        /// Convert xml element value to DateTime?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static DateTime? DateTimeValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? (DateTime?)null : DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert xml element value to DateTime?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <param name="cultureInfo">CultureInfo object</param>
        /// <returns></returns>
        public static DateTime? DateTimeValueOf(this XElement x, string name, CultureInfo cultureInfo)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : DateTime.Parse(value, cultureInfo);
        }

        /// <summary>
        /// Convert xml element value to decimal?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static decimal? DecimalValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert xml element value to decimal?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <param name="numberStyles">NumberStyles for parsing fractional units</param>
        /// <param name="cultureInfo">CultureInfo object</param>
        /// <returns></returns>
        public static decimal? DecimalValueOf(this XElement x, string name, NumberStyles numberStyles, CultureInfo cultureInfo)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : decimal.Parse(value, numberStyles, cultureInfo);
        }

        /// <summary>
        /// Deserializes xml to the object
        /// </summary>
        /// <typeparam name="T">Type to deserialize</typeparam>
        /// <param name="x">Input xml</param>
        /// <returns>Deserialized object</returns>
        public static T DeserializeXmlTo<T>(this XElement x)
        {
            if (x != null)
            {
                return x.ToString().DeserializeXmlTo<T>();
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// Convert xml element value to double?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static double? DoubleValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert xml element value to double?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <param name="numberStyles">NumberStyles for parsing fractional units</param>
        /// <param name="cultureInfo">CultureInfo object</param>
        /// <returns></returns>
        public static double? DoubleValueOf(this XElement x, string name, NumberStyles _NumberStyles, CultureInfo CultureInfo)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : double.Parse(value, _NumberStyles, CultureInfo);
        }

        /// <summary>
        /// Convert xml element value to float?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <param name="numberStyles">NumberStyles for parsing fractional units</param>
        /// <param name="cultureInfo">CultureInfo object</param>
        /// <returns></returns>
        public static float? FloatValueOf(this XElement x, string name, NumberStyles _NumberStyles, CultureInfo CultureInfo)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : float.Parse(value, _NumberStyles, CultureInfo);
        }

        /// <summary>
        /// Convert xml element value to float?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static float? FloatValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : float.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Convert xml element value to Guid?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static Guid? GuidValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : Guid.Parse(value);
        }

        /// <summary>
        /// Convert xml element value to int?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static int? IntValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : int.Parse(value);
        }

        /// <summary>
        /// Convert xml element value to long?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static long? LongValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : long.Parse(value);
        }

        /// <summary>
        /// Convert xml element value to short?
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static short? ShortValueOf(this XElement x, string name)
        {
            var value = x.ValueOf(name);
            return string.IsNullOrWhiteSpace(value) ? null : short.Parse(value);
        }

        /// <summary>
        /// Convert xml element value to string
        /// </summary>
        /// <param name="x">input xml</param>
        /// <param name="name">xml element name</param>
        /// <returns></returns>
        public static string ValueOf(this XElement x, string name)
        {
            var element = x?.Element(name);
            return element?.Value;
        }
        #endregion Methods
    }
}