using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CG_Final.Properties;
using CG_Final.Util;

namespace CG_Final
{
    [Serializable]
    [XmlInclude(typeof(PerspectiveCamera))]
    public class Camera : ModelBase
    {
        #region Properties
        public Vector ViewUp
        {
            get { return _viewUp; }
            set
            {
                SetProperty(ref _viewUp, value);
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
                SetProperty(ref _p, value);
            }
        }

        public Point Vrp
        {
            get { return _vrp; }
            set
            {
                SetProperty(ref _vrp, value);
            }
        }

        [XmlAttribute]
        public double WorldMaxWidth
        {
            get { return _worldMaxWidth; }
            set
            {
                SetProperty(ref _worldMaxWidth, value);
            }
        }

        [XmlAttribute]
        public double WorldMaxHeight
        {
            get { return _worldMaxHeight; }
            set
            {
                SetProperty(ref _worldMaxHeight, value);
            }
        }

        [XmlAttribute]
        public double WorldMinWidth
        {
            get { return _worldMinWidth; }
            set
            {
                SetProperty(ref _worldMinWidth, value);
            }
        }

        [XmlAttribute]
        public double WorldMinHeight
        {
            get { return _worldMinHeight; }
            set
            {
                SetProperty(ref _worldMinHeight, value);
            }
        }

        [XmlIgnore]
        public Matrix SruSrc => _sru_src;

        [XmlIgnore]
        public Matrix SrcSrt => _src_srt;

        [XmlIgnore]
        public Matrix TransformationMatrix => _transformationMatrix;

        [XmlIgnore]
        public ZBuffer ZBuffer => _canvas;
        #endregion

        #region Fields
        [NonSerialized]
        [XmlIgnore]
        private readonly ZBuffer _canvas;

        [NonSerialized]
        [XmlIgnore]
        private Point _p;

        [NonSerialized]
        [XmlIgnore]
        private Point _vrp;

        [NonSerialized]
        [XmlIgnore]
        protected Matrix _sru_src;

        [NonSerialized]
        [XmlIgnore]
        protected Matrix _src_srt;

        [NonSerialized]
        [XmlIgnore]
        private double _worldMaxWidth;

        [NonSerialized]
        [XmlIgnore]
        private double _worldMaxHeight;

        [NonSerialized]
        [XmlIgnore]
        private double _worldMinWidth;

        [NonSerialized]
        [XmlIgnore]
        private double _worldMinHeight;

        [NonSerialized]
        [XmlIgnore]
        private Vector _viewUp;

        [NonSerialized]
        [XmlIgnore]
        protected Matrix _transformationMatrix;

        [NonSerialized]
        [XmlIgnore]
        protected IDrawer _drawAction;
        #endregion

        public Camera() : this(new Point(), new Point(z: 100), new Vector(y: 1))
        {

        }

        public Camera(Point p, Point vrp, Vector viewUp)
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
            _viewUp = viewUp;
            _drawAction = new OccultWire(this);
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
                [0, 0] = (ZBuffer.WindowMaxWidth - ZBuffer.WindowMinWidth) / (double)(WorldMaxWidth - WorldMinWidth),
                [1, 1] = (ZBuffer.WindowMinHeight - ZBuffer.WindowMaxHeight) / (double)(WorldMaxHeight - WorldMinHeight),
                [3, 0] = -WorldMinWidth * ((ZBuffer.WindowMaxWidth - ZBuffer.WindowMinWidth) / (WorldMaxWidth - WorldMinWidth)) + ZBuffer.WindowMinWidth,
                [3, 1] = WorldMinHeight * ((ZBuffer.WindowMaxHeight - ZBuffer.WindowMinHeight) / (WorldMaxHeight - WorldMinHeight)) + ZBuffer.WindowMaxHeight
            };

            ApplyTransformation();
        }

        protected virtual void ApplyTransformation()
        {
            _transformationMatrix = new Matrix();

            _transformationMatrix.Concatenate(_src_srt);
            _transformationMatrix.Concatenate(_sru_src);
        }

        public virtual void DrawScene()
        {
            UpdateCameraParameters();

            _drawAction.Draw();

            OnPropertyChanged(nameof(ZBuffer));
        }

        public virtual Point TransformPoint(Point p)
        {
            return _transformationMatrix * p;
        }

        public void ToOccultWireframe()
        {
            _drawAction = new OccultWire(this);
        }

        public void ToWireframe()
        {
            _drawAction = new Wireframe(this);
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
                SetProperty(ref _dp, value);
            }
        }

        public PerspectiveCamera() : this(new Point(), new Point(z: 100), new Vector(y: 1))
        {
        }

        public PerspectiveCamera(Point p, Point vrp, Vector viewUp) : base(p, vrp, viewUp)
        {
            _dp = 150;
        }

        protected override void UpdateCameraParameters()
        {
            _pers = new Matrix
            {
                [2, 3] = -1 / Dp,
                [3, 3] = 0
            };

            base.UpdateCameraParameters();
        }

        protected override void ApplyTransformation()
        {
            _transformationMatrix = new Matrix();

            _transformationMatrix.Concatenate(_src_srt);
            _transformationMatrix.Concatenate(_pers);
            _transformationMatrix.Concatenate(_sru_src);
        }

        public override Point TransformPoint(Point p)
        {
            var point = _sru_src * p;
            var z = point.Z;

            point = _transformationMatrix * point;
            point.Z = z;

            return point;

            //return _transformationMatrix * p;
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
            var color = Settings.Default.LineDefaultColor;

            foreach (var objectBase in currs.Objects)
            {
                foreach (var edge in objectBase.Edges)
                {
                    if (!_points.ContainsKey(edge.Init))
                        _points.Add(edge.Init, _owner.TransformPoint(objectBase.TransformVertex(edge.Init)));
                    if (!_points.ContainsKey(edge.End))
                        _points.Add(edge.End, _owner.TransformPoint(objectBase.TransformVertex(edge.End)));

                    _owner.ZBuffer.DrawLine(_points[edge.Init], _points[edge.End], color);
                }
            }

            _points.Clear();
        }
    }

    public class OccultWire : IDrawer
    {
        private readonly Camera _owner;
        private readonly Dictionary<Vertex, Point> _points;

        public OccultWire(Camera owner)
        {
            _owner = owner;
            _points = new Dictionary<Vertex, Point>();
        }

        public void Draw()
        {
            var currs = Scene.CurrentScene;
            var color = Settings.Default.LineDefaultColor;

            foreach (var objectBase in currs.Objects)
            {
                foreach (var face in objectBase.Faces.Where(f => f.NormalVector(objectBase).DotProduct(_owner.N) > 0))
                {
                    _owner.ZBuffer.DrawWiredPolygon(color, face.GetVerticesClockWise().Select(objectBase.TransformVertex).Select(_owner.TransformPoint).ToList());
                }
            }

            _points.Clear();
        }
    }

    public class FlatShading : IDrawer
    {
        private readonly Camera _owner;

        public FlatShading(Camera owner)
        {
            _owner = owner;
        }

        public void Draw()
        {
            var curs = Scene.CurrentScene;
            var color = Settings.Default.LineDefaultColor;

            foreach (var objectBase in curs.Objects)
            {
                //foreach (var face in objectBase.Faces.Where(f => f.NormalVector().DotProduct(_owner.N) > 0))
                {
                    //_owner.ZBuffer.DrawPolygon(color, face.GetVerticesClockWise().Select(objectBase.TransformVertex).Select(_owner.TransformPoint).ToArray());
                }
            }
        }
    }
}
