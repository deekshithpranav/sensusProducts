﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace sensusProducts.ViewModel.Helpers
{
    public class OptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedOption = value as string;
            string targetOption = parameter as string;

            return selectedOption == targetOption;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = (bool)value;
            string targetOption = parameter as string;

            return isSelected ? targetOption : null;
        }
    }

}
