using GHIElectronics.NETMF.FEZ;

namespace Typhoon.Server.Hardware
{
    static class HardwareConfiguration
    {
        public const short SenseResistor = 680; // in milliOhms

        public const FEZ_Pin.Digital PinMainBoosterEnable = FEZ_Pin.Digital.IO20;
        public const FEZ_Pin.Digital PinMainBoosterOverloadLED = FEZ_Pin.Digital.IO21;
        public const FEZ_Pin.AnalogIn PinMainBoosterSense = FEZ_Pin.AnalogIn.AD6;
        public const FEZ_Pin.Digital PinMainOutputGenerator = FEZ_Pin.Digital.IO16;

        public const FEZ_Pin.Digital PinProgBoosterEnable = FEZ_Pin.Digital.IO22;
        public const FEZ_Pin.Digital PinProgBoosterOverloadLED = FEZ_Pin.Digital.IO23;
        public const FEZ_Pin.AnalogIn PinProgBoosterSense = FEZ_Pin.AnalogIn.AD5;
        public const FEZ_Pin.Digital PinProgOutputGenerator = FEZ_Pin.Digital.IO17;

        public const FEZ_Pin.PWM PinNetworkLED = FEZ_Pin.PWM.PWM1;


    }
}
