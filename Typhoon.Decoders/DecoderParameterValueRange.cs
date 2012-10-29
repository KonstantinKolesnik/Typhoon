using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderParameterValueRange
    {
        #region Fields
        private uint min = 0;
        private uint max = 255;
        #endregion

        #region Properties
        public uint Min
        {
            get { return min; }
        }
        public uint Max
        {
            get { return max; }
        }
        #endregion

        #region Constructors
        private DecoderParameterValueRange()
        {
        }
        public DecoderParameterValueRange(uint min, uint max)
        {
            this.min = min;
            this.max = max;
        }
        #endregion

        #region XmlSerialization
        private bool ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("Min"))
                min = uint.Parse(el.GetAttribute("Min"));
            else
                return false;

            if (el.HasAttribute("Max"))
                max = uint.Parse(el.GetAttribute("Max"));
            else
                return false;

            return true;
        }
        public void WriteToXmlElement(ref XmlElement el)
        {
            el.SetAttribute("Min", min.ToString());
            el.SetAttribute("Max", max.ToString());
        }

        public static DecoderParameterValueRange FromXmlElement(XmlElement el)
        {
            DecoderParameterValueRange valueRange = new DecoderParameterValueRange();
            return (valueRange.ReadFromXmlElement(el) ? valueRange : null);
        }
        #endregion
    }
}
