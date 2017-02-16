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
        public double WorldMaxWidth
        {
            get { return _worldMaxWidth; }
            set
            {
                if (Math.Abs(value - _worldMaxWidth) < 1)
                    return;
                _worldMaxWidth = value;
                UpdateCameraParameters();
            }
        }

        [XmlAttribute]
        public double WorldMaxHeight
        {
            get { return _worldMaxHeight; }
            set
            {
                if (Math.Abs(value - _worldMaxHeight) < 1)
                    return;
                _worldMaxHeight = value;
                UpdateCameraParameters();
            }
        }

        [XmlAttribute]
        public double WorldMinWidth
        {
            get { return _worldMinWidth; }
            set
            {
                if (Math.Abs(value - _worldMinWidth) < 1)
                    return;
                _worldMinWidth = value;
                UpdateCameraParameters();
            }
        }

        [XmlAttribute]
        public double WorldMinHeight
        {
            get { return _worldMinHeight; }
            set
            {
                if (Math.Abs(value - _worldMinHeight) < 1)
                    return;
                _worldMinHeight = value;
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
        private double _worldMaxWidth;

        [NonSerialized]
        private double _worldMaxHeight;

        [NonSerialized]
        private double _worldMinWidth;

        [NonSerialized]
        private double _worldMinHeight;

        [NonSerialized]
        private Vector _viewUp;

        [NonSerialized]
        protected Matrix _transformationMatrix;

        [NonSerialized]
        protected IDrawer _drawAction;
        #endregion

        public Camera() : this(new Point(), new Point(z: 100))
        {

        }

        public Camera(Point p, Point vrp)
        {
            _canvas = new ZBuffer();
            _p = p;
            _vrp = vrp;
            _worldMaxWidth = ZBuffer.Width * 0.8;
            _worldMaxWidth -= _worldMaxWidth / 2;
            _worldMaxHeight = ZBuffer.Height * 0.8;
            _worldMaxHeight -= _worldMaxHeight / 2;
            _worldMinWidth = -_worldMaxWidth;
            _worldMinHeight = -_worldMaxHeight;
            _viewUp = new Vector(y: 1);
            _drawAction = new Wireframe(this);
        }

        protected virtual void UpdateCameraParameters()
        {
            N = Vector.Normalize(_vrp - _p);
            V = Vector.Normalize(ViewUp - ViewUp.DotProduct(N) * N);
            U = Vector.VectorialProduct(V, N);

            _sru_src = new Matrix
            {
                [0, 0] = U.X,
                [1, 0] = U.Y,
                [2, 0] = U.Z,
                [3, 0] = -Vector.DotProduct(new Vector(_vrp), U),
                [0, 1] = V.X,
                [1, 1] = V.Y,
                [2, 1] = V.Z,
                [3, 1] = -Vector.DotProduct(new Vector(_vrp), V),
                [0, 2] = N.X,
                [1, 2] = N.Y,
                [2, 2] = N.Z,
                [3, 2] = -Vector.DotProduct(new Vector(_vrp), N)
            };

            _src_srt = new Matrix
            {
                [0, 0] = (ZBuffer.WindowMaxWidth - ZBuffer.WindowMinWidth) / (WorldMaxWidth - WorldMinWidth),
                [1, 1] = (ZBuffer.WindowMinHeight - ZBuffer.WindowMaxHeight) / (WorldMaxHeight - WorldMinHeight),
                [3, 0] = -WorldMinWidth * ((ZBuffer.WindowMaxWidth - ZBuffer.WindowMinWidth) / (WorldMaxWidth - WorldMinWidth)) + ZBuffer.WindowMinWidth,
                [3, 1] = WorldMinHeight * ((ZBuffer.WindowMaxHeight - ZBuffer.WindowMinHeight) / (WorldMaxHeight - WorldMinHeight)) + ZBuffer.WindowMaxHeight
            };
        }

        protected virtual void ApplyMatrices()
        {
            _transformationMatrix = new Matrix();

            _transformationMatrix.Concatenate(_src_srt);
            _transformationMatrix.Concatenate(_sru_src);
        }

        public void DrawScene()
        {
            UpdateCameraParameters();
            ApplyMatrices();

            _drawAction.Draw();

            OnPropertyChanged(nameof(ZBuffer));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Point TransformPoint(Point p)
        {
            return _transformationMatrix*p;
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

        public PerspectiveCamera() : this (new Point(), new Point(z: 100))
        {
        }

        public PerspectiveCamera(Point p, Point vrp) : base(p, vrp)
        {
            _dp = 50;
        }

        protected override void UpdateCameraParameters()
        {
            base.UpdateCameraParameters();

            _pers = new Matrix
            {
                [2, 3] = -1 / Dp,
                [3, 3] = 0
            };
        }

        protected override void ApplyMatrices()
        {
            _transformationMatrix = new Matrix();

            _transformationMatrix.Concatenate(_src_srt);
            _transformationMatrix.Concatenate(_pers);
        }

        public override Point TransformPoint(Point p)
        {
            var point = _sru_src * p;
            var z = point.Z;

            point = _transformationMatrix * p;
            point.Z = z;

            return point;
        }
    }

    public class Wireframe : IDrawer
    {
        private readonly Camera _owner;
        private readonly Dictionary<Vertex, Point> _points;

        public Wireframe(Camera owner)
        {
            _owner = owner;
            _points = new Dictionary<Vertex, Point>();
        }


        public void Draw()
        {
            var currs = Scene.CurrentScene;

            foreach (var objectBase in currs.Objects)
            {
                foreach (var edge in objectBase.Edges)
                {
                    if (!_points.ContainsKey(edge.Init))
                        _points.Add(edge.Init, _owner.TransformPoint((Point) edge.Init));
                    if (!_points.ContainsKey(edge.End))
                        _points.Add(edge.End, _owner.TransformPoint((Point) edge.End));

                    var line = new Line(_points[edge.Init], _points[edge.End]);

                    line.Draw(_owner.ZBuffer);
                }
            }

            _points.Clear();
        }
    }
}
