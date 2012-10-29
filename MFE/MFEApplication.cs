using System;
using System.Threading;
using GHIElectronics.NETMF.System;
using MFE.Device;

namespace MFE
{
    public class MFEApplication
    {
        #region Constructor
        public MFEApplication()
        {
            if (DeviceManager.IsEmulator)
                NormalBoot();
            else
            {
                switch (SystemUpdate.GetMode())
                {
                    case SystemUpdate.SystemUpdateMode.NonFormatted:
                        SystemUpdate.EnableBootloader();
                        break;
                    case SystemUpdate.SystemUpdateMode.Bootloader:
                        throw new Exception("Invalid Boot Mode!");
                    case SystemUpdate.SystemUpdateMode.Application:
                        NormalBoot();
                        break;
                }
            }
        }
        #endregion

        #region Private methods
        private void NormalBoot()
        {
            Run();
            Thread.Sleep(Timeout.Infinite);
        }
        #endregion

        #region Protected methods
        protected virtual void Run()
        {
        }
        #endregion
    }
}
