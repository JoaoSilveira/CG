using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using CG_Final.Annotations;
using Color = System.Drawing.Color;

namespace CG_Final
{
    [Serializable]
    public class Scene : INotifyPropertyChanged
    {
        public static Scene CurrentScene { get; set; }

        private readonly Camera _camera1;
        private readonly Camera _camera2;
        private readonly Camera _camera3;
        private readonly Camera _camera4;

        [XmlIgnore]
        public ImageSource Cam1Source => Convert(_camera1.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam2Source => Convert(_camera2.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam3Source => Convert(_camera3.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam4Source => Convert(_camera4.ZBuffer.Bitmap);

        public Camera Camera1 => _camera1;

        public Camera Camera2 => _camera2;

        public Camera Camera3 => _camera3;

        public Camera Camera4 => _camera4;

        public Color AmbientLight { get; set; }

        [XmlArray]
        public List<Lamp> Lamps { get; set; }

        [XmlArray]
        public List<ObjectBase> Objects { get; set; }

        public Scene()
        {
            _camera1 = new PerspectiveCamera(new Point(), new Point(z: 100), new Vector(y: 1));
            _camera2 = new Camera(new Point(), new Point(z: 100), new Vector(y: 1));
            _camera3 = new Camera(new Point(), new Point(y: 100), new Vector(z: -1));
            _camera4 = new Camera(new Point(), new Point(100), new Vector(y: 1));

            _camera1.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam1Source));
            _camera2.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam2Source));
            _camera3.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam3Source));
            _camera4.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam4Source));

            Lamps = new List<Lamp>();
            Objects = new List<ObjectBase>();
            Objects.Add(new ObjectBase());
        }

        public BitmapSource Convert(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  bitmap.GetHbitmap(),
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}