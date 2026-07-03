using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SixtyThreeBits.Core.Libraries.Extensions
{
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// return null when provided condition is satisfied for the Source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputObject">Object to test</param>
        /// <param name="predicate">Predicate method for null test</param>
        /// <returns>null or initial value</returns>
        public static T NullIf<T>(this T inputObject, Func<T, bool> predicate) where T : class
        {
            if(predicate(inputObject))
            {
                return null;
            }
            else
            {
                return inputObject;
            }
        }

        public static string ToJson(this object inputObject, Newtonsoft.Json.Formatting jsonFormat = Newtonsoft.Json.Formatting.None, DateFormatHandling dateFormatHandlingOption = DateFormatHandling.IsoDateFormat, DateTimeZoneHandling dateTimeZoneHandlingOption = DateTimeZoneHandling.Unspecified)
        {
            if (inputObject == null)
            {
                return null;
            }
            else
            {
                var settings = new JsonSerializerSettings { DateFormatHandling = dateFormatHandlingOption, DateTimeZoneHandling = dateTimeZoneHandlingOption, NullValueHandling = NullValueHandling.Ignore };
                return JsonConvert.SerializeObject(inputObject, jsonFormat, settings);
            }
        }

        /// <summary>
        /// Converts flat sequence to recursive
        /// </summary>
        /// <typeparam name="T1">Type of items</typeparam>
        /// <typeparam name="T2">Type of ParentID</typeparam>
        /// <param name="treeNodesFlat">Flat list of items</param>
        /// <param name="IDPropertyName">Property name for ID</param>
        /// <param name="parentIDPropertyName">Property name for ParentID</param>
        /// <param name="childrenPropertyName">Property name for Children</param>
        /// <param name="ParentID">ParentID must be null for initial call</param>
        /// <returns>Recursive sequence</returns>
        public static void ToRecursive<T1>(this List<T1> treeNodesFlat, string IDPropertyName, string parentIDPropertyName = "ParentID", string childrenPropertyName = "Children", T1 parentNode = null) where T1 : class
        {
            if (treeNodesFlat?.Count > 0)
            {
                if (parentNode == null)
                {
                    var topLevelItems = treeNodesFlat.Where(Item => Item.GetType().GetProperty(parentIDPropertyName).GetValue(Item) == null).ToList();
                    topLevelItems.ForEach(Item =>
                    {
                        treeNodesFlat.ToRecursive(IDPropertyName: IDPropertyName, parentIDPropertyName: parentIDPropertyName, childrenPropertyName: childrenPropertyName, parentNode: Item);
                    });
                    treeNodesFlat.RemoveAll(Item => Item.GetType().GetProperty(parentIDPropertyName).GetValue(Item) != null);
                }
                else
                {
                    var parentID = parentNode.GetType().GetProperty(IDPropertyName).GetValue(parentNode);
                    var children = treeNodesFlat.Where(Item => Item.GetType().GetProperty(parentIDPropertyName).GetValue(Item) != null && Item.GetType().GetProperty(parentIDPropertyName).GetValue(Item).Equals(parentID)).ToList();
                    
                    if (children.Count > 0)
                    {
                        var propertyInfo = parentNode.GetType().GetProperty(childrenPropertyName);
                        propertyInfo.SetValue(parentNode, children);

                        children.ForEach(Item =>
                        {
                            treeNodesFlat.ToRecursive(IDPropertyName: IDPropertyName, parentIDPropertyName: parentIDPropertyName, childrenPropertyName: childrenPropertyName, parentNode: Item);
                        });
                    }
                }                
            }
        }

        /// <summary>
        /// Serializes object to xml
        /// </summary>
        /// <param name="inputObject">Object to serialize</param>
        /// <returns>Xml string</returns>
        public static string ToXml(this object inputObject)
        {
            if (inputObject == null)
            {
                return null;
            }
            else
            {
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                var sb = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
                {
                    new XmlSerializer(inputObject.GetType()).Serialize(writer, inputObject, namespaces);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// Serializes object to xml and creates XElement object
        /// </summary>
        /// <param name="inputObject">Object to serialize</param>
        /// <returns>XElement object</returns>
        public static XElement ToXElement<T>(this T inputObject)
        {
            if (inputObject == null)
            {
                return null;
            }
            else
            {
                var xmlString = inputObject.GetType() == typeof(string) ? inputObject.ToString() : inputObject.ToXml();
                return string.IsNullOrWhiteSpace(xmlString) ? null : XElement.Parse(xmlString);
            }
        }
    }
}