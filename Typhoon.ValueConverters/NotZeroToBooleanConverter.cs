using System;
using System.Globalization;
using System.Windows.Data;

namespace Typhoon.ValueConverters
{
    [ValueConversion(typeof(int), typeof(Boolean))]
    public class NotZeroToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value != 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
