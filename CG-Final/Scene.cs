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
using CG_Final.Util;
using Color = System.Drawing.Color;

namespace CG_Final
{
    [Serializable]
    public class Scene : ModelBase
    {
        public static Scene CurrentScene { get; set; }

        [NonSerialized]
        private readonly Camera[] _cameras;

        [NonSerialized]
        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
            }
        }

        #region Camera Properties
        [XmlIgnore]
        public ImageSource Cam1Source => Convert(Camera1.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam2Source => Convert(Camera2.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam3Source => Convert(Camera3.ZBuffer.Bitmap);

        [XmlIgnore]
        public ImageSource Cam4Source => Convert(Camera4.ZBuffer.Bitmap);
        
        public Camera Camera1
        {
            get { return _cameras[0]; }
            set
            {
                if (!SetProperty(ref _cameras[0], value)) return;

                OnPropertyChanged(nameof(Cam1Source));
                Camera1.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam1Source));
            }
        }
        
        public Camera Camera2
        {
            get { return _cameras[1]; }
            set
            {
                if (!SetProperty(ref _cameras[1], value)) return;

                OnPropertyChanged(nameof(Cam2Source));
                Camera2.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam2Source));
            }
        }
        
        public Camera Camera3
        {
            get { return _cameras[2]; }
            set
            {
                if (!SetProperty(ref _cameras[2], value)) return;
                OnPropertyChanged(nameof(Cam3Source));
                Camera3.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam3Source));
            }
        }
        
        public Camera Camera4
        {
            get { return _cameras[3]; }
            set
            {
                if (!SetProperty(ref _cameras[3], value)) return;

                OnPropertyChanged(nameof(Cam4Source));
                Camera4.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(Cam4Source));
            }
        }
        #endregion

        public Color AmbientLight { get; set; }

        [XmlArray]
        public List<Lamp> Lamps { get; set; }

        [XmlArray]
        public ObservableCollection<ObjectBase> Objects { get; set; }

        public Scene()
        {
            _cameras = new Camera[4];
            Camera1 = new PerspectiveCamera(new Point(), new Point(z: 100), new Vector(y: 1));
            Camera2 = new Camera(new Point(), new Point(z: 100), new Vector(y: 1));
            Camera3 = new Camera(new Point(), new Point(y: 100), new Vector(z: -1));
            Camera4 = new Camera(new Point(), new Point(100), new Vector(y: 1));

            Lamps = new List<Lamp>();
            Objects = new ObservableCollection<ObjectBase>();
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

        public void Redraw()
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
    }
}