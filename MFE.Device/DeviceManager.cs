using Microsoft.SPOT.Hardware;

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
                        case (byte)DeviceType.USBizi_100: return DeviceType.USBizi_100;
                        case (byte)DeviceType.USBizi_144: return DeviceType.USBizi_144;
                        case (byte)DeviceType.EmbeddedMasterModule_NonTFT: return DeviceType.EmbeddedMasterModule_NonTFT;
                        case (byte)DeviceType.EmbeddedMasterModule_TFT: return DeviceType.EmbeddedMasterModule_TFT;
                        case (byte)DeviceType.EMX: return DeviceType.EMX;
                        case (byte)DeviceType.ChipworkX: return DeviceType.ChipworkX;
                        case (byte)DeviceType.CANxtra: return DeviceType.CANxtra;
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
