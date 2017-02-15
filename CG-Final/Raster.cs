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

        public void Draw(ZBuffer img)
        {
            var a = (int)EndPoint.Y - (int)InitPoint.Y;
            var b = (int)EndPoint.X - (int)InitPoint.X;
            var v = 2 * a + b; //valor inicial de V
            var incrE = 2 * a; //Mover para E
            var incrNE = 2 * (a + b); //Mover para NE
            var x = (int)InitPoint.X;
            var y = (int)InitPoint.Y;

            // mudar a profundidade
            img.SetPixel(x, y, 0, Color);

            while (x < EndPoint.X)
            {
                if (v <= 0) v += incrE; //escolhe E
                else
                { //escolhe NE
                    v += incrNE;
                    ++y;
                }
                ++x;

                // mudar a profundidade
                img.SetPixel(x, y, 0, Color); //Plota o ponto final
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
