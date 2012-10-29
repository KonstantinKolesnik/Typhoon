using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class Panel : Container
    {
        #region Fields
        private Color backColor = Colors.LightGray;
        private Bitmap backImage = null;
        private ushort opacity = Bitmap.OpacityOpaque;
        #endregion

        #region  Properties
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }
        public Bitmap BackgroundImage
        {
            get { return backImage; }
            set { backImage = value; }
        }
        public ushort Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }
        #endregion

        #region Constructors
        public Panel(int x, int y, int width, int height)
        {
            area = new Rect(x, y, width, height);
        }
        public Panel(int x, int y, int width, int height, Color backColor)
            : this(x, y, width, height)
        {
            BackColor = backColor;
        }
        public Panel(int x, int y, int width, int height, Bitmap backgroundImage)
            : this(x, y, width, height)
        {
            BackgroundImage = backgroundImage;
        }
        #endregion

        #region Touch Invokes
        public override void TouchDown(object sender, Point e)
        {
            if (!visible)
                return;

            // Check controls
            Control child = null;
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                child = GetChildAt(i);
                if (child.ScreenArea.Contains(e))
                {
                    child.TouchDown(this, e);
                    return;
                }
            }

            isPenDown = true;
        }
        public override void TouchUp(object sender, Point e)
        {
            if (!visible)
                return;

            bool ignoreUp = false;
            bool ret = false;

            // Check controls
            try
            {
                Control child = null;
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    child = (Control)Children[i];
                    if (child.ScreenArea.Contains(e) && !ignoreUp)
                    {
                        child.TouchUp(this, e);
                        ret = true;
                        ignoreUp = true;
                    }
                    else if (child.IsPenDown)
                    {
                        child.TouchUp(this, e);
                    }
                }
            }
            catch (Exception)
            {
                // just move on
            }

            if (!ret && isPenDown)
                OnTap(this, new Point(e.X, e.Y));
        }
        public override void TouchMove(object sender, Point e)
        {
            try
            {
                Control child = null;
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    child = GetChildAt(i);
                    child.TouchMove(sender, e);
                }
            }
            catch (Exception)
            {
                // just move on
            }
        }
        #endregion

        #region GUI
        public override void Render(ref Bitmap screenBuffer)
        {
            if (parent == null || !visible)
                return;

            screenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

            if (backImage != null)
                screenBuffer.Scale9Image(Left, Top, Width, Height, backImage, 0, 0, 0, 0, opacity);
            else
                screenBuffer.DrawRectangle(backColor, 1, Left, Top, Width, Height, 0, 0, backColor, Left, Top, backColor, Left + Width, Top + Height, opacity);

            for (int i = 0; i < Children.Count; i++)
                GetChildAt(i).Render(ref screenBuffer);
        }
        #endregion
    }
}
