using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using MFE.Net;
using MFE.Net.Managers;
using MFE.Net.Messaging;
using MFE.Storage;
using MFE.Utilities;
using Microsoft.SPOT;
using Typhoon.API;
using Typhoon.DCC;
using Typhoon.MF.Layouts;
using Typhoon.MF.Layouts.LayoutItems;
using Typhoon.Server.Hardware;

namespace Typhoon.Server
{
    public class Model
    {
        #region Fields
        private static Options options;
        private const uint optionsID = 0;
        private static string root = @"\SD";

        private static DBManager dbManager;

        private static Booster mainBooster;
        private static Booster progBooster;
        private static Timer timerBoostersCurrent;
        //private static AcknowledgementDetector ackDetector;

        private static INetworkManager networkManager;

        private static NetworkMessageFormat msgFormat = NetworkMessageFormat.Text;
        //private static DiscoveryListener discoveryListener;
        //private static TCPServer tcpServer;
        private static WebSocketServer wsServer;

        private static HttpServer httpServer;

        private static Buttons btns;
        #endregion

        #region Properties
        public static Booster MainBooster
        {
            get { return mainBooster; }
        }
        public static Booster ProgBooster
        {
            get { return progBooster; }
        }
        //public static AcknowledgementDetector AckDetector
        //{
        //    get { return ackDetector; }
        //}

        public static bool IsPowerOn
        {
            get { return mainBooster.IsActive; }
            set
            {
                if (mainBooster.IsActive != value)
                {
                    mainBooster.IsActive = value;
                    mainBooster.ClearCommands();
                    if (mainBooster.IsActive)
                    {
                        DCCCommand cmdReset = DCCCommand.LocoBroadcastReset();
                        cmdReset.Repeats = 20;
                        mainBooster.AddCommand(cmdReset);
                    }
                }
            }
        }
        public static string Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }
        #endregion

        #region Constructor
        public Model()
        {
            DriveManager.DriveAdded += new DriveEventHandler(DriveManager_DriveAdded);

            InitData();
            InitHardware();
            //InitDB();
            InitNetwork();

            // for test only!!!
            //IsPowerOn = true;
        }
        #endregion

        #region Private methods
        private static void InitData()
        {
            options = Options.LoadFromFlash(optionsID);

            //options = Options.LoadFromSD();
            //ApplyOptions();
        }
        private static void InitHardware()
        {
            mainBooster = new Booster(
                true,
                HardwareConfiguration.PinMainBoosterEnable,
                HardwareConfiguration.PinMainBoosterEnableLED,
                HardwareConfiguration.PinMainBoosterSense,
                HardwareConfiguration.PinMainBoosterOverloadLED,
                HardwareConfiguration.SenseResistor,
                options.MainBridgeCurrentThreshould,
                HardwareConfiguration.PinMainOutputGenerator,
                DCCCommand.Idle().ToTimings()
                );
            mainBooster.PropertyChanged += new PropertyChangedEventHandler(Booster_PropertyChanged);

            progBooster = new Booster(
                true,
                HardwareConfiguration.PinProgBoosterEnable,
                HardwareConfiguration.PinProgBoosterEnableLED,
                HardwareConfiguration.PinProgBoosterSense,
                HardwareConfiguration.PinProgBoosterOverloadLED,
                HardwareConfiguration.SenseResistor,
                options.ProgBridgeCurrentThreshould,
                HardwareConfiguration.PinProgOutputGenerator,
                DCCCommand.Idle().ToTimings(true)
                );
            progBooster.PropertyChanged += new PropertyChangedEventHandler(Booster_PropertyChanged);

            if (options.BroadcastBoostersCurrent)
                timerBoostersCurrent = new Timer(TimerBustersCurrent_Tick, null, 0, 1000);

            //ackDetector = new AcknowledgementDetector(HardwareConfiguration.PinAcknowledgementSense);

            btns = new Buttons();
        }
        private static void InitNetwork()
        {
            //discoveryListener = new DiscoveryListener();

            //tcpServer = new TCPServer(Options.IPPort);
            //tcpServer.SessionConnected += new TCPSessionEventHandler(Session_Connected);
            //tcpServer.SessionDataReceived += new TCPSessionDataReceived(Session_DataReceived);
            //tcpServer.SessionDisconnected += new TCPSessionEventHandler(Session_Disconnected);

            wsServer = new WebSocketServer(Options.WSPort);
            wsServer.SessionConnected += new TCPSessionEventHandler(Session_Connected);
            wsServer.SessionDataReceived += new TCPSessionDataReceivedEventHandler(Session_DataReceived);
            wsServer.SessionDisconnected += new TCPSessionEventHandler(Session_Disconnected);

            httpServer = new HttpServer();
            httpServer.OnGetRequest += new GETRequestHandler(httpServer_OnGetRequest);

            if (options.UseWiFi)
                networkManager = new WiFiManager(true, HardwareConfiguration.PinNetworkLED, options.WiFiSSID, options.WiFiPassword);
            else
                networkManager = new EthernetManager(HardwareConfiguration.PinNetworkLED);
            networkManager.Started += new EventHandler(Network_Started);
            networkManager.Stopped += new EventHandler(Network_Stopped);

            StartNetwork();
        }
        private static void StartNetwork()
        {
            new Thread(delegate { networkManager.Start(); }).Start();
        }
        private static void InitDB()
        {
            dbManager = new DBManager();
            //root = @"\NAND";
            dbManager.Open(root + "\\Layout.dat");

            //dbManager.Add(new Consist() { Name = "Consist 1", });
            //dbManager.Add(new Consist() { Name = "Consist 2", });
            //dbManager.Add(new Consist() { Name = "Consist 3", });

            //dbManager.Add(new Locomotive() { Name = "Loco 1", Protocol = ProtocolType.DCC14 });
            //dbManager.Add(new Locomotive() { Name = "Loco 2", Protocol = ProtocolType.DCC28 });
            //dbManager.Add(new Locomotive() { Name = "Loco 3", Protocol = ProtocolType.DCC128 });

            Debug.Print(Utils.FreeRAM(true));

            for (int i = 0; i < 100; i++)
                dbManager.Add(new Locomotive() { Name = "Loco " + i, Protocol = ProtocolType.DCC28 });

            Debug.Print("DB size: " + Utils.FormatSize(dbManager.Size));



            Debug.Print(Utils.FreeRAM(true));
            ArrayList locos = dbManager.GetLocomotives();
            Debug.Print(Utils.FreeRAM(true));

            //Locomotive loc = (Locomotive)locos[0];
            //Locomotive loc2 = dbManager.GetLocomotive(loc.ID);
            //Debug.Print(loc2.Name);

            //ArrayList consists = dbManager.GetConsists();
            //Consist consist = (Consist)consists[1];
            //Consist consist2 = dbManager.GetConsist(consist.ID);
            //Debug.Print(consist2.Name);
        }

        private static void ApplyOptions()
        {
            if (mainBooster != null)
                mainBooster.CurrentThreshould = options.MainBridgeCurrentThreshould;
            if (progBooster != null)
                progBooster.CurrentThreshould = options.ProgBridgeCurrentThreshould;
            if (options.BroadcastBoostersCurrent)
            {
                if (timerBoostersCurrent == null)
                    timerBoostersCurrent = new Timer(TimerBustersCurrent_Tick, null, 0, 1000);
            }
            else
            {
                timerBoostersCurrent.Dispose();
                timerBoostersCurrent = null;
            }
        }

        private static void BroadcastPowerState()
        {
            if (wsServer != null && wsServer.IsActive)
            {
                networkManager.OnBeforeMessage();
                NetworkMessage msg = GetPowerMessage();
                wsServer.SendToAll(msg.PackToString(msgFormat));
                networkManager.OnAfterMessage();
            }
        }
        private static void BroadcastBoostersCurrent()
        {
            if (wsServer != null && wsServer.IsActive)
            {
                networkManager.OnBeforeMessage();
                NetworkMessage msg = GetBoostersCurrentMessage();
                wsServer.SendToAll(msg.PackToString(msgFormat));
                networkManager.OnAfterMessage();
            }
        }
        private static void BroadcastOptions()
        {
            if (wsServer != null && wsServer.IsActive)
            {
                networkManager.OnBeforeMessage();
                NetworkMessage msg = GetOptionsMessage();
                wsServer.SendToAll(msg.PackToString(msgFormat));
                networkManager.OnAfterMessage();
            }
        }
        
        private static void SendDCCCommand(DCCCommand cmd)
        {
            mainBooster.AddCommand(cmd);
        }
        #endregion

        #region Event handlers
        private static void DriveManager_DriveAdded(string Root)
        {
            if (Root == "\\SD")
            {
            //    options = Options.LoadFromSD();
            //    ApplyOptions();
            }
        }
        
        private static void Network_Started(object sender, EventArgs e)
        {
            //discoveryListener.Start(Options.UDPPort, "TyphoonCentralStation");
            //tcpServer.Start();

            httpServer.Start("http", 80);
            wsServer.Start();

            NameService ns = new NameService();
            ns.AddName("TYPHOON", NameService.NameType.Unique, NameService.MsSuffix.Default);

            Beeper.PlaySound(Beeper.SoundID.Click);
        }
        private static void Network_Stopped(object sender, EventArgs e)
        {
            Beeper.PlaySound(Beeper.SoundID.PowerOff);
            
            httpServer.Stop();
            wsServer.Stop();

            Thread.Sleep(1000);

            StartNetwork();
        }

        private static void Booster_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.Property == "IsOverloaded" && (bool)e.NewValue == true)
                Beeper.PlaySound(Beeper.SoundID.Overload);
            if (e.Property == "IsActive")
                Beeper.PlaySound(mainBooster.IsActive ? Beeper.SoundID.PowerOn : Beeper.SoundID.PowerOff);
            if (e.Property == "IsActive" || e.Property == "IsOverloaded")
                BroadcastPowerState();
        }

        private static void TimerBustersCurrent_Tick(object o)
        {
            BroadcastBoostersCurrent();
        }

        private static void Session_Connected(TCPSession session)
        {
            session.Tag = new NetworkMessageReceiver(msgFormat);
        }
        private static bool Session_DataReceived(TCPSession session, byte[] data)
        {
            networkManager.OnBeforeMessage();

            NetworkMessageReceiver nmr = session.Tag as NetworkMessageReceiver;
            NetworkMessage[] msgs = nmr.Process(data);
            if (msgs != null)
                foreach (NetworkMessage msg in msgs)
                {
                    NetworkMessage response = ProcessNetworkMessage(msg);
                    if (response != null)
                        session.Send(WebSocketDataFrame.WrapString(response.PackToString(nmr.MessageFormat)));
                }

            networkManager.OnAfterMessage();

            return false; // don't disconnect
        }
        private static void Session_Disconnected(TCPSession session)
        {
            // TODO: release locos and accessories
        }

        private static void httpServer_OnGetRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            if (path.ToLower() == "\\admin") // There is one particular URL that we process differently
            {
                //httpServer.ProcessPasswordProtectedArea(request, response);
            }
            else if (path.ToLower().IndexOf("json") != -1)
                ProcessJSONRequest(path, parameters, response);
            else
                httpServer.SendFile(root + path, response);
        }
        private static void ProcessJSONRequest(string path, Hashtable parameters, HttpListenerResponse response)
        {
            path = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);

            //if (path.ToLower() == @"\content\decoders")
            //    DecodersRead(root + path, response);
            //else if (path.ToLower() == @"\content\layout")
            //    LayoutRead(root + path, response);
            //else if (path.ToLower() == @"\content\layout\create")
            //    LayoutCreate(root + path, response);


        }
        private static void DecodersRead(string path, HttpListenerResponse response)
        {
            string[] files = Directory.GetFiles(path);
            if (files.Length != 0)
            {
                response.ContentType = "application/json";

                byte[] b = Encoding.UTF8.GetBytes("[");
                response.OutputStream.Write(b, 0, b.Length);

                foreach (string fileName in files)
                {
                    int bufferSize = 1024 * 1024; // 4
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        long fileLength = fs.Length;
                        //response.ContentLength64 = fileLength;
                        //Debug.Print("File: " + strFilePath + "; length = " + fileLength);

                        //byte[] buf = new byte[bufferSize];
                        for (long bytesSent = 0; bytesSent < fileLength; )
                        {
                            long bytesToRead = fileLength - bytesSent;
                            bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

                            byte[] buf = new byte[bytesToRead];

                            int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
                            response.OutputStream.Write(buf, 0, bytesRead);
                            bytesSent += bytesRead;

                            //Thread.Sleep(1);
                        }

                        fs.Close();
                    }
                }

                b = Encoding.UTF8.GetBytes("]");
                response.OutputStream.Write(b, 0, b.Length);
            }
        }
        private static void LayoutRead(string path, HttpListenerResponse response)
        {
            response.ContentType = "application/json";

            int bufferSize = 1024 * 1024; // 4
            using (FileStream fs = new FileStream(path + ".json", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long fileLength = fs.Length;
                response.ContentLength64 = fileLength;
                //Debug.Print("File: " + strFilePath + "; length = " + fileLength);

                //byte[] buf = new byte[bufferSize];
                for (long bytesSent = 0; bytesSent < fileLength; )
                {
                    long bytesToRead = fileLength - bytesSent;
                    bytesToRead = bytesToRead < bufferSize ? bytesToRead : bufferSize;

                    byte[] buf = new byte[bytesToRead];

                    int bytesRead = fs.Read(buf, 0, (int)bytesToRead);
                    response.OutputStream.Write(buf, 0, bytesRead);
                    bytesSent += bytesRead;

                    //Thread.Sleep(1);
                }

                fs.Close();
            }
        }
        private static void LayoutCreate(string path, HttpListenerResponse response)
        {

        }
        #endregion

        #region Network message processing
        private static NetworkMessage ProcessNetworkMessage(NetworkMessage msg)
        {
            NetworkMessage response = null;

            if (msg != null)
            {
                #region common vars
                int address;
                bool isLong;
                int speed;
                bool forward;
                #endregion

                switch (msg.ID)
                {
                    #region Power
                    case NetworkMessageID.Power:
                        if (msg.ParametersCount == 0) // client asks power state
                            response = GetPowerMessage();
                        else // client sets power state
                            IsPowerOn = msg["Power"] == bool.TrueString;
                        break;
                    #endregion

                    #region BoostersCurrent
                    case NetworkMessageID.BoostersCurrent: // client asks for boosters current
                        response = GetBoostersCurrentMessage();
                        break;
                    #endregion

                    #region Options
                    case NetworkMessageID.Options:
                        if (msg.ParametersCount == 0) // client asks for options
                            response = GetOptionsMessage();
                        else // client sets options
                        {
                            options.MainBridgeCurrentThreshould = int.Parse(msg["MainBridgeCurrentThreshould"]);
                            options.ProgBridgeCurrentThreshould = int.Parse(msg["ProgBridgeCurrentThreshould"]);
                            options.BroadcastBoostersCurrent = msg["BroadcastBoostersCurrent"] == bool.TrueString;
                            options.UseWiFi = msg["UseWiFi"] == bool.TrueString;
                            options.WiFiSSID = msg["WiFiSSID"];
                            options.WiFiPassword = msg["WiFiPassword"];
                            
                            options.SaveToFlash();
                            ApplyOptions();
                            BroadcastOptions();

                            response = GetOKMessage(null);
                        }
                        break;
                    #endregion

                    #region Version
                    case NetworkMessageID.Version:
                        response = GetVersionMessage();
                        break;
                    #endregion

                    #region Broadcast
                    case NetworkMessageID.BroadcastBrake: SendDCCCommand(DCCCommand.LocoBroadcastBrake()); break;
                    case NetworkMessageID.BroadcastStop: SendDCCCommand(DCCCommand.LocoBroadcastStop()); break;
                    case NetworkMessageID.BroadcastReset: SendDCCCommand(DCCCommand.LocoBroadcastReset()); break;
                    #endregion

                    #region LocoSpeed14, LocoSpeed28, LocoSpeed128
                    case NetworkMessageID.LocoSpeed14:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        speed = int.Parse(msg["Speed"]);
                        forward = msg["Forward"] == bool.TrueString;
                        bool light = msg["Light"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoSpeed14(new LocomotiveAddress(address, isLong), speed, forward, light));
                        break;

                    case NetworkMessageID.LocoSpeed28:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        speed = int.Parse(msg["Speed"]);
                        forward = msg["Forward"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoSpeed28(new LocomotiveAddress(address, isLong), speed, forward));
                        break;

                    case NetworkMessageID.LocoSpeed128:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        speed = int.Parse(msg["Speed"]);
                        forward = msg["Forward"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoSpeed128(new LocomotiveAddress(address, isLong), speed, forward));
                        break;
                    #endregion

                    #region Function groups
                    case NetworkMessageID.LocoFunctionGroup1:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        bool f0 = msg["F0"] == bool.TrueString;
                        bool f1 = msg["F1"] == bool.TrueString;
                        bool f2 = msg["F2"] == bool.TrueString;
                        bool f3 = msg["F3"] == bool.TrueString;
                        bool f4 = msg["F4"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoFunctionGroup1(new LocomotiveAddress(address, isLong), f0, f1, f2, f3, f4));
                        break;

                    case NetworkMessageID.LocoFunctionGroup2:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        bool f5 = msg["F5"] == bool.TrueString;
                        bool f6 = msg["F6"] == bool.TrueString;
                        bool f7 = msg["F7"] == bool.TrueString;
                        bool f8 = msg["F8"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoFunctionGroup2(new LocomotiveAddress(address, isLong), f5, f6, f7, f8));
                        break;

                    case NetworkMessageID.LocoFunctionGroup3:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        bool f9 = msg["F9"] == bool.TrueString;
                        bool f10 = msg["F10"] == bool.TrueString;
                        bool f11 = msg["F11"] == bool.TrueString;
                        bool f12 = msg["F12"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoFunctionGroup3(new LocomotiveAddress(address, isLong), f9, f10, f11, f12));
                        break;

                    case NetworkMessageID.LocoFunctionGroup4:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        bool f13 = msg["F13"] == bool.TrueString;
                        bool f14 = msg["F14"] == bool.TrueString;
                        bool f15 = msg["F15"] == bool.TrueString;
                        bool f16 = msg["F16"] == bool.TrueString;
                        bool f17 = msg["F17"] == bool.TrueString;
                        bool f18 = msg["F18"] == bool.TrueString;
                        bool f19 = msg["F19"] == bool.TrueString;
                        bool f20 = msg["F20"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoFunctionGroup4(new LocomotiveAddress(address, isLong), f13, f14, f15, f16, f17, f18, f19, f20));
                        break;

                    case NetworkMessageID.LocoFunctionGroup5:
                        address = int.Parse(msg["Address"]);
                        isLong = msg["Long"] == bool.TrueString;
                        bool f21 = msg["F21"] == bool.TrueString;
                        bool f22 = msg["F22"] == bool.TrueString;
                        bool f23 = msg["F23"] == bool.TrueString;
                        bool f24 = msg["F24"] == bool.TrueString;
                        bool f25 = msg["F25"] == bool.TrueString;
                        bool f26 = msg["F26"] == bool.TrueString;
                        bool f27 = msg["F27"] == bool.TrueString;
                        bool f28 = msg["F28"] == bool.TrueString;

                        SendDCCCommand(DCCCommand.LocoFunctionGroup5(new LocomotiveAddress(address, isLong), f21, f22, f23, f24, f25, f26, f27, f28));
                        break;
                    #endregion






                    default: break;
                }
            }

            return response;
        }

        private static NetworkMessage GetOKMessage(string text)
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.OK);
            if (text != null)
                msg["Msg"] = text;
            return msg;
        }
        private static NetworkMessage GetPowerMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Power);
            msg["Power"] = IsPowerOn ? bool.TrueString : bool.FalseString;
            msg["MainActive"] = mainBooster.IsActive ? bool.TrueString : bool.FalseString;
            msg["MainOverload"] = mainBooster.IsOverloaded ? bool.TrueString : bool.FalseString;
            msg["ProgActive"] = progBooster.IsActive ? bool.TrueString : bool.FalseString;
            msg["ProgOverload"] = progBooster.IsOverloaded ? bool.TrueString : bool.FalseString;
            return msg;
        }
        private static NetworkMessage GetBoostersCurrentMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.BoostersCurrent);
            msg["Main"] = mainBooster.Current.ToString();
            msg["Prog"] = progBooster.Current.ToString();
            return msg;
        }
        private static NetworkMessage GetOptionsMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Options);
            msg["MainBridgeCurrentThreshould"] = options.MainBridgeCurrentThreshould.ToString();
            msg["ProgBridgeCurrentThreshould"] = options.ProgBridgeCurrentThreshould.ToString();
            msg["BroadcastBoostersCurrent"] = options.BroadcastBoostersCurrent ? bool.TrueString : bool.FalseString;
            msg["UseWiFi"] = options.UseWiFi ? bool.TrueString : bool.FalseString;
            msg["WiFiSSID"] = options.WiFiSSID;
            msg["WiFiPassword"] = options.WiFiPassword;
            return msg;
        }
        private static NetworkMessage GetVersionMessage()
        {
            NetworkMessage msg = new NetworkMessage(NetworkMessageID.Version);
            msg["Version"] = Version;
            return msg;
        }
        #endregion
    }
}
