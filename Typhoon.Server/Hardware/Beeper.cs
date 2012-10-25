using System.Threading;
using Microsoft.SPOT.Hardware;

namespace Typhoon.Server.Hardware
{
    public class Beeper
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

        public static void PlaySound(SoundID id)
        {
            //new Thread(delegate {
                switch (id)
                {
                    case SoundID.PowerOn:
                        Utility.Piezo(1000, 100);
                        break;
                    case SoundID.PowerOff:
                        Utility.Piezo(100, 100);
                        break;
                    case SoundID.Overload:
                        Utility.Piezo(200, 1000);
                        Thread.Sleep(1500);
                        Utility.Piezo(200, 1000);
                        Thread.Sleep(1500);
                        Utility.Piezo(200, 1000);
                        break;
                    case SoundID.Click:
                        Utility.Piezo(2000, 80);
                        break;
                    case SoundID.USBDeviceInserted:
                        Utility.Piezo(500, 100);
                        Utility.Piezo(1000, 100);
                        break;
                    case SoundID.USBDeviceRemoved:
                        Utility.Piezo(1000, 100);
                        Utility.Piezo(500, 100);
                        break;



                    default:
                        break;
                }
            //}) { Priority = ThreadPriority.Normal }.Start();
        }
    }
}
