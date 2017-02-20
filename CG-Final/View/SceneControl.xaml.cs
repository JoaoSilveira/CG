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
                _selectedCamera = value;
                OnPropertyChanged();
            }
        }

        public SceneControl()
        {
            InitializeComponent();
            SelectedCamera = Scene.CurrentScene.Camera1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Scene.CurrentScene.Redraw();
        }

        private void camera_Click(object sender, RoutedEventArgs e)
        {
            Camera newCamera;
            if (SelectedCamera is PerspectiveCamera)
            {
                newCamera = new Camera(SelectedCamera.P, SelectedCamera.Vrp, SelectedCamera.ViewUp)
                {
                    WorldMinWidth = SelectedCamera.WorldMinWidth,
                    WorldMaxWidth = SelectedCamera.WorldMaxWidth,
                    WorldMinHeight = SelectedCamera.WorldMinHeight,
                    WorldMaxHeight = SelectedCamera.WorldMaxHeight
                };
            }
            else
            {
                newCamera = new PerspectiveCamera(SelectedCamera.P, SelectedCamera.Vrp, SelectedCamera.ViewUp)
                {
                    WorldMinWidth = SelectedCamera.WorldMinWidth,
                    WorldMaxWidth = SelectedCamera.WorldMaxWidth,
                    WorldMinHeight = SelectedCamera.WorldMinHeight,
                    WorldMaxHeight = SelectedCamera.WorldMaxHeight
                };
            }

            ChangeCamera(newCamera);
        }

        private void ChangeCamera(Camera newCamera)
        {
            newCamera.DrawScene();
            if (ReferenceEquals(SelectedCamera, Scene.CurrentScene.Camera1))
            {
                Scene.CurrentScene.Camera1 = newCamera;
                SelectedCamera = Scene.CurrentScene.Camera1;
            }
            else if (ReferenceEquals(SelectedCamera, Scene.CurrentScene.Camera2))
            {
                Scene.CurrentScene.Camera2 = newCamera;
                SelectedCamera = Scene.CurrentScene.Camera2;
            }
            else if (ReferenceEquals(SelectedCamera, Scene.CurrentScene.Camera3))
            {
                Scene.CurrentScene.Camera3 = newCamera;
                SelectedCamera = Scene.CurrentScene.Camera3;
            }
            else if (ReferenceEquals(SelectedCamera, Scene.CurrentScene.Camera4))
            {
                Scene.CurrentScene.Camera4 = newCamera;
                SelectedCamera = Scene.CurrentScene.Camera4;
            }
        }

        private void OccultWireClick(object sender, RoutedEventArgs e)
        {
            SelectedCamera.ToOccultWireframe();
            Scene.CurrentScene.Redraw();
        }

        private void WireframeClick(object sender, RoutedEventArgs e)
        {
            SelectedCamera.ToWireframe();
            Scene.CurrentScene.Redraw();
        }
    }
}
