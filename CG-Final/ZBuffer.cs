using System;
using System.Drawing;
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

        public ZBuffer()
        {
            Bitmap = new Bitmap(Width, Height);
            Depth = new double[Height, Width];
            BackGroundColor = Settings.Default.CameraDefaultBackground;
            Clear();
        }

        public void Clear()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    Bitmap.SetPixel(j, i, BackGroundColor);
                    Depth[i, j] = double.MinValue;
                }
            }
        }

        public void SetPixel(int x, int y, double depth, Color color)
        {
            if (!(depth > Depth[y, x])) return;

            Bitmap.SetPixel(x, y, color);
            Depth[y, x] = depth;
        }

        public static bool Contains(Point p)
        {
            return p.X >= WindowMinWidth && p.X < WindowMaxWidth && p.Y >= WindowMinHeight && p.Y < WindowMaxHeight;
        }
    }
}