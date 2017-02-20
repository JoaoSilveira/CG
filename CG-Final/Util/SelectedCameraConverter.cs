using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CG_Final.Util
{
    class SelectedCameraConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int par;
            if (!int.TryParse(parameter.ToString(), out par))
                return null;
            if (ReferenceEquals(value, Scene.CurrentScene.Camera1) && par == 1)
                return true;
            if (ReferenceEquals(value, Scene.CurrentScene.Camera2) && par == 2)
                return true;
            if (ReferenceEquals(value, Scene.CurrentScene.Camera3) && par == 3)
                return true;
            return ReferenceEquals(value, Scene.CurrentScene.Camera4) && par == 4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int par;
            if (!int.TryParse(parameter.ToString(), out par))
                return null;
            switch (par)
            {
                case 1:
                    return Scene.CurrentScene.Camera1;
                case 2:
                    return Scene.CurrentScene.Camera2;
                case 3:
                    return Scene.CurrentScene.Camera3;
                case 4:
                    return Scene.CurrentScene.Camera4;
                default:
                    return null;
            }
        }
    }
}
