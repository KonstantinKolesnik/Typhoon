using System;
using System.Runtime.CompilerServices;
using System.Threading;
using MF.Engine.GUI;
using Microsoft.SPOT;
using Microsoft.SPOT.Touch;

namespace MF.Engine.Managers
{
    internal delegate void OnTouchEvent(Point e);

    internal class TouchListener : IEventListener
    {
        [MethodImplAttribute(MethodImplOptions.Synchronized)]
        public void InitializeForEventSource()
        {
        }

        public bool OnEvent(BaseEvent e)
        {
            return true;
        }
    }

    internal class InputManager : MarshalByRefObject
    {
        #region Fields
        private TouchListener touchListener;
        private Thread touchThread;
        private bool isPenDown;
        private int moveDelta = 5;
        private bool _bVirtKey = true;
        #endregion

        #region Properties
        public bool UseVirtualKeyboard
        {
            get { return _bVirtKey; }
        }
        #endregion

        #region Events
        public event OnTouchEvent TouchDown;
        public event OnTouchEvent TouchMove;
        public event OnTouchEvent TouchUp;
        #endregion

        #region Constructor
        public InputManager()
        {
            touchListener = new TouchListener();

            Touch.Initialize(touchListener);
            TouchCollectorConfiguration.CollectionMode = CollectionMode.InkAndGesture;//.GestureOnly;
            TouchCollectorConfiguration.CollectionMethod = CollectionMethod.Native;
            //TouchCollectorConfiguration.TouchMoveFrequency = 50; // in ms

            Reset();
        }
        #endregion

        #region Internal Methods
        internal Point GetTouchPoint()
        {
            int x = 0;
            int y = 0;
            Point ptLast = Point.Zero;

            while (true)
            {
                TouchCollectorConfiguration.GetLastTouchPoint(ref x, ref y);
                if (x != 1022 && x > 0 || y != 1022 && y > 0)
                {
                    // Pen down
                    if (!isPenDown)
                    {
                        // Pen Down
                        isPenDown = true;
                        ptLast = new Point(x, y);
                        return ptLast;
                    }
                }
                else
                {
                    if (isPenDown)
                    {
                        isPenDown = false;
                        return ptLast;
                    }
                }

                Thread.Sleep(0);
            }
        }
        internal void Pause()
        {
            if (touchThread != null)
                touchThread.Suspend();
        }
        internal void Reset()
        {
            if (touchThread != null)
            {
                try
                {
                    touchThread.Suspend();
                    touchThread = null;
                }
                catch (Exception) { }
            }

            touchThread = new Thread(TouchThreadWork);
            touchThread.Priority = ThreadPriority.Highest;
            touchThread.Start();
        }
        internal void Resume()
        {
            if (touchThread != null)
                touchThread.Resume();
        }
        #endregion

        #region Private Methods
        private void TouchThreadWork()
        {
            int x = 0;
            int y = 0;
            Point ptLast = Point.Zero;

            while (true)
            {
                TouchCollectorConfiguration.GetLastTouchPoint(ref x, ref y);
                if (x != 1022 && x > 0 || y != 1022 && y > 0)
                {
                    // Pen is down
                    if (isPenDown)
                    {
                        // pen still is down, so it's TouchMove
                        if (System.Math.Abs(ptLast.X - x) > moveDelta || System.Math.Abs(ptLast.Y - y) > moveDelta)
                        {
                            ptLast = new Point(x, y);
                            if (TouchMove != null)
                                TouchMove(ptLast);
                        }
                    }
                    else
                    {
                        // Pen just downed
                        isPenDown = true;
                        ptLast = new Point(x, y);
                        if (TouchDown != null)
                            TouchDown(ptLast);
                    }
                }
                else
                {
                    if (isPenDown)
                    {
                        // Pen was down
                        isPenDown = false;
                        if (TouchUp != null)
                            TouchUp(ptLast);
                    }
                }

                Thread.Sleep(0);
            }
        }
        #endregion
    }
}
