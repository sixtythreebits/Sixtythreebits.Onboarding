using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SixtyThreeBits.Core.Libraries.Common
{
    [XmlType("KeyValueSelectedTuple")]
    public class KeyValueSelectedTuple<TKey, TValue> : KeyValueTuple<TKey, TValue>
    {
        #region Properties        
        [XmlIgnore]
        [JsonIgnore]
        public bool IsSelected { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Key}: {Value} - {(IsSelected ? "Selected" : null)}";
        }        
        #endregion
    }
}