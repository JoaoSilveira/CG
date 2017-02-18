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
        private readonly byte[] bytes;

        private int xmin;
        private int ymin;
        private int xmax;
        private int ymax;

        public ZBuffer()
        {
            Bitmap = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            Depth = new double[Height, Width];
            BackGroundColor = Settings.Default.CameraDefaultBackground;
            xmin = 0;
            ymin = 0;
            xmax = Width - 1;
            ymax = Height - 1;
            bytes = new byte[Width * Height*4];

            Parallel.ForEach(Enumerable.Range(0, Width * Height).Select(i => i * 4), i =>
            {
                bytes[i] = BackGroundColor.A;
                bytes[i + 1] = BackGroundColor.R;
                bytes[i + 2] = BackGroundColor.G;
                bytes[i + 3] = BackGroundColor.B;
            });
            Clear();
        }

        public void Clear()
        {
            if (xmin > xmax || ymin > ymax)
                return;

            var rectangle = new Rectangle(0, 0, Width, Height);
            var bitmapData = Bitmap.LockBits(rectangle, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            var pt = bitmapData.Scan0;
            Marshal.Copy(bytes, 0, pt, bytes.Length);
            Bitmap.UnlockBits(bitmapData);

            Parallel.For(ymin, ymax + 1, i =>
            {
                for (var j = xmin; j < xmax + 1; j++)
                {
                    Depth[i, j] = double.MinValue;
                }
            });

            xmin = Width;
            xmax = 0;
            ymax = 0;
            ymin = Height;
        }

        public void SetPixel(int x, int y, double depth, Color color)
        {
            if (!(depth > Depth[y, x])) return;

            if (x < xmin)
                xmin = x;

            if (x > xmax)
                xmax = x;

            if (y < ymin)
                ymin = y;

            if (y > ymax)
                ymax = y;

            Bitmap.SetPixel(x, y, color);
            Depth[y, x] = depth;
        }

        public static bool Contains(Point p)
        {
            return p.X >= WindowMinWidth && p.X < WindowMaxWidth && p.Y >= WindowMinHeight && p.Y < WindowMaxHeight;
        }
    }
}