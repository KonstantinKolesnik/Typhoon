using MFE.Graphics.Native.Media;
using Microsoft.SPOT;
using Brush = Microsoft.SPOT.Presentation.Media.Brush;
using Colors = Microsoft.SPOT.Presentation.Media.Colors;
using SolidColorBrush = Microsoft.SPOT.Presentation.Media.SolidColorBrush;

namespace MFE.Graphics.Native.Controls
{
    public class Control : UIElement
    {
        protected internal Brush _background = null;
        protected internal Brush _foreground = new SolidColorBrush(Colors.Black);
        protected internal Font _font;

        public Brush Background
        {
            get { return _background; }
            set
            {
                if (_background != value)
                {
                    _background = value;
                    Invalidate();
                }
            }
        }
        public Font Font
        {
            get { return _font; }
            set
            {
                if (_font != value)
                {
                    _font = value;
                    InvalidateMeasure();
                }
            }
        }
        public Brush Foreground
        {
            get { return _foreground; }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    Invalidate();
                }
            }
        }

        public override void OnRender(DrawingContext dc)
        {
            if (_background != null)
                dc.DrawRectangle(_background, null, 0, 0, ActualWidth, ActualHeight);
        }
    }
}
