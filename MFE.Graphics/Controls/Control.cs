using System.Collections;
using System.Threading;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;

namespace MFE.Graphics.Controls
{
    #region Event Delegates
    //[Serializable]
    //public delegate void OnTap(object sender, Point e);

    //[Serializable]
    //public delegate void OnTapHold(object sender, Point e);

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

    //[Serializable]
    //public delegate void OnValueChanged(object sender, int value);
    #endregion

    public abstract class Control
    {
        #region Fields
        private string name;
        private Control parent;
        private ArrayList hierarchy = new ArrayList();
        private Rect area; // in client coordinates
        private bool enabled = true;
        private bool visible = true;
        protected internal ControlCollection children;
        private object tag;

        private Rect dirtyArea = Rect.Empty; // in screen coordinates
        private bool isSuspended = false;
        #endregion

        #region  Properties
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public virtual Control Parent
        {
            get { return parent; }
            internal set
            {
                if (parent != value)
                {
                    parent = value;
                    Invalidate();
                }
            }
        }
        public virtual bool Enabled
        {
            get { return parent != null ? enabled && parent.Enabled : enabled; }
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (visible)
                        Invalidate();
                    else if (parent != null)
                        parent.Invalidate();
                }
            }
        }
        public virtual bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    Invalidate();
                }
            }
        }

        public virtual Rect Area
        {
            get { return area; }
        }
        public virtual int X
        {
            get { return area.X; }
            set
            {
                if (area.X != value)
                {
                    area.X = value;
                    Invalidate();
                }
            }
        }
        public virtual int Y
        {
            get { return area.Y; }
            set
            {
                if (area.Y != value)
                {
                    area.Y = value;
                    Invalidate();
                }
            }
        }
        public virtual int Width
        {
            get { return area.Width; }
            set
            {
                if (area.Width != value)
                {
                    area.Width = value;
                    //SizeChanged(area.Width, area.Height);
                    Invalidate();
                }
            }
        }
        public virtual int Height
        {
            get { return area.Height; }
            set
            {
                if (area.Height != value)
                {
                    area.Height = value;
                    //SizeChanged(area.Width, area.Height);
                    Invalidate();
                }
            }
        }

        public virtual Rect ScreenArea
        {
            get
            {
                return RootControl != GraphicsManager.Desktop ? Rect.Empty : new Rect(
                    area.X + (parent == null ? 0 : parent.ScreenArea.X),
                    area.Y + (parent == null ? 0 : parent.ScreenArea.Y),
                    area.Width,
                    area.Height);
            }
        }

        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private Control RootControl
        {
            get
            {
                // we use two pointers to atomically check / iterate through parents
                Control p = null;
                Control pp = this;
                do
                {
                    p = pp;
                    pp = p.Parent;
                } while (pp != null);

                return p;
            }
        }
        //private ArrayList Hierarchy
        //{
        //    get
        //    {
        //        hierarchy.Clear();

        //        if (RootControl == GraphicsManager.Desktop)
        //        {
        //            Control p = null;
        //            Control pp = this;
        //            do
        //            {
        //                p = pp;
        //                pp = p.parent;
        //                hierarchy.Insert(0, p);
        //            } while (pp != null);
        //        }

        //        return hierarchy;
        //    }
        //}
        #endregion

        #region Events
        //public event TouchEventHandler SizeChanged;
        
        public event TouchEventHandler TouchDown;
        public event TouchEventHandler TouchMove;
        public event TouchEventHandler TouchUp;

        public event TouchGestureEventHandler TouchGestureStarted;
        public event TouchGestureEventHandler TouchGestureChanged;
        public event TouchGestureEventHandler TouchGestureEnded;
        #endregion

        #region Constructor
        protected Control(int x, int y, int width, int height)
        {
            children = new ControlCollection(this);

            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        #endregion

        #region Public methods
        public bool ContainsScreenPoint(Point p)
        {
            return ScreenArea.Contains(p);
        }
        
        public Control TouchableChildFromScreenPoint(Point p)
        {
            Control targetElement = null;

            if (visible && enabled && ContainsScreenPoint(p))
            {
                targetElement = this;

                for (int i = children.Count - 1; i >= 0; i--)
                {
                    if (children[i].ScreenArea.Intersects(ScreenArea))
                    {
                        Control target = children[i].TouchableChildFromScreenPoint(p);
                        if (target != null)
                        {
                            targetElement = target;
                            break;
                        }
                    }
                }
            }

            return targetElement;
        }
        public Control TouchableParentFromScreenPoint(Point p)
        {
            Control ppp = this;
            Control pp = null;
            do
            {
                pp = ppp;
                ppp = pp.Parent;
            } while (ppp != null && !ppp.ContainsScreenPoint(p));

            return ppp;// == this ? null : ppp;
        }

        public void PointToScreen(ref Point p)
        {
            Control client = this;
            while (client != null)
            {
                p.X += client.ScreenArea.X;
                p.Y += client.ScreenArea.Y;

                client = client.Parent;
            }
        }
        public void PointToClient(ref Point p)
        {
            p.X -= ScreenArea.X;
            p.Y -= ScreenArea.Y;
        }

        public void Translate(int dx, int dy)
        {
            area.Translate(dx, dy);
            Invalidate();
        }

        public void SuspendLayout()
        {
            isSuspended = true;
        }
        public void ResumeLayout()
        {
            isSuspended = false;
            Invalidate();
        }

        public void Invalidate()
        {
            if (!isSuspended)
            {
                Rect r = dirtyArea;
                r.Combine(ScreenArea);

                ProcessTask(new RenderTask(this, r));
                dirtyArea = ScreenArea;
            }
        }
        #endregion

        #region Touch handlers
        internal void RaiseTouchDownEvent(TouchEventArgs e)
        {
            OnTouchDown(e);
        }
        internal void RaiseTouchMoveEvent(TouchEventArgs e)
        {
            OnTouchMove(e);
        }
        internal void RaiseTouchUpEvent(TouchEventArgs e)
        {
            OnTouchUp(e);
        }
        internal void RaiseTouchGestureStartedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureStarted(e);
        }
        internal void RaiseTouchGestureChangedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureChanged(e);
        }
        internal void RaiseTouchGestureEndedEvent(TouchGestureEventArgs e)
        {
            OnTouchGestureEnded(e);
        }

        protected virtual void OnTouchDown(TouchEventArgs e)
        {
            if (TouchDown != null)
                TouchDown(this, e);
        }
        protected virtual void OnTouchMove(TouchEventArgs e)
        {
            if (TouchMove != null)
                TouchMove(this, e);
        }
        protected virtual void OnTouchUp(TouchEventArgs e)
        {
            if (TouchUp != null)
                TouchUp(this, e);
        }
        protected virtual void OnTouchGestureStarted(TouchGestureEventArgs e)
        {
            if (TouchGestureStarted != null)
                TouchGestureStarted(this, e);

            if (parent != null)
                parent.OnTouchGestureStarted(e);
        }
        protected virtual void OnTouchGestureChanged(TouchGestureEventArgs e)
        {
            if (TouchGestureChanged != null)
                TouchGestureChanged(this, e);

            if (parent != null)
                parent.OnTouchGestureChanged(e);
        }
        protected virtual void OnTouchGestureEnded(TouchGestureEventArgs e)
        {
            if (TouchGestureEnded != null)
                TouchGestureEnded(this, e);

            if (parent != null)
                parent.OnTouchGestureEnded(e);
        }
        #endregion

        #region Protected methods
        internal virtual void ProcessTask(RenderTask task)
        {
            if (!isSuspended)
            {
                if (parent != null)
                    parent.ProcessTask(task);
                else if (this == GraphicsManager.Desktop)
                    GraphicsManager.PostRenderTask(task);
            }
        }
        
        internal void OnChildrenChanged(Control added, Control removed, int indexAffected)
        {
            if (added != null)
                added.Invalidate();
            else
                Invalidate();
        }
        internal void RenderRecursive(DrawingContext dc)
        {
            if (!visible || isSuspended)
                return;

            dc.PushClippingRectangle(ScreenArea);
            if (!dc.ClippingRectangle.IsZero)
            {
                OnRender(dc);

                for (int i = 0; i < children.Count; i++)
                {
                    Control child = children[i];
                    if (child != null)
                        child.RenderRecursive(dc);
                }
            }
            dc.PopClippingRectangle();
        }
        
        //internal void RenderHierarchyLine(DrawingContext dc)
        //{
        //    RenderLineRecursive(dc, Hierarchy, 0);
        //}
        //private void RenderLineRecursive(DrawingContext dc, ArrayList hierarchy, int level)
        //{
        //    if (level <= hierarchy.Count - 1)
        //    {
        //        Control ctrl = (Control)hierarchy[level];
        //        if (level == hierarchy.Count - 1)
        //            ctrl.RenderRecursive(dc);
        //        else
        //        {
        //            if (ctrl.visible)
        //            {
        //                if (dc.ClippingRectangle.Intersects(ctrl.ScreenArea))
        //                {
        //                    dc.PushClippingRectangle(ctrl.ScreenArea);
        //                    if (!dc.ClippingRectangle.IsZero)
        //                    {
        //                        ctrl.OnRender(dc);

        //                        level++;
        //                        RenderLineRecursive(dc, hierarchy, level);
        //                    }
        //                    dc.PopClippingRectangle();
        //                }
        //            }
        //        }
        //    }
        //}

        public virtual void OnRender(DrawingContext dc)
        {
        }
        #endregion
    }
}
