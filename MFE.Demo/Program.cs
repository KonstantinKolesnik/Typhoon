using GHI.Premium.System;
using MFE.Core;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT.Hardware;

namespace MFE.Demo
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
            if (!Utils.IsEmulator)
            {
                bool reboot = false;
                reboot |= LCDManager.SetLCDConfiguration_800_480();
                //reboot |= LCDManager.SetLCDConfiguration_320_240();
                //reboot |= LCDManager.SetBootLogo(null, 0, 0);
                reboot |= LCDManager.SetLCDBootupMessages(true);

                if (reboot)
                {
                    Util.FlushExtendedWeakReferences();
                    PowerState.RebootDevice(false);
                }
            }

            GraphicsManager.Initialize();
            
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

                MainWindow wndMain = new MainWindow();
                wndMain.Show();

                // place your code here
            }
        }
    }
}
