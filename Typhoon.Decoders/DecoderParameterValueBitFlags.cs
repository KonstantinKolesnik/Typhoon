using System.Collections.Generic;
using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderParameterValueBitFlags
    {
        #region Fields
        private List<DecoderParameterValueBitFlag> values = new List<DecoderParameterValueBitFlag>();
        #endregion

        #region Properties
        public List<DecoderParameterValueBitFlag> Values
        {
            get { return values; }
        }
        #endregion

        #region Constructors
        private DecoderParameterValueBitFlags()
        {
        }
        #endregion

        #region XmlSerialization
        private bool ReadFromXmlElement(XmlElement el)
        {
            XmlNodeList lst = el.SelectNodes("ParameterBitFlagValue");
            foreach (XmlElement child in lst)
                values.Add(DecoderParameterValueBitFlag.FromXmlElement(child));

            return (values.Count != 0);
        }
        public void WriteToXmlElement(ref XmlElement el)
        {
            foreach (DecoderParameterValueBitFlag v in values)
            {
                XmlElement child = el.OwnerDocument.CreateElement("ParameterBitFlagValue");
                v.WriteToXmlElement(ref child);
                el.AppendChild(child);
            }
        }

        public static DecoderParameterValueBitFlags FromXmlElement(XmlElement el)
        {
            DecoderParameterValueBitFlags valueFlags = new DecoderParameterValueBitFlags();
            return (valueFlags.ReadFromXmlElement(el) ? valueFlags : null);
        }
        #endregion
    }
}
