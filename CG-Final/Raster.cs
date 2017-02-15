using System;
using System.Collections.Generic;
using System.Drawing;
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
        // NOT the president
        public Color Color { get; set; }
        public Point InitPoint { get; set; }
        public Point EndPoint { get; set; }

        public Line(Point init, Point end)
        {
            InitPoint = init;
            EndPoint = end;
            Color = Settings.Default.LineDefaultColor;
        }

        /*
         * 
         */

        public void Draw(ZBuffer img)
        {
            Clip(img);

            MidpointLine((int)InitPoint.X, (int)InitPoint.Y, (int)EndPoint.X, (int)EndPoint.Y, img);
        }

        private void MidpointLine(int x, int y, int x2, int y2, ZBuffer img)
        {
            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;

            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);

            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1;
                else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
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

        public void Clip(ZBuffer img)
        {
            Clip(ref InitPoint.X, 0, ZBuffer.Width);
            Clip(ref InitPoint.Y, 0, ZBuffer.Height);
        }

        private static void Clip(ref double val, double min, double max)
        {
            if (val < min)
                val = min;
            if (val >= max)
                val = max;
        }
    }

    class Polygon
    {
        ObjectBase _obj;
        Dictionary<Point, Point> _points;
    }
}
