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

    public static class LineDrawer
    {
        public static void DrawLine(this ZBuffer img, Point initPoint, Point endPoint, Color color)
        {
            var init = new Point(initPoint.X, initPoint.Y, initPoint.Z);
            var end = new Point(endPoint.X, endPoint.Y, endPoint.Z);
            if (!Clip(ref init, ref end, ZBuffer.MinSize, ZBuffer.MaxSize))
                return;

            var x = (int)init.X;
            var y = (int)init.Y;
            var z = init.Z;

            var xf = (int)end.X;
            var yf = (int)end.Y;

            var dx = Math.Abs(xf - x);
            var sx = x < xf ? 1 : -1;
            var dy = Math.Abs(yf - y);
            var sy = y < yf ? 1 : -1;
            var err = (dx > dy ? dx : -dy) / 2;
            var txz = (end.Z - init.Z) / (dx == 0 ? 1 : dx);
            var tyz = (end.Z - init.Z) / (dy == 0 ? 1 : dy);

            while (true)
            {
                img.SetPixel(x, y, z, color);
                if (x == xf && y == yf)
                    break;

                var e2 = err;

                if (e2 > -dx)
                {
                    err -= dy;
                    x += sx;
                    z += txz;
                }

                if (e2 >= dy)
                    continue;

                err += dx;
                y += sy;
                z += tyz;
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
                var z = 0.0;

                var outcodeOut = outcode0 != 0 ? outcode0 : outcode1;

                if ((outcodeOut & CohenFlags.Top) == CohenFlags.Top)
                {
                    x = val1.X + (val2.X - val1.X) * (min.Y - val1.Y) / (val2.Y - val1.Y);
                    z = val1.Z + (val2.Z - val1.Z) * (min.Y - val1.Y) / (val2.Y - val1.Y);
                    y = min.Y;
                }
                else if ((outcodeOut & CohenFlags.Bottom) == CohenFlags.Bottom)
                {
                    x = val1.X + (val2.X - val1.X) * (max.Y - val1.Y) / (val2.Y - val1.Y);
                    z = val1.Z + (val2.Z - val1.Z) * (max.Y - val1.Y) / (val2.Y - val1.Y);
                    y = max.Y;
                }
                else if ((outcodeOut & CohenFlags.Right) == CohenFlags.Right)
                {
                    y = val1.Y + (val2.Y - val1.Y) * (max.X - val1.X) / (val2.X - val1.X);
                    z = val1.Z + (val2.Z - val1.Z) * (max.X - val1.X) / (val2.X - val1.X);
                    x = max.X;
                }
                else if ((outcodeOut & CohenFlags.Left) == CohenFlags.Left)
                {
                    y = val1.Y + (val2.Y - val1.Y) * (min.X - val1.X) / (val2.X - val1.X);
                    z = val1.Z + (val2.Z - val1.Z) * (min.X - val1.X) / (val2.X - val1.X);
                    x = min.X;
                }

                if (outcodeOut == outcode0)
                {
                    val1.X = x;
                    val1.Y = y;
                    val1.Z = z;
                    outcode0 = InitFlags(val1, min, max);
                }
                else
                {
                    val2.X = x;
                    val2.Y = y;
                    val2.Z = z;
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

    public static class PolygonDrawer
    {
        public static void DrawPolygon(this ZBuffer img, Color color, params Point[] points)
        {
            
        }

        private static List<Point> 
    }
    class Polygon
    {
        private readonly List<PolEdge> _object;

        public Polygon(Face face, ObjectBase obj, Camera c)
        {
            _object = new List<PolEdge>();

            foreach (var edge in face.GetEdgesClockWise())
            {
                _object.Add(edge.Left == face ? new PolEdge(c.TransformPoint(obj.TransformVertex(edge.End)), c.TransformPoint(obj.TransformVertex(edge.Init))) : new PolEdge(c.TransformPoint(obj.TransformVertex(edge.Init)), c.TransformPoint(obj.TransformVertex(edge.End))));
            }
        }

        public void Draw(ZBuffer img)
        {
            var clipped = _object.Select(p => p.Init).ToList();

            for (var y = ZBuffer.WindowMinHeight; y < ZBuffer.WindowMaxHeight; y++)
            {
                var nodeX = new List<int>();
                var points = 0;
                int index;
                var j = clipped.Count - 1;

                for (index = 0; index < clipped.Count; index++)
                {
                    if (clipped[index].Y < y && clipped[j].Y >= y || clipped[j].Y < y && clipped[index].Y >= y)
                    {
                        nodeX.Insert(points++, (int)(clipped[index].X + (y - clipped[index].Y) / (clipped[j].Y - clipped[index].Y) * (clipped[j].X - clipped[index].X)));
                    }
                    j = index;
                }

                nodeX = nodeX.OrderBy(p => p).ToList();

                for (index = 0; index < points; index += 2)
                {
                    if (nodeX[index] >= ZBuffer.WindowMaxWidth)
                        break;
                    if (nodeX[index + 1] <= ZBuffer.WindowMinWidth)
                        continue;

                    if (nodeX[index] < ZBuffer.WindowMinWidth)
                        nodeX[index] = ZBuffer.WindowMinWidth;
                    if (nodeX[index + 1] >= ZBuffer.WindowMaxWidth)
                        nodeX[index + 1] = ZBuffer.WindowMaxWidth - 1;

                    for (var x = nodeX[index]; x < nodeX[index + 1]; x++)
                        img.SetPixel(x, y, 0, Color.Blue);
                }
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
                return DoesHit(this, pol);
            }

            private static Point DoesHit(PolEdge a, PolEdge b)
            {
                var adx = a.Final.X - a.Init.X;
                var ady = a.Final.Y - a.Init.Y;
                var bdx = b.Final.X - b.Init.X;
                var bdy = b.Final.Y - b.Init.Y;
                var det = bdx * ady - bdy * adx;

                if (Math.Abs(det) < .00001)
                {
                    return null;
                }

                var s = (bdx * (b.Init.Y - a.Init.Y) - bdy * (b.Init.X - a.Init.X)) / det;

                if (s < 0 || s > 1)
                    return null;

                return new Point(a.Init.X + s * (a.Final.X - a.Init.X), a.Init.Y + s * (a.Final.Y - a.Init.Y), a.Init.Z + s * (a.Final.Z - a.Init.Z));
            }
        }
    }

    class WiredPolygon
    {
        private readonly List<PolEdge> _object;

        public WiredPolygon(Face face, ObjectBase obj, Camera c)
        {
            _object = new List<PolEdge>();

            foreach (var edge in face.GetEdgesClockWise())
            {
                _object.Add(edge.Left == face ? new PolEdge(c.TransformPoint(obj.TransformVertex(edge.End)), c.TransformPoint(obj.TransformVertex(edge.Init))) : new PolEdge(c.TransformPoint(obj.TransformVertex(edge.Init)), c.TransformPoint(obj.TransformVertex(edge.End))));
            }
        }

        public void Draw(ZBuffer img)
        {
            var backGround = Settings.Default.CameraDefaultBackground;
            var clipped = _object.Select(p => p.Init).ToList();

            for (var y = ZBuffer.WindowMinHeight; y < ZBuffer.WindowMaxHeight; y++)
            {
                var nodeX = new List<int>();
                var points = 0;
                int index;
                var j = clipped.Count - 1;

                for (index = 0; index < clipped.Count; index++)
                {
                    if (clipped[index].Y < y && clipped[j].Y >= y || clipped[j].Y < y && clipped[index].Y >= y)
                    {
                        nodeX.Insert(points++, (int)(clipped[index].X + (y - clipped[index].Y) / (clipped[j].Y - clipped[index].Y) * (clipped[j].X - clipped[index].X)));
                    }
                    j = index;
                }

                nodeX = nodeX.OrderBy(p => p).ToList();

                for (index = 0; index < points; index += 2)
                {
                    if (nodeX[index] >= ZBuffer.WindowMaxWidth)
                        break;
                    if (nodeX[index + 1] <= ZBuffer.WindowMinWidth)
                        continue;

                    if (nodeX[index] < ZBuffer.WindowMinWidth)
                        nodeX[index] = ZBuffer.WindowMinWidth;
                    if (nodeX[index + 1] >= ZBuffer.WindowMaxWidth)
                        nodeX[index + 1] = ZBuffer.WindowMaxWidth - 1;

                    for (var x = nodeX[index]; x < nodeX[index + 1]; x++)
                        img.SetPixel(x, y, 0, backGround);
                }
            }

            foreach (var polEdge in _object.Select(p => new Line(p.Init, p.Final)))
            {
                polEdge.Draw(img);
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
                return DoesHit(this, pol);
            }

            private static Point DoesHit(PolEdge a, PolEdge b)
            {
                var adx = a.Final.X - a.Init.X;
                var ady = a.Final.Y - a.Init.Y;
                var bdx = b.Final.X - b.Init.X;
                var bdy = b.Final.Y - b.Init.Y;
                var det = bdx * ady - bdy * adx;

                if (Math.Abs(det) < .00001)
                {
                    return null;
                }

                var s = (bdx * (b.Init.Y - a.Init.Y) - bdy * (b.Init.X - a.Init.X)) / det;

                if (s < 0 || s > 1)
                    return null;

                return new Point(a.Init.X + s * (a.Final.X - a.Init.X), a.Init.Y + s * (a.Final.Y - a.Init.Y), a.Init.Z + s * (a.Final.Z - a.Init.Z));
            }
        }
    }

    [Flags]
    public enum CohenFlags : byte
    {
        Left = 0x01,
        Right = 0x02,
        Top = 0x10,
        Bottom = 0x20,
        None = 0
    }
}
