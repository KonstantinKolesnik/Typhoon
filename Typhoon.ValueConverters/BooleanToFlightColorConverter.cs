using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Typhoon.ValueConverters
{
    [ValueConversion(typeof(Boolean), typeof(SolidColorBrush))]
    public class BooleanToFlightColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new SolidColorBrush(value == DependencyProperty.UnsetValue || value == null || !(value is bool) || !(bool)value ? Colors.DimGray : Colors.ForestGreen);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }
    }
}
