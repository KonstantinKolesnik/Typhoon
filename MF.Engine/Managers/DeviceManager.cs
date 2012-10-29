using Microsoft.SPOT.Hardware;

namespace MF.Engine.Managers
{
    public enum DeviceType
    {
        Emulator = 0,
        Cobra = 1,
        ChipworkX = 2,
        Unknown = 99,
    }

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
                        case 5: return DeviceType.Cobra;
                        case 6: return DeviceType.ChipworkX;
                        default: return DeviceType.Unknown;
                    }
                }
            }
        }

        public static bool IsEmulator
        {
            get { return ActiveDevice == DeviceType.Emulator; }
        }
        public static bool IsCobra
        {
            get { return ActiveDevice == DeviceType.Cobra; }
        }
        public static bool IsChipworkX
        {
            get { return ActiveDevice == DeviceType.ChipworkX; }
        }
        #endregion
    }
}
