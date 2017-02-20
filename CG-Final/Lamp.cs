using System;
using System.Drawing;

namespace CG_Final
{
    [Serializable]
    public class Lamp
    {
        public Point Location { get; set; }

        public Color Intensity { get; set; }
    }
}