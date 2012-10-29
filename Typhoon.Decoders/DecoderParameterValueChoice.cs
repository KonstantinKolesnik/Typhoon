using System;
using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderParameterValueChoice
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
        private DecoderParameterValueChoice()
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

        public static DecoderParameterValueChoice FromXmlElement(XmlElement el)
        {
            DecoderParameterValueChoice v = new DecoderParameterValueChoice();
            v.ReadFromXmlElement(el);
            return v;
        }
        #endregion
    }
}
