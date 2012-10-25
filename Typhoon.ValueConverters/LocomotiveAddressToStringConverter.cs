using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Typhoon.ValueConverters
{
    public class LocomotiveAddressToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue || values[2] == DependencyProperty.UnsetValue)
                return "";

            uint primaryAddress = (uint)values[0];
            uint extendedAddress = (uint)values[1];
            bool useExtendedAddress = (bool)values[2];

            return (useExtendedAddress ? String.Format("{0} (e)", extendedAddress) : primaryAddress.ToString());
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
