using System.Xml;

namespace Typhoon.Layouts.LayoutItems
{
    public class Turntable : LayoutItem
    {


        #region Constructor
        public Turntable() : base(LayoutItemType.Turntable)
        {
        }
        #endregion

        #region XmlSerialization
        public override void WriteToXmlElement(XmlElement el)
        {
            base.WriteToXmlElement(el);


        }
        public override void ReadFromXmlElement(XmlElement el)
        {
            base.ReadFromXmlElement(el);


        }
        #endregion
    }
}
