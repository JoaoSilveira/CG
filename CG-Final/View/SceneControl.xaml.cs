using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CG_Final.Annotations;
using CG_Final.Util;

namespace CG_Final.View
{
    /// <summary>
    /// Interaction logic for SceneControl.xaml
    /// </summary>
    public partial class SceneControl : INotifyPropertyChanged
    {
        private Camera _selectedCamera;

        public Camera SelectedCamera
        {
            get { return _selectedCamera; }
            set
            {
                if (!ReferenceEquals(value, _selectedCamera))
                    _selectedCamera = value;
                OnPropertyChanged();
            }
        }

        public SceneControl()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(cm1, sender))
                SelectedCamera = Scene.CurrentScene.Camera1;
            else if (ReferenceEquals(cm2, sender))
                SelectedCamera = Scene.CurrentScene.Camera2;
            else if (ReferenceEquals(cm3, sender))
                SelectedCamera = Scene.CurrentScene.Camera3;
            else if (ReferenceEquals(cm4, sender))
                SelectedCamera = Scene.CurrentScene.Camera4;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Scene.CurrentScene.Redraw();
        }
    }
}
