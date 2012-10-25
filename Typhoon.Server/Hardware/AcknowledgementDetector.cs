using System;
using Microsoft.SPOT;
using GHIElectronics.NETMF.Hardware;

namespace Typhoon.Server.Hardware
{
    public class AcknowledgementDetector
    {
        #region Fields
        private AnalogIn sensorPin;

        //private bool isProgPhase = false;
        //private bool isAckDetected = false;

        #endregion

        #region Constructor
        public AcknowledgementDetector(AnalogIn.Pin sensorPin)
        {
            //this.sensorPin = new AnalogIn(sensorPin);
            //this.sensorPin.SetLinearScale(0, 3300);
        }
        #endregion
    }
}
