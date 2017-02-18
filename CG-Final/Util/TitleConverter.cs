using System;
using System.Globalization;
using System.Windows.Data;

namespace CG_Final.Util
{
    public class TitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + " - Blenda Ultimate Pro 4K3D Modeler";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}