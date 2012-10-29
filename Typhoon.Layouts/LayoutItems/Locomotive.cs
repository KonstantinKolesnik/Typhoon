using System.ComponentModel;
using System.Xml;
using Typhoon.Decoders;
using System.Xml.Linq;

namespace Typhoon.Layouts.LayoutItems
{
    public class Locomotive : LayoutItem
    {
        #region Fields
        private Decoder decoder = null;
        private Decoder decoderSound = null;
        private bool isUsedInConsist = false;
        #endregion

        #region Properties
        public Decoder DecoderMobile
        {
            get { return decoder; }
            set
            {
                if (decoder != value)
                {
                    decoder = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DecoderMobile"));
                    if (decoder != null)
                        decoder.PropertyChanged += Decoder_PropertyChanged;
                }
            }
        }
        public Decoder DecoderSound
        {
            get { return decoderSound; }
            set
            {
                if (decoderSound != value)
                {
                    decoderSound = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DecoderSound"));
                    if (decoderSound != null)
                        decoderSound.PropertyChanged += DecoderSound_PropertyChanged;
                }
            }
        }
        public bool IsUsedInConsist
        {
            get { return isUsedInConsist; }
            set
            {
                if (isUsedInConsist != value)
                {
                    isUsedInConsist = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsUsedInConsist"));
                }
            }
        }
        #endregion

        #region Constructor
        public Locomotive() : base(LayoutItemType.Locomotive)
        {
        }
        #endregion

        #region Event handlers
        private void Decoder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("DecoderMobile"));
        }
        private void DecoderSound_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("DecoderSound"));
        }
        #endregion

        #region XmlSerialization
        public override void WriteToXmlElement(XmlElement el)
        {
            base.WriteToXmlElement(el);

            el.SetAttribute("IsUsedInConsist", isUsedInConsist.ToString());
            if (decoder != null)
                el.SetAttribute("Decoder", decoder.SaveToXml());
        }
        public override void ReadFromXmlElement(XmlElement el)
        {
            base.ReadFromXmlElement(el);

            if (el.HasAttribute("IsUsedInConsist"))
                isUsedInConsist = bool.Parse(el.GetAttribute("IsUsedInConsist"));
            if (el.HasAttribute("Decoder"))
            {
                decoder = null;
                Decoder d = new Decoder();
                if (d.LoadFromXml(el.GetAttribute("Decoder")))
                {
                    decoder = d;
                    decoder.PropertyChanged += Decoder_PropertyChanged;
                }
            }
        }

        public override void ReadFromXElement(XElement el)
        {
            base.ReadFromXElement(el);

            if (el.Attribute("IsUsedInConsist") != null)
                isUsedInConsist = bool.Parse(el.Attribute("IsUsedInConsist").Value);
            if (el.Attribute("Decoder") != null)
            {
                decoder = null;
                Decoder d = new Decoder();
                if (d.LoadFromXml(el.Attribute("Decoder").Value))
                {
                    decoder = d;
                    decoder.PropertyChanged += Decoder_PropertyChanged;
                }
            }
        }
        #endregion
    }
}
