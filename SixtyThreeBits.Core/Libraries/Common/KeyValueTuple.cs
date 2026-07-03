using System.Xml.Serialization;

namespace SixtyThreeBits.Core.Libraries.Common
{
    [XmlType("KeyValueTuple")]
    public class KeyValueTuple<TKey, TValue>
    {
        #region Properties
        [XmlElement("Key")]
        public TKey Key { get; set; }
        [XmlElement("Value")]
        public TValue Value { get; set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            return $"{Key}: {Value}";
        }
        public bool ShouldSerializeKey() => Key != null;
        public bool ShouldSerializeValue() => Value != null;
        #endregion
    }
}