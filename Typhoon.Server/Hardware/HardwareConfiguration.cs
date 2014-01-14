using GHI.Hardware.EMX;
using Microsoft.SPOT.Hardware;

namespace Typhoon.Server.Hardware
{
    static class HardwareConfiguration
    {
        public const short SenseResistor = 680; // in milliOhms

        public const Cpu.Pin PinMainBoosterEnable = Pin.IO18;
        public const Cpu.Pin PinMainBoosterEnableLED = Pin.IO20;
        public const Cpu.Pin PinMainBoosterOverloadLED = Pin.IO21;
        public const Cpu.AnalogChannel PinMainBoosterSense = Cpu.AnalogChannel.ANALOG_6;
        public const Cpu.Pin PinMainOutputGenerator = Pin.IO16;

        public const Cpu.Pin PinProgBoosterEnable = Pin.IO19;
        public const Cpu.Pin PinProgBoosterEnableLED = Pin.IO22;
        public const Cpu.Pin PinProgBoosterOverloadLED = Pin.IO23;
        public const Cpu.AnalogChannel PinProgBoosterSense = Cpu.AnalogChannel.ANALOG_5;
        public const Cpu.Pin PinProgOutputGenerator = Pin.IO17;

        public const Cpu.PWMChannel PinNetworkLED = Cpu.PWMChannel.PWM_1;

        public const Cpu.AnalogChannel PinAcknowledgementSense = Cpu.AnalogChannel.ANALOG_2;
    }
}
