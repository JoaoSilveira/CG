using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using CG_Final.Annotations;

namespace CG_Final.Commands
{
    public class CreateNewObject : INotifyPropertyChanged, IObjectCommand
    {
        public const int MaxVertices = 17;
        private int _vertices;
        private ObjectBase _obj;

        public int Vertices
        {
            get { return _vertices; }
            set
            {
                if (value > MaxVertices)
                    return;
                if (value < 0 || value == _vertices)
                    return;

                _vertices = value;
                OnPropertyChanged();
            }
        }

        public CreateNewObject(IInputElement owner)
        {
            owner.MouseWheel += Owner_MouseWheel;
            owner.KeyUp += Owner_KeyUp;
            PropertyChanged += (o, e) => _obj.ChangeVertices(Vertices);
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
    }
}