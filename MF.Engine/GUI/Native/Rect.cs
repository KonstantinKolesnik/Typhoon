using System;

namespace MF.Engine.GUI
{
    [Serializable]
    public struct Rect
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(int x, int y)
        {
            return (x >= X && x < X + Width && y >= Y && y < Y + Height);
        }
        public bool Contains(Point e)
        {
            return Contains(e.X, e.Y);
        }
        public bool Intersects(Rect area)
        {
            return !(area.X >= (X + Width)
                    || (area.X + area.Width) <= X
                    || area.Y >= (Y + Height)
                    || (area.Y + area.Height) <= Y
                    );
        }
        public void Combine(Rect newRect)
        {
            if (this.Width == 0)
            {
                this.X = newRect.X;
                this.Y = newRect.Y;
                this.Width = newRect.Width;
                this.Height = newRect.Height;
                return;
            }

            int x1, y1, x2, y2;
            x1 = (this.X < newRect.X) ? this.X : newRect.X;
            y1 = (this.Y < newRect.Y) ? this.Y : newRect.Y;
            x2 = (this.X + this.Width > newRect.X + newRect.Width) ? this.X + this.Width : newRect.X + newRect.Width;
            y2 = (this.Y + this.Height > newRect.Y + newRect.Height) ? this.Y + this.Height : newRect.Y + newRect.Height;
            this.X = x1;
            this.Y = y1;
            this.Width = x2 - x1;
            this.Height = y2 - y1;
        }
        public Rect Combine(Rect region1, Rect region2)
        {
            if (region1.Width == 0) return region2;
            if (region2.Width == 0) return region1;

            int x1, y1, x2, y2;
            x1 = (region1.X < region2.X) ? region1.X : region2.X;
            y1 = (region1.Y < region2.Y) ? region1.Y : region2.Y;
            x2 = (region1.X + region1.Width > region2.X + region2.Width) ? region1.X + region1.Width : region2.X + region2.Width;
            y2 = (region1.Y + region1.Height > region2.Y + region2.Height) ? region1.Y + region1.Height : region2.Y + region2.Height;
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }
        public static Rect Intersect(Rect region1, Rect region2)
        {
            if (!region1.Intersects(region2))
                return new Rect(0, 0, 0, 0);

            Rect rct = new Rect();

            // For X1 & Y1 we'll want the highest value
            rct.X = (region1.X > region2.X) ? region1.X : region2.X;
            rct.Y = (region1.Y > region2.Y) ? region1.Y : region2.Y;

            // For X2 & Y2 we'll want the lowest value
            int r1V2 = region1.X + region1.Width;
            int r2V2 = region2.X + region2.Width;
            rct.Width = (r1V2 < r2V2) ? r1V2 - rct.X : r2V2 - rct.X;
            r1V2 = region1.Y + region1.Height;
            r2V2 = region2.Y + region2.Height;
            rct.Height = (r1V2 < r2V2) ? r1V2 - rct.Y : r2V2 - rct.Y;

            return rct;
        }
    }
}
