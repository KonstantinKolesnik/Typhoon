using GHI.Premium.Hardware;
using Microsoft.SPOT.Hardware;

namespace Typhoon.Server.Hardware
{
    static class HardwareConfiguration
    {
        public const short SenseResistor = 680; // in milliOhms

        public const Cpu.Pin PinMainBoosterEnable = EMX.Pin.IO18;
        public const Cpu.Pin PinMainBoosterEnableLED = EMX.Pin.IO20;
        public const Cpu.Pin PinMainBoosterOverloadLED = EMX.Pin.IO21;
        public const Cpu.AnalogChannel PinMainBoosterSense = Cpu.AnalogChannel.ANALOG_6;
        public const Cpu.Pin PinMainOutputGenerator = EMX.Pin.IO16;

        public const Cpu.Pin PinProgBoosterEnable = EMX.Pin.IO19;
        public const Cpu.Pin PinProgBoosterEnableLED = EMX.Pin.IO22;
        public const Cpu.Pin PinProgBoosterOverloadLED = EMX.Pin.IO23;
        public const Cpu.AnalogChannel PinProgBoosterSense = Cpu.AnalogChannel.ANALOG_5;
        public const Cpu.Pin PinProgOutputGenerator = EMX.Pin.IO17;

        public const Cpu.PWMChannel PinNetworkLED = Cpu.PWMChannel.PWM_1;

        public const Cpu.AnalogChannel PinAcknowledgementSense = Cpu.AnalogChannel.ANALOG_2;
    }
}
