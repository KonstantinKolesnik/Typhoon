using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Xml;
using Typhoon.Core;
using Typhoon.Localization;
using Typhoon.NMRA;

namespace Typhoon.Decoders
{
    public class Decoder : INotifyPropertyChanged
    {
        #region Fields
        private DecoderType type = DecoderType.Locomotive;
        private SpeedSteps speedSteps = SpeedSteps.Speed28;
        private ObservableCollection<DecoderParameter> parameters = new ObservableCollection<DecoderParameter>();
        private ObservableCollection<DecoderFeature> features = new ObservableCollection<DecoderFeature>();
        #endregion

        #region Properties
        public DecoderType Type
        {
            get { return type; }
        }
        public ObservableCollection<DecoderFeature> Features
        {
            get { return features; }
        }
        public ObservableCollection<DecoderParameter> Parameters
        {
            get { return parameters; }
        }

        #region Helper properties
        public string Model
        {
            get
            {
                foreach (DecoderFeature feature in features)
                    if (feature.Name == "Model")
                        return feature.Value;
                return "";
            }
        }
        public string Manufacturer
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsManufacturer)
                        return NMRAManufacturers.Get((byte)param.Value.Value);
                return "";
            }
        }

        public uint LocomotivePrimaryAddress
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsPrimaryAddress && param.Value.HasValue)
                        return param.Value.Value;
                return 3;
            }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsPrimaryAddress)
                    {
                        value = AdjustParameterValue(value, param);
                        if (!param.Value.HasValue || param.Value != value)
                        {
                            param.Value = value;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotivePrimaryAddress"));
                        }
                        break;
                    }
            }
        }
        public uint LocomotiveExtendedAddress
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsExtendedAddress && param.Value.HasValue)
                        return param.Value.Value;
                return 0;
            }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsExtendedAddress)
                    {
                        value = AdjustParameterValue(value, param);
                        if (!param.Value.HasValue || param.Value != value)
                        {
                            param.Value = value;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveExtendedAddress"));
                        }
                        break;
                    }
            }
        }
        public bool LocomotiveUseExtendedAddress
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsUseExtendedAddress && param.Value.HasValue)
                        return param.Value.Value == 1;
                return false;
            }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsUseExtendedAddress)
                    {
                        uint v = (uint)(value ? 1 : 0);
                        if (!param.Value.HasValue || param.Value != v)
                        {
                            param.Value = v;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveUseExtendedAddress"));
                        }
                        break;
                    }
            }
        }
        public LocomotiveAddress LocomotiveAddress
        {
            get { return new LocomotiveAddress((LocomotiveUseExtendedAddress ? LocomotiveExtendedAddress : LocomotivePrimaryAddress), LocomotiveUseExtendedAddress); }
        }

        public SpeedSteps LocomotiveSpeedSteps
        {
            get { return speedSteps; }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.Is28_128SpeedSteps)
                    {
                        if (speedSteps != value)
                        {
                            speedSteps = value;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveSpeedSteps"));

                            uint v = (uint)(value != SpeedSteps.Speed14 ? 1 : 0);
                            if (!param.Value.HasValue || param.Value != v)
                            {
                                param.Value = v;
                            }
                        }
                        break;
                    }
            }
        }

        public uint LocomotiveConsistAddress
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsConsistAddress && param.Value.HasValue)
                        return param.Value.Value;
                return 0;
            }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsConsistAddress)
                    {
                        value = AdjustParameterValue(value, param);
                        if (!param.Value.HasValue || param.Value != value)
                        {
                            param.Value = value;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveConsistAddress"));
                        }
                        break;
                    }
            }
        }
        public bool LocomotiveConsistNormalDirection
        {
            get
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsConsistDirection && param.Value.HasValue)
                        return param.Value.Value == 0;
                return true;
            }
            set
            {
                foreach (DecoderParameter param in parameters)
                    if (param.IsConsistDirection)
                    {
                        uint v = (uint)(value ? 0 : 1);
                        if (!param.Value.HasValue || param.Value != v)
                        {
                            param.Value = v;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveConsistNormalDirection"));
                        }
                        break;
                    }
            }
        }
        #endregion
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public Decoder()
        {
        }
        #endregion

        #region Public methods
        public bool LoadFromFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fileName);
                    return LoadFromXmlDocument(doc);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }
        public bool LoadFromXml(string xml)
        {
            if (!String.IsNullOrEmpty(xml))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    return LoadFromXmlDocument(doc);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return false;
        }
        public void SaveToFile(string fileName)
        {
            SaveToXmlDocument().Save(fileName);
        }
        public string SaveToXml()
        {
            return SaveToXmlDocument().OuterXml;
        }
        public override string ToString()
        {
            return Manufacturer + " " + Model;
        }
        #endregion

        #region Private methods
        private void Feature_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("Features"));
        }
        private void Parameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DecoderParameter param = sender as DecoderParameter;
            if (param.IsPrimaryAddress)
                OnPropertyChanged(new PropertyChangedEventArgs("LocomotivePrimaryAddress"));
            else if (param.IsExtendedAddress)
                OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveExtendedAddress"));
            else if (param.IsUseExtendedAddress)
                OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveUseExtendedAddress"));
            else if (param.Is28_128SpeedSteps)
            {
                if (!param.Value.HasValue)
                {
                    if (speedSteps != SpeedSteps.Speed28)
                    {
                        speedSteps = SpeedSteps.Speed28;
                        OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveSpeedSteps"));
                    }
                }
                else
                {
                    if (param.Value.Value == 0) // 14 speed steps
                    {
                        if (speedSteps != Decoders.SpeedSteps.Speed14)
                        {
                            speedSteps = Decoders.SpeedSteps.Speed14;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveSpeedSteps"));
                        }
                    }
                    else // 28/128 14 speed steps
                    {
                        if (speedSteps == Decoders.SpeedSteps.Speed14)
                        {
                            speedSteps = Decoders.SpeedSteps.Speed28;
                            OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveSpeedSteps"));
                        }
                    }
                }
            }
            else if (param.IsConsistAddress)
                OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveConsistAddress"));
            else if (param.IsConsistDirection)
                OnPropertyChanged(new PropertyChangedEventArgs("LocomotiveConsistNormalDirection"));
            else
                OnPropertyChanged(new PropertyChangedEventArgs("Parameter")); // just to inform that any other parameter changed
        }

        private uint AdjustParameterValue(uint value, DecoderParameter param)
        {
            if (param.IsValueRange)
            {
                value = Math.Min(value, param.ValueRange.Max);
                value = Math.Max(value, param.ValueRange.Min);
            }
            return value;
        }

        private bool LoadFromXmlDocument(XmlDocument doc)
        {
            if (doc == null || doc.DocumentElement.Name != "Decoder")
            {
                string s = LanguageDictionary.Current.Translate<string>("InvalidDecoderFile", "Text", "Invalid decoder file!");
                MessageBox.Show(s, Application.Current.MainWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (doc.DocumentElement.HasAttribute("Type"))
                type = Helpers.GetEnumValue<DecoderType>(doc.DocumentElement.GetAttribute("Type"));

            if (type == DecoderType.Locomotive && doc.DocumentElement.HasAttribute("SpeedSteps"))
            {
                uint a;
                if (uint.TryParse(doc.DocumentElement.GetAttribute("SpeedSteps"), out a))
                {
                    if (Enum.GetName(typeof(SpeedSteps), a) != null)
                        speedSteps = (SpeedSteps)a;
                }
            }

            features.Clear();
            XmlNodeList lst = doc.SelectNodes("/Decoder/Feature");
            foreach (XmlElement el in lst)
            {
                DecoderFeature feature = DecoderFeature.FromXmlElement(el);
                if (feature != null)
                {
                    features.Add(feature);
                    feature.PropertyChanged += new PropertyChangedEventHandler(Feature_PropertyChanged);
                }
            }

            parameters.Clear();
            lst = doc.SelectNodes("/Decoder/Parameter");
            foreach (XmlElement el in lst)
            {
                DecoderParameter param = DecoderParameter.FromXmlElement(el);
                if (param != null)
                {
                    param.PropertyChanged += Parameter_PropertyChanged;
                    parameters.Add(param);

                    if (param.Is28_128SpeedSteps && param.Value.HasValue)
                    {
                        if (param.Value.Value == 0) // 14 speed steps
                        {
                            if (speedSteps != SpeedSteps.Speed14)
                                speedSteps = SpeedSteps.Speed14;
                        }
                        else // 28/128 14 speed steps
                        {
                            if (speedSteps == SpeedSteps.Speed14)
                                speedSteps = SpeedSteps.Speed28;
                        }
                    }
                }
            }

            return true;
        }
        private XmlDocument SaveToXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Decoder/>");
            doc.DocumentElement.SetAttribute("Type", Helpers.GetEnumValueName<DecoderType>(type));
            if (type == DecoderType.Locomotive)
                doc.DocumentElement.SetAttribute("SpeedSteps", ((int)speedSteps).ToString());

            foreach (DecoderFeature feature in features)
            {
                XmlElement el = doc.CreateElement("Feature");
                feature.WriteToXmlElement(ref el);
                doc.DocumentElement.AppendChild(el);
            }
            
            foreach (DecoderParameter param in parameters)
            {
                XmlElement el = doc.CreateElement("Parameter");
                param.WriteToXmlElement(ref el);
                doc.DocumentElement.AppendChild(el);
            }

            return doc;
        }
        #endregion
    }
}
