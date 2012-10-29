using System;
using MF.Engine.Managers;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class PictureBox : Control
    {
        #region Fields
        private ScaleMode _scale = ScaleMode.Normal;
        private bool _autosize = true;
        private BorderStyle _border = BorderStyle.BorderNone;
        private Color _bkg = Colors.DarkGray;
        private Bitmap _bmp = null;
        #endregion

        #region Constructors
        public PictureBox(Bitmap image, int x, int y)
        {
            _bmp = image;
            if (_bmp != null)
                _bmp.MakeTransparent(_bmp.GetPixel(0, 0));
            x = x;
            y = y;
        }
        public PictureBox(Bitmap image, int x, int y, int width, int height)
            : this(image, x, y)
        {
            w = width;
            h = height;
            _autosize = false;
        }
        public PictureBox(Bitmap image, int x, int y, int width, int height, BorderStyle border, ScaleMode scale)
            : this(image, x, y, width, height)
        {
            _border = border;
            _scale = scale;
        }
        #endregion

        #region  Properties
        public bool AutoSize
        {
            get { return _autosize; }
            set { _autosize = value; Render(true); }
        }
        public Color Background
        {
            get { return _bkg; }
            set { _bkg = value; Render(true); }
        }
        public ScaleMode ScaleMode
        {
            get { return _scale; }
            set { _scale = value; Render(true); }
        }
        public BorderStyle BorderStyle
        {
            get { return _border; }
            set { _border = value; Render(true); }
        }
        public Bitmap Image
        {
            get { return _bmp; }
            set
            {
                _bmp = value;
                if (_bmp != null)
                    _bmp.MakeTransparent(_bmp.GetPixel(0, 0));

                Render(true);
            }
        }
        #endregion

        #region GUI
        public override void Render(bool flush)
        {
            if (parent == null || parent.ScreenBuffer == null || !visible || suspend)
                return;

            int dsW, dsH, dX, dY;

            // Set clipping region in case we're off parent somewhere (can happen w/ scroll)
            parent.ScreenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

            // Get Border offset
            int bOffset = 0;
            switch (_border)
            {
                case BorderStyle.Border3D:
                    bOffset = 2;
                    break;
                case BorderStyle.BorderFlat:
                    bOffset = 1;
                    break;
            }

            // Check auto-size first
            if (_autosize)
            {
                w = _bmp.Width + bOffset + bOffset;
                h = _bmp.Height + bOffset + bOffset;
            }

            // Render border & background
            switch (_border)
            {
                case BorderStyle.Border3D:
                    parent.ScreenBuffer.DrawRectangle(Colors.White, 1, Left, Top, w, h, 0, 0, _bkg, 0, 0, _bkg, 0, 0, 256);
                    parent.ScreenBuffer.DrawRectangle(Colors.Gray, 0, Left + 1, Top + 1, w - 2, h - 2, 0, 0, _bkg, 0, 0, _bkg, 0, 0, 256);
                    break;
                case BorderStyle.BorderFlat:
                    parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, Top, w, h, 0, 0, _bkg, 0, 0, _bkg, 0, 0, 256);
                    break;
                case BorderStyle.BorderNone:
                    //_parent.ScreenBuffer.DrawRectangle(_bkg, 0, Left, Top, _w, _h, 0, 0, _bkg, 0, 0, _bkg, 0, 0, 256);
                    break;
            }

            // Render Image
            if (_bmp != null)
            {
                Rect Region = Rect.Intersect(new Rect(parent.Left, parent.Top, parent.Width, parent.Height), new Rect(Left + bOffset, Top + bOffset, w - bOffset - bOffset, h - bOffset - bOffset));
                parent.ScreenBuffer.SetClippingRectangle(Region.X, Region.Y, Region.Width, Region.Height);
                switch (_scale)
                {
                    case ScaleMode.Normal:
                        parent.ScreenBuffer.DrawImage(Left + bOffset, Top + bOffset, _bmp, 0, 0, _bmp.Width, _bmp.Height);
                        break;
                    case ScaleMode.Stretch:
                        parent.ScreenBuffer.DrawImage(Left + bOffset, Top + bOffset, _bmp, 0, 0, w - bOffset - bOffset, h - bOffset - bOffset);
                        break;
                    case ScaleMode.Scale:
                        float multiplier;
                        int dH = h - bOffset - bOffset;
                        int dW = w - bOffset - bOffset;


                        if (_bmp.Height > _bmp.Width)
                        {
                            // Portrait
                            if (dH > dW)
                            {
                                multiplier = (float)dW / (float)_bmp.Width;
                            }
                            else
                            {
                                multiplier = (float)dH / (float)_bmp.Height;
                            }
                        }
                        else
                        {
                            // Landscape
                            if (dH > dW)
                            {
                                multiplier = (float)dW / (float)_bmp.Width;
                            }
                            else
                            {
                                multiplier = (float)dH / (float)_bmp.Height;
                            }
                        }

                        dsW = (int)((float)_bmp.Width * multiplier);
                        dsH = (int)((float)_bmp.Height * multiplier);
                        dX = Left + bOffset + (int)((float)dW / 2 - (float)dsW / 2);
                        dY = Top + bOffset + (int)((float)dH / 2 - (float)dsH / 2);

                        parent.ScreenBuffer.DrawImage(dX, dY, _bmp, 0, 0, dsW, dsH);
                        break;
                    case ScaleMode.Center:
                        dX = w / 2 - _bmp.Width / 2;
                        dY = h / 2 - _bmp.Height / 2;
                        parent.ScreenBuffer.DrawImage(dX, dY, _bmp, 0, 0, _bmp.Width, _bmp.Height);
                        break;
                    case ScaleMode.Tile:
                        for (dX = 0; dX < w; dX += _bmp.Width)
                        {
                            for (dY = 0; dY < h; dY += _bmp.Height)
                                parent.ScreenBuffer.DrawImage(dX, dY, _bmp, 0, 0, _bmp.Width, _bmp.Height);
                        }
                        break;
                }
            }

            if (flush)
            {
                parent.ScreenBuffer.Flush(Left, Top, w, h);
                parent.ScreenBuffer.SetClippingRectangle(0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
            }
        }
        #endregion
    }
}
