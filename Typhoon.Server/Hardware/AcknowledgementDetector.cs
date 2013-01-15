using Microsoft.SPOT.Hardware;

namespace Typhoon.Server.Hardware
{
    public class AcknowledgementDetector
    {
        #region Fields
        private AnalogInput portSense;

        //private bool isProgPhase = false;
        //private bool isAckDetected = false;

        #endregion

        #region Constructor
        public AcknowledgementDetector(Cpu.AnalogChannel sensorPin)
        {
            portSense = new AnalogInput(sensorPin);
            portSense.Scale = 3300;
        }
        #endregion
    }
}
