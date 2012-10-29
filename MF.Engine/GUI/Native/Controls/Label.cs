using System;
using MF.Engine.Managers;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class Label : Control
    {
        #region Fields
        private Font font;
        private string text = "";
        private bool autosize = true;
        private Color foreColor = Color.Black;
        #endregion

        #region  Properties
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public bool AutoSize
        {
            get { return autosize; }
            set { autosize = value; }
        }
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }
        #endregion

        #region Constructors
        public Label(string text, Font font, int x, int y)
        {
            area = new Rect(x, y, 0, 0);
            Text = text;
            Font = font;
            autosize = true;
        }
        public Label(string text, Font font, int x, int y, int width, int height)
            : this(text, font, x, y)
        {
            Width = width;
            Height = height;
            autosize = false;
        }
        public Label(string text, Font font, Color foreColor, int x, int y, int width, int height)
            : this(text, font, x, y, width, height)
        {
            ForeColor = foreColor;
        }
        public Label(string text, Font font, Color ForeColor, int x, int y, int width, int height, bool visible)
            : this(text, font, ForeColor, x, y, width, height)
        {
            Visible = visible;
        }
        #endregion

        #region GUI
        public override void Render(ref Bitmap screenBuffer)
        {
            if (parent == null || !visible)
                return;

            screenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

            // Auto-Size first
            int w = Width;
            int h = Height;
            if (autosize)
                font.ComputeExtent(text, out w, out h);

            // Draw String
            //Bitmap bmp = new Bitmap(w, h);
            ////bmp.MakeTransparent(bmp.GetPixel(0, 0));
            //bmp.DrawTextInRect(text, 0, 0, w, h, Bitmap.DT_AlignmentLeft + Bitmap.DT_WordWrap, forecolor, font);
            //screenBuffer.DrawImage(Left, Top, bmp, 0, 0, w, h);

            //screenBuffer.DrawRectangle(
            //    Colors.White, 0,
            //    Left, Top, w, h,
            //    0, 0,
            //    Colors.White, Left, Top,
            //    Colors.Black, Left + w, Top + h,
            //    Bitmap.OpacityTransparent);
            screenBuffer.DrawTextInRect(text, Left, Top, w, h, Bitmap.DT_AlignmentLeft + Bitmap.DT_WordWrap, foreColor, font);
        }
        #endregion
    }
}
