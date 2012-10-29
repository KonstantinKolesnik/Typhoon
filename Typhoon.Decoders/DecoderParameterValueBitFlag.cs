using System;
using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderParameterValueBitFlag
    {
        #region Fields
        private uint value;
        private string name;
        #endregion

        #region Properties
        public uint Value
        {
            get { return value; }
        }
        public string Name
        {
            get { return name; }
        }
        #endregion

        #region Constructors
        private DecoderParameterValueBitFlag()
        {
        }
        #endregion

        #region XmlSerialization
        private void ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("Value"))
                value = uint.Parse(el.GetAttribute("Value"));
            if (el.HasAttribute("Name"))
                name = el.GetAttribute("Name");
        }
        public void WriteToXmlElement(ref XmlElement el)
        {
            if (!String.IsNullOrEmpty(Name))
                el.SetAttribute("Name", name);
            el.SetAttribute("Value", value.ToString());
        }

        public static DecoderParameterValueBitFlag FromXmlElement(XmlElement el)
        {
            DecoderParameterValueBitFlag v = new DecoderParameterValueBitFlag();
            v.ReadFromXmlElement(el);
            return v;
        }
        #endregion
    }
}
