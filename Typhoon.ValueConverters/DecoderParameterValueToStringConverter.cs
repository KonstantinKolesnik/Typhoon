using System;
using System.Globalization;
using System.Windows.Data;
using Typhoon.Decoders;

namespace Typhoon.ValueConverters
{
    public class DecoderParameterValueToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                uint? value = (values[0] == null ? null : (uint?)values[0]);
                if (!value.HasValue)
                    return "";

                DecoderParameter param = values[1] as DecoderParameter;
                if (param == null)
                    return "";

                if (param.ValueChoices != null)
                {
                    foreach (DecoderParameterValueChoice v in param.ValueChoices.Values)
                        if (v.Value == value.Value)
                            return v.Name;
                }
                else if (param.ValueBitFlags != null)
                {
                    string res = "";
                    string end = ", ";
                    foreach (DecoderParameterValueBitFlag v in param.ValueBitFlags.Values)
                        if ((v.Value & value.Value) != 0)
                            res += v.Name + end;
                    if (res.EndsWith(end))
                        res = res.Remove(res.Length - end.Length);
                    return (String.IsNullOrEmpty(res) ? "-" : res);
                }
                else
                    return value.Value.ToString();
            }
            catch (Exception)
            {
            }

            return "";
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
