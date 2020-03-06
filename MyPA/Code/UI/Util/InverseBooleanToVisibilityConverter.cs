using System;
using System.Globalization;
using System.Windows.Data;

namespace MyPA.Code
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rValue = "Hidden";

            if (value is bool)
            {
                if ((bool)value == true)
                    rValue = "Hidden";
                else
                    rValue = "Visible";
            }
            return rValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "Visible":
                    return false;
                case "Hidden":
                    return true;
            }
            return false;
        }
    }
}
