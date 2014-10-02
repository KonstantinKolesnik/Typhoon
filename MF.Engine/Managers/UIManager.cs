using GHI.Premium.System;
using MF.Engine.Graphics;
using MF.Engine.GUI;
using MF.Engine.GUI.Controls;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;
using System;
using System.Threading;

namespace MF.Engine.Managers
{
    public class UIManager : MarshalByRefObject
    {
        #region Fields
        private InputManager inputManager;
        
        private Bitmap screenBuffer;
        private Form activeForm = null;
        private Thread rendererThread = null;
        //private FPSCounter fps;
        //private DateTime t1, t2;

        private int idx = 0;

        private class CalibrationPointsID { }
        private static ExtendedWeakReference ewrCalibrationPoints;
        private static CalibrationPoints calibrationPoints = null;
        #endregion

        #region Properties
        public Form ActiveForm
        {
            get { return activeForm; }
            set
            {
                if (activeForm != value)
                    activeForm = value;
            }
        }
        //public double FPS
        //{
        //    get { return fps.FPS; }
        //}
        public bool IsLCDPresent
        {
            get { return AppearanceManager.ScreenWidth != 0; }
        }
        public bool IsCalibrated
        {
            get { return calibrationPoints != null; }
        }
        #endregion

        #region Constructor
        public UIManager()
        {
            //screenBuffer = new Bitmap(AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);

            InitTouch();
            //InitRenderer();
        }
        #endregion

        #region Touch block
        private void InitTouch()
        {
            Touch.Initialize(Engine.Application);
            TouchCollectorConfiguration.CollectionMode = CollectionMode.InkAndGesture;//.GestureOnly;
            TouchCollectorConfiguration.CollectionMethod = CollectionMethod.Native;
            //TouchCollectorConfiguration.TouchMoveFrequency = 50; // in ms

            if (IsLCDPresent)
            {
                //inputManager = new InputManager();
                //inputManager.TouchDown += new OnTouchEvent(inputManager_TouchDown);
                //inputManager.TouchMove += new OnTouchEvent(inputManager_TouchMove);
                //inputManager.TouchUp += new OnTouchEvent(inputManager_TouchUp);

                LoadCalibrationPoints();
                if (calibrationPoints != null)
                    ApplyCalibrationPoints();

                //if (calibrationPoints == null)
                //    CalibrateScreen();
                //else
                //    ApplyCalibrationPoints();
            }
        }
        private void inputManager_TouchDown(Point e)
        {
            bool bIgnore = false;
            //if (Engine.desktop != null)
            //    if (Engine.desktop._mnu.InvokeTouchDown(e))
            //        bIgnore = true;

            if (!bIgnore)
            {
                //if (Engine._ActiveCM != null)
                //{
                //    if (!Engine._ActiveCM.InvokeTouchDown(e))
                //        Engine.HideContextMenu();
                //}
                //else
                if (activeForm != null)
                    activeForm.TouchDown(null, e);
                //else if (Engine.desktop != null)
                //    Engine.desktop.TouchDown(null, e);
                //else
                //    Engine.RaiseUnhandledTouch(true, e);
            }
        }
        private void inputManager_TouchMove(Point e)
        {
            //if (Engine._ActiveCM != null)
            //    Engine._ActiveCM.TouchMove(null, e);
            //else
            if (activeForm != null)
                activeForm.TouchMove(null, e);
        }
        private void inputManager_TouchUp(Point e)
        {
            bool bIgnore = false;
            //if (Engine.desktop != null)
            //    if (Engine.desktop._mnu.InvokeTouchUp(e))
            //        bIgnore = true;

            if (!bIgnore)
            {
                //if (Engine._ActiveCM != null)
                //    Engine._ActiveCM.InvokeTouchUp(e);
                //else
                if (activeForm != null)
                    activeForm.TouchUp(null, e);
                //else if (Engine.desktop != null)
                //    Engine.desktop.TouchUp(null, e);
                //else
                //    Engine.RaiseUnhandledTouch(false, e);
            }
        }
        #endregion

        #region Calibration
        private static void LoadCalibrationPoints()
        {
            ewrCalibrationPoints = ExtendedWeakReference.RecoverOrCreate(typeof(CalibrationPointsID), 0, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);
            ewrCalibrationPoints.Priority = (int)ExtendedWeakReference.PriorityLevel.System;
            calibrationPoints = (CalibrationPoints)ewrCalibrationPoints.Target;
        }
        private static void SaveCalibrationPoints()
        {
            //if (AppDomain.CurrentDomain.FriendlyName != "default")
            //    throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));

            ewrCalibrationPoints.Target = calibrationPoints;
            Util.FlushExtendedWeakReferences();
        }
        private static void PrepareCalibrationPoints()
        {
            calibrationPoints = new CalibrationPoints();

            int calibrationPointCount = 0;
            Touch.ActiveTouchPanel.GetCalibrationPointCount(ref calibrationPointCount);

            calibrationPoints.ScreenX = new short[calibrationPointCount];
            calibrationPoints.ScreenY = new short[calibrationPointCount];
            calibrationPoints.TouchX = new short[calibrationPointCount];
            calibrationPoints.TouchY = new short[calibrationPointCount];

            // Get the points for calibration.
            for (int index = 0; index < calibrationPointCount; index++)
            {
                int x = 0;
                int y = 0;
                Touch.ActiveTouchPanel.GetCalibrationPoint(index, ref x, ref y);
                calibrationPoints.ScreenX[index] = (short)x;
                calibrationPoints.ScreenY[index] = (short)y;
            }
        }
        private static void ApplyCalibrationPoints()
        {
            Touch.ActiveTouchPanel.SetCalibration(calibrationPoints.PointCount, calibrationPoints.ScreenX, calibrationPoints.ScreenY, calibrationPoints.TouchX, calibrationPoints.TouchY);
        }

        public void CalibrateScreen()
        {
            if (!DeviceManager.IsEmulator)
            {
                //myTouch = new Thread(CalibrationWork);
                //myTouch.Priority = ThreadPriority.Highest;
                //myTouch.Start();

                //ModalBlock();

                CalibrationThreadWork();
            }
        }
        private void CalibrationThreadWork()
        {
            //ManualResetEvent localBlocker = new ManualResetEvent(false);

            //isCalibrating = true;
            idx = 0;

            Form tmp = activeForm;
            activeForm = null;

            PrepareCalibrationPoints();
            Touch.ActiveTouchPanel.StartCalibration();

            inputManager.Pause();
            //rendererThread.Suspend();

            // start the calibration process.
            Point p;
            while (true)
            {
                DrawCalibrationScreen();

                p = inputManager.GetTouchPoint();
                ++idx;
                calibrationPoints.TouchX[idx - 1] = (short)p.X;
                calibrationPoints.TouchY[idx - 1] = (short)p.Y;
                p = inputManager.GetTouchPoint(); // Don't forget about the touch up

                if (idx == calibrationPoints.PointCount)
                {
                    // the last point has been reached, so set the calibration.
                    ApplyCalibrationPoints();
                    SaveCalibrationPoints();
                    //isCalibrating = false;
                    break;
                }
                Thread.Sleep(1000);
            }

            inputManager.Resume();
            activeForm = tmp;
            //if (_activeBlock != null)
            //    _activeBlock.Set();
            //rendererThread.Resume();
        }
        //private void CalibrationWork()
        //{
        //    // Remove Active Form (w/o rendering)
        //    Form prv = _ActiveForm;
        //    _ActiveForm = null;

        //    // Copy out the current buffer
        //    Bitmap buffer2 = new Bitmap(AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
        //    buffer2.DrawImage(0, 0, _buffer, 0, 0, buffer2.Width, buffer2.Height);

        //    // prepare points
        //    SettingsManager.PrepareCalibrationPoints();

        //    // tart the calibration process.
        //    TouchEventArgs e;
        //    int idx = 0;
        //    Touch.ActiveTouchPanel.StartCalibration();
        //    IM.Pause();

        //    // get Calibration Points
        //    while (true)
        //    {
        //        // draw crosshair
        //        Color background = Color.White;
        //        _buffer.DrawRectangle(Color.Black, 0, 0, 0, buffer2.Width, buffer2.Height, 0, 0, background, 0, 0, background, 0, 0, 256);
        //        DrawCrossHair(idx);
        //        _buffer.Flush();

        //        e = IM.GetTouchPoint();
        //        ++idx;
        //        SettingsManager.CalibrationPoints.TouchX[idx - 1] = (short)e.location.X;
        //        SettingsManager.CalibrationPoints.TouchY[idx - 1] = (short)e.location.Y;
        //        e = IM.GetTouchPoint(); // Don't forget about the touch up

        //        if (idx == SettingsManager.CalibrationPoints.PointCount)
        //        {
        //            // the last point has been reached, so set the calibration.
        //            SettingsManager.ApplyCalibrationPoints();
        //            SettingsManager.SaveCalibrationPoints();
        //            break;
        //        }

        //        //Thread.Sleep(1000);

        //    }

        //    // Restore Screen
        //    _ActiveForm = prv;
        //    _buffer.DrawImage(0, 0, buffer2, 0, 0, buffer2.Width, buffer2.Height);
        //    _buffer.Flush();

        //    // Restore Input State
        //    IM.Resume();
        //    if (_activeBlock != null)
        //        _activeBlock.Set();
        //}
        private void DrawCalibrationScreen()
        {
            // backgroung
            Color background = Color.White;
            screenBuffer.DrawRectangle(background, 0, 0, 0, screenBuffer.Width, screenBuffer.Height, 0, 0, background, 0, 0, background, 0, 0, Bitmap.OpacityOpaque);

            // crosshair
            int x = calibrationPoints.ScreenX[idx];
            int y = calibrationPoints.ScreenY[idx];

            screenBuffer.DrawLine(Colors.Red, 1, x - 10, y, x - 2, y);
            screenBuffer.DrawLine(Colors.Red, 1, x + 10, y, x + 2, y);
            screenBuffer.DrawLine(Colors.Red, 1, x, y - 10, x, y - 2);
            screenBuffer.DrawLine(Colors.Red, 1, x, y + 10, x, y + 2);

            screenBuffer.Flush();
        }
        #endregion

        #region Renderer block
        private void InitRenderer()
        {
            //fps = new FPSCounter();
            //fps.Interval = 2000;

            rendererThread = new Thread(RendererWork);
            //rendererThread.Priority = ThreadPriority.Highest;
            rendererThread.Start();
        }
        private void RendererWork()
        {
            while (true)
            {
                //fps.Update();
                if (activeForm != null)
                {
                    //Update();
                    //Prerender();
                    Render();
                }
            }
        }
        private void Update()
        {
            //activeForm.Update();
        }
        private void Prerender()
        {
            //activeForm.Prerender();
        }
        private void Render()
        {
            screenBuffer.Clear();
            //screenBuffer.MakeTransparent(screenBuffer.GetPixel(0, 0));
            //t1 = DateTime.Now;
            activeForm.Render(ref screenBuffer);
            screenBuffer.Flush();
            //t2 = DateTime.Now;

            //Thread.Sleep(5);
            //Thread.Sleep(40);
            //Thread.Sleep(100);

            //Thread.Sleep(250);
        }
        #endregion
    }
}
