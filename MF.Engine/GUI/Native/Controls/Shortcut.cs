using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using System;

namespace MF.Engine.GUI.Controls
{
    /*
    [Serializable]
    public class Shortcut : StackPanel
    {
        #region Fields
        private string name;
        private Icon icon;
        private Text text;
        private Bitmap backgroundImage;
        private ushort opacity = 255;
        private bool pressed = false;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }
        public Bitmap BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                if (backgroundImage != value)
                {
                    backgroundImage = value;
                    Invalidate();
                }
            }
        }
        public ushort Opacity
        {
            get { return opacity; }
            set
            {
                CheckAccess();
                if (opacity != value)
                {
                    opacity = value;
                    Invalidate();
                }
            }
        }
        public Color ForeColor
        {
            get { return text.ForeColor; }
            set
            {
                if (text.ForeColor != value)
                    text.ForeColor = value;
            }
        }
        #endregion

        #region Events
        public event EventHandler Clicked;
        #endregion

        #region Constructors
        public Shortcut(string name, Font font, string txt, Bitmap bmp)
            : this(name, font, txt, bmp, 32)
        {
        }
        public Shortcut(string name, Font font, string txt, Bitmap bmp, int iconSize)
        {
            this.name = name;
            Orientation = Orientation.Vertical;

            if (bmp != null)
            {
                icon = new Icon(bmp, iconSize);
                icon.HorizontalAlignment = HorizontalAlignment.Center;
                Children.Add(icon);
            }

            text = new Text(font, txt);
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.TextAlignment = TextAlignment.Center;
            Children.Add(text);
        }
        #endregion

        #region Event handlers
        public override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawRectangle(
                pressed ? new SolidColorBrush(ColorUtility.ColorFromRGB(50, 50, 50)) : null,
                null,
                0, 0, ActualWidth, ActualHeight);

            if (backgroundImage != null)
                dc.Scale9Image(1, 1, ActualWidth - 2, ActualHeight - 2, backgroundImage, 0, 0, 0, 0, opacity);
        }
        #endregion

        #region Event handlers
        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                pressed = true;
                TouchCapture.Capture(this);
                Invalidate();
            }
        }
        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (IsEnabled)
            {
                pressed = false;
                TouchCapture.Capture(this, CaptureMode.None);
                Invalidate();

                int x, y;
                e.GetPosition(this, 0, out x, out y);
                if (x >= 0 && x < ActualWidth && y >= 0 && y < ActualHeight)
                {
                    if (Clicked != null)
                        Clicked(this, EventArgs.Empty);
                }
            }
        }
        #endregion
    }
    */
}
