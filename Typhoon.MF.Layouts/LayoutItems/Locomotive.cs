using System.Ext.Xml;
using System.Xml;
using MFE.Utilities;
using Microsoft.SPOT;
using Typhoon.Layouts;
//using Typhoon.Decoders;

namespace Typhoon.MF.Layouts.LayoutItems
{
    public class Locomotive : LayoutItem
    {
        #region Fields
        private ProtocolType protocol;
        //private Decoder decoder = null;
        //private Decoder decoderSound = null;
        private bool isUsedInConsist = false;
        #endregion

        #region Properties
        public ProtocolType Protocol
        {
            get { return protocol; }
            set
            {
                if (protocol != value)
                {
                    ProtocolType old = protocol;
                    protocol = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name", old, protocol));
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
                    OnPropertyChanged(new PropertyChangedEventArgs("IsUsedInConsist", !isUsedInConsist, isUsedInConsist));
                }
            }
        }

        //public Decoder DecoderMobile
        //{
        //    get { return decoder; }
        //    set
        //    {
        //        if (decoder != value)
        //        {
        //            if (value == null && decoder != null)
        //            {
        //                //if (BeforeDecoderMobileClear != null)
        //                //    BeforeDecoderMobileClear(this, EventArgs.Empty);
        //            }

        //            decoder = value;
        //            OnPropertyChanged(new PropertyChangedEventArgs("DecoderMobile"));
        //            if (decoder != null)
        //                decoder.PropertyChanged += Decoder_PropertyChanged;
        //        }
        //    }
        //}
        //public Decoder DecoderSound
        //{
        //    get { return decoderSound; }
        //    set
        //    {
        //        if (decoderSound != value)
        //        {
        //            decoderSound = value;
        //            OnPropertyChanged(new PropertyChangedEventArgs("DecoderSound"));
        //            if (decoderSound != null)
        //                decoderSound.PropertyChanged += DecoderSound_PropertyChanged;
        //        }
        //    }
        //}
        #endregion

        #region Events
        //public event EventHandler BeforeDecoderMobileClear;
        #endregion

        #region Constructor
        public Locomotive()
            :base(LayoutItemType.Locomotive)
        {
            protocol = ProtocolType.DCC28;
        }
        #endregion

        #region Event handlers
        //private void Decoder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs("DecoderMobile"));
        //}
        //private void DecoderSound_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs("DecoderSound"));
        //}
        #endregion

        #region XmlSerialization
        public override void WriteToXml(XmlWriter xmlWriter)
        {
            base.WriteToXml(xmlWriter);

            xmlWriter.WriteAttributeString("IsUsedInConsist", IsUsedInConsist ? bool.TrueString : bool.FalseString);
            //if (decoder != null)
            //    xmlWriter.WriteAttributeString("Decoder", decoder.SaveToXml());
        }
        public override void ReadFromXml(XmlReader xmlReader)
        {
            base.ReadFromXml(xmlReader);

            if (!Utils.IsStringNullOrEmpty(xmlReader.GetAttribute("IsUsedInConsist")))
                IsUsedInConsist = xmlReader.GetAttribute("IsUsedInConsist") == bool.TrueString;

        //    if (el.HasAttribute("Decoder"))
        //    {
        //        decoder = null;
        //        Decoder d = new Decoder();
        //        if (d.LoadFromXml(el.GetAttribute("Decoder")))
        //        {
        //            decoder = d;
        //            decoder.PropertyChanged += Decoder_PropertyChanged;
        //        }
        //    }
        }
        #endregion
    }
}
