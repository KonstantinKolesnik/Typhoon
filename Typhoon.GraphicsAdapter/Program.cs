using System.Collections;
using System.Reflection;
using GHIElectronics.NETMF.System;
using MFE;
using MFE.Graphics;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Typhoon.GraphicsAdapter.UI;
using Typhoon.Server;

namespace Typhoon.GraphicsAdapter
{
    public class Program : MFEApplication
    {
        public static void Main()
        {
            Instance = new Program();
        }

        #region Fields
        public static Program Instance;

        private static Hashtable bitmaps = new Hashtable();
        private static Hashtable fonts = new Hashtable();

        #endregion

        #region Properties
        public static AssemblyName Info
        {
            get { return Assembly.GetExecutingAssembly().GetName(); }
        }
        //Reflection.GetAssemblyMemoryInfo(Assembly.GetExecutingAssembly(), );

        internal static Bitmap Bitmaps(Resources.BitmapResources id)
        {
            if (!bitmaps.Contains(id))
                bitmaps.Add(id, Resources.GetBitmap(id));
            else if ((Bitmap)bitmaps[id] == null)
                bitmaps[id] = Resources.GetBitmap(id);

            return (Bitmap)bitmaps[id];
        }
        internal static Font Fonts(Resources.FontResources id)
        {
            if (!fonts.Contains(id))
                fonts.Add(id, Resources.GetFont(id));
            else if ((Font)bitmaps[id] == null)
                fonts[id] = Resources.GetFont(id);

            return (Font)fonts[id];
        }


        public static Color LabelTextColor
        {
            get { return Color.LightGray; }
        }
        public static Color ButtonTextColor
        {
            get { return Color.White; }
        }

        public static Bitmap Background
        {
            //get { return Resources.GetBitmap(Resources.BitmapResources.Background_800_600); }
            get { return Resources.GetBitmap(Resources.BitmapResources.Train_800_480); }
        }
        public static Bitmap ToolbarBackground
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Bar); }
        }
        public static Bitmap PanelBackground
        {
            get
            {
                Bitmap bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, (Microsoft.SPOT.Presentation.Media.Color)Color.Black);
                return bmp;
            }
        }
        public static Bitmap ButtonBackground
        {
            get
            {
                Bitmap bmp = Resources.GetBitmap(Resources.BitmapResources.ButtonBackground);
                //bmp.MakeTransparent(bmp.GetPixel(0, 0));
                return bmp;
            }
        }

        public static ushort PanelOpacity
        {
            get { return 80; }
        }
        public static ushort ToolbarOpacity
        {
            get { return 255; }//180
        }

        public static int MessageBoxWidth
        {
            get { return 180; }
        }

        public static int ButtonIconSize
        {
            get { return 16; }
        }
        public static int ButtonLargeIconSize
        {
            get { return 32; }
        }
        public static int ShortcutIconSize
        {
            get { return 128; }
        }

        public static Font FontRegular
        {
            get { return Resources.GetFont(Resources.FontResources.LucidaSansUnicode_8); }
        }
        public static Font FontCourierNew9
        {
            get { return Resources.GetFont(Resources.FontResources.CourierNew_9); }
        }
        public static Font FontCourierNew10
        {
            get { return Resources.GetFont(Resources.FontResources.CourierNew_10); }
        }
        public static Font FontCourierNew12
        {
            get { return Resources.GetFont(Resources.FontResources.CourierNew_12); }
        }

        public static Bitmap ImageHome
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Home); }
        }

        public static Bitmap ImageLedGreen
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.LedGreen); }
        }
        public static Bitmap ImageLedRed
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.LedRed); }
        }
        public static Bitmap ImageLedGray
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.LedGray); }
        }

        public static Bitmap ImagePowerOn
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.PowerOn); }
        }
        public static Bitmap ImagePowerOff
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.PowerOff); }
        }

        public static Bitmap ImageNetworkOn
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.NetworkOn); }
        }
        public static Bitmap ImageNetworkOff
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.NetworkOff); }
        }

        public static Bitmap ImageDrive
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Drive); }
        }
        public static Bitmap ImageMouse
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Mouse); }
        }
        public static Bitmap ImageKeyboard
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Keyboard); }
        }

        public static Bitmap ImageDatabase
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Database); }
        }
        public static Bitmap ImageOperation
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Operation); }
        }
        public static Bitmap ImageLayout
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Layout); }
        }
        public static Bitmap ImageSettings
        {
            get { return Resources.GetBitmap(Resources.BitmapResources.Settings); }
        }
        #endregion

        #region Public methods
        //public static void DoEvents()
        //{
        //    DispatcherFrame frame = new DispatcherFrame();
        //    Dispatcher.CurrentDispatcher.BeginInvoke(ExitFrameHandler, frame);
        //    Dispatcher.PushFrame(frame);
        //}
        //private static object ExitFrameHandler(object f)
        //{
        //    ((DispatcherFrame)f).Continue = false;
        //    return null;
        //}
        #endregion

        #region Application entry point
        protected override void Run()
        {
            bool reboot = false;
            if (LCDManager.IsScreenAvailable)
            {
                reboot |= LCDManager.SetLCDConfiguration_800_480();
                reboot |= LCDManager.SetBootLogo(null, 0, 0); //Resources.GetBitmap(Resources.BitmapResources.Train_800_480))
                reboot |= LCDManager.SetLCDBootupMessages(true);

                if (reboot)
                {
                    Util.FlushExtendedWeakReferences();
                    PowerState.RebootDevice(false);
                }

                GraphicsManager.Initialize();
            }
            else
            {
                reboot |= LCDManager.SetLCDConfiguration_None();

                if (reboot)
                {
                    Util.FlushExtendedWeakReferences();
                    PowerState.RebootDevice(false);
                }
            }

            //if (!DeviceManager.IsEmulator)
            //{
            //    MyNetwork.AutoConnect = SettingsManager.BootSettings.ConnectionType;

            //    if (SettingsManager.Boot.EnableRTC)
            //        SetDateTime(Utils.GetNetworkTime());
            //     Restore Time Zone
            //    TimeService.SetTimeZoneOffset(GetTimeZoneMinutes(SettingsManager.Boot.TimeZone));
            //}
            
            
            
            
            Model model = new Model();

            //if (LCDManager.IsScreenAvailable)
            //{
            //    if (!CalibrationManager.IsCalibrated)
            //    {
            //        CalibrationWindow winCal = new CalibrationWindow();
            //        winCal.Background = new LinearGradientBrush(Color.Blue, Color.Black);
            //        winCal.CrosshairPen = new Pen(Color.LimeGreen, 1);

            //        TextBlock text = new TextBlock(0, winCal.Height / 4, winCal.Width, 40, Resources.GetFont(Resources.FontResources.CourierNew_10), "Please tap the crosshairs to calibrate the screen");
            //        text.ForeColor = Color.White;
            //        text.TextAlignment = TextAlignment.Center;
            //        text.TextVerticalAlignment = VerticalAlignment.Center;
            //        text.TextWrap = true;
            //        winCal.Children.Add(text);

            //        winCal.ShowModal();
            //    }

            //    MainWindow wndMain = new MainWindow();
            //    wndMain.Show();
            //}
        }
        #endregion

        #region Private methods
        //private static void BootScreen()
        //{
        //    int opacity = 0;
        //    int iInc = 12;

        //    //Bitmap bmpText = new Bitmap(Resources.GetBytes(Resources.BinaryResources.boot1), Bitmap.BitmapImageType.Jpeg);
        //    Bitmap bmpLogo = Resources.GetBitmap(Resources.BitmapResources.Operation);
        //    Bitmap bmp = new Bitmap(SystemMetrics.ScreenWidth, SystemMetrics.ScreenHeight);

        //    Color bkg = ColorUtility.ColorFromRGB(226, 223, 219);
        //    //bmp.DrawRectangle(Colors.Black, 0, 0, 0, bmp.Width, bmp.Height, 0, 0, bkg, 0, 0, bkg, 0, 0, 256);
        //    //bmp.DrawImage(bmp.Width / 2 - bmpText.Width / 2, bmp.Height / 2 - bmpText.Height / 2, bmpText, 0, 0, bmpText.Width, bmpText.Height);

        //    bmpLogo.MakeTransparent(bmpLogo.GetPixel(0, 0));
        //    bmp.DrawImage(bmp.Width / 2 - bmpLogo.Width / 2, bmp.Height / 2 - bmpLogo.Height / 2, bmpLogo, 0, 0, bmpLogo.Width, bmpLogo.Height, (ushort)255);
        //    bmp.Flush();
        //    Thread.Sleep(20);
        //    return;

        //    while (true)
        //    {
        //        //bmp.DrawImage(bmp.Width / 2 - bmpText.Width / 2, bmp.Height / 2 - bmpText.Height / 2, bmpText, 0, 0, bmpText.Width, bmpText.Height);
        //        bmp.DrawImage(bmp.Width / 2 - bmpLogo.Width / 2, bmp.Height / 2 - bmpLogo.Height / 2, bmpLogo, 0, 0, bmpLogo.Width, bmpLogo.Height, (ushort)opacity);
        //        bmp.Flush();

        //        opacity += iInc;
        //        if (opacity >= 256)
        //        {
        //            opacity = 255;
        //            iInc = -iInc;
        //        }
        //        else if (opacity <= 0)
        //        {
        //            opacity = 1;
        //            iInc = -iInc;
        //        }

        //        Thread.Sleep(20);
        //    }
        //}
        //private static void PrepBootScreen()
        //{
        //    //int i = 0;
        //    //int xx = (int)(((float)AppearanceManager.ScreenWidth / 2) - 58);
        //    //int x = xx;
        //    //int y = AppearanceManager.ScreenHeight - 22;
        //    //Bitmap bmpL0 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.load0), Bitmap.BitmapImageType.Jpeg);
        //    //Bitmap bmpL1 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.load1), Bitmap.BitmapImageType.Jpeg);
        //    //Bitmap bmpL2 = new Bitmap(Resources.GetBytes(Resources.BinaryResources.load2), Bitmap.BitmapImageType.Jpeg);
        //    //Bitmap bmp = new Bitmap(AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);

        //    //switch (AppearanceManager.ScreenWidth)
        //    //{
        //    //    case 320:
        //    //        bmp.DrawImage(-80, -26, new Bitmap(Resources.GetBytes(Resources.BinaryResources.boot), Bitmap.BitmapImageType.Jpeg), 0, 0, 480, 272);
        //    //        break;
        //    //    case 480:
        //    //        bmp.DrawImage(0, 0, new Bitmap(Resources.GetBytes(Resources.BinaryResources.boot), Bitmap.BitmapImageType.Jpeg), 0, 0, 480, 272);
        //    //        break;
        //    //    case 800:
        //    //        break;
        //    //}

        //    //Font fnt = Resources.GetFont(Resources.FontResources.tahoma11);
        //    //bmp.DrawTextInRect("Preparing In-Field Update Region", 0, y - fnt.Height - 4, AppearanceManager.ScreenWidth, fnt.Height, Bitmap.DT_AlignmentCenter, Colors.Black, fnt);

        //    //while (true)
        //    //{
        //    //    x = xx;

        //    //    bmp.DrawImage(x, y, (i == 1 || i == 3) ? bmpL1 : (i == 2) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 2 || i == 4) ? bmpL1 : (i == 3) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 3 || i == 5) ? bmpL1 : (i == 4) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 4 || i == 6) ? bmpL1 : (i == 5) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 5 || i == 7) ? bmpL1 : (i == 6) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 6 || i == 8) ? bmpL1 : (i == 7) ? bmpL2 : bmpL0, 0, 0, 14, 14);
        //    //    x += 17;

        //    //    bmp.DrawImage(x, y, (i == 7 || i == 9) ? bmpL1 : (i == 8) ? bmpL2 : bmpL0, 0, 0, 14, 14);

        //    //    i++;
        //    //    if (i > 9) i = 0;
        //    //    bmp.Flush();
        //    //    Thread.Sleep(150);
        //    //}
        //}
        #endregion
    }
}
