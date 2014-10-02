using MFE.Core;
using Microsoft.SPOT;
using System;
using System.Ext.Xml;
using System.Xml;

namespace Typhoon.MF.Layouts.LayoutItems
{
    public class Locomotive : LayoutItem
    {
        #region Fields
        private ProtocolType protocol;
        private Guid consistID = Guid.Empty;
        private bool consistForward = true;
        //private Decoder decoder = null;
        //private Decoder decoderSound = null;
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
                    OnPropertyChanged(new PropertyChangedEventArgs("Protocol", old, protocol));
                }
            }
        }
        public Guid ConsistID
        {
            get { return consistID; }
            set
            {
                if (consistID.ToString() != value.ToString())
                {
                    Guid old = consistID;
                    consistID = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ConsistID", old, consistID));
                }
            }
        }
        public bool ConsistForward
        {
            get { return consistForward; }
            set
            {
                if (consistForward != value)
                {
                    bool old = consistForward;
                    consistForward = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ConsistForward", old, consistForward));
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

        #region Constructors
        public Locomotive()
            : this(Guid.NewGuid())
        {
        }
        public Locomotive(Guid id)
            : base(id)
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

            if (consistID.ToString() != Guid.Empty.ToString())
            {
                xmlWriter.WriteAttributeString("ConsistID", Utils.ToBase64String(consistID));
                xmlWriter.WriteAttributeString("ConsistForward", consistForward ? bool.TrueString : bool.FalseString);
            }
            //if (decoder != null)
            //    xmlWriter.WriteAttributeString("Decoder", decoder.SaveToXml());
        }
        public override void ReadFromXml(XmlReader xmlReader)
        {
            base.ReadFromXml(xmlReader);

            if (!Utils.StringIsNullOrEmpty(xmlReader.GetAttribute("ConsistID")))
            {
                consistID = Utils.FromBase64StringToGuid(xmlReader.GetAttribute("ConsistID"));
                if (!Utils.StringIsNullOrEmpty(xmlReader.GetAttribute("ConsistForward")))
                    consistForward = xmlReader.GetAttribute("ConsistForward") == bool.TrueString;
            }
            //if (el.HasAttribute("Decoder"))
            //{
            //    decoder = null;
            //    Decoder d = new Decoder();
            //    if (d.LoadFromXml(el.GetAttribute("Decoder")))
            //    {
            //        decoder = d;
            //        decoder.PropertyChanged += Decoder_PropertyChanged;
            //    }
            //}
        }
        #endregion
    }
}
