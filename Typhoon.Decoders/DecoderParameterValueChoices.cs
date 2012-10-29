using System.Collections.Generic;
using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderParameterValueChoices
    {
        #region Fields
        private List<DecoderParameterValueChoice> values = new List<DecoderParameterValueChoice>();
        #endregion

        #region Properties
        public List<DecoderParameterValueChoice> Values
        {
            get { return values; }
        }
        #endregion

        #region Constructors
        private DecoderParameterValueChoices()
        {
        }
        #endregion

        #region XmlSerialization
        private bool ReadFromXmlElement(XmlElement el)
        {
            XmlNodeList lst = el.SelectNodes("ParameterChoiceValue");
            foreach (XmlElement child in lst)
                values.Add(DecoderParameterValueChoice.FromXmlElement(child));

            return (values.Count != 0);
        }
        public void WriteToXmlElement(ref XmlElement el)
        {
            foreach (DecoderParameterValueChoice v in values)
            {
                XmlElement child = el.OwnerDocument.CreateElement("ParameterChoiceValue");
                v.WriteToXmlElement(ref child);
                el.AppendChild(child);
            }
        }

        public static DecoderParameterValueChoices FromXmlElement(XmlElement el)
        {
            DecoderParameterValueChoices valueSet = new DecoderParameterValueChoices();
            return (valueSet.ReadFromXmlElement(el) ? valueSet : null);
        }
        #endregion
    }
}
