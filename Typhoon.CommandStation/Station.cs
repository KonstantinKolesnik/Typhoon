using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
//using Typhoon.Adapter;
using Typhoon.Core;
using Typhoon.Decoders;
using Typhoon.NMRA;

namespace Typhoon.CommandStation
{

    //public class Station : INotifyPropertyChanged
    //{
    //    #region Fields
    //    private AdapterUSB adapter = new AdapterUSB();
    //    private static ReaderWriterLock queueLock = new ReaderWriterLock();
    //    private DispatcherTimer timerStatus;

    //    private bool mainTrackActive = false;
    //    private bool progTrackActive = false;
    //    private bool mainTrackShortCircuitBlocked = false;
    //    private bool progTrackShortCircuitBlocked = false;
    //    private bool railcomActive = false;
    //    private bool ackOn = false;

    //    private volatile NMRACommandCollection queueProgram = new NMRACommandCollection();
    //    private volatile NMRACommandCollection queueOperation = new NMRACommandCollection();

    //    private const int stopRepeats = 3;
    //    private const int speedRepeat = 3; // dcc speed commands are repeated this time
    //    private const int funcRepeat = 2; // dcc func commands are repeated this time
    //    private const int accRepeat = 2; // dcc accessory commands are repeated this time
    //    private const int pomRepeat = 3; // dcc pom commands are repeated this time (min 2)
    //    private const int serviceRepeat = 1; // dcc service prog commands are repeated this time
    //    #endregion

    //    #region Events
    //    public event EventHandler AdapterConnected;
    //    public event EventHandler AdapterDisconnected;

    //    public event EventHandler AllBreak;
    //    public event EventHandler AllStop;
    //    public event EventHandler AllReset;

    //    public event EventHandler ProgramOK;
    //    public event EventHandler ProgramError;

    //    public event PropertyChangedEventHandler PropertyChanged;
    //    private void OnPropertyChanged(PropertyChangedEventArgs e)
    //    {
    //        CommandManager.InvalidateRequerySuggested();
    //        if (PropertyChanged != null)
    //            PropertyChanged(this, e);
    //    }
    //    #endregion

    //    #region Properties
    //    public bool Connected
    //    {
    //        get { return adapter.IsConnected; }
    //    }
    //    public bool MainTrackActive
    //    {
    //        get { return mainTrackActive; }
    //        private set
    //        {
    //            if (mainTrackActive != value)
    //            {
    //                mainTrackActive = value;
    //                OnPropertyChanged(new PropertyChangedEventArgs("MainTrackActive"));
    //            }
    //        }
    //    }
    //    public bool ProgramTrackActive
    //    {
    //        get { return progTrackActive; }
    //        private set
    //        {
    //            if (progTrackActive != value)
    //            {
    //                progTrackActive = value;
    //                OnPropertyChanged(new PropertyChangedEventArgs("ProgramTrackActive"));
    //            }
    //        }
    //    }
    //    public bool MainTrackShortCircuitBlocked
    //    {
    //        get { return mainTrackShortCircuitBlocked; }
    //        private set
    //        {
    //            if (mainTrackShortCircuitBlocked != value)
    //            {
    //                mainTrackShortCircuitBlocked = value;
    //                OnPropertyChanged(new PropertyChangedEventArgs("MainTrackShortCircuitBlocked"));
    //            }
    //        }
    //    }
    //    public bool ProgramTrackShortCircuitBlocked
    //    {
    //        get { return progTrackShortCircuitBlocked; }
    //        private set
    //        {
    //            if (progTrackShortCircuitBlocked != value)
    //            {
    //                progTrackShortCircuitBlocked = value;
    //                OnPropertyChanged(new PropertyChangedEventArgs("ProgramTrackShortCircuitBlocked"));
    //            }
    //        }
    //    }
    //    public bool RailcomActive
    //    {
    //        get { return railcomActive; }
    //        private set
    //        {
    //            if (railcomActive != value)
    //            {
    //                railcomActive = value;
    //                OnPropertyChanged(new PropertyChangedEventArgs("RailcomActive"));
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Constructor
    //    public Station()
    //    {
    //        adapter.Connected += OnAdapterConnected;
    //        adapter.Disconnected += OnAdapterDisconnected;

    //        timerStatus = new DispatcherTimer(DispatcherPriority.Normal);
    //        timerStatus.Interval = TimeSpan.FromMilliseconds(10);
    //        timerStatus.Tick += timerStatus_Tick;
    //        timerStatus.IsEnabled = true;
    //    }
    //    #endregion

    //    #region Public methods
    //    public void Connect()
    //    {
    //        adapter.Connect();
    //    }
    //    public void Disconnect()
    //    {
    //        timerStatus.IsEnabled = false;
    //        Off();
    //        adapter.Disconnect();
    //    }

    //    public void OperationOn()
    //    {
    //        SendMainTrackOn();
    //        System.Threading.Thread.Sleep(5);

    //        AddOperationCommand(NMRACommand.LocoBroadcastReset(), 20);
    //        AddOperationCommand(NMRACommand.Idle(), 10);
    //    }
    //    public void OperationOff()
    //    {
    //        SendMainTrackOff();
    //        queueOperation.Clear();
    //    }
        
    //    private void ServiceOn()
    //    {
    //        SendProgTrackOn();
    //    }
    //    private void ServiceOff()
    //    {
    //        SendProgTrackOff();
    //        queueProgram.Clear();
    //    }
        
    //    public void Off()
    //    {
    //        OperationOff();
    //        ServiceOff();
    //    }

    //    public void RailcomOn()
    //    {
    //        SendRailcomOn();
    //    }
    //    public void RailcomOff()
    //    {
    //        SendRailcomOff();
    //    }

    //    public void BroadcastBreak()
    //    {
    //        AddOperationCommand(NMRACommand.LocoBroadcastBrake());
    //        if (AllBreak != null)
    //            AllBreak(this, EventArgs.Empty);
    //    }
    //    public void BroadcastStop()
    //    {
    //        AddOperationCommand(NMRACommand.LocoBroadcastStop());
    //        if (AllStop != null)
    //            AllStop(this, EventArgs.Empty);
    //    }
    //    public void BroadcastReset()
    //    {
    //        AddOperationCommand(NMRACommand.LocoBroadcastReset());
    //        if (AllReset != null)
    //            AllReset(this, EventArgs.Empty);
    //    }

    //    public void ReadLocoDecoderParameter(DecoderParameter param, NMRAProgramMode mode, LocomotiveAddress address)
    //    {
    //        if (param == null || address == null)
    //            return;

    //        List<DecoderParameterProgramUnit> list = param.CurrentValueToProgramUnits();
    //        if (list.Count == 0)
    //            param.Verified = false;
    //        else
    //        {
    //            bool verified = true;
    //            foreach (DecoderParameterProgramUnit pu in list)
    //            {
    //                if (pu.BitPosition == -1) // whole CV
    //                {
    //                    // we don't gonna verify each of 256 possible values!!! :-)
    //                    // just simply verify each of 8 bits!
    //                    List<byte> resultBits = new List<byte>();
    //                    for (int bitPosition = 0; bitPosition < 8; bitPosition++)
    //                    {
    //                        byte b = 0;
    //                        resultBits.Add(b);

    //                        switch (mode)
    //                        {
    //                            case NMRAProgramMode.POM: verified &= POMBitRead(address, pu.CV, (uint)bitPosition, ref b); break;
    //                            case NMRAProgramMode.ServiceDirect: verified &= ServiceDirectBitRead(pu.CV, (uint)bitPosition, ref b); break;
    //                            case NMRAProgramMode.ServiceRegister: break;
    //                            case NMRAProgramMode.ServicePage: break;
    //                            default: verified = false; break;
    //                        }

    //                        if (!verified)
    //                            break;
    //                        resultBits[bitPosition] = b;
    //                    }
    //                    if (verified)
    //                        pu.Value = Helpers.BitsToByte(resultBits);
    //                }
    //                else // single bit
    //                {
    //                    switch (mode)
    //                    {
    //                        case NMRAProgramMode.POM: verified &= POMBitRead(address, pu); break;
    //                        case NMRAProgramMode.ServiceDirect: verified &= ServiceDirectBitRead(pu); break;
    //                        case NMRAProgramMode.ServiceRegister: break;
    //                        case NMRAProgramMode.ServicePage: break;
    //                        default: verified = false; break;
    //                    }
    //                }
    //            }
    //            param.Verified = verified;
    //        }

    //        // so far we have pu-list filled whith values from decoder;
    //        // arrange the new Value of param
    //        if (param.Verified)
    //            param.CurrentValueFromProgramUnits(list);

    //        FireProgramEvent(param.Verified);
    //    }
    //    public void WriteLocoDecoderParameter(DecoderParameter param, NMRAProgramMode mode, LocomotiveAddress address)
    //    {
    //        if (param == null || address == null)
    //            return;

    //        List<DecoderParameterProgramUnit> list = param.CurrentValueToProgramUnits();
    //        if (list.Count == 0)
    //            param.Verified = false;
    //        else
    //        {
    //            bool verified = true;
    //            foreach (DecoderParameterProgramUnit pu in list)
    //            {
    //                bool wholeCV = pu.BitPosition == -1;
    //                switch (mode)
    //                {
    //                    case NMRAProgramMode.POM: verified &= wholeCV ? POMCV(address, pu.CV, pu.Value.Value, false) : POMBit(address, pu.CV, (uint)pu.BitPosition, pu.Value.Value, false); break;
    //                    case NMRAProgramMode.ServiceDirect: verified &= wholeCV ? ServiceDirectCV(pu.CV, pu.Value.Value, false) : ServiceDirectBit(pu.CV, (uint)pu.BitPosition, pu.Value.Value, false); break;
    //                    case NMRAProgramMode.ServiceRegister: break;
    //                    case NMRAProgramMode.ServicePage: break;
    //                    default: verified = false; break;
    //                }
    //            }
    //            param.Verified = verified;
    //        }

    //        FireProgramEvent(param.Verified);
    //    }
    //    public void ResetLocoDecoder(NMRAProgramMode mode, LocomotiveAddress address, Decoder decoder)
    //    {
    //        if (address == null)
    //            return;

    //        bool verified = true;
    //        switch (mode)
    //        {
    //            case NMRAProgramMode.POM: verified &= POMCV(address, 8, 8, false); break;
    //            case NMRAProgramMode.ServiceDirect: verified &= ServiceDirectCV(8, 8, false); break;
    //            case NMRAProgramMode.ServiceRegister: break;
    //            case NMRAProgramMode.ServicePage: break;
    //            default: verified = false; break;
    //        }

    //        if (verified)
    //            foreach (DecoderParameter p in decoder.Parameters)
    //                p.Verified = false;

    //        FireProgramEvent(verified);
    //    }
        
    //    private void FireProgramEvent(bool verified)
    //    {
    //        EventHandler eh = verified ? ProgramOK : ProgramError;
    //        if (eh != null)
    //            eh.BeginInvoke(this, EventArgs.Empty, null, null);
    //    }
    //    #endregion

    //    #region Event handlers
    //    private void OnAdapterConnected(object sender, EventArgs e)
    //    {
    //        OnPropertyChanged(new PropertyChangedEventArgs("Connected"));
    //        if (AdapterConnected != null)
    //            AdapterConnected.BeginInvoke(this, null, null, null);
    //    }
    //    private void OnAdapterDisconnected(object sender, EventArgs e)
    //    {
    //        OnPropertyChanged(new PropertyChangedEventArgs("Connected"));
    //        if (AdapterDisconnected != null)
    //            AdapterDisconnected.BeginInvoke(this, null, null, null);
    //    }
    //    private void timerStatus_Tick(object sender, EventArgs e)
    //    {
    //        timerStatus.IsEnabled = false;
    //        Dequeue();
    //        timerStatus.IsEnabled = true;
    //    }
    //    #endregion

    //    #region Private methods
    //    private void GetStatus()
    //    {
    //        if (adapter.IsConnected)
    //        {
    //            AdapterStatus status = adapter.GetStatus();
    //            if (status != null)
    //            {
    //                MainTrackActive = status.MainTrackActive;
    //                ProgramTrackActive = status.ProgramTrackActive;
    //                MainTrackShortCircuitBlocked = status.MainTrackShortCircuitBlocked;
    //                ProgramTrackShortCircuitBlocked = status.ProgramTrackShortCircuitBlocked;
    //                RailcomActive = status.RailcomActive;
    //                ackOn = status.AckOn;
    //            }
    //            Thread.Sleep(1);
    //        }
    //    }
    //    private void Dequeue()
    //    {
    //        if (adapter.IsConnected)
    //        {
    //            GetStatus();

    //            if (queueOperation.Count != 0 && MainTrackActive)
    //            {
    //                queueLock.AcquireReaderLock(-1);
    //                NMRACommand cmd = queueOperation[0];
    //                queueLock.ReleaseReaderLock();

    //                adapter.Send(cmd);

    //                queueLock.AcquireWriterLock(-1);
    //                queueOperation.RemoveAt(0);
    //                queueLock.ReleaseWriterLock();
    //            }
    //        }
    //    }
    //    #endregion

    //    #region Operation mode
    //    private void AddOperationCommand(NMRACommand cmd, int repeats)
    //    {
    //        cmd.Repeats = repeats;
    //        AddOperationCommand(cmd);
    //    }
    //    public void AddOperationCommand(NMRACommand cmd)
    //    {
    //        if (adapter.IsConnected && MainTrackActive)
    //        {
    //            if (cmd.Repeats == 0)
    //            {
    //                switch (cmd.Type)
    //                {
    //                    case NMRACommandType.Stop: cmd.Repeats = stopRepeats; break;
    //                    case NMRACommandType.Speed: cmd.Repeats = speedRepeat; break;
    //                    case NMRACommandType.Function: cmd.Repeats = funcRepeat; break;
    //                    case NMRACommandType.Accessory: cmd.Repeats = accRepeat; break;
    //                    case NMRACommandType.POM: cmd.Repeats = pomRepeat; break;
    //                    default: cmd.Repeats = 1; break;
    //                }
    //            }

    //            OptimizeOperationQueue();

    //            queueLock.AcquireWriterLock(-1);
    //            queueOperation.Add(cmd);
    //            queueLock.ReleaseWriterLock();
    //        }
    //    }
    //    private void OptimizeOperationQueue()
    //    {
    //    }

    //    #region POM mode
    //    private bool POMBitRead(LocomotiveAddress address, DecoderParameterProgramUnit pu)
    //    {
    //        byte b = 0;
    //        bool res = POMBitRead(address, pu.CV, (uint)pu.BitPosition, ref b);
    //        if (res)
    //            pu.Value = b;
    //        return res;
    //    }
    //    private bool POMBitRead(LocomotiveAddress address, uint cv, uint bitPosition, ref byte value)
    //    {
    //        if (POMBit(address, cv, bitPosition, 0, true))
    //        {
    //            value = 0;
    //            return true;
    //        }
    //        else if (POMBit(address, cv, bitPosition, 1, true))
    //        {
    //            value = 1;
    //            return true;
    //        }
    //        else
    //            return false;
    //    }
    //    //-------------------------------------------------------------------
    //    private bool POMCV(LocomotiveAddress address, uint cv, byte value, bool read)
    //    {
    //        AddOperationCommand(NMRACommand.pomLocoCV(address, cv, value, read));
    //        return true;
    //    }
    //    private bool POMBit(LocomotiveAddress address, uint cv, uint bitPosition, byte bitValue, bool read)
    //    {
    //        AddOperationCommand(NMRACommand.pomLocoCVBit(address, cv, bitPosition, bitValue, read));
    //        return true;
    //    }
    //    #endregion

    //    #endregion

    //    #region Service mode
    //    /*
    //    // Overview of Service Modes:
    //    //
    //    // Mode          |Reset|Page reset|Reset|Command|Reset (read/write)
    //    //-------------------------------------------------------------------
    //    // Direct        |  3  |    -     |  -  |   5   |    1 / 6
    //    // Address only  |  3  |    5     |  9  |   7   |    1 / 10
    //    // Registere     |  3  |    5     |  9  |   7   |    1 / 10
    //    // Paged         |same as Register mode, but page access in advance        
    //    //-------------------------------------------------------------------
    //    */

    //    #region Service Direct
    //    private bool ServiceDirectBitRead(DecoderParameterProgramUnit pu)
    //    {
    //        byte b = 0;
    //        bool res = ServiceDirectBitRead(pu.CV, (uint)pu.BitPosition, ref b);
    //        if (res)
    //            pu.Value = b;
    //        return res;
    //    }
    //    private bool ServiceDirectBitRead(uint cv, uint bitPosition, ref byte value)
    //    {
    //        if (ServiceDirectBit(cv, bitPosition, 0, true))
    //        {
    //            value = 0;
    //            return true;
    //        }
    //        else if (ServiceDirectBit(cv, bitPosition, 1, true))
    //        {
    //            value = 1;
    //            return true;
    //        }
    //        else
    //            return false;
    //    }

    //    private bool ServiceDirectCV(uint cv, byte value, bool read)
    //    {
    //        queueProgram.Clear();
    //        AddProgCommand(NMRACommand.progReset(), 20); // power on cycle

    //        // Write:  3+ Reset, 5+ Write,  6+ Write/Reset
    //        // Verify: 3+ Reset, 5+ Verify, 1+ Reset if an acknowledgement is detected
    //        AddProgCommand(NMRACommand.progReset(), 3);
    //        AddProgCommand(NMRACommand.progDirectModeCV(cv, value, read), 5);
    //        AddProgCommand(NMRACommand.progReset(), (!read ? 6 : 1));

    //        return ProcessProgramQueue();
    //    }
    //    private bool ServiceDirectBit(uint cv, uint bitPosition, byte bitValue, bool read)
    //    {
    //        queueProgram.Clear();
    //        AddProgCommand(NMRACommand.progReset(), 20); // power on cycle

    //        // Write:  3+ Reset, 5+ Write,  6+ Write/Reset
    //        // Verify: 3+ Reset, 5+ Verify, 1+ Reset if an acknowledgement is detected
    //        AddProgCommand(NMRACommand.progReset(), 3);
    //        AddProgCommand(NMRACommand.progDirectModeCVBit(cv, bitPosition, bitValue, read), 5);
    //        AddProgCommand(NMRACommand.progReset(), (!read ? 6 : 1));

    //        return ProcessProgramQueue();
    //    }
    //    #endregion
    //    #region Service Register
    //    private bool ServiceRegister(uint cv, byte value, bool read)
    //    {
    //        queueProgram.Clear();
    //        AddProgCommand(NMRACommand.progReset(), 20); // power on cycle

    //        // Write:  3+ Reset, 5+ PagePreset, 6+ Reset, [Power-Off, Power-On-Cycle], 3+ Reset, 5+ Write, 6+ Write/Reset
    //        // Verify: 3+ Reset, 5+ PagePreset, 6+ Reset, [Power-Off, Power-On-Cycle], 3+ Reset, 7+ Verify

    //        AddProgCommand(NMRACommand.progReset(), 3);
    //        AddProgCommand(NMRACommand.progPagePreset(), 5);
    //        AddProgCommand(NMRACommand.progReset(), 6);
    //        // [Power-Off, Power-On-Cycle], omitted - it's for older decoders
    //        AddProgCommand(NMRACommand.progReset(), 3);
    //        if (!read)
    //        {
    //            AddProgCommand(NMRACommand.progRegisterMode(cv, value, read), 5);
    //            AddProgCommand(NMRACommand.progReset(), 6);
    //        }
    //        else
    //            AddProgCommand(NMRACommand.progRegisterMode(cv, value, read), 7);

    //        return ProcessProgramQueue();
    //    }
    //    #endregion

    //    private void AddProgCommand(NMRACommand cmd, int repeats)
    //    {
    //        if (adapter.IsConnected)
    //        {
    //            cmd.Repeats = repeats;
    //            if (cmd.Repeats == 0)
    //                cmd.Repeats = serviceRepeat;
    //            queueProgram.Add(cmd);
    //        }
    //    }
    //    private bool ProcessProgramQueue()
    //    {
    //        timerStatus.IsEnabled = false;
            
    //        SendPrepareForProgramming();
    //        for (int i = 0; i < queueProgram.Count; i++)
    //            adapter.Send(queueProgram[i]);
    //        ServiceOn();

    //        // check each tryInterval ms tryCount times
    //        int tryCount = 10;
    //        int tryInterval = 80;
    //        for (int i = 0; i < tryCount; i++)
    //        {
    //            App.DoEvents2();
    //            if (adapter.IsConnected)
    //            {
    //                GetStatus();
    //                if (ackOn)
    //                {
    //                    ServiceOff();
    //                    timerStatus.IsEnabled = true;
    //                    return true;
    //                }
    //            }
    //            Thread.Sleep(tryInterval);
    //        }

    //        ServiceOff();
    //        timerStatus.IsEnabled = true;
    //        return false;
    //    }
    //    #endregion

    //    #region System commands
    //    private void SendMainTrackOn()
    //    {
    //        SendSystemCommand("S0");
    //    }
    //    private void SendMainTrackOff()
    //    {
    //        SendSystemCommand("S1");
    //    }
    //    private void SendProgTrackOn()
    //    {
    //        SendSystemCommand("S2");
    //    }
    //    private void SendProgTrackOff()
    //    {
    //        SendSystemCommand("S3");
    //    }
    //    private void SendRailcomOn()
    //    {
    //        SendSystemCommand("S4");
    //    }
    //    private void SendRailcomOff()
    //    {
    //        SendSystemCommand("S5");
    //    }
    //    private void SendPrepareForProgramming()
    //    {
    //        ackOn = false;
    //        SendSystemCommand("S6");
    //    }

    //    private void SendSystemCommand(string cmd)
    //    {
    //        adapter.Send(cmd);
    //    }
    //    #endregion
    //}

}
