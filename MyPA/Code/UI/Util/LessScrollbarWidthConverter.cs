using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyPA.Code.UI.Util
{
    public class LessScrollbarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) - (SystemParameters.VerticalScrollBarWidth * 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
