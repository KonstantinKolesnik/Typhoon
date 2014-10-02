using GHI.Premium.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System;
using System.Threading;

namespace MF.Engine.Managers
{
    public static class AppearanceManager
    {
        #region Fields
        private static int screenWidth;
        private static int screenHeight;
        private static int bitsPerPixel;
        private static int orientationDeg;
        #endregion

        #region Properties
        public static int ScreenWidth
        {
            get { return screenWidth; }
        }
        public static int ScreenHeight
        {
            get { return screenHeight; }
        }
        public static int BitsPerPixel
        {
            get { return bitsPerPixel; }
        }
        public static int OrientationDegrees
        {
            get { return orientationDeg; }
        }
        #endregion

        #region Constructor
        static AppearanceManager()
        {
            HardwareProvider.HwProvider.GetLCDMetrics(out screenWidth, out screenHeight, out bitsPerPixel, out orientationDeg);
        }
        #endregion

        #region Public Methods
        public static bool SetBootLogo(Bitmap logo, int x, int y)
        {
            // Only allow default domain to set the boot logo
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                return false;

            // Don't set logo in emulator
            if (DeviceManager.IsEmulator)
                return false;

            if (logo != null)
            {
                // New settings were saved, must reboot
                // The start-up logo will take effect after the first reset
                bool res = Configuration.StartUpLogo.Set(logo.GetBitmap(), x, y);
                Thread.Sleep(1000); // to set logo bitmap
                Configuration.StartUpLogo.Enabled = true;

                return res;
                //return Configuration.StartUpLogo.Set(logo.GetBitmap(), (int)(ScreenWidth - logo.Width) / 2, (int)(ScreenHeight - logo.Height) / 2);
            }
            else
            {
                if (Configuration.StartUpLogo.Enabled)
                {
                    Configuration.StartUpLogo.Enabled = false;
                    return true;
                }
                else
                    return false;
            }
        }
        public static bool SetLCDResolution()
        {
            switch (ScreenWidth)
            {
                case 800: return SetLCDConfiguration_800_480();
                case 640: return SetLCDConfiguration_640_480();
                case 480: return SetLCDConfiguration_480_272();
                case 320: return SetLCDConfiguration_320_240();
                default: return false;
            }
        }
        public static bool SetLCDBootupMessages(bool show)
        {
            return DeviceManager.IsEmulator ? false : Configuration.LCD.EnableLCDBootupMessages(show);
        }
        public static bool SetLCDRotation(Configuration.LCD.Rotation rotation)
        {
            return DeviceManager.IsEmulator ? false : Configuration.LCD.SetRotation(rotation);
        }
        #endregion

        #region Private Methods
        private static bool SetLCDConfiguration_800_480() // 7" LCD with resolution of 800x480
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));
            if (DeviceManager.IsEmulator)
                return false;

            //if (Configuration.Heap.SetCustomHeapSize(1024 * 1024 * 4)) // 4 MB
            //    return true;

            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 800;
            lcdConfig.Height = 480;

            //lcdConfig.PriorityEnable = true;
            lcdConfig.OutputEnableIsFixed = false;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.PixelPolarity = false;

            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;

            lcdConfig.HorizontalSyncPulseWidth = 150; // For EMX
            // lcdConfig.HorizontalSyncPulseWidth = 60; // On ChipworkX, there is a limited range for the HorizontalSyncPulseWidth. Set it to 60 instead.
            lcdConfig.HorizontalBackPorch = 150;
            lcdConfig.HorizontalFrontPorch = 150;

            lcdConfig.VerticalSyncPulseWidth = 2;
            lcdConfig.VerticalBackPorch = 2;
            lcdConfig.VerticalFrontPorch = 2;

            //lcdConfig.PixelClockDivider = 4;

            return Configuration.LCD.Set(lcdConfig);
        }
        private static bool SetLCDConfiguration_640_480() // VGA display with resolution of 640x480 (actual timings for 480x480) on EMX.
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));
            if (DeviceManager.IsEmulator)
                return false;

            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 480;
            lcdConfig.Height = 480;

            //lcdConfig.PriorityEnable = true; // VGA requires high refresh rate, enable bus priority.
            lcdConfig.OutputEnableIsFixed = true;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.PixelPolarity = true;

            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;

            lcdConfig.HorizontalSyncPulseWidth = 69;
            lcdConfig.HorizontalBackPorch = 20;
            lcdConfig.HorizontalFrontPorch = 3;

            lcdConfig.VerticalSyncPulseWidth = 2;
            lcdConfig.VerticalBackPorch = 32;
            lcdConfig.VerticalFrontPorch = 11;

            //lcdConfig.PixelClockDivider = 4;

            return Configuration.LCD.Set(lcdConfig);
        }
        private static bool SetLCDConfiguration_480_272() // 4.3" LCD with resolution of 480x272 used on ChipworkX and Embedded Master development systems. Part number: LQ043T1DG01.
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));
            if (DeviceManager.IsEmulator)
                return false;

            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 480;
            lcdConfig.Height = 272;

            //lcdConfig.PriorityEnable = false;
            lcdConfig.OutputEnableIsFixed = true;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.PixelPolarity = false;

            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;

            lcdConfig.HorizontalSyncPulseWidth = 41;
            lcdConfig.HorizontalBackPorch = 2;
            lcdConfig.HorizontalFrontPorch = 2;

            lcdConfig.VerticalSyncPulseWidth = 10;
            lcdConfig.VerticalBackPorch = 2;
            lcdConfig.VerticalFrontPorch = 2;

            //lcdConfig.PixelClockDivider = 8; // for EMX
            //lcdConfig.PixelClockDivider = 4; // for ChipworkX

            return Configuration.LCD.Set(lcdConfig);
        }
        private static bool SetLCDConfiguration_320_240() // 3.5" LCD with resolution of 320x240 used on EMX development system. Part number: PT0353224T-A802. 
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));
            if (DeviceManager.IsEmulator)
                return false;

            Configuration.LCD.Configurations lcdConfig = new Configuration.LCD.Configurations();

            lcdConfig.Width = 320;
            lcdConfig.Height = 240;

            //lcdConfig.PriorityEnable = false;
            lcdConfig.OutputEnableIsFixed = true;
            lcdConfig.OutputEnablePolarity = true;
            lcdConfig.PixelPolarity = false;//true
            lcdConfig.HorizontalSyncPolarity = false;
            lcdConfig.VerticalSyncPolarity = false;

            lcdConfig.HorizontalSyncPulseWidth = 41;
            lcdConfig.HorizontalBackPorch = 27;
            lcdConfig.HorizontalFrontPorch = 51;

            lcdConfig.VerticalSyncPulseWidth = 10;
            lcdConfig.VerticalBackPorch = 8;
            lcdConfig.VerticalFrontPorch = 16;

            //lcdConfig.PixelClockDivider = 8;

            return Configuration.LCD.Set(lcdConfig);
        }
        #endregion
    }
}
