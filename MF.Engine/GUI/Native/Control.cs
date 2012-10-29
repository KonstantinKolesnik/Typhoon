using System;
using System.Threading;
using Microsoft.SPOT;

namespace MF.Engine.GUI
{
    #region Event Delegates
    [Serializable]
    public delegate void OnTap(object sender, Point e);

    [Serializable]
    public delegate void OnTapHold(object sender, Point e);

    //[Serializable]
    //public delegate void OnFormTap(Point e);

    //[Serializable]
    //public delegate void OnNodeTap(TreeviewNode node, Point e);

    //[Serializable]
    //public delegate void OnNodeExpanded(object sender, TreeviewNode node);

    //[Serializable]
    //public delegate void OnNodeCollapsed(object sender, TreeviewNode node);

    //[Serializable]
    //public delegate void OnSelectedIndexChange(object sender, int index);

    //[Serializable]
    //public delegate void OnSelectedFileChanged(object sender, string path);

    //[Serializable]
    //public delegate void OnTextChanged(object sender);

    //[Serializable]
    //public delegate void OnVirtualKeyboardClosed(object sender);

    [Serializable]
    public delegate void OnValueChanged(object sender, int value);
    #endregion

    [Serializable]
    public class Control : MarshalByRefObject
    {
        #region Fields
        protected internal Control parent;
        protected internal Rect area;
        protected internal bool visible = true;
        protected internal bool enabled = true;
        protected internal object tag;

        protected internal bool isPenDown = false;
        protected internal Point tapPoint;
        protected internal TapState tapState;          // to wait for ContextMenus
        #endregion

        #region  Properties
        public virtual Control Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public virtual Rect Area
        {
            get { return area; }
            set { area = value; }
        }
        public virtual Rect ScreenArea
        {
            get
            {
                return new Rect(
                    area.X + (parent == null ? 0 : parent.ScreenArea.X),
                    area.Y + (parent == null ? 0 : parent.ScreenArea.Y),
                    area.Width,
                    area.Height);
            }
        }
        public virtual bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public virtual bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public virtual int X
        {
            get { return area.X; }
            set { area.X = value; }
        }
        public virtual int Y
        {
            get { return area.Y; }
            set { area.Y = value; }
        }
        public virtual int Left
        {
            get { return ScreenArea.X; }
        }
        public virtual int Top
        {
            get { return ScreenArea.Y; }
        }
        public virtual int Width
        {
            get { return area.Width; }
            set { area.Width = value; }
        }
        public virtual int Height
        {
            get { return area.Height; }
            set { area.Height = value; }
        }

        public virtual Control TopLevelContainer
        {
            get { return parent == null ? null : parent.TopLevelContainer; }
        }

        public virtual bool IsPenDown
        {
            get { return isPenDown; }
        }
        public virtual object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        #endregion

        #region Events
        public event OnTap Tap;
        protected virtual void OnTap(object sender, Point e)
        {
            if (Tap != null)
                Tap(sender, e);
        }

        public event OnTapHold TapHold;
        protected virtual void OnTapHold(object sender, Point e)
        {
            if (TapHold != null)
                TapHold(sender, e);
        }
        #endregion

        #region Touch Methods
        public virtual void TouchDown(object sender, Point e)
        {
            if (!enabled || !visible)
                return;

            isPenDown = true;

            // Begin TapHold
            tapPoint = e;
            tapState = TapState.Waiting;
            new Thread(delegate()
            {
                Thread.Sleep(750);
                if (tapState != TapState.Normal)
                    OnTapHold(this, tapPoint);
            }
            ).Start();
        }
        public virtual void TouchUp(object sender, Point e)
        {
            if (!enabled || !visible)
                return;

            // Check Tap Hold
            if (tapState == TapState.Complete)
            {
                tapState = TapState.Normal;
                return;
            }

            // Perform normal tap
            if (isPenDown)
            {
                isPenDown = false;
                if (ScreenArea.Contains(e))
                    OnTap(this, new Point(e.X - Left, e.Y - Top));
            }
        }
        public virtual void TouchMove(object sender, Point e)
        {
        }
        #endregion

        #region Render
        public virtual void Update()
        {
        }
        public virtual void Prerender()
        {
        }
        public virtual void Render(ref Bitmap screenBuffer)
        {
        }
        #endregion
    }
}
