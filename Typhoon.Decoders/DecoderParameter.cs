using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Typhoon.Core;

namespace Typhoon.Decoders
{
    public class DecoderParameter : INotifyPropertyChanged
    {
        #region Fields
        private string group = "";
        private string name = "";
        private bool isReadonly = false;
        private string text = "";
        private List<DecoderParameterCVMap> cvMaps = new List<DecoderParameterCVMap>(); // goes in order from LSB to MSB

        private DecoderParameterValueRange valueRange = null;
        private DecoderParameterValueChoices valueChoices = null;
        private DecoderParameterValueBitFlags valueBitFlags = null;
        private List<CheckBox> valueBitFlagsStates = new List<CheckBox>();
        private bool manualBitFlagChange = true;

        private uint? defaultValue = null;
        private uint? value = null;
        private bool verified = false;
        #endregion

        #region Properties
        public string Group
        {
            get { return group; }
        }
        public string Name
        {
            get { return name; }
        }
        public bool ReadOnly
        {
            get { return isReadonly; }
        }
        public string Text
        {
            get { return text; }
        }

        public bool IsValueRange
        {
            get { return valueRange != null; }
        }
        public DecoderParameterValueRange ValueRange
        {
            get { return valueRange; }
        }

        public bool IsValueChoices
        {
            get { return valueChoices != null; }
        }
        public DecoderParameterValueChoices ValueChoices
        {
            get { return valueChoices; }
        }

        public bool IsValueBitFlags
        {
            get { return valueBitFlags != null; }
        }
        public DecoderParameterValueBitFlags ValueBitFlags
        {
            get { return valueBitFlags; }
        }
        public List<CheckBox> ValueBitFlagsStates
        {
            get { return valueBitFlagsStates; }
        }

        public uint? DefaultValue
        {
            get { return defaultValue; }
        }
        public uint? Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    SetCheckBoxValues();
                    Verified = false;
                    OnPropertyChanged(new PropertyChangedEventArgs("Value"));
                }
            }
        }
        public bool Verified
        {
            get { return verified; }
            set
            {
                if (verified != value)
                {
                    verified = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Verified"));
                }
            }
        }

        #region Helper props
        public bool IsPrimaryAddress
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 1)
                        return true;
                return false;
            }
        }
        public bool IsExtendedAddress
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 17 || cvMap.CV == 18)
                        return true;
                return false;
            }
        }
        public bool IsUseExtendedAddress
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 29 && cvMap.Mask == 32)
                        return true;
                return false;
            }
        }
        public bool Is28_128SpeedSteps
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 29 && cvMap.Mask == 2)
                        return true;
                return false;
            }
        }
        public bool IsManufacturer
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 8)
                        return true;
                return false;
            }
        }
        public bool IsVersion
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 7)
                        return true;
                return false;
            }
        }
        public bool IsConsistAddress
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 19 && cvMap.Mask == 127)
                        return true;
                return false;
            }
        }
        public bool IsConsistDirection
        {
            get
            {
                foreach (DecoderParameterCVMap cvMap in cvMaps)
                    if (cvMap.CV == 19 && cvMap.Mask == 128)
                        return true;
                return false;
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
        private DecoderParameter()
        {
        }
        #endregion

        #region Public methods
        public List<DecoderParameterProgramUnit> CurrentValueToProgramUnits()
        {
            // check for null Value done in WriteDecoderParameter_CanExecute;
            // for reading, value of Value doesn't matter

            List<DecoderParameterProgramUnit> list = new List<DecoderParameterProgramUnit>();
            if (cvMaps.Count == 1) // all except ExtendedAddress
            {
                DecoderParameterCVMap src = cvMaps[0];
                if (src.Mask == 255) // whole CV used
                {
                    if (Value.HasValue)
                        list.Add(new DecoderParameterProgramUnit(src.CV, (byte)Value.Value));
                    else
                        list.Add(new DecoderParameterProgramUnit(src.CV, null));
                }
                else // set of bits used
                {
                    List<bool> maskbits = Helpers.ByteToBits(src.Mask); // bits used in CV marked as true
                    if (Value.HasValue)
                    {
                        List<bool> valuebits = Helpers.ByteToBits((byte)Value.Value); // 8 bits of parameter Value
                        int v = 0;
                        for (int bitPosition = 0; bitPosition < 8; bitPosition++) // go through mask bits
                        {
                            if (maskbits[bitPosition]) // bit marked as used
                            {
                                list.Add(new DecoderParameterProgramUnit(src.CV, bitPosition, (byte)(valuebits[v] ? 1 : 0)));
                                v++;
                            }
                        }
                    }
                    else
                    {
                        for (int bitPosition = 0; bitPosition < 8; bitPosition++) // go through mask bits
                        {
                            if (maskbits[bitPosition]) // bit is used
                                list.Add(new DecoderParameterProgramUnit(src.CV, bitPosition, null));
                        }
                    }
                }
            }
            else if (cvMaps.Count == 2) // ExtendedAddress
            {
                if (Value.HasValue)
                {
                    byte cv17, cv18;
                    Helpers.ExtendedAddressToCV17_18(Value.Value, out cv17, out cv18);
                    list.Add(new DecoderParameterProgramUnit(18, cv18));
                    list.Add(new DecoderParameterProgramUnit(17, cv17));
                }
                else
                {
                    list.Add(new DecoderParameterProgramUnit(18, null));
                    list.Add(new DecoderParameterProgramUnit(17, null));
                }
            }
            return list;
        }
        public void CurrentValueFromProgramUnits(List<DecoderParameterProgramUnit> list)
        {
            if (list.Count == 2 && list[0].CV == 18 && list[1].CV == 17) // ExtendedAddress
                Value = Helpers.CV17_18ToExtendedAddress(list[1].Value.Value, list[0].Value.Value);
            else
            {
                // suppose parameter doesn't occupied more than 1 CV!!!
                // else TODO: ...

                DecoderParameterCVMap src = cvMaps[0];
                if (src.Mask == 255) // whole CV used
                    Value = list[0].Value.Value;
                else
                {
                    List<byte> bits = new List<byte>();
                    for (int i = 0; i < 8; i++)
                        bits.Add(0);

                    int j = 0;
                    foreach (DecoderParameterProgramUnit pu in list)
                        bits[j++] = pu.Value.Value;

                    Value = Helpers.BitsToByte(bits);
                }
            }
            // parameter is already verified in Station
            Verified = true;
        }
        #endregion

        #region Private methods
        private void CVMapsFromXml(string src)
        {
            cvMaps.Clear();
            string[] res = src.Split(new char[] { ';' });
            foreach (string value in res)
                cvMaps.Add(new DecoderParameterCVMap(value.Trim()));
        }
        private string CVMapsToXml()
        {
            string s = "";
            foreach (DecoderParameterCVMap cvMap in cvMaps)
                s += cvMap.CV.ToString() + "-" + cvMap.Mask.ToString() + ";";

            if (s.EndsWith(";"))
                s = s.Remove(s.Length - 1);

            return s;
        }
        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (manualBitFlagChange)
            {
                uint res = 0;
                foreach (CheckBox chb in valueBitFlagsStates)
                    res += (uint)chb.Tag * (uint)(chb.IsChecked.HasValue ? (chb.IsChecked.Value ? 1 : 0) : 0);
                Value = res;
            }
        }
        private void SetCheckBoxValues()
        {
            manualBitFlagChange = false;
            if (value.HasValue)
                foreach (CheckBox chb in valueBitFlagsStates)
                    chb.IsChecked = (value.Value & (uint)chb.Tag) != 0;
            manualBitFlagChange = true;
        }
        #endregion

        #region XmlSerialization
        private void ReadFromXmlElement(XmlElement el)
        {
            if (el.HasAttribute("Group"))
                group = el.GetAttribute("Group");
            if (el.HasAttribute("Name"))
                name = el.GetAttribute("Name");
            if (el.HasAttribute("ReadOnly") && el.GetAttribute("ReadOnly") == "1")
                isReadonly = true;
            if (el.HasAttribute("Text"))
                text = el.GetAttribute("Text");

            if (el.HasAttribute("CVMaps"))
                CVMapsFromXml(el.GetAttribute("CVMaps"));

            if (el.HasAttribute("Default"))
                defaultValue = uint.Parse(el.GetAttribute("Default"));
            if (el.HasAttribute("Value"))
                value = uint.Parse(el.GetAttribute("Value"));
            else if (defaultValue.HasValue)
                value = defaultValue;
            //else
            //    currentValue = 0; // ???
            if (el.HasAttribute("Verified"))
                verified = el.GetAttribute("Verified") == "1";

            valueRange = DecoderParameterValueRange.FromXmlElement(el);
            valueChoices = DecoderParameterValueChoices.FromXmlElement(el);
            valueBitFlags = DecoderParameterValueBitFlags.FromXmlElement(el);
            valueBitFlagsStates.Clear();
            if (valueBitFlags != null)
            {
                foreach (DecoderParameterValueBitFlag v in valueBitFlags.Values)
                {
                    CheckBox chb = new CheckBox();
                    chb.Content = v.Name;
                    chb.Tag = v.Value;
                    chb.IsChecked = false;
                    chb.Checked += CheckBox_CheckedChanged;
                    chb.Unchecked += CheckBox_CheckedChanged;
                    valueBitFlagsStates.Add(chb);
                }
                SetCheckBoxValues();
            }
        }
        public void WriteToXmlElement(ref XmlElement el)
        {
            if (!String.IsNullOrEmpty(group))
                el.SetAttribute("Group", group);
            if (!String.IsNullOrEmpty(name))
                el.SetAttribute("Name", name);
            if (isReadonly)
                el.SetAttribute("ReadOnly", "1");
            if (!String.IsNullOrEmpty(text))
                el.SetAttribute("Text", text);

            if (cvMaps.Count != 0)
                el.SetAttribute("CVMaps", CVMapsToXml());

            if (valueRange != null)
                valueRange.WriteToXmlElement(ref el);
            if (valueChoices != null)
                valueChoices.WriteToXmlElement(ref el);
            if (valueBitFlags != null)
                valueBitFlags.WriteToXmlElement(ref el);
            
            if (defaultValue.HasValue)
                el.SetAttribute("Default", defaultValue.Value.ToString());
            if (value.HasValue)
                el.SetAttribute("Value", value.Value.ToString());
            el.SetAttribute("Verified", (verified ? "1" : "0"));
        }

        public static DecoderParameter FromXmlElement(XmlElement el)
        {
            DecoderParameter param = new DecoderParameter();
            param.ReadFromXmlElement(el);
            return param;
        }
        #endregion
    }
}
