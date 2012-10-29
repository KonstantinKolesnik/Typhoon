using GHIElectronics.NETMF.System;
using MFE;
using MFE.LCD;
using Microsoft.SPOT.Hardware;
using Typhoon.Server;

namespace Typhoon.Adapter
{
    public class Program : MFEApplication
    {
        public static void Main()
        {
            Instance = new Program();
        }

        #region Fields
        public static Program Instance;
        #endregion

        #region Application entry point
        protected override void Run()
        {
            bool reboot = false;
            reboot |= LCDManager.SetLCDConfiguration_None();

            if (reboot)
            {
                Util.FlushExtendedWeakReferences();
                PowerState.RebootDevice(false);
            }

            Model model = new Model();
        }
        #endregion
    }
}
