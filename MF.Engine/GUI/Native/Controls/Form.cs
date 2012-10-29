using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using MF.Engine.Managers;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class Form : Container
    {
        //#region Enumerations
        internal enum WindowType
        {
            standard = 0,
            fullborder = 1,
            container = 2,
        }
        //#endregion

        #region Fields
        private bool autoScroll;
        private Color backColor = Colors.LightGray;
        private Bitmap backImage = null;
        private ushort opacity = Bitmap.OpacityOpaque;
        private WindowType winType = WindowType.standard;
        //private static Engine API;
        //private ScrollBar hScroll;
        //private ScrollBar vScroll;
        #endregion

        #region Properties
        public override Control Parent
        {
            get { return null; }
        }
        public bool AutoScroll
        {
            get { return autoScroll; }
            set
            {
                autoScroll = value;
                if (!autoScroll)
                {
                    //hScroll = null;
                    //vScroll = null;
                }
                CheckAutoScroll();
            }
        }
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
        //public override int Height
        //{
        //    get { return h; }
        //    //set
        //    //{
        //    //}
        //}
        //public override Control TopLevelContainer
        //{
        //    get { return this; }
        //}
        #endregion

        #region Events
        //public event OnFormTap FormTapped;
        //protected virtual void OnFormTap(Point e)
        //{
        //    if (FormTapped != null)
        //        FormTapped(e);
        //}
        #endregion

        #region Constructors
        public Form()
        {
            //API = Engine.Instance;
            area = new Rect(0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);

            //if (AppDomain.CurrentDomain.FriendlyName != "default")
            //    API.RegisterForm(this, AppDomain.CurrentDomain);
        }
        public Form(Color backColor)
            : this()
        {
            BackColor = backColor;
        }
        public Form(Color backColor, bool enabled, bool visible)
            : this(backColor)
        {
            Enabled = enabled;
            Visible = visible;
        }

        internal Form(Color backColor, int x, int y, int width, int height, bool enabled, bool visible)
            : this(backColor, enabled, visible)
        {
            Area = new Rect(x, y, width, height);
        }
        internal Form(Color backColor, int x, int y, int width, int height, bool enabled, bool visible, WindowType Type)
            : this(backColor, x, y, width, height, enabled, visible)
        {
            winType = Type;
        }
        #endregion

        #region Public Methods
        public override void AddChild(Control child)
        {
            base.AddChild(child);
            CheckAutoScroll();
        }
        public override void ClearChildren()
        {
            base.ClearChildren();
            CheckAutoScroll();
        }
        public override void RemoveChild(Control child)
        {
            base.RemoveChild(child);
            CheckAutoScroll();
        }
        public override void RemoveChildAt(int index)
        {
            base.RemoveChildAt(index);
            CheckAutoScroll();
        }
        #endregion

        #region Touch Methods
        public override void TouchDown(object sender, Point e)
        {
            if (!visible)
                return;

            // Scrollbars first
            //if (hScroll != null)
            //{
            //    if (hScroll.Visible && hScroll.Enabled && hScroll.ScreenBounds.Contains(e))
            //    {
            //        hScroll.TouchDown(this, e);
            //        return;
            //    }
            //}
            //if (vScroll != null)
            //{
            //    if (vScroll.Visible && vScroll.Enabled && vScroll.ScreenBounds.Contains(e))
            //    {
            //        vScroll.TouchDown(this, e);
            //        return;
            //    }
            //}

            // Check controls
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Control child = GetChildAt(i);
                if (child.Visible && child.Enabled && child.ScreenArea.Contains(e))
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
            {
                isPenDown = false;
                return;
            }

            bool ret = false;
            bool ignoreUp = false;


            // Scrollbars first
            //if (hScroll != null)
            //{
            //    if (hScroll.ScreenBounds.Contains(e) && !ignoreUp)
            //    {
            //        ignoreUp = true;
            //        ret = true;
            //        hScroll.TouchUp(this, e);
            //        isPenDown = false;
            //        return;
            //    }
            //    else if (hScroll.IsPenDown)
            //        hScroll.TouchUp(this, e);
            //}
            //if (vScroll != null)
            //{
            //    if (vScroll.ScreenBounds.Contains(e) && !ignoreUp)
            //    {
            //        ignoreUp = true;
            //        ret = true;
            //        vScroll.TouchUp(this, e);
            //        isPenDown = false;
            //        return;
            //    }
            //    else if (vScroll.IsPenDown)
            //        vScroll.TouchUp(this, e);
            //}

            // Check controls
            Control child = null;
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                try
                {
                    child = GetChildAt(i);
                    if (child.ScreenArea.Contains(e) && !ignoreUp)
                    {
                        ignoreUp = true;
                        ret = true;
                        child.TouchUp(this, e);
                        isPenDown = false;
                    }
                    else if (child.IsPenDown)
                    {
                        child.TouchUp(this, e);
                    }
                }
                catch (Exception)
                {
                    // This can happen if the user clears the Form during a tap
                }
            }

            //if (!ret && isPenDown)
            //    OnFormTap(new Point(e.X, e.Y - 22));
        }
        public override void TouchMove(object sender, Point e)
        {
            if (!visible)
                return;

            // Scrollbars first
            //if (hScroll != null)
            //{
            //    if (hScroll.Visible && hScroll.Enabled && hScroll.ScreenBounds.Contains(e))
            //    {
            //        hScroll.TouchMove(this, e);
            //        return;
            //    }
            //}
            //if (vScroll != null)
            //{
            //    if (vScroll.Visible && vScroll.Enabled && vScroll.ScreenBounds.Contains(e))
            //    {
            //        vScroll.TouchMove(this, e);
            //        return;
            //    }
            //}

            // Check controls
            for (int i = Children.Count - 1; i >= 0; i--)
            {
                Control child = GetChildAt(i);
                if (child.Visible && child.Enabled && child.ScreenArea.Contains(e))
                {
                    child.TouchMove(this, e);
                    return;
                }
            }

            isPenDown = true;
        }
        #endregion

        #region Render
        public override void Render(ref Bitmap screenBuffer)
        {
            screenBuffer.SetClippingRectangle(Left, Top, Width, Height);

            if (backImage != null)
                screenBuffer.Scale9Image(0, 0, Width, Height, backImage, 0, 0, 0, 0, opacity);
            else
                screenBuffer.DrawRectangle(backColor, 1, X, Y, Width, Height, 0, 0, backColor, 0, 0, backColor, 0, 0, opacity);

            // Windowize
            //switch (winType)
            //{
            //    case WindowType.container:
            //        //API.ScreenBuffer.DrawRectangle(Color.Black, 1, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 175);
            //        screenBuffer.DrawRectangle(Color.Black, 1, x, y, w, h, 0, 0, backColor, 0, 0, Colors.Gray, 0, 0, Bitmap.OpacityOpaque);
            //        screenBuffer.DrawRectangle(Color.Black, 1, x + 3, y + 3, w - 6, h - 6, 0, 0, backColor, 0, 0, backColor, 0, 0, Bitmap.OpacityOpaque);
            //        break;
            //    case WindowType.fullborder:
            //        //API.ScreenBuffer.DrawRectangle(Color.Black, 1, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 175);
            //        screenBuffer.DrawRectangle(Color.Black, 1, x, y, w, h, 0, 0, backColor, 0, 0, Colors.Gray, 0, 0, Bitmap.OpacityOpaque);
            //        screenBuffer.DrawRectangle(Color.Black, 1, x + 3, y + 3, w - 6, h - 6, 0, 0, backColor, 0, 0, backColor, 0, 0, Bitmap.OpacityOpaque);
            //        //API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x + 3, _y + 3, _w - 6, FontManager.Arial.Height + 4, 0, 0, Colors.Blue, 0, 0, Colors.Blue, 0, 0, Bitmap.OpacityOpaque);
            //        break;
            //    default:
            //        // Do Nothing
            //        break;
            //}

            // Render Controls
            for (int i = 0; i < Children.Count; i++)
            {
                Control el = GetChildAt(i);
                el.Render(ref screenBuffer);
                //Debug.GC(true);
            }



            //if (hScroll != null)
            //    hScroll.Render();
            //if (vScroll != null)
            //    vScroll.Render();
        }
        #endregion

        #region Private Methods
        private void CheckAutoScroll()
        {
            if (!autoScroll)
                return;

            //int maxX = 0;
            //int maxY = 0;
            //int adjX = 0;
            //int adjY = 0;

            //// Check Controls
            //for (int i = 0; i < Children.Count; i++)
            //{
            //    Control ele = GetChildAt(i);
            //    if (ele.X + ele.Width > maxX)
            //        maxX = ele.X + ele.Width;
            //    if (ele.Y + ele.Height > maxY)
            //        maxY = ele.Y + ele.Height;
            //}

            //// Update adjustments
            //if (maxX > w)
            //    adjY = 16;
            //if (maxY > h - adjY)
            //    adjX = 16;

            //// Double-check horizontal
            //if (adjY == 0 && adjX == 16 && maxX > w - 16)
            //    adjY = 16;

            //// Setup Scrollbars
            //if (maxX > w - adjX)
            //{
            //    hScroll = new ScrollBar(0, h - 16, w - adjX, Orientation.Horizontal);
            //    hScroll.Minimum = 0;
            //    hScroll.Maximum = (maxX - w) + adjX + 5;
            //    hScroll.ValueChanged += new OnValueChanged((object sender, int value) => UpdateChildOffsets());
            //    hScroll.Parent = this;
            //    hScroll.SetOffset(this, new Point(0, 22));
            //}
            //else
            //    hScroll = null;

            //if (maxY > h - adjY)
            //{
            //    vScroll = new ScrollBar(w - 16, 0, h - adjY, Orientation.Vertical);
            //    vScroll.Minimum = 0;
            //    vScroll.Maximum = (maxY - h) + adjY + 5;
            //    vScroll.ValueChanged += new OnValueChanged((object sender, int value) => UpdateChildOffsets());
            //    vScroll.Parent = this;
            //    vScroll.SetOffset(this, new Point(0, 22));
            //}
            //else
            //    vScroll = null;
        }
        private void UpdateChildOffsets()
        {
            //int x = (hScroll == null) ? 0 : -hScroll.Value;
            //int y = (vScroll == null) ? 22 : 22 - vScroll.Value;

            //for (int i = 0; i < Children.Count; i++)
            //{
            //    Control ele = GetChildAt(i);
            //    ele.SetOffset(this, new Point(x, y));
            //}
        }
        #endregion
    }
}
