using System;
using System.Globalization;
using System.Windows.Data;

namespace Typhoon.ValueConverters
{
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int v = (int)value;
            Array values = Enum.GetValues(targetType);
            foreach (var val in values)
            {
                if ((int)val == v)
                    return val;
            }

            return null;
        }
    }
}
