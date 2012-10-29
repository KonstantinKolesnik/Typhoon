using System;
using MF.Engine.Managers;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class ProgressBar : Control
    {
        #region Fields
        private int _min = 0;
        private int _max = 100;
        private int _val = 0;
        #endregion

        #region Constructors
        public ProgressBar(int x, int y, int width, int height)
        {
            x = x;
            y = y;
            w = width;
            h = height;
        }
        public ProgressBar(int x, int y, int width, int height, int minvalue, int maxvalue, int value)
            : this(x, y, width, height)
        {
            _min = minvalue;
            _max = maxvalue;
            _val = value;
        }
        #endregion

        #region  Properties
        public int Minimum
        {
            get { return _min; }
            set
            {
                if (value >= _max) value = _max - 1;
                _min = value;
                Render(true);
            }
        }
        public int Maximum
        {
            get { return _max; }
            set
            {
                if (value <= _min) value = _min + 1;
                _max = value;
                Render(true);
            }
        }
        public int Value
        {
            get { return _val; }
            set
            {
                if (value < _min) value = _min;
                if (value > _max) value = _max;
                _val = value;
                Render(true);
            }
        }
        public override int Height
        {
            get { return h; }
            set
            {
                if (value < 30) value = 30;
                h = value;
                if (parent != null) parent.Render();
            }
        }
        #endregion

        #region GUI
        public override void Render(bool flush)
        {
            if (parent == null || parent.ScreenBuffer == null || !visible || suspend) return;

            // Set clipping region in case we're off parent somewhere (can happen w/ scroll)
            parent.ScreenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

            Color Border = ColorUtility.ColorFromRGB(161, 161, 161);
            Color Bkg = ColorUtility.ColorFromRGB(227, 227, 227);
            Color BkgDark = ColorUtility.ColorFromRGB(210, 210, 210);
            Color BkgLight = ColorUtility.ColorFromRGB(244, 244, 244);
            Color Fill = ColorUtility.ColorFromRGB(149, 193, 252);
            Color ValLight = ColorUtility.ColorFromRGB(228, 236, 241);
            Color ValDark = ColorUtility.ColorFromRGB(114, 159, 219);

            // Draw Border & Background
            parent.ScreenBuffer.DrawRectangle(Border, 1, Left, Top, w, h, 0, 0, Bkg, 0, 0, Bkg, 0, 0, 256);
            parent.ScreenBuffer.DrawLine(BkgLight, 1, Left + 1, Top + h - 2, Left + w - 2, Top + h - 2);
            parent.ScreenBuffer.DrawLine(BkgDark, 1, Left + 1, Top + 1, Left + w - 2, Top + 1);

            // Figure out progress fill
            if (_val == _min) return;
            int rng = _max - _min;
            int val = _val - _min;
            float prc = ((float)val / (float)rng);
            int wid = (int)((float)(w - 2) * prc);
            if (wid < 1) return;

            // Draw progress fill
            parent.ScreenBuffer.DrawRectangle(Fill, 0, Left + 1, Top + 1, wid, h - 2, 0, 0, Fill, 0, 0, Fill, 0, 0, 256);
            if (wid > 2) parent.ScreenBuffer.DrawLine(ValLight, 1, Left + 2, Top + h - 2, Left + wid - 1, Top + h - 2);
            if (wid > 2) parent.ScreenBuffer.DrawLine(ValDark, 1, Left + 2, Top + 1, Left + wid - 1, Top + 1);
            parent.ScreenBuffer.DrawLine(ValDark, 1, Left + wid, Top + 1, Left + wid, Top + h - 2);
            parent.ScreenBuffer.DrawLine(ValLight, 1, Left + 1, Top + 1, Left + 1, Top + h - 2);

            if (flush)
            {
                parent.ScreenBuffer.Flush(Left, Top, w, h);
                parent.ScreenBuffer.SetClippingRectangle(0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
            }
        }
        #endregion
    }
}
