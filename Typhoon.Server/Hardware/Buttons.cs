using System;
using GHIElectronics.NETMF.FEZ;
using Microsoft.SPOT.Hardware;
using Typhoon.DCC;

namespace Typhoon.Server.Hardware
{
    class Buttons
    {
        private InterruptPort btnUp, btnSelect, btnDown;
        
        public Buttons()
        {
            btnUp = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.IO4, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnUp.OnInterrupt += new NativeEventHandler(btnUp_OnInterrupt);

            btnSelect = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.IO30, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnSelect.OnInterrupt += new NativeEventHandler(btnSelect_OnInterrupt);

            btnDown = new InterruptPort((Cpu.Pin)FEZ_Pin.Interrupt.IO0, true, Port.ResistorMode.PullUp, Port.InterruptMode.InterruptEdgeLow);
            btnDown.OnInterrupt += new NativeEventHandler(btnDown_OnInterrupt);
        }

        private void btnUp_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //FF();
            acc0_0_On();

            Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private void btnSelect_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //SS();


            Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private void btnDown_OnInterrupt(uint data1, uint data2, DateTime time)
        {
            //RR();
            acc0_0_Off();

            Beeper.PlaySound(Beeper.SoundID.Click);
        }

        #region speed

        private int speed = 0;
        private bool forward = true;
        private LocomotiveAddress address = new LocomotiveAddress(3, false);

        private void FF()
        {
            Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed++;
            speed = Math.Min(speed, 28);
            forward = speed > 0;
            Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, Math.Abs(speed), forward));

            //for (int i = speed; i >= 0; i--)
            //    Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, i, forward));

            //for (int i = 0; i <= speed; i++)
            //    Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, i, forward));
            //Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, 0, forward));
        }
        private void SS()
        {
            Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed = 0;
            Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, speed, forward));
        }
        private void RR()
        {
            Model.MainBooster.AddCommand(DCCCommand.LocoFunctionGroup1(address, true, false, false, false, false));

            speed--;
            speed = Math.Max(speed, -28);
            forward = speed > 0;
            Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, Math.Abs(speed), forward));

            //for (int i = speed; i >= 0; i--)
            //    Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, i, forward));

            //for (int i = 0; i <= speed; i++)
            //    Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, i, forward));
            //Model.MainBooster.AddCommand(DCCCommand.LocoSpeed28(address, 0, forward));
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
