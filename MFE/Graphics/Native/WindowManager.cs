using MFE.Graphics.Native.Controls;
using MFE.Graphics.Native.Media;
using MSMedia = Microsoft.SPOT.Presentation.Media;
using MSPresentation = Microsoft.SPOT.Presentation;

namespace MFE.Graphics.Native
{
    public delegate void PostRenderEventHandler(DrawingContext dc);

    public class WindowManager : Canvas
    {
        public static WindowManager Instance;
        private PostRenderEventHandler _postRenderHandler;

        public event PostRenderEventHandler PostRender
        {
            add { _postRenderHandler += value; }
            remove { _postRenderHandler -= value; }
        }

        private WindowManager()
        {
            Instance = this;

            // WindowManagers have no parents but they are Visible.
            _flags = _flags | Flags.IsVisibleCache;

            Measure(MSMedia.Constants.MaxExtent, MSMedia.Constants.MaxExtent);

            int desiredWidth, desiredHeight;
            GetDesiredSize(out desiredWidth, out desiredHeight);

            Arrange(0, 0, desiredWidth, desiredHeight);
        }

        internal static WindowManager EnsureInstance()
        {
            if (Instance == null)
            {
                WindowManager wm = new WindowManager();
                // implicitly the window manager is responsible for posting renders
                wm._flags |= Flags.ShouldPostRender;
            }

            return Instance;
        }
        internal void SetTopMost(Window window)
        {
            UIElementCollection children = LogicalChildren;
            if (!IsTopMost(window))
            {
                children.Remove(window);
                children.Add(window);
            }
        }
        internal bool IsTopMost(Window window)
        {
            int index = LogicalChildren.IndexOf(window);
            return (index >= 0 && index == LogicalChildren.Count - 1);
        }

        // this was added for aux, behavior needs to change for watch.
        protected internal override void OnChildrenChanged(UIElement added, UIElement removed, int indexAffected)
        {
            base.OnChildrenChanged(added, removed, indexAffected);

            UIElementCollection children = LogicalChildren;
            int last = children.Count - 1;

            // something was added, and it's the topmost. Make sure it is visible before setting focus
            if (added != null && indexAffected == last && added.Visibility == MSPresentation.Visibility.Visible)
            {
                //Microsoft.SPOT.Input.Buttons.Focus(added);
                //Microsoft.SPOT.Input.TouchCapture.Capture(added);
            }

            // something was removed and it lost focus to us.
            if (removed != null)// && IsFocused)
            {
                // we still have a window left, so make it focused.
                if (last >= 0)
                {
                    //Microsoft.SPOT.Input.Buttons.Focus(children[last]);
                    //Microsoft.SPOT.Input.TouchCapture.Capture(children[last]);
                }
            }
        }
        protected internal override void RenderRecursive(DrawingContext dc)
        {
            base.RenderRecursive(dc);

            if (_postRenderHandler != null)
                _postRenderHandler(dc);
        }
        
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            base.MeasureOverride(availableWidth, availableHeight, out desiredWidth, out desiredHeight);
            desiredWidth = LCDManager.ScreenWidth;
            desiredHeight = LCDManager.ScreenHeight;
        }
    }
}
