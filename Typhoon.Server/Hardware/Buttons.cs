using System;
using GHI.Premium.Hardware;
using Microsoft.SPOT.Hardware;
using Typhoon.DCC;

namespace Typhoon.Server.Hardware
{
    class Buttons
    {
        private InterruptPort btnUp, btnSelect, btnDown;
        
        public Buttons()
        {
            btnUp = new InterruptPort(EMX.Pin.IO4, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnUp.OnInterrupt += new NativeEventHandler(btnUp_OnInterrupt);

            btnSelect = new InterruptPort(EMX.Pin.IO30, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnSelect.OnInterrupt += new NativeEventHandler(btnSelect_OnInterrupt);

            btnDown = new InterruptPort(EMX.Pin.IO0, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnDown.OnInterrupt += new NativeEventHandler(btnDown_OnInterrupt);

            SetAddresses();
       }

        private void btnUp_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            FF();
            //acc0_0_On();

            Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private void btnSelect_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            SS();


            Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private void btnDown_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            RR();
            //acc0_0_Off();

            Beeper.PlaySound(Beeper.SoundID.Click);
        }

        #region speed

        private int speed = 0;
        private bool forward = true;

        private LocomotiveAddress[] addresses = new LocomotiveAddress[2];

        private void SetAddresses()
        {
            addresses[0] = new LocomotiveAddress(7, false);
            addresses[1] = new LocomotiveAddress(3, false);
        }
        private void FF()
        {
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed++;
            speed = Math.Min(speed, 28);
            forward = speed > 0;
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, Math.Abs(speed), forward));
        }
        private void SS()
        {
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed = 0;
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, speed, forward));
        }
        private void RR()
        {
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed--;
            speed = Math.Max(speed, -28);
            forward = speed > 0;
            foreach (LocomotiveAddress address in addresses)
                Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, Math.Abs(speed), forward));
        }

        #endregion

        #region acc
        
        ushort decoderAddress = 1;
        byte outputNumber = 0;
        byte coilNumber = 0;

        private void acc0_0_On()
        {
            //Model.MainBooster.AddCommand(DCCCommand.BasicAccessory(decoderAddress, outputNumber, coilNumber, true));
            Model.MainBooster.AddCommand(DCCCommand.BasicAccessory(1, coilNumber, true));

            //Model.MainBooster.AddCommand(DCCCommand.ExtendedAccessory(decoderAddress, 0));
        }
        private void acc0_0_Off()
        {
            //Model.MainBooster.AddCommand(DCCCommand.BasicAccessory(decoderAddress, outputNumber, coilNumber, false));
            Model.MainBooster.AddCommand(DCCCommand.BasicAccessory(1, coilNumber, false));

            //Model.MainBooster.AddCommand(DCCCommand.ExtendedAccessory(decoderAddress, 1));
        }

        #endregion
    }
}
