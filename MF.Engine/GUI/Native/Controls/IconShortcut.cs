using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using MF.Engine.Utilities;

namespace MF.Engine.GUI.Controls
{
    public class IconShortcut : Control
    {
        #region Variables
        private Image32 icon;
        private Font font;
        private string title;
        #endregion

        #region Properties
        public Image32 Icon
        {
            get { return icon; }
            set
            {
                //if (value.Width > 32 || value.Height > 32)
                //    throw new Exception("Image too large");

                icon = value;
                if (parent != null)
                    parent.Render(true);
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                if (parent != null)
                    parent.Render(true);
            }
        }
        public Font Font
        {
            get { return font; }
            set
            {
                font = value;
                if (parent != null)
                    parent.Render();
            }
        }
        #endregion

        #region Constructors
        public IconShortcut(Image32 image, string title, Font font, int x, int y, int width, int height)
        {
            icon = image;
            this.title = title;
            this.font = font;
            x = x;
            y = y;
            w = width;
            h = height;
        }
        #endregion

        #region GUI
        public override void Render(bool flush)
        {
            if (parent == null || parent.ScreenBuffer == null || !visible || suspend)
                return;

            if (icon != null)
                icon.Draw(parent.ScreenBuffer, Left + ((w / 2) - (icon.Width / 2)), Top);

            if (font != null && !Utils.IsStringEmpty(title))
            {
                int w, h;
                int offset = 0;

                font.ComputeTextInRect(title, out w, out h, w - 4);
                w += 4;
                h += 4;

                if (h > h - 35)
                    h = h - 35;

                offset = (w / 2) - (w / 2);

                //_parent.ScreenBuffer.DrawRectangle(Color.Black, 0, Left + offset, Top + 36, w, h, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 150);
                parent.ScreenBuffer.DrawTextInRect(title, Left + 2 + offset, Top + 38, w - 4, h - 4, Bitmap.DT_WordWrap + Bitmap.DT_AlignmentCenter, Color.White, font);
            }
        }
        #endregion
    }
}
