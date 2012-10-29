using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class Checkbox : Control
    {
        #region Fields
        private bool isChecked = false;
        #endregion

        #region  Properties
        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }
        #endregion

        #region Constructors
        public Checkbox(int x, int y)
        {
            area = new Rect(x, y, 18, 18);
        }
        public Checkbox(int x, int y, bool selected)
            : this(x, y)
        {
            isChecked = selected;
        }
        #endregion

        #region Touch Invokes
        public override void TouchUp(object sender, Point e)
        {
            if (isPenDown)
            {
                if (ScreenArea.Contains(e))
                {
                    if (enabled)
                        isChecked = !isChecked;

                    OnTap(this, new Point(e.X - Left, e.Y - Top));
                }
                isPenDown = false;
            }
        }
        #endregion

        #region GUI
        public override void Render(ref Bitmap screenBuffer)
        {
            if (parent == null || !visible || !parent.ScreenArea.Intersects(ScreenArea))
                return;

            screenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

            screenBuffer.DrawRectangle(
                enabled ? Colors.Black : Colors.DarkGray, 1,
                Left, Top, 16, 16,
                0, 0,
                enabled ? Colors.White : Colors.Gray, Left, Top,
                enabled ? Colors.DarkGray : Colors.Gray, Left, Top + 14,
                Bitmap.OpacityOpaque);

            if (isChecked)
            {
                Color chk = (enabled) ? Colors.Black : Colors.LightGray;

                screenBuffer.DrawLine(chk, 1, Left + 3, Top + 3, Left + Width - 6, Top + Height - 6);
                screenBuffer.DrawLine(chk, 1, Left + 3, Top + Height - 6, Left + Width - 6, Top + 3);

                screenBuffer.DrawLine(chk, 1, Left + 4, Top + 3, Left + Width - 6, Top + Height - 7);
                screenBuffer.DrawLine(chk, 1, Left + 3, Top + Height - 7, Left + Width - 7, Top + 3);

                screenBuffer.DrawLine(chk, 1, Left + 3, Top + 4, Left + Width - 7, Top + Height - 6);
                screenBuffer.DrawLine(chk, 1, Left + 4, Top + Height - 6, Left + Width - 6, Top + 4);
            }
        }
        #endregion
    }
}
