using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CG_Final.Properties;

namespace CG_Final
{
    public class ZBuffer
    {
        public const int Width = 490;
        public const int Height = 330;

        public const int WindowMinWidth = 0;
        public const int WindowMinHeight = 0;
        public const int WindowMaxWidth = Width;
        public const int WindowMaxHeight = Height;

        public static readonly Point MinSize = new Point(WindowMinWidth, WindowMinHeight);
        public static readonly Point MaxSize = new Point(WindowMaxWidth - 1, WindowMaxHeight - 1);

        public static Color BackGroundColor;

        public readonly Bitmap Bitmap;
        public readonly double[,] Depth;
        //private readonly byte[] bytes;

        private int _xmin;
        private int _ymin;
        private int _xmax;
        private int _ymax;

        public ZBuffer()
        {
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Depth = new double[Height, Width];
            BackGroundColor = Settings.Default.CameraDefaultBackground;
            _xmin = 0;
            _ymin = 0;
            _xmax = Width - 1;
            _ymax = Height - 1;
            //bytes = new byte[Width * Height*4];

            //Parallel.ForEach(Enumerable.Range(0, Width * Height).Select(i => i * 4), i =>
            //{
            //    bytes[i] = BackGroundColor.A;
            //    bytes[i + 2] = BackGroundColor.R;
            //    bytes[i + 3] = BackGroundColor.G;
            //    bytes[i + 1] = BackGroundColor.B;
            //});
            Clear();
        }

        public void Clear()
        {
            if (_xmin > _xmax || _ymin > _ymax)
                return;

            var init = DateTime.Now;
            using (var g = Graphics.FromImage(Bitmap))
            {
                g.FillRectangle(new SolidBrush(BackGroundColor), _xmin, _ymin, _xmax - _xmin + 1, _ymax - _ymin + 1);
            }

            //var rectangle = new Rectangle(0, 0, Width, Height);
            //var bitmapData = Bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            //var pt = bitmapData.Scan0;
            //Marshal.Copy(bytes, 0, pt, bytes.Length);
            //Bitmap.UnlockBits(bitmapData);

            Console.WriteLine((DateTime.Now - init).TotalMilliseconds);
            Parallel.For(_ymin, _ymax + 1, i =>
            {
                for (var j = _xmin; j < _xmax + 1; j++)
                {
                    Depth[i, j] = double.MinValue;
                }
            });

            _xmin = Width;
            _xmax = 0;
            _ymax = 0;
            _ymin = Height;
        }

        public void SetPixel(int x, int y, double depth, Color color)
        {
            if (!(depth > Depth[y, x])) return;

            if (x < _xmin)
                _xmin = x;

            if (x > _xmax)
                _xmax = x;

            if (y < _ymin)
                _ymin = y;

            if (y > _ymax)
                _ymax = y;

            Bitmap.SetPixel(x, y, color);
            Depth[y, x] = depth;
        }

        public static bool Contains(Point p)
        {
            return p.X >= WindowMinWidth && p.X < WindowMaxWidth && p.Y >= WindowMinHeight && p.Y < WindowMaxHeight;
        }
    }
}