using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI
{
    public struct ColorEX
    {
        public int X;
        public int Y;
        public ushort Alpha;
        public Color Color;

        public ColorEX(int x, int y, ushort alpha, Color color)
        {
            X = x;
            Y = y;
            Alpha = (ushort)(alpha > 0 ? alpha + 1 : 0);
            Color = color;
        }
    }
}
