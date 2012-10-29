using System;
using Microsoft.SPOT;
using System.Threading;
using Microsoft.SPOT.Presentation.Media;
using MF.Engine.Managers;

namespace MF.Engine.GUI.Controls
{
    [Serializable]
    public class ScrollBar : Control
    {
        #region Variables
        private Orientation orientation;
        private bool _gDown = false;
        private int minimum;
        private int maximum;
        private int value;
        private int smallChange = 1;
        private int largeChange = 10;

        private Rect _decRect;
        private Rect _incRect;
        private Rect _grpRect;
        private int _chgVal;
        private Thread _changer;

        private int _mY = 0;
        private int _mX = 0;

        private bool _auto;
        #endregion

        #region  Properties
        public bool AutoRepeating
        {
            get { return _auto; }
            set { _auto = value; }
        }
        public int Minimum
        {
            get { return minimum; }
            set
            {
                if (value >= maximum)
                    value = maximum - 1;
                minimum = value;
            }
        }
        public int Maximum
        {
            get { return maximum; }
            set
            {
                if (value <= minimum)
                    value = minimum + 1;
                maximum = value;
            }
        }
        public int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    if (value < minimum)
                        value = minimum;
                    if (value > maximum)
                        value = maximum;
                    this.value = value;
                    OnValueChanged(this, value);
                }
            }
        }
        public int SmallChange
        {
            get { return smallChange; }
            set { smallChange = value; }
        }
        public int LargeChange
        {
            get { return largeChange; }
            set { largeChange = value; }
        }
        public override int Width
        {
            get { return base.Width; }
        }
        public override int Height
        {
            get { return base.Height; }
        }
        #endregion

        #region Events
        public event OnValueChanged ValueChanged;
        protected virtual void OnValueChanged(Object sender, int value)
        {
            if (ValueChanged != null)
                ValueChanged(sender, value);
        }
        #endregion

        #region Constructors
        public ScrollBar(int x, int y, int size, Orientation orientation)
        {
            if (size < 40) size = 40;
            orientation = orientation;
            x = x;
            y = y;
            if (orientation == Orientation.Horizontal)
            {
                w = size;
                h = 16;
            }
            else
            {
                h = size;
                w = 16;
            }

            minimum = 0;
            maximum = 100;
            value = 0;
        }
        public ScrollBar(int x, int y, int size, Orientation orientation, int Minimum, int Maximum)
        {
            if (size < 40) size = 40;
            orientation = orientation;
            x = x;
            y = y;
            if (orientation == Orientation.Horizontal)
            {
                w = size;
                h = 16;
            }
            else
            {
                h = size;
                w = 16;
            }

            minimum = Minimum;
            maximum = Maximum;
            value = 0;
        }
        public ScrollBar(int x, int y, int size, Orientation orientation, int Minimum, int Maximum, int value)
        {
            if (size < 40) size = 40;
            orientation = orientation;
            x = x;
            y = y;
            if (orientation == Orientation.Horizontal)
            {
                w = size;
                h = 16;
            }
            else
            {
                h = size;
                w = 16;
            }

            minimum = Minimum;
            maximum = Maximum;
            value = value;
        }
        #endregion

        #region Touch Invokes
        public override void TouchDown(object sender, Point e)
        {
            isPenDown = true;

            if (!enabled) return;
            if (_decRect.Contains(e))
            {
                Value = value - smallChange;
                _chgVal = -smallChange;
                if (_auto)
                {
                    _changer = new Thread(AutoIncDec);
                    _changer.Priority = ThreadPriority.AboveNormal;
                    _changer.Start();
                }
                return;
            }

            if (_incRect.Contains(e))
            {
                Value = value + smallChange;
                _chgVal = smallChange;
                if (_auto)
                {
                    _changer = new Thread(AutoIncDec);
                    _changer.Priority = ThreadPriority.AboveNormal;
                    _changer.Start();
                }
                return;
            }

            if (_grpRect.Contains(e))
            {
                _mX = e.X;
                _mY = e.Y;
                _gDown = true;
                return;
            }

            if (orientation == Orientation.Horizontal)
            {
                Value = value + ((e.X < _grpRect.X) ? -largeChange : largeChange);
            }
            else
            {
                Value = value + ((e.Y < _grpRect.Y) ? -largeChange : largeChange);
            }

        }
        public override void TouchUp(object sender, Point e)
        {
            if (isPenDown)
            {
                if (this.ScreenBounds.Contains(e)) OnTap(this, new Point(e.X - Left, e.Y - Top));
                isPenDown = false;
            }
            _gDown = false;
        }
        public override void TouchMove(object sender, Point e)
        {
            // Only respond if we have the gripper
            if (!_gDown) return;

            if (orientation == Orientation.Horizontal)
            {
                // Calculate value from New Position
                // Difference (DIF)         = current X - initial X
                // New X (NWX)              = (GripX + DIF) - left - 16 (remove offset left & dec CommandButton)
                // Slide Range (ASR)        = Width - 32 - GripWidth (32 for both CommandButtons)
                // Percent of Slide (PoS)   = NWX / ASR
                // value (VAL)              = ((max - min) * PoS) + min
                Value = (int)((maximum - minimum) * ((float)((_grpRect.X + (e.X - _mX)) - Left - 16) / (float)(w - 32 - _grpRect.Width))) + minimum;
            }
            else
            {
                // Calculate value from New Position
                // Difference (DIF)         = current Y - initial Y
                // New Y (NWY)              = (GripY + DIF) - top - 16 (remove offset top & dec CommandButton)
                // Slide Range (ASR)        = Height - 32 - GripHeight (32 for both CommandButtons)
                // Percent of Slide (PoS)   = NWY / ASR
                // value (VAL)              = ((max - min) * PoS) + min
                Value = (int)((maximum - minimum) * ((float)((_grpRect.Y + (e.Y - _mY)) - Top - 16) / (float)(h - 32 - _grpRect.Height))) + minimum;
            }

            _mX = e.X;
            _mY = e.Y;
        }
        #endregion

        #region GUI
        //public override void Render(bool flush)
        //{
        //    if (parent == null || parent.ScreenBuffer == null || !visible || suspend) return;

        //    int iGripSize = 0;

        //    // Set clipping region in case we're off parent somewhere (can happen w/ scroll)
        //    parent.ScreenBuffer.SetClippingRectangle(parent.Left, parent.Top, parent.Width, parent.Height);

        //    // Background is the same regardless of orientation
        //    parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, Top, w, h, 0, 0, Colors.Gray, 0, 0, Colors.Gray, 0, 0, 256);

        //    if (_orientation == Orientation.Horizontal)
        //    {
        //        // Draw the Left/Right CommandButtons
        //        parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, Top, 16, 16, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //        //_parent.ScreenBuffer.DrawImage(Left + 4, Top + 4, Resources.GetBitmap(Resources.BitmapResources.left), 0, 0, 5, 9);
        //        parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left + w - 16, Top, 16, 16, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //        //_parent.ScreenBuffer.DrawImage(Left + _w - 11, Top + 4, Resources.GetBitmap(Resources.BitmapResources.right), 0, 0, 5, 9);

        //        if (enabled)
        //        {
        //            // Update Rects
        //            // We do this every render incase our offset has changed
        //            _decRect = new Rect(Left, Top, 16, 16);
        //            _incRect = new Rect(Left + w - 16, Top, 16, 16);

        //            // Calculate Gripper Size
        //            // Total Value Range (TVR)      = max - min
        //            // Total Travel Range (TTR)     = TVR / SmallChange
        //            // Available Grip Range (AGR)   = Size - 32
        //            // Grip Size                    = AGR - TTR
        //            iGripSize = (w - 32) - ((_max - _min) / _sml);
        //            if (iGripSize < 8) iGripSize = 8;

        //            // Calculate Gripper left (remember we might change gripper size to meet minimum)
        //            // Available Draw Range (ADR)   = (Size - 32 - GripSize)
        //            // Percent of Total (PoT)       = (value - min) / (max - min)
        //            // Offset                       = ADR * PoT
        //            int iOffset = (int)((w - 32 - iGripSize) * ((float)(_val - _min) / (float)(_max - _min)));

        //            // Draw the Gripper
        //            parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, iOffset + Left + 16, Top, iGripSize, 16, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //            _grpRect = new Rect(iOffset + Left + 16, Top, iGripSize, 16);
        //        }
        //    }
        //    else
        //    {
        //        // Draw the Up/Down CommandButtons
        //        parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, Top, 16, 16, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //        //_parent.ScreenBuffer.DrawImage(Left + 4, Top + 5, Resources.GetBitmap(Resources.BitmapResources.up), 0, 0, 9, 5);
        //        parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, Top + h - 16, 16, 16, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //        //_parent.ScreenBuffer.DrawImage(Left + 4, Top + _h - 11, Resources.GetBitmap(Resources.BitmapResources.down), 0, 0, 9, 5);

        //        if (enabled)
        //        {
        //            // Update Rects
        //            // We do this every render incase our offset has changed
        //            _decRect = new Rect(Left, Top, 16, 16);
        //            _incRect = new Rect(Left, Top + h - 16, 16, 16);

        //            // Calculate Gripper Size
        //            // Total Value Range (TVR)      = max - min
        //            // Total Travel Range (TTR)     = TVR / SmallChange
        //            // Available Grip Range (AGR)   = Size - 32
        //            // Grip Size                    = AGR - TTR
        //            iGripSize = (h - 32) - ((_max - _min) / _sml);
        //            if (iGripSize < 8) iGripSize = 8;

        //            // Calculate Gripper left (remember we might change gripper size to meet minimum)
        //            // Available Draw Range (ADR)   = (Size - 32 - GripSize)
        //            // Percent of Total (PoT)       = (value - min) / (max - min)
        //            // Offset                       = ADR * PoT
        //            int iOffset = (int)((h - 32 - iGripSize) * ((float)(_val - _min) / (float)(_max - _min)));

        //            // Draw the Gripper
        //            parent.ScreenBuffer.DrawRectangle(Colors.Black, 1, Left, iOffset + Top + 16, 16, iGripSize, 0, 0, Colors.DarkGray, 0, 0, Colors.DarkGray, 0, 0, 256);
        //            _grpRect = new Rect(Left, iOffset + Top + 16, 16, iGripSize);
        //        }
        //    }

        //    if (flush)
        //    {
        //        parent.ScreenBuffer.Flush(Left, Top, w, h);
        //        parent.ScreenBuffer.SetClippingRectangle(0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
        //    }
        //}
        #endregion

        #region Private Methods
        private void AutoIncDec()
        {
            int iWait = 750;
            while (isPenDown)
            {
                Thread.Sleep(iWait);
                if (!isPenDown) return;
                Value = value + _chgVal;
                switch (iWait)
                {
                    case 750:
                        iWait = 500;
                        break;
                    case 500:
                        iWait = 250;
                        break;
                    case 250:
                        iWait = 75;
                        break;
                }
            }
        }
        #endregion
    }
}
