using MFE.Device;
using Microsoft.SPOT.Hardware;
using System.Threading;

namespace Typhoon.Server.Hardware
{
    public static class AudioPlayer
    {
        public enum SoundID
        {
            PowerOn,
            PowerOff,
            Click,
            USBDeviceInserted,
            USBDeviceRemoved,
            Overload,




        }

        #region Public methods
        public static void PlaySound(SoundID id)
        {
            switch (DeviceManager.ActiveDevice)
            {
                case DeviceType.EMX: Piezo(id); break;
                case DeviceType.ChipworkX: Audio(id); break;
            }
        }
        #endregion

        #region Private methods
        private static void Piezo(SoundID id)
        {
            switch (id)
            {
                case SoundID.PowerOn:
                    Utility.Piezo(1000, 200);
                    break;
                case SoundID.PowerOff:
                    Utility.Piezo(100, 200);
                    break;
                case SoundID.Overload:
                    Utility.Piezo(200, 1000);
                    Thread.Sleep(1500);
                    Utility.Piezo(200, 1000);
                    Thread.Sleep(1500);
                    Utility.Piezo(200, 1000);
                    break;
                case SoundID.Click:
                    Utility.Piezo(2000, 100);
                    break;
                case SoundID.USBDeviceInserted:
                    Utility.Piezo(500, 100);
                    Utility.Piezo(1000, 100);
                    break;
                case SoundID.USBDeviceRemoved:
                    Utility.Piezo(1000, 100);
                    Utility.Piezo(500, 100);
                    break;




            }
        }
        private static void Audio(SoundID id)
        {
            //if (!MP3Control.InUse())
            //{
            //    switch (id)
            //    {
            //        case ID_BALL_HIT_WALL: MP3Control.SetSource(Resources.BinaryResources.ballHitWall); break;
            //        case ID_BALL_HIT_PADDLE: MP3Control.SetSource(Resources.BinaryResources.ballHitPaddle); break;
            //        case ID_BALL_MISS: MP3Control.SetSource(Resources.BinaryResources.ballMiss); break;
            //        default: MP3Control.SetSource(Resources.BinaryResources.beep); break;
            //    }

            //    MP3Control.Play();
            //}
        }
        #endregion
    }
}
