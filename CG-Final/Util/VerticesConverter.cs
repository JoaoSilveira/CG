using System;
using System.Globalization;
using System.Windows.Data;

namespace CG_Final.Util
{
    public class VerticesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            if (int.TryParse(value.ToString(), out val))
                return val + 3;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return null;

            int val;
            if (!int.TryParse(value.ToString(), out val))
                return null;
            if (val < 3 || val > 20)
                return null;

            return val - 3;
        }
    }
}