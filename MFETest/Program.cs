using GHI.Premium.System;
using MFE;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT.Hardware;

namespace MFETest
{
    public class Program : MFEApplication
    {
        public static Program Instance;

        public static void Main()
        {
            Instance = new Program();
        }

        protected override void Run()
        {
            if (LCDManager.IsScreenAvailable)
            {
                bool reboot = false;
                reboot |= LCDManager.SetLCDConfiguration_800_480();
                reboot |= LCDManager.SetBootLogo(null, 0, 0);
                reboot |= LCDManager.SetLCDBootupMessages(true);

                if (reboot)
                {
                    Util.FlushExtendedWeakReferences();
                    PowerState.RebootDevice(false);
                }

                GraphicsManager.Initialize();
            }
            
            if (LCDManager.IsScreenAvailable)
            {
                if (!CalibrationManager.IsCalibrated)
                {
                    CalibrationWindow winCal = new CalibrationWindow();
                    winCal.Background = new LinearGradientBrush(Color.Blue, Color.Black);
                    winCal.CrosshairPen = new Pen(Color.LimeGreen, 1);

                    TextBlock text = new TextBlock(0, winCal.Height / 4, winCal.Width, 40, Resources.GetFont(Resources.FontResources.CourierNew_10), "Please tap the crosshairs to calibrate the screen");
                    text.ForeColor = Color.White;
                    text.TextAlignment = TextAlignment.Center;
                    text.TextVerticalAlignment = VerticalAlignment.Center;
                    text.TextWrap = true;
                    winCal.Children.Add(text);

                    winCal.ShowModal();
                }

                wndMain wndMain = new wndMain();
                wndMain.Show();

                //Thread.Sleep(2000);

                //wndModal dlg = new wndModal(0, 0, 0, 0);
                //dlg.ShowModal();

                //int a = 0;
                //int b = a;


                //Window wnd = new Window(400, 200, 200, 200);
                //new Thread(delegate()
                //{
                //    wnd.Background.Opacity = 0;
                //    Thread.Sleep(2000);
                //    wnd.Show();
                //    for (int i = 0; i < 256; i += 40)
                //    {
                //        wnd.Background.Opacity = (ushort)i;
                //        wnd.Invalidate();
                //        //Thread.Sleep(50);
                //    }
                //    Thread.Sleep(4000);

                //    for (int i = 256; i >= 0; i -= 40)
                //    {
                //        wnd.Background.Opacity = (ushort)i;
                //        wnd.Invalidate();
                //        //Thread.Sleep(50);
                //    }
                //    wnd.Close();
                //}
                //).Start();
            }
        }
    }
}
