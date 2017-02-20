using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CG_Final.Util;

namespace CG_Final.Commands
{
    class ScaleCommand : ModelBase, IObjectCommand
    {
        private readonly ObjectBase _obj;

        private double _x;
        private double _y;
        private double _z;

        public double X
        {
            get { return _x; }
            set { SetProperty(ref _x, value); }
        }

        public double Y
        {
            get { return _y; }
            set { SetProperty(ref _y, value); }
        }

        public double Z
        {
            get { return _z; }
            set { SetProperty(ref _z, value); }
        }

        public ScaleCommand(IInputElement e, ObjectBase o)
        {
            X = 1;
            Y = 1;
            Z = 1;
            _obj = o;
            o.AttachCommand(this);
            e.KeyUp += E_KeyUp;

            PropertyChanged += (sender, args) => Scene.CurrentScene.Redraw();
        }

        private void E_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Apply();
        }

        public void Deactivate(IInputElement e)
        {
            e.KeyUp -= E_KeyUp;
            _obj.DeattachCommand();
        }

        public void Apply()
        {
            _obj.ApplyTransformation(Matrix.ScaleMatrix(X, Y, Z));
            OnApply?.Invoke();
        }

        public Matrix Transformation => Matrix.ScaleMatrix(X, Y, Z);

        public event Action OnApply;
    }
}
