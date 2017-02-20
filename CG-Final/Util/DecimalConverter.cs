using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CG_Final.Util
{
    class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (!double.TryParse(value.ToString(), out val))
                return null;
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            if (!double.TryParse(value.ToString(), out val))
                return null;
            return val;
        }
    }
}
