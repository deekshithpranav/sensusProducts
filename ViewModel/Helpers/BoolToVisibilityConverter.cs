using System.Globalization;
using System.Windows.Data;
using System.Windows;
using System;

namespace sensusProducts.ViewModel.Helpers
{

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && targetType == typeof(Visibility))
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibilityValue && targetType == typeof(bool))
            {
                return visibilityValue == Visibility.Visible;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}