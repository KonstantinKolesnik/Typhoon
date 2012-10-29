
namespace Typhoon.Decoders
{
    public class DecoderParameterCVMap
    {
        #region Fields
        private uint cv;
        private byte mask = 0;
        #endregion

        #region Properties
        public uint CV
        {
            get { return cv; }
        }
        public byte Mask
        {
            get { return mask; }
        }
        #endregion

        #region Constructors
        public DecoderParameterCVMap(uint cv, byte mask)
        {
            this.cv = cv;
            this.mask = mask;
        }
        public DecoderParameterCVMap(string value)
        {
            string[] res = value.Split(new char[] { '-' });
            cv = uint.Parse(res[0].Trim());
            mask = byte.Parse(res[1].Trim());
        }
        #endregion

        #region XmlSerialization
        //private bool ReadFromXmlElement(XmlElement el)
        //{
        //    if (el.HasAttribute("Min"))
        //        min = uint.Parse(el.GetAttribute("Min"));
        //    else
        //        return false;

        //    if (el.HasAttribute("Max"))
        //        max = uint.Parse(el.GetAttribute("Max"));
        //    else
        //        return false;

        //    return true;
        //}
        //public void WriteToXmlElement(ref XmlElement el)
        //{
        //    el.SetAttribute("Min", min.ToString());
        //    el.SetAttribute("Max", max.ToString());
        //}

        //public static DecoderParameterValueRange FromXmlElement(XmlElement el)
        //{
        //    DecoderParameterValueRange valueRange = new DecoderParameterValueRange();
        //    return (valueRange.ReadFromXmlElement(el) ? valueRange : null);
        //}
        #endregion



    }
}
