using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CG_Final.Properties;

namespace CG_Final
{
    class Raster
    {
        public void DrawObject(Camera camera, ObjectBase obj)
        {
            foreach (var face in obj.Faces)
            {
                if (camera.N.DotProduct(face.NormalVector()) <= 0)
                    continue;


            }
        }
    }

    // DESOCULTADO \o/
    class Line
    {
        private Point _initPoint;
        private Point _endPoint;

        // NOT the president
        public Color Color { get; set; }

        public Point InitPoint
        {
            get { return _initPoint; }
            set { _initPoint = value; }
        }

        public Point EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }


        public Line(Point init, Point end)
        {
            InitPoint = init;
            EndPoint = end;
            Color = Settings.Default.LineDefaultColor;
        }

        public void Draw(ZBuffer img)
        {
            if (!Clip(ref _initPoint, ref _endPoint, ZBuffer.MinSize, ZBuffer.MaxSize))
                return;

            MidpointLine((int)InitPoint.X, (int)InitPoint.Y, (int)EndPoint.X, (int)EndPoint.Y, img);
        }

        private void MidpointLine(int x, int y, int x2, int y2, ZBuffer img)
        {
            var w = x2 - x;
            var h = y2 - y;
            var dx1 = 0;
            var dy1 = 0;
            var dx2 = 0;
            var dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            var longest = Math.Abs(w);
            var shortest = Math.Abs(h);

            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            var numerator = longest >> 1;
            for (var i = 0; i <= longest; i++)
            {
                img.SetPixel(x, y, 0, Color);
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }
        }

        private static bool Clip(ref Point val1, ref Point val2, Point min, Point max)
        {
            var outcode0 = InitFlags(val1, min, max);
            var outcode1 = InitFlags(val2, min, max);
            var accept = false;

            while (true)
            {
                if ((outcode0 | outcode1) == CohenFlags.None)
                {
                    accept = true;
                    break;
                }
                if ((outcode0 & outcode1) != CohenFlags.None)
                {
                    break;
                }

                var x = 0.0;
                var y = 0.0;

                var outcodeOut = outcode0 != 0 ? outcode0 : outcode1;

                if ((outcodeOut & CohenFlags.Top) == CohenFlags.Top)
                {
                    x = val1.X + (val2.X - val1.X) * (min.Y - val1.Y) / (val2.Y - val1.Y);
                    y = min.Y;
                }
                else if ((outcodeOut & CohenFlags.Bottom) == CohenFlags.Bottom)
                {
                    x = val1.X + (val2.X - val1.X) * (max.Y - val1.Y) / (val2.Y - val1.Y);
                    y = max.Y;
                }
                else if ((outcodeOut & CohenFlags.Right) == CohenFlags.Right)
                {
                    y = val1.Y + (val2.Y - val1.Y) * (max.X - val1.X) / (val2.X - val1.X);
                    x = max.X;
                }
                else if ((outcodeOut & CohenFlags.Left) == CohenFlags.Left)
                {
                    y = val1.Y + (val2.Y - val1.Y) * (min.X - val1.X) / (val2.X - val1.X);
                    x = min.X;
                }

                // Now we move outside point to intersection point to clip
                // and get ready for next pass.
                if (outcodeOut == outcode0)
                {
                    val1.X = x;
                    val1.Y = y;
                    outcode0 = InitFlags(val1, min, max);
                }
                else
                {
                    val2.X = x;
                    val2.Y = y;
                    outcode1 = InitFlags(val2, min, max);
                }
            }

            return accept;
        }

        private static CohenFlags InitFlags(Point p, Point min, Point max)
        {
            var code = CohenFlags.None;

            if (p.X < min.X)
                code |= CohenFlags.Left;
            else if (p.X > max.X)
                code |= CohenFlags.Right;

            if (p.Y < min.Y)
                code |= CohenFlags.Top;
            else if (p.Y > max.Y)
                code |= CohenFlags.Bottom;

            return code;
        }
    }

    class Polygon
    {
        private double xmin;
        private double xmax;
        private double ymin;
        private double ymax;
        private readonly List<PolEdge> _object;
        private readonly List<PolEdge> _window;

        public Polygon(Face face)
        {
            _object = new List<PolEdge>();
            _window = new List<PolEdge>();
            foreach (var edge in face.GetEdgesClockWise())
            {
                _object.Add(edge.Left == face
                    ? new PolEdge((Point)edge.End, (Point)edge.Init)
                    : new PolEdge((Point)edge.Init, (Point)edge.End));
            }
            _window.Add(new PolEdge(new Point(ZBuffer.WindowMinWidth, ZBuffer.WindowMinHeight), new Point(ZBuffer.WindowMaxWidth, ZBuffer.WindowMinHeight)));
            _window.Add(new PolEdge(new Point(ZBuffer.WindowMaxWidth, ZBuffer.WindowMinHeight), new Point(ZBuffer.WindowMaxWidth, ZBuffer.WindowMaxHeight)));
            _window.Add(new PolEdge(new Point(ZBuffer.WindowMaxWidth, ZBuffer.WindowMaxHeight), new Point(ZBuffer.WindowMinWidth, ZBuffer.WindowMaxHeight)));
            _window.Add(new PolEdge(new Point(ZBuffer.WindowMinWidth, ZBuffer.WindowMaxHeight), new Point(ZBuffer.WindowMinWidth, ZBuffer.WindowMinHeight)));
        }

        private void ClipPolygon()
        {
            var polList = new List<Point>();
            var winList = new List<Point>();
            var entList = new List<Point>();
            var dic = new Dictionary<PolEdge, List<Point>>();

            foreach (var polEdge in _object)
            {
                foreach (var edge in _window)
                {
                    var inter = polEdge.Intersection(edge);

                    if (inter == null) continue;

                    polList.Add(inter);
                    if (ZBuffer.Contains(polEdge.Final))
                        entList.Add(inter);

                    if (!dic.ContainsKey(edge))
                        dic.Add(edge, new List<Point>());
                    dic[edge].Add(inter);
                    break;
                }
                polList.Add(polEdge.Final);
            }

            winList.Add(_window[0].Init);
            if (dic.ContainsKey(_window[0]))
                winList.AddRange(dic[_window[0]].OrderBy(p => p.X - _window[0].Init.X));

            winList.Add(_window[1].Init);
            if (dic.ContainsKey(_window[1]))
                winList.AddRange(dic[_window[1]].OrderBy(p => p.Y - _window[1].Init.Y));

            winList.Add(_window[2].Init);
            if (dic.ContainsKey(_window[2]))
                winList.AddRange(dic[_window[2]].OrderBy(p => _window[2].Init.X - p.X));

            winList.Add(_window[3].Init);
            if (dic.ContainsKey(_window[3]))
                winList.AddRange(dic[_window[3]].OrderBy(p => _window[3].Init.Y - p.Y));

            dic = null;

            var clipped = new List<Point>();

            var activeList = polList;
            var otherList = winList;

            while (entList.Count > 0)
            {
                var p = entList[0];
                entList.RemoveAt(0);
                do
                {
                    clipped.Add(p);
                    var index = activeList.IndexOf(p);
                    var last = index;
                    index++;
                    index %= activeList.Count;

                    p = activeList[index];
                    activeList.RemoveAt(last);

                    if (!otherList.Contains(p)) continue;

                    activeList.Remove(p);

                    var aux = activeList;
                    activeList = otherList;
                    otherList = aux;

                    if (entList.Contains(p))
                        entList.Remove(p);
                } while (!Equals(p, entList[0]));
            }
        }

        private class PolEdge
        {
            public Point Init { get; set; }
            public Point Final { get; set; }

            public PolEdge(Point init, Point end)
            {
                Init = init;
                Final = end;
            }

            public Point Intersection(PolEdge pol)
            {
                var det = (pol.Final.X - pol.Init.X) * (Final.Y - Init.Y) - (pol.Final.Y - pol.Init.Y) * (Final.X - Init.X);

                if (Math.Abs(det) < .00001)
                    return null;

                var u = ((pol.Final.X - pol.Init.X) * (pol.Init.Y - Init.Y) - (pol.Final.Y - pol.Init.Y) * (pol.Init.X - Init.X)) / det;


                return new Point(Init.X + u * (Final.X - Init.X), Init.X + u * (Final.X - Init.X), Init.X + u * (Final.X - Init.X));
            }
        }
    }

    [Flags]
    public enum CohenFlags : byte
    {
        Left = 0x01, Right = 0x02, Top = 0x10, Bottom = 0x20, None = 0
    }
}
