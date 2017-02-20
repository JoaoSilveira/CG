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
            //foreach (var face in obj.Faces)
            //{
            //    if (camera.N.DotProduct(face.NormalVector()) <= 0)
            //        continue;


            //}
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

        public static CohenFlags InitFlags(Point p, Point min, Point max)
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
        public static void DrawWiredPolygon(this ZBuffer img, Color color, List<Point> points)
        {
            if (points.Count < 3)
                return;

            var a = points[2] - points[1];
            var b = points[0] - points[1];
            var normal = b.VectorialProduct(a);
            normal.Normalize();

            if (normal.Z > 0)
                return;

            var p = points.Last();

            foreach (var point in points)
            {
                img.DrawLine(p, point, color);
                p = point;
            }
        }
        
        public static void DrawPolygon(this ZBuffer img, Color color, params Point[] points)
        {
            if (points.Length < 3)
                return;

            var a = points[2] - points[1];
            var b = points[0] - points[1];
            var normal = b.VectorialProduct(a);
            normal.Normalize();

            if (normal.Z <= 0)
                return;

            var poly = Clip(points, ZBuffer.MinSize, ZBuffer.MaxSize);

            var last = poly.Last();

            var ymax = 0;
            var ymin = ZBuffer.Height;
            var xmax = 0;
            var xmin = ZBuffer.Width;

            foreach (var point in poly)
            {
                if (point.X < xmin)
                    xmin = (int)point.X;
                if (point.X > xmax)
                    xmax = (int)point.X;
                if (point.Y < ymin)
                    ymin = (int)point.Y;
                if (point.Y > ymax)
                    ymax = (int)point.Y;
            }

            Draw(img, color, xmin, xmax, ymin, ymax, poly);
        }

        private static void Draw(ZBuffer img, Color color, int xmin, int xmax, int ymin, int ymax, List<Point> clipped)
        {
            for (var y = ymin; y < ymax; y++)
            {
                var inter = new List<Point>();
                int index;
                var j = clipped.Count - 1;

                for (index = 0; index < clipped.Count; index++)
                {
                    if (clipped[index].Y < y && clipped[j].Y >= y || clipped[j].Y < y && clipped[index].Y >= y)
                    {
                        var pt = DoesHit(clipped[j], clipped[index], new Point(0, y), new Point(ZBuffer.Width - 1, y));

                        if (pt != null)
                            inter.Add(pt);
                    }
                    j = index;
                }

                inter = inter.OrderBy(p => p.X).ToList();

                for (index = 0; index < inter.Count; index += 2)
                {
                    var z = inter[index].Z;
                    var tx = (inter[index + 1].Z - inter[index].Z) / (inter[index + 1].X - inter[index].X);
                    var end = (int) inter[index + 1].X;
                    for (var x = (int) inter[index].X; x < end; x++)
                    {
                        img.SetPixel(x, y, z, color);
                        z += tx;
                    }
                }
            }
        }

        private static List<Point> Clip(Point[] points, Point min, Point max)
        {
            var flag = false;

            var list = points.Select(p =>
            {
                var pointFlag = LineDrawer.InitFlags(p, ZBuffer.MinSize, ZBuffer.MaxSize);

                if (pointFlag != CohenFlags.None)
                    flag = true;

                return new CohenPoint(pointFlag, p.X, p.Y, p.Z);
            }).ToList();

            if (!flag)
                return points.ToList();

            var clippedList = new List<CohenPoint>();

            while (flag)
            {
                var p0 = list.Last();
                flag = false;

                foreach (var point in list)
                {
                    redo:
                    // aceito trivialmente
                    if ((p0.Flags | point.Flags) == CohenFlags.None)
                    {
                        clippedList.Add(point);
                        p0 = point;
                        continue;
                    }

                    // rejeitado trivialmente
                    if ((p0.Flags & point.Flags) != CohenFlags.None)
                    {
                        p0 = point;
                        continue;
                    }

                    var x = 0.0;
                    var y = 0.0;
                    var z = 0.0;

                    var outcodeOut = p0.Flags != 0 ? p0.Flags : point.Flags;

                    if ((outcodeOut & CohenFlags.Top) == CohenFlags.Top)
                    {
                        x = p0.X + (point.X - p0.X) * (min.Y - p0.Y) / (point.Y - p0.Y);
                        z = p0.Z + (point.Z - p0.Z) * (min.Y - p0.Y) / (point.Y - p0.Y);
                        y = min.Y;
                    }
                    else if ((outcodeOut & CohenFlags.Bottom) == CohenFlags.Bottom)
                    {
                        x = p0.X + (point.X - p0.X) * (max.Y - p0.Y) / (point.Y - p0.Y);
                        z = p0.Z + (point.Z - p0.Z) * (max.Y - p0.Y) / (point.Y - p0.Y);
                        y = max.Y;
                    }
                    else if ((outcodeOut & CohenFlags.Right) == CohenFlags.Right)
                    {
                        y = p0.Y + (point.Y - p0.Y) * (max.X - p0.X) / (point.X - p0.X);
                        z = p0.Z + (point.Z - p0.Z) * (max.X - p0.X) / (point.X - p0.X);
                        x = max.X;
                    }
                    else if ((outcodeOut & CohenFlags.Left) == CohenFlags.Left)
                    {
                        y = p0.Y + (point.Y - p0.Y) * (min.X - p0.X) / (point.X - p0.X);
                        z = p0.Z + (point.Z - p0.Z) * (min.X - p0.X) / (point.X - p0.X);
                        x = min.X;
                    }

                    var cohenFlags = p0.Flags;
                    p0.X = x;
                    p0.Y = y;
                    p0.Z = z;
                    p0.Flags = LineDrawer.InitFlags(new Point(p0.X, p0.Y), min, max);


                    if (p0.Flags != CohenFlags.None)
                        flag = true;

                    clippedList.Add(p0);

                    if (cohenFlags != CohenFlags.None)
                        goto redo;

                    p0 = point;
                }

                var aux = list;
                list = clippedList;
                clippedList = aux;
                clippedList.Clear();
            }

            return list.Select(p => new Point(p.X, p.Y, p.Z)).ToList();
        }

        private static Point DoesHit(Point aInit, Point aEnd, Point bInit, Point bEnd)
        {
            var adx = aEnd.X - aInit.X;
            var ady = aEnd.Y - aInit.Y;
            var bdx = bEnd.X - bInit.X;
            var bdy = bEnd.Y - bInit.Y;
            var det = bdx * ady - bdy * adx;

            if (Math.Abs(det) < .00001)
            {
                return null;
            }

            var s = (bdx * (bInit.Y - aInit.Y) - bdy * (bInit.X - aInit.X)) / det;

            if (s < 0 || s > 1)
                return null;

            return new Point(aInit.X + s * (aEnd.X - aInit.X), aInit.Y + s * (aEnd.Y - aInit.Y), aInit.Z + s * (aEnd.Z - aInit.Z));
        }

        private struct CohenPoint
        {
            public CohenFlags Flags;
            public double X;
            public double Y;
            public double Z;

            public CohenPoint(CohenFlags flags, double x, double y, double z)
            {
                Flags = flags;
                X = x;
                Y = y;
                Z = z;
            }
        }
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

            //foreach (var polEdge in _object.Select(p => new Line(p.Init, p.Final)))
            //{
            //    polEdge.Draw(img);
            //}
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
        None = 0,
        TopBottom = 0x30,
        LeftRight = 0x03,
        All = 0x33
    }
}
