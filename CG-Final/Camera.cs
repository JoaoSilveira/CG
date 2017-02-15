using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CG_Final
{
    [Serializable]
    [XmlInclude(typeof(PerspectiveCamera))]
    public class Camera : INotifyPropertyChanged 
    {
        #region Properties
        public Vector ViewUp
        {
            get { return _viewUp; }
            set
            {
                if (value.Equals(_viewUp))
                    return;

                _viewUp = value;
                UpdateCameraParameters();
            }
        }

        [XmlIgnore]
        public Vector U { get; private set; }

        [XmlIgnore]
        public Vector V { get; private set; }

        [XmlIgnore]
        public Vector N { get; private set; }

        public Point P
        {
            get { return _p; }
            set
            {
                if (_p.Equals(value))
                    return;
                _p = value;
                UpdateCameraParameters();
            }
        }

        public Point VRP
        {
            get { return _vrp; }
            set
            {
                if (_vrp.Equals(value))
                    return;
                _vrp = value;
                UpdateCameraParameters();
            }
        }

        [XmlAttribute]
        public double WorldWidth
        {
            get { return _worldWidth; }
            set
            {
                if (Math.Abs(value - _worldWidth) < 1)
                    return;
                _worldWidth = value;
                UpdateCameraParameters();
            }
        }

        [XmlAttribute]
        public double WorldHeight
        {
            get { return _worldHeight; }
            set
            {
                if (Math.Abs(value - _worldHeight) < 1)
                    return;
                _worldHeight = value;
                UpdateCameraParameters();
            }
        }

        [XmlIgnore]
        public ZBuffer ZBuffer => _canvas;
        #endregion

        #region Fields
        [NonSerialized]
        private readonly ZBuffer _canvas;

        [NonSerialized]
        private Point _p;

        [NonSerialized]
        private Point _vrp;

        [NonSerialized]
        protected Matrix _sru_src;

        [NonSerialized]
        protected Matrix _src_srt;

        [NonSerialized]
        private double _worldWidth;

        [NonSerialized]
        private double _worldHeight;

        [NonSerialized]
        private Vector _viewUp;

        [NonSerialized]
        protected Matrix _transformationMatrix;
        #endregion

        public Camera() : this(new Point(), new Point(z: 50))
        {

        }

        public Camera(Point p, Point vrp)
        {
            _canvas = new ZBuffer();
            _p = p;
            _vrp = vrp;
            _worldWidth = ZBuffer.Width;
            _worldHeight = ZBuffer.Height;
            _viewUp = new Vector(y: 1);
        }

        protected virtual void UpdateCameraParameters()
        {
            N = Vector.Normalize(_vrp - _p);
            V = Vector.Normalize(ViewUp - ViewUp.DotProduct(N) * N);
            U = Vector.VectorialProduct(V, N);

            _sru_src = new Matrix
            {
                [0, 0] = U.X,
                [0, 1] = U.Y,
                [0, 2] = U.Z,
                [0, 3] = -Vector.DotProduct(new Vector(_vrp), U),
                [1, 0] = V.X,
                [1, 1] = V.Y,
                [1, 2] = V.Z,
                [1, 3] = -Vector.DotProduct(new Vector(_vrp), V),
                [2, 0] = N.X,
                [2, 1] = N.Y,
                [2, 2] = N.Z,
                [2, 3] = -Vector.DotProduct(new Vector(_vrp), N)
            };

            _src_srt = new Matrix
            {
                [0, 0] = WorldWidth/ZBuffer.Width,
                [1, 1] = WorldHeight/ZBuffer.Height
            };
        }

        public void DrawScene()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    [Serializable]
    public class PerspectiveCamera : Camera
    {
        [NonSerialized]
        private Matrix _pers;

        [NonSerialized]
        private double _dp;

        [XmlAttribute]
        public double Dp
        {
            get { return _dp; }
            set
            {
                if (Math.Abs(value - _dp) < 0.075)
                    return;
                _dp = value;
                UpdateCameraParameters();
            }
        }

        public PerspectiveCamera()
        {
            _dp = 50;
        }

        public PerspectiveCamera(Point p, Point vrp) : base(p, vrp)
        {

        }

        protected override void UpdateCameraParameters()
        {
            base.UpdateCameraParameters();

            _pers = new Matrix
            {
                [3, 2] = -1/Dp,
                [3, 3] = 0
            };
        }
    }
}
