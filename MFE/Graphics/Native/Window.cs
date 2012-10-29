using System.Runtime.CompilerServices;
using MFE.Graphics.Native.Controls;
using MSMedia = Microsoft.SPOT.Presentation.Media;
using MSPresentation = Microsoft.SPOT.Presentation;

namespace MFE.Graphics.Native
{
    public class Window : ContentControl
    {
        #region Private Fields
        private MSPresentation.SizeToContent _sizeToContent;
        private WindowManager _windowManager;
        #endregion

        #region Public Properties
        /// <summary>
        /// Auto size Window to its content's size
        /// </summary>
        /// <remarks>
        /// 1. SizeToContent can be applied to Width Height independently
        /// 2. After SizeToContent is set, setting Width/Height does not take affect if that
        ///    dimension is sizing to content.
        /// </remarks>
        /// <value>
        /// Default value is SizeToContent.Manual
        /// </value>
        public MSPresentation.SizeToContent SizeToContent
        {
            get { return _sizeToContent; }
            set { _sizeToContent = value; }
        }
        public int Top
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }
        public int Left
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }
        public bool Topmost
        {
            get { return _windowManager.IsTopMost(this); }
            set { _windowManager.SetTopMost(this); }
        }
        #endregion

        #region Constructors
        /// <summary>
        ///     Constructs a window object
        /// </summary>
        /// <remarks>
        ///     Automatic determination of current Dispatcher. Use alternative constructor
        ///     that accepts a Dispatcher for best performance.
        /// REFACTOR -- consider specifying app default window sizes to cover Aux case for default window size.
        /// </remarks>
        ///     Initializes the Width/Height, Top/Left properties to use windows
        ///     default. Updates Application object properties if inside app.
        public Window()
        {
            //There is only one WindowManager. All Windows currently are forced to be created
            //and to live on the same thread.
            _windowManager = WindowManager.EnsureInstance();

            _background = new MSMedia.SolidColorBrush(MSMedia.Colors.White);
            //
            // dependency property initialization.
            // we don't have them, so we just update the properties on the base class,
            // like normal *bleep* fearing developers.
            //
            // Visibility HAS to be set to Collapsed prior to adding this child to the
            // window manager, otherwise the window manager sets the focus to this window
            Visibility = MSPresentation.Visibility.Collapsed;

            // register us with the window manager, like a good little boy
            _windowManager.Children.Add(this);

            //Application app = Application.Current;
            //// check if within an app && on the same thread
            //if (app != null)
            //{
            //    if (app.Dispatcher.Thread == Dispatcher.CurrentDispatcher.Thread)
            //    {
            //        // add to window collection
            //        // use internal version since we want to update the underlying collection
            //        app.WindowsInternal.Add(this);
            //        if (app.MainWindow == null)
            //            app.MainWindow = this;
            //    }
            //    else
            //        app.NonAppWindowsInternal.Add(this);
            //}
        }
        #endregion

        #region Public Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close()
        {
            //Application app = Application.Current;
            //if (app != null)
            //{
            //    app.WindowsInternal.Remove(this);
            //    app.NonAppWindowsInternal.Remove(this);
            //}
            if (_windowManager != null)
            {
                _windowManager.Children.Remove(this);
                _windowManager = null;
            }
        }
        #endregion

        #region Protected Methods
        // REFACTOR -- need to track if our parent changes.

        /// <summary>
        ///     Measurement override. Implements content sizing logic.
        /// </summary>
        /// <remarks>
        ///     Deducts the frame size from the constraint and then passes it on
        ///     to it's child.  Only supports one Visual child (just like control)
        /// </remarks>
        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            UIElementCollection children = LogicalChildren;
            if (children.Count > 0)
            {
                UIElement child = (UIElement)children[0];
                if (child != null)
                {
                    // REFACTOR --we need to subtract the frame & chrome around the visual child.
                    child.Measure(availableWidth, availableHeight);
                    child.GetDesiredSize(out desiredWidth, out desiredHeight);
                    return;
                }
            }

            desiredWidth = availableWidth;
            desiredHeight = availableHeight;
        }

        /// <summary>
        ///     ArrangeOverride allows for the customization of the positioning of children.
        /// </summary>
        /// <remarks>
        ///     Deducts the frame size of the window from the constraint and then
        ///     arranges it's child.  Supports only one child.
        /// </remarks>
        protected override void ArrangeOverride(int arrangeWidth, int arrangeHeight)
        {
            UIElementCollection children = LogicalChildren;
            if (children.Count > 0)
            {
                UIElement child = children[0] as UIElement;
                if (child != null)
                    child.Arrange(0, 0, arrangeWidth, arrangeHeight);
            }
        }
        #endregion
    }
}
