using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
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

        private Camera _camera1;
        private Camera _camera2;
        private Camera _camera3;
        private Camera _camera4;
        public event PropertyChangedEventHandler PropertyChanged;

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != null && _title.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }


        #region Camera Properties
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
        #endregion

        public Color AmbientLight { get; set; }

        [XmlArray]
        public List<Lamp> Lamps { get; set; }

        [XmlArray]
        public ObservableCollection<ObjectBase> Objects { get; set; }

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
            Objects = new ObservableCollection<ObjectBase> {new ObjectBase()};
            Title = "Untitled";
        }

        public ObjectBase CreateNew()
        {
            var o = new ObjectBase();
            Objects.Add(o);
            return o;
        }

        public BitmapSource Convert(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  bitmap.GetHbitmap(),
                  IntPtr.Zero,
                  Int32Rect.Empty,
                  BitmapSizeOptions.FromEmptyOptions());
        }

        public async Task Redraw()
        {
            Camera1.ZBuffer.Clear();
            Camera1.DrawScene();
            Camera2.ZBuffer.Clear();
            Camera2.DrawScene();
            Camera3.ZBuffer.Clear();
            Camera3.DrawScene();
            Camera4.ZBuffer.Clear();
            Camera4.DrawScene();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}