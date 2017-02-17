using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

namespace CG_Final
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public Scene CurrentScene => Scene.CurrentScene;

        public MainWindow()
        {
            Scene.CurrentScene = new Scene();
            InitializeComponent();
            Scene.CurrentScene.Camera1.DrawScene();
            Scene.CurrentScene.Camera2.DrawScene();
            Scene.CurrentScene.Camera3.DrawScene();
            Scene.CurrentScene.Camera4.DrawScene();
        }
    }
}
