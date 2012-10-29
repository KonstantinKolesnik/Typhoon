using System;
using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;
using MFE.Graphics.Media;
using MFE.Graphics.Touching;
using MFE.LCD;
using Microsoft.SPOT;

namespace MFE.Graphics
{
    public static class GraphicsManager
    {
        #region Fields
        private static Bitmap screen;
        internal static Desktop Desktop;
        internal static Window modalWindow = null;

        private static Control lastEventTarget = null;

        public static DateTime dt0;
        public static TimeSpan ts;
        #endregion

        #region Public methods
        public static void Initialize()
        {
            if (!LCDManager.IsScreenAvailable)
                throw new Exception("No LCD screen available!");

            TouchManager.TouchDown += new TouchEventHandler(TouchManager_TouchDown);
            TouchManager.TouchMove += new TouchEventHandler(TouchManager_TouchMove);
            TouchManager.TouchUp += new TouchEventHandler(TouchManager_TouchUp);
            TouchManager.TouchGestureStarted += new TouchGestureEventHandler(TouchManager_TouchGestureStarted);
            TouchManager.TouchGestureChanged += new TouchGestureEventHandler(TouchManager_TouchGestureChanged);
            TouchManager.TouchGestureEnded += new TouchGestureEventHandler(TouchManager_TouchGestureEnded);
            TouchManager.Initialize();

            screen = new Bitmap(LCDManager.ScreenWidth, LCDManager.ScreenHeight);
            Desktop = new Desktop();
            Desktop.Invalidate();

            if (CalibrationManager.IsCalibrated)
                CalibrationManager.ApplyCalibrationPoints();
        }
        #endregion

        #region Touch event handlers
        private static void TouchManager_TouchDown(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchDownEvent(e);
        }
        private static void TouchManager_TouchMove(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchMoveEvent(e);
        }
        private static void TouchManager_TouchUp(object sender, TouchEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchUpEvent(e);
        }
        private static void TouchManager_TouchGestureStarted(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureStartedEvent(e);
        }
        private static void TouchManager_TouchGestureChanged(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureChangedEvent(e);
        }
        private static void TouchManager_TouchGestureEnded(object sender, TouchGestureEventArgs e)
        {
            Control touchTarget = GetTouchTarget(e.Point);
            if (touchTarget != null)
                touchTarget.RaiseTouchGestureEndedEvent(e);
        }
        #endregion

        #region Private methods
        private static Control GetTouchTarget(Point p)
        {
            Control res = null;
            //dt0 = DateTime.Now;
            if (TouchCapture.Captured != null)
                res = TouchCapture.Captured;
            else if (modalWindow != null)
                res = modalWindow is CalibrationWindow ? modalWindow : FindTouchTarget(modalWindow, p);
            else
                res = FindTouchTarget(Desktop, p);
            lastEventTarget = res;
            //ts = DateTime.Now - dt0;
            return res;
        }
        private static Control FindTouchTarget(Control root, Point p)
        {
            if (lastEventTarget != null)
            {
                Control target = lastEventTarget.TouchableChildFromScreenPoint(p);
                if (target != null)
                    return target;
                
                Control par = lastEventTarget.TouchableParentFromScreenPoint(p);
                return par.TouchableChildFromScreenPoint(p);
            }
            else
                return root.TouchableChildFromScreenPoint(p);
        }

        internal static void PostRenderTask(RenderTask task)
        {
            if (Desktop == null)
                return;

            Rect dirtyRect = Desktop.ScreenArea;
            if (task != null)
            {
                dirtyRect = task.DirtyArea.Intersection(dirtyRect);
                if (dirtyRect.IsZero)
                    return;
            }

            //Debug.Print(task.Control.ToString() + ": " + dirtyRect.ToString());

            DrawingContext dc = new DrawingContext(screen);
            dt0 = DateTime.Now;
            dc.PushClippingRectangle(dirtyRect);
            if (!dc.ClippingRectangle.IsZero)
                Desktop.RenderRecursive(dc);
            dc.PopClippingRectangle();
            dc.Close();
            ts = DateTime.Now - dt0;
            screen.Flush(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height);


            Rect dirtyRect2 = new Rect(0, 0, 300, 20);
            dc = new DrawingContext(screen);
            dc.PushClippingRectangle(dirtyRect2);
            Font font = Resources.GetFont(Resources.FontResources.Regular);
            dc.DrawRectangle(new SolidColorBrush(Color.White), null, 0, 0, 300, 20);
            dc.DrawText(dirtyRect.ToString() + "; Render: " + ts.ToString(), font, Color.Red, 3, 3);
            dc.PopClippingRectangle();
            dc.Close();
            screen.Flush(dirtyRect2.X, dirtyRect2.Y, dirtyRect2.Width, dirtyRect2.Height);

            //Debug.Print("Render task: " + ts.ToString());
        }
        #endregion
    }
}

/*
Bitmap Clear, 800x480: 00:00:00.0120309
Bitmap Flush, 800x480: 00:00:00.0371465
Bitmap GetPixel, 800x480: 00:00:00.0003250
Bitmap SetPixel, 800x480: 00:00:00.0003901
Bitmap Blend a=256, 800x480: 00:00:00.0399479
Bitmap Blend a=127, 800x480: 00:00:01.2195883
Bitmap Blend a=10, 800x480: 00:00:01.2195683
Bitmap Blend a=0, 800x480: 00:00:00.0006304
Bitmap DrawImage 1, 800x480: 00:00:00.0400499
Bitmap DrawImage 2, 800x480: 00:00:00.0403511
Bitmap DrawImage 2, 200x200, center: 00:00:00.0046052
Bitmap DrawImage 2, 20x20, center: 00:00:00.0006930
Bitmap DrawRectangle, 800x480: 00:00:00.1247743
new Bitmap: 00:00:00.0117430
Bitmap Dispose: 00:00:00.0003555
*/
