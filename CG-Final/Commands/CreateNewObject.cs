using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CG_Final.Annotations;
using CG_Final.Util;

namespace CG_Final.Commands
{
    public class CreateNewObject : ModelBase, IObjectCommand, IValueConverter
    {
        public const int MaxVertices = 17;
        private int _vertices;
        private readonly ObjectBase _obj;

        public int Vertices
        {
            get { return _vertices; }
            set
            {
                if (value > MaxVertices)
                    return;
                if (value < 0)
                    return;

                SetProperty(ref _vertices, value);
            }
        }

        public CreateNewObject(IInputElement owner, ObjectBase obj)
        {
            owner.MouseWheel += Owner_MouseWheel;
            owner.KeyUp += Owner_KeyUp;
            PropertyChanged += (o, e) =>
            {
                _obj.ChangeVertices(Vertices);
                Scene.CurrentScene.Redraw();
            };
            _obj = obj;
            Vertices = 0;
        }

        private void Owner_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Right:
                    ++Vertices;
                    break;
                case Key.Down:
                case Key.Left:
                    --Vertices;
                    break;
                case Key.Enter:
                    Apply();
                    break;
            }
        }

        private void Owner_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                ++Vertices;
            else
                --Vertices;
        }

        public void Deactivate(IInputElement e)
        {
            e.KeyUp -= Owner_KeyUp;
            e.MouseWheel -= Owner_MouseWheel;
        }

        public void Apply()
        {
            OnApply?.Invoke();
        }

        public event Action OnApply;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val;
            if (int.TryParse(value.ToString(), out val))
                return val - 3;

            return Vertices;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
                return (Vertices + 3).ToString();

            throw new ArgumentException();
        }
    }
}