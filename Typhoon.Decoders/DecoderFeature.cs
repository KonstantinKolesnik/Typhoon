using System;
using System.ComponentModel;
using System.Xml;

namespace Typhoon.Decoders
{
    public class DecoderFeature : INotifyPropertyChanged
    {
        #region Fields
        private string name = "";
        private string value = "";
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
        public string Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Value"));
                }
            }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructors
        public DecoderFeature()
        {
        }
        public DecoderFeature(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        #endregion

        #region XmlSerialization
        public void WriteToXmlElement(ref XmlElement el)
        {
            if (!String.IsNullOrEmpty(name))
                el.SetAttribute("Name", name);
            if (!String.IsNullOrEmpty(value))
                el.SetAttribute("Value", value);
        }
        public void ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("Name"))
                name = el.GetAttribute("Name");
            if (el.HasAttribute("Value"))
                value = el.GetAttribute("Value");
        }

        public static DecoderFeature FromXmlElement(XmlElement el)
        {
            DecoderFeature feature = new DecoderFeature();
            feature.ReadFromXmlElement(el);
            return feature;
        }
        #endregion
    }
}
