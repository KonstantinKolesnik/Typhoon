using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using MF.Engine.Managers;

namespace MF.Engine.GUI.Controls
{
    /*
    [Serializable]
    public class FormOld : Container
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
        private Color _bkg = Colors.LightGray;
        private Bitmap _img = null;

        private WindowType _winType = WindowType.standard;

        private static Engine API;

        private ScrollBar HScroll;              // Horizontal Scrollbar for Auto-Scrolling
        private ScrollBar VScroll;              // Vertial Scrollbar for Auto-Scrolling
        private bool _bAutoScroll;              // Auto-Scroll when True
        #endregion

        #region Constructors
        public FormOld()
        {
            API = Engine.Instance;

            _x = 0;
            _y = 0;// 22;
            _w = AppearanceManager.ScreenWidth;
            _h = AppearanceManager.ScreenHeight;// -22;
            _enabled = true;
            _visible = true;

            //if (AppDomain.CurrentDomain.FriendlyName != "default")
            //    API.RegisterForm(this, AppDomain.CurrentDomain);

        }
        public FormOld(Color backcolor)
            : this()
        {
            _bkg = backcolor;
        }
        public FormOld(Color backcolor, bool enabled, bool visible)
            : this(backcolor)
        {
            _enabled = enabled;
            _visible = visible;
        }

        internal FormOld(Color backcolor, int x, int y, int width, int height, bool enabled, bool visible)
            : this(backcolor, enabled, visible)
        {
            _x = x;
            _y = y;
            _w = width;
            _h = height;
        }
        internal FormOld(Color backcolor, int x, int y, int width, int height, bool enabled, bool visible, WindowType Type)
            : this(backcolor, x, y, width, height, enabled, visible)
        {
            _winType = Type;
        }
        #endregion

        #region Properties
        public Engine APIRef
        {
            get { return API; }
            internal set
            {
                API = value;
            }
        }
        public bool AutoScroll
        {
            get { return _bAutoScroll; }
            set
            {
                _bAutoScroll = value;
                if (value == false)
                {
                    HScroll = null;
                    VScroll = null;
                }
                else
                    CheckAutoScroll();

                Render(true);
            }
        }
        public Color Backcolor
        {
            get { return _bkg; }
            set
            {
                _bkg = value;
                Render(true);
            }
        }
        public Bitmap BackgroundImage
        {
            get { return _img; }
            set
            {
                _img = value;
                Render(true);
            }
        }
        public override Control Parent
        {
            get { return null; }
            set { throw new Exception("Forms cannot have parents."); }
        }
        public override int Height
        {
            get { return _h; }
            set
            {
            }
        }
        public override Bitmap ScreenBuffer
        {
            get { return API.ScreenBuffer; }
        }
        public override Control TopLevelContainer
        {
            get { return this; }
        }
        #endregion

        #region Events
        public event OnFormTap FormTapped;
        protected virtual void OnFormTap(Point e)
        {
            if (FormTapped != null)
                FormTapped(e);
        }
        #endregion

        #region Touch Methods
        public override void TouchDown(object sender, Point e)
        {
            if (!_visible)
                return;

            // Scrollbars first
            if (HScroll != null)
            {
                if (HScroll.Visible && HScroll.Enabled && HScroll.ScreenBounds.contains(e))
                {
                    HScroll.TouchDown(this, e);
                    return;
                }
            }
            if (VScroll != null)
            {
                if (VScroll.Visible && VScroll.Enabled && VScroll.ScreenBounds.contains(e))
                {
                    VScroll.TouchDown(this, e);
                    return;
                }
            }

            // Check controls
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                Control child = (Control)_children[i];
                if (child.Visible && child.Enabled && child.ScreenBounds.contains(e))
                {
                    child.TouchDown(this, e);
                    return;
                }
            }

            _mDown = true;
        }
        public override void TouchUp(object sender, Point e)
        {
            if (!_visible)
            {
                _mDown = false;
                return;
            }

            bool ret = false;
            bool ignoreUp = false;


            // Scrollbars first
            if (HScroll != null)
            {
                if (HScroll.ScreenBounds.contains(e) && !ignoreUp)
                {
                    ignoreUp = true;
                    ret = true;
                    HScroll.TouchUp(this, e);
                    _mDown = false;
                    return;
                }
                else if (HScroll.PenDown)
                    HScroll.TouchUp(this, e);
            }
            if (VScroll != null)
            {
                if (VScroll.ScreenBounds.contains(e) && !ignoreUp)
                {
                    ignoreUp = true;
                    ret = true;
                    VScroll.TouchUp(this, e);
                    _mDown = false;
                    return;
                }
                else if (VScroll.PenDown)
                    VScroll.TouchUp(this, e);
            }

            // Check controls
            Control myControl = null;
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                try
                {
                    myControl = (Control)_children[i];
                    if (myControl.ScreenBounds.contains(e) && !ignoreUp)
                    {
                        ignoreUp = true;
                        ret = true;
                        myControl.TouchUp(this, e);
                        _mDown = false;
                    }
                    else if (myControl.PenDown)
                    {
                        myControl.TouchUp(this, e);
                    }
                }
                catch (Exception)
                {
                    // This can happen if the user clears the Form during a tap
                }
            }

            if (!ret && _mDown)
                OnFormTap(new Point(e.X, e.Y - 22));
        }
        public override void TouchMove(object sender, Point e)
        {
            if (!_visible)
                return;

            // Scrollbars first
            if (HScroll != null)
            {
                if (HScroll.Visible && HScroll.Enabled && HScroll.ScreenBounds.contains(e))
                {
                    HScroll.TouchMove(this, e);
                    return;
                }
            }
            if (VScroll != null)
            {
                if (VScroll.Visible && VScroll.Enabled && VScroll.ScreenBounds.contains(e))
                {
                    VScroll.TouchMove(this, e);
                    return;
                }
            }

            // Check controls
            for (int i = _children.Count - 1; i >= 0; i--)
            {
                Control child = (Control)_children[i];
                if (child.Visible && child.Enabled && child.ScreenBounds.contains(e))
                {
                    child.TouchMove(this, e);
                    return;
                }
            }

            _mDown = true;
        }
        #endregion

        #region Public Methods
        public override void AddChild(Control child)
        {
            child.Parent = this;
            child.SetOffset(this, new Point(_x, _y));
            _children.Add(child);
            
            if (_bAutoScroll)
                CheckAutoScroll();

            Render(true);
        }
        public override void ClearChildren()
        {
            _children.Clear();
            if (_bAutoScroll)
                CheckAutoScroll();
            Render(true);
        }
        public override Control GetChildAt(int index)
        {
            return (Control)_children[index];
        }
        public override void RemoveChild(Control child)
        {
            _children.Remove(child);
            if (_bAutoScroll)
                CheckAutoScroll();
            Render(true);
        }
        public override void RemoveChildAt(int index)
        {
            _children.RemoveAt(index);
            if (_bAutoScroll)
                CheckAutoScroll();
            Render(true);
        }
        
        
        
        public override void Render()
        {
            Render(true);
        }
        public override void Render(bool flush)
        {
            if (API == null)
                return;
            if (API.ActiveForm != this)
                return;

            // Set clipping region in case we're off parent somewhere (can happen w/ scroll)
            API.ScreenBuffer.SetClippingRectangle(Left, Top, _w, _h);

            // Render Solid Background
            API.ScreenBuffer.DrawRectangle(_bkg, 1, _x, _y, _w, _h, 0, 0, _bkg, 0, 0, _bkg, 0, 0, Bitmap.OpacityOpaque);

            // Render Image
            if (_img != null)
                API.ScreenBuffer.DrawImage(0, 0, _img, 0, 0, _w, _h);

            // Windowize
            switch (_winType)
            {
                case WindowType.container:
                    //API.ScreenBuffer.DrawRectangle(Color.Black, 1, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 175);
                    API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x, _y, _w, _h, 0, 0, _bkg, 0, 0, Colors.Gray, 0, 0, Bitmap.OpacityOpaque);
                    API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x + 3, _y + 3, _w - 6, _h - 6, 0, 0, _bkg, 0, 0, _bkg, 0, 0, Bitmap.OpacityOpaque);
                    break;
                case WindowType.fullborder:
                    //API.ScreenBuffer.DrawRectangle(Color.Black, 1, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Color.Black, 0, 0, Color.Black, 0, 0, 175);
                    API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x, _y, _w, _h, 0, 0, _bkg, 0, 0, Colors.Gray, 0, 0, Bitmap.OpacityOpaque);
                    API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x + 3, _y + 3, _w - 6, _h - 6, 0, 0, _bkg, 0, 0, _bkg, 0, 0, Bitmap.OpacityOpaque);
                    //API.ScreenBuffer.DrawRectangle(Color.Black, 1, _x + 3, _y + 3, _w - 6, FontManager.Arial.Height + 4, 0, 0, Colors.Blue, 0, 0, Colors.Blue, 0, 0, Bitmap.OpacityOpaque);
                    break;
                default:
                    // Do Nothing
                    break;
            }

            // Render Controls
            for (int i = 0; i < _children.Count; i++)
            {
                Control el = (Control)_children[i];
                el.Render();
                Debug.GC(true);
            }

            if (HScroll != null)
                HScroll.Render();
            if (VScroll != null)
                VScroll.Render();

            if (flush)
            {
                API.ScreenBuffer.SetClippingRectangle(0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
                API.ScreenBuffer.Flush(_x, _y, _w, _h);
            }
        }
        #endregion

        #region Private Methods
        private void CheckAutoScroll()
        {
            int maxX = 0;
            int maxY = 0;
            int adjX = 0;
            int adjY = 0;

            // Check Controls
            for (int i = 0; i < _children.Count; i++)
            {
                Control ele = (Control)_children[i];
                if (ele.X + ele.Width > maxX)
                    maxX = ele.X + ele.Width;
                if (ele.Y + ele.Height > maxY)
                    maxY = ele.Y + ele.Height;
            }

            // Update adjustments
            if (maxX > _w)
                adjY = 16;
            if (maxY > _h - adjY)
                adjX = 16;

            // Double-check horizontal
            if (adjY == 0 && adjX == 16 && maxX > _w - 16)
                adjY = 16;

            // Setup Scrollbars
            if (maxX > _w - adjX)
            {
                HScroll = new ScrollBar(0, _h - 16, _w - adjX, Orientation.Horizontal);
                HScroll.Minimum = 0;
                HScroll.Maximum = (maxX - _w) + adjX + 5;
                HScroll.ValueChanged += new OnValueChanged((object sender, int value) => UpdateChildOffsets());
                HScroll.Parent = this;
                HScroll.SetOffset(this, new Point(0, 22));
            }
            else
                HScroll = null;

            if (maxY > _h - adjY)
            {
                VScroll = new ScrollBar(_w - 16, 0, _h - adjY, Orientation.Vertical);
                VScroll.Minimum = 0;
                VScroll.Maximum = (maxY - _h) + adjY + 5;
                VScroll.ValueChanged += new OnValueChanged((object sender, int value) => UpdateChildOffsets());
                VScroll.Parent = this;
                VScroll.SetOffset(this, new Point(0, 22));
            }
            else
                VScroll = null;
        }
        private void UpdateChildOffsets()
        {
            int x = (HScroll == null) ? 0 : -HScroll.Value;
            int y = (VScroll == null) ? 22 : 22 - VScroll.Value;

            for (int i = 0; i < _children.Count; i++)
            {
                Control ele = (Control)_children[i];
                ele.SetOffset(this, new Point(x, y));
            }
            Render(true);
        }
        #endregion
    }
    */
}
