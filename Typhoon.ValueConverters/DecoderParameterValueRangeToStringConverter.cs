using System;
using System.Globalization;
using System.Windows.Data;
using Typhoon.Decoders;

namespace Typhoon.ValueConverters
{
    [ValueConversion(typeof(DecoderParameterValueRange), typeof(string))]
    public class DecoderParameterValueRangeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DecoderParameterValueRange valueRange = value as DecoderParameterValueRange;
            if (valueRange != null)
                return String.Format("{0}...{1}", valueRange.Min, valueRange.Max);
            else
                return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
