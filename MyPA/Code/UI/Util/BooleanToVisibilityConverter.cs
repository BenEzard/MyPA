using System;
using System.Globalization;
using System.Windows.Data;

namespace MyPA.Code
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rValue = "Hidden";

            if (value is bool)
            {
                if ((bool)value == true)
                    rValue = "Visible";
                else
                    rValue = "Hidden";
            }
            return rValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "Hidden":
                    return false;
                case "Visible":
                    return true;
            }
            return false;
        }
    }
}
