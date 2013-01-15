using System.Collections;
using System.Threading;
using GHI.Premium.Hardware;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Typhoon.DCC;

namespace Typhoon.Server.Hardware
{
    public class Booster
    {
        private class QueueEntry
        {
            public uint[] Timings;
            public byte Repeats;

            public QueueEntry(uint[] timings, byte repeats)
            {
                Timings = timings;
                Repeats = repeats;
            }
        }

        #region Fields

        private const int MaxTimingsCount = 200;
        private SignalGenerator portGenerator;
        private Queue commands = new Queue();
        private uint[] idleTimings;
        
        private bool ledToVcc = false;
        private OutputPort portEnable;
        private OutputPort portEnableLED;
        private OutputPort portOverloadLED;
        private AnalogInput portSense;
        private int checkOverloadPeriod = 200; // in msec
        private int blockPeriod = 5000; // in msec
        private short senseResistor; // in mOhms
        private int currentThreshould; // in mA
        private int current;
        private bool overloaded = false;
        #endregion

        #region Properties
        public bool IsActive
        {
            get { return portEnable.Read(); }
            set
            {
                if (portEnable.Read() != value)
                {
                    portEnable.Write(value);
                    portEnableLED.Write(ledToVcc ? !value : value);
                    OnPropertyChanged(new PropertyChangedEventArgs("IsActive", !value, value));
                    IsOverloaded = false;
                }
            }
        }
        public bool IsOverloaded
        {
            get { return overloaded; }
            private set
            {
                if (overloaded != value)
                {
                    bool old = overloaded;
                    overloaded = value;

                    if (overloaded)
                    {
                        // IsActive is already set to false
                        portOverloadLED.Write(!ledToVcc);
                        new Timer(TimerBlock_Expired, null, blockPeriod, 0);
                    }

                    OnPropertyChanged(new PropertyChangedEventArgs("IsOverloaded", old, overloaded));
                }
            }
        }
        public int Current // in mA
        {
            get { return current; }
            private set
            {
                if (current != value)
                {
                    int old = current;
                    current = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Current", old, current));
                }
            }
        }
        public int CurrentThreshould // in mA
        {
            get { return currentThreshould; }
            set { currentThreshould = value; }
        }
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        #endregion

        #region Constructor
        public Booster(
            bool ledToVcc,
            Cpu.Pin pinEnable,
            Cpu.Pin pinEnableLED,
            Cpu.AnalogChannel pinSense,
            Cpu.Pin pinOverloadLED,
            short senseResistor,
            int currentThreshould,
            Cpu.Pin pinGenerator,
            uint[] idleTimings
            )
        {
            portEnable = new OutputPort(pinEnable, false);
            portEnableLED = new OutputPort(pinEnableLED, ledToVcc);

            this.ledToVcc = ledToVcc;
            portOverloadLED = new OutputPort(pinOverloadLED, ledToVcc);
            portSense = new AnalogInput(pinSense);
            portSense.Scale = 3300;
            this.senseResistor = senseResistor;
            this.currentThreshould = currentThreshould;
            new Timer(TimerCurrent_Tick, null, 0, checkOverloadPeriod);

            portGenerator = new SignalGenerator((Cpu.Pin)pinGenerator, false, MaxTimingsCount);
            this.idleTimings = idleTimings;

            //new Thread(GeneratorWork) { Priority = ThreadPriority.AboveNormal }.Start();
            new Timer(TimerGenerator, null, 0, 2);
        }
        #endregion

        #region Public methods
        public void AddCommand(DCCCommand cmd)
        {
            if (IsActive)
            {
                QueueEntry qe = new QueueEntry(cmd.ToTimings(), cmd.Repeats);
                lock (commands.SyncRoot)
                    commands.Enqueue(qe);
            }
        }
        public void ClearCommands()
        {
            lock (commands.SyncRoot)
                commands.Clear();
        }
        #endregion

        #region Event handlers
        private void TimerBlock_Expired(object o)
        {
            portOverloadLED.Write(ledToVcc);
            IsActive = true;
        }
        private void TimerCurrent_Tick(object o)
        {
            double voltage = portSense.Read(); // mV
            Current = (int)(1000 * voltage / senseResistor); // mA
            if (Current > CurrentThreshould) // short circuit
            {
                IsActive = false;
                IsOverloaded = true;
            }
        }
        private void TimerGenerator(object o)
        {
            if (!portGenerator.IsActive)
                ReadCommand();
        }
        #endregion

        #region Generator
        //private void GeneratorWork()
        //{
        //    while (true)
        //    {
        //        Thread.Sleep(0); // necessary!!!
        //        ReadCommand();
        //    }
        //}
        private void ReadCommand()
        {
            lock (commands.SyncRoot)
            {
                if (commands.Count != 0)
                {
                    QueueEntry qe = (QueueEntry)commands.Dequeue();
                    Output(qe.Timings, qe.Repeats);
                }
                else
                    Output(idleTimings, 1);
            }
        }
        private void Output(uint[] buffer, byte repeats)
        {
            for (byte i = 0; i < repeats; i++)
                portGenerator.SetBlocking(true, buffer, 0, buffer.Length, 0, false);
        }
        #endregion
    }
}
