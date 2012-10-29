using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.System;

namespace MFE.Device
{
    public static class DeviceManager
    {
        #region Properties
        public static DeviceType ActiveDevice
        {
            get
            {
                if (SystemInfo.SystemID.SKU == 3)
                    return DeviceType.Emulator;
                else
                {
                    switch (SystemInfo.SystemID.Model)
                    {
                        case (byte)SystemModelType.USBizi_100: return DeviceType.USBizi_100;
                        case (byte)SystemModelType.USBizi_144: return DeviceType.USBizi_144;
                        case (byte)SystemModelType.EmbeddedMasterModule_NonTFT: return DeviceType.EmbeddedMasterModule_NonTFT;
                        case (byte)SystemModelType.EmbeddedMasterModule_TFT: return DeviceType.EmbeddedMasterModule_TFT;
                        case (byte)SystemModelType.EMX: return DeviceType.EMX;
                        case (byte)SystemModelType.ChipworkX: return DeviceType.ChipworkX;
                        case (byte)SystemModelType.CANxtra: return DeviceType.CANxtra;
                        default: return DeviceType.Unknown;
                    }
                }
            }
        }

        public static bool IsEmulator
        {
            get { return ActiveDevice == DeviceType.Emulator; }
        }
        public static bool IsEMX
        {
            get { return ActiveDevice == DeviceType.EMX; }
        }
        public static bool IsChipworkX
        {
            get { return ActiveDevice == DeviceType.ChipworkX; }
        }
        #endregion
    }
}
