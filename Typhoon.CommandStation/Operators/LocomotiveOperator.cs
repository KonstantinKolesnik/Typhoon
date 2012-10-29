using System;
using System.Collections.Generic;
using System.ComponentModel;
using Typhoon.Decoders;

namespace Typhoon.CommandStation.Operators
{
    public class LocomotiveOperator : INotifyPropertyChanged
    {
        #region Fields
        private Decoder decoder = null;

        private bool forward = true;
        private int speed = 0;
        private List<bool> functions = new List<bool>();

        private bool sendCommand = true;
        #endregion

        #region Properties
        public Decoder Decoder
        {
            get { return decoder; }
            set
            {
                if (decoder != value)
                {
                    decoder = value;
                    Reset();
                    if (decoder != null)
                        decoder.PropertyChanged += Decoder_PropertyChanged;
                }
            }
        }
        public bool IsOperable
        {
            get
            {
                return
                    App.Model != null &&
                    App.Model.TCPClient != null &&
                    App.Model.TCPClient.IsStarted &&
                    App.Model.MainBoosterIsActive &&
                    decoder != null;
            }
        }

        public bool Forward
        {
            get { return forward; }
            set
            {
                if (forward != value)
                {
                    forward = value;
                    SendLocoSpeedCommand(speed);
                    OnPropertyChanged(new PropertyChangedEventArgs("Forward"));
                }
            }
        }
        public int Speed
        {
            get { return speed; }
            set
            {
                //if (speed != value)
                {
                    speed = (value == -1 ? 0 : value);
                    SendLocoSpeedCommand(value);
                    OnPropertyChanged(new PropertyChangedEventArgs("Speed"));
                }
            }
        }

        public bool F0
        {
            get { return functions[0]; }
            set { SetFunction(0, value); }
        }
        public bool F1
        {
            get { return functions[1]; }
            set { SetFunction(1, value); }
        }
        public bool F2
        {
            get { return functions[2]; }
            set { SetFunction(2, value); }
        }
        public bool F3
        {
            get { return functions[3]; }
            set { SetFunction(3, value); }
        }
        public bool F4
        {
            get { return functions[4]; }
            set { SetFunction(4, value); }
        }
        public bool F5
        {
            get { return functions[5]; }
            set { SetFunction(5, value); }
        }
        public bool F6
        {
            get { return functions[6]; }
            set { SetFunction(6, value); }
        }
        public bool F7
        {
            get { return functions[7]; }
            set { SetFunction(7, value); }
        }
        public bool F8
        {
            get { return functions[8]; }
            set { SetFunction(8, value); }
        }
        public bool F9
        {
            get { return functions[9]; }
            set { SetFunction(9, value); }
        }
        public bool F10
        {
            get { return functions[10]; }
            set { SetFunction(10, value); }
        }
        public bool F11
        {
            get { return functions[11]; }
            set { SetFunction(11, value); }
        }
        public bool F12
        {
            get { return functions[12]; }
            set { SetFunction(12, value); }
        }
        public bool F13
        {
            get { return functions[13]; }
            set { SetFunction(13, value); }
        }
        public bool F14
        {
            get { return functions[14]; }
            set { SetFunction(14, value); }
        }
        public bool F15
        {
            get { return functions[15]; }
            set { SetFunction(15, value); }
        }
        public bool F16
        {
            get { return functions[16]; }
            set { SetFunction(16, value); }
        }
        public bool F17
        {
            get { return functions[17]; }
            set { SetFunction(17, value); }
        }
        public bool F18
        {
            get { return functions[18]; }
            set { SetFunction(18, value); }
        }
        public bool F19
        {
            get { return functions[19]; }
            set { SetFunction(19, value); }
        }
        public bool F20
        {
            get { return functions[20]; }
            set { SetFunction(20, value); }
        }
        public bool F21
        {
            get { return functions[21]; }
            set { SetFunction(21, value); }
        }
        public bool F22
        {
            get { return functions[22]; }
            set { SetFunction(22, value); }
        }
        public bool F23
        {
            get { return functions[23]; }
            set { SetFunction(23, value); }
        }
        public bool F24
        {
            get { return functions[24]; }
            set { SetFunction(24, value); }
        }
        public bool F25
        {
            get { return functions[25]; }
            set { SetFunction(25, value); }
        }
        public bool F26
        {
            get { return functions[26]; }
            set { SetFunction(26, value); }
        }
        public bool F27
        {
            get { return functions[27]; }
            set { SetFunction(27, value); }
        }
        public bool F28
        {
            get { return functions[28]; }
            set { SetFunction(28, value); }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public LocomotiveOperator()
        {
            MessageSender.SentAllBrake += MessageSender_AllBrake;
            MessageSender.SentAllStop += MessageSender_AllStop;
            MessageSender.SentAllReset += MessageSender_AllReset;

            if (App.Model != null)
                App.Model.PropertyChanged += Model_PropertyChanged;

            for (int i = 0; i <= 28; i++)
                functions.Add(false);

            Reset();
        }
        #endregion

        #region Event handlers
        private void MessageSender_AllBrake(object sender, EventArgs e)
        {
            sendCommand = false;
            Brake();
            sendCommand = true;
        }
        private void MessageSender_AllStop(object sender, EventArgs e)
        {
            sendCommand = false;
            Stop();
            sendCommand = true;
        }
        private void MessageSender_AllReset(object sender, EventArgs e)
        {
            sendCommand = false;
            Reset();
            sendCommand = true;
        }
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MainBoosterIsActive" || e.PropertyName == "MainBoosterIsOverloaded")
            {
                sendCommand = false;
                Reset();
                sendCommand = true;
            }
        }
        private void Decoder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocomotiveSpeedSteps")
                Reset();
        }
        #endregion

        #region Public methods
        public void Stop()
        {
            Speed = -1;
        }
        public void Brake()
        {
            Speed = 0;
        }
        public void Reset()
        {
            Stop();
            Forward = true;

            for (int i = 0; i <= 28; i++)
            {
                functions[i] = false;
                OnPropertyChanged(new PropertyChangedEventArgs("F" + i.ToString()));
            }
            SendAllFunctionsCommand();
        }
        public void ToggleDirection()
        {
            Forward = !Forward;
        }
        #endregion

        #region Private methods
        private void SetFunction(int idx, bool value)
        {
            if (functions[idx] != value)
            {
                functions[idx] = value;
                if (decoder != null)
                {
                    if (idx == 0 && decoder.LocomotiveSpeedSteps == SpeedSteps.Speed14)
                        SendLocoSpeedCommand(speed);
                    else
                        SendFunctionCommand(idx);
                }
                OnPropertyChanged(new PropertyChangedEventArgs("F" + idx.ToString()));
            }
        }
        
        private void SendLocoSpeedCommand(int speed) // speed = -1, 0,1,2,3...(int)loco.SpeedSteps
        {
            if (sendCommand && IsOperable)
            {
                switch (decoder.LocomotiveSpeedSteps)
                {
                    case SpeedSteps.Speed14: MessageSender.SetLocoSpeed14(decoder.LocomotiveAddress, speed, forward, functions[0]); break;
                    case SpeedSteps.Speed28: MessageSender.SetLocoSpeed28(decoder.LocomotiveAddress, speed, forward); break;
                    case SpeedSteps.Speed128: MessageSender.SetLocoSpeed128(decoder.LocomotiveAddress, speed, forward); break;
                }
            }
        }
        private void SendFunctionCommand(int idx)
        {
            if (sendCommand && IsOperable)
            {
                if (idx >= 0 && idx <= 4)
                    MessageSender.SetLocoFunctionGroup1(decoder.LocomotiveAddress, functions[0], functions[1], functions[2], functions[3], functions[4]);
                else if (idx >= 5 && idx <= 8)
                    MessageSender.SetLocoFunctionGroup2(decoder.LocomotiveAddress, functions[5], functions[6], functions[7], functions[8]);
                else if (idx >= 9 && idx <= 12)
                    MessageSender.SetLocoFunctionGroup3(decoder.LocomotiveAddress, functions[9], functions[10], functions[11], functions[12]);
                else if (idx >= 13 && idx <= 20)
                    MessageSender.SetLocoFunctionGroup4(decoder.LocomotiveAddress, functions[13], functions[14], functions[15], functions[16], functions[17], functions[18], functions[19], functions[20]);
                else if (idx >= 21 && idx <= 28)
                    MessageSender.SetLocoFunctionGroup5(decoder.LocomotiveAddress, functions[21], functions[22], functions[23], functions[24], functions[25], functions[26], functions[27], functions[28]);
            }
        }
        private void SendAllFunctionsCommand()
        {
            if (sendCommand && IsOperable)
            {
                if (decoder.LocomotiveSpeedSteps == SpeedSteps.Speed14)
                    MessageSender.SetLocoSpeed14(decoder.LocomotiveAddress, speed, forward, functions[0]);

                MessageSender.SetLocoFunctionGroup1(decoder.LocomotiveAddress, functions[0], functions[1], functions[2], functions[3], functions[4]);
                MessageSender.SetLocoFunctionGroup2(decoder.LocomotiveAddress, functions[5], functions[6], functions[7], functions[8]);
                MessageSender.SetLocoFunctionGroup3(decoder.LocomotiveAddress, functions[9], functions[10], functions[11], functions[12]);
                MessageSender.SetLocoFunctionGroup4(decoder.LocomotiveAddress, functions[13], functions[14], functions[15], functions[16], functions[17], functions[18], functions[19], functions[20]);
                MessageSender.SetLocoFunctionGroup5(decoder.LocomotiveAddress, functions[21], functions[22], functions[23], functions[24], functions[25], functions[26], functions[27], functions[28]);
            }
        }
        #endregion
    }
}
