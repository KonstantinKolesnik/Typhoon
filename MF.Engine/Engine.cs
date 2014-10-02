using GHI.Premium.Hardware;
using GHI.Premium.System;
using MF.Engine.GUI;
using MF.Engine.GUI.Controls;
using MF.Engine.Managers;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Time;
using System;
using System.Collections;
using System.Threading;

namespace MF.Engine
{
    #region Event Delegates
    //[Serializable]
    //public delegate void OnButtonPressed(PyxisButton Button, ButtonState State);

    [Serializable]
    public delegate void OnModalStateChanged(bool IsModalActive);

    [Serializable]
    public delegate void OnDateTimeChanged();

    [Serializable]
    public delegate void OnUnhandledTouch(bool Down, Point e);
    #endregion

    public class Engine : MarshalByRefObject
    {
        #region Constants
        public static readonly string Product = "MF.ENGINE";//"Pyxis";
        public static readonly string Version = "1.0.0.0";//"2.0.1.0";
        public static readonly string Copyright = "Konstantin Kolesnik";//"2010 - 2011 Thomas W. Holtquist";
        public static readonly int BuildDate = 20111231;
        #endregion

        #region Fields
        internal static Engine Instance;
        internal static Application Application;


        // for displaying boot screens
        private static Thread bootThread = null;
        

        //internal Bitmap _buffer;                                // Main Bitmap that gets flushed to the LCD

        //protected internal Form _ActiveForm = null;             // Currently active Form
        //protected internal ContextMenu _ActiveCM = null;        // Currently active Context Menu

        private UIManager UI;
        private DriveManager DM;                                // Drive Manager
        private USBManager usbManager;                          // USB Manager
        //private NetworkManager NM;                              // Network Manager
        //private FileManager FM;                                 // FileManager

        private ArrayList _blockers = new ArrayList();          // List of ManualResetEvents for blocking
        private ManualResetEvent _activeBlock = null;           // Used to display Modal Forms
        private PromptResult _promptResult;                     // Used by Prompt
        private Thread myTouch;                                 // Gathers touches during Modal display
        private string _inputResult = String.Empty;             // Houses result of all input gathering Modals
        private Color _colorResult;                             // Result from Color Selection Dialog
        private int _selResult;
        private static bool _IsSave;                            // Open/Save Dialogs
        private static bool _CheckExist;                        // Save Dialog (don't overwrite)

        //internal Desktop desktop;                               // Desktop Instance
        internal ArrayList _runningApps = new ArrayList();      // ArrayList of running Applications
        internal ArrayList _services = new ArrayList();         // ArrayList of all installed Services

        private Thread _launcher;                               // App Launching Thread
        private string _sRelay;                                 // Used for Launching apps
        private string _sTitle;                                 // Used for Launching apps
        private string[] _sParams;                              // Parameters for Application Launch
        //private PyxisApplication _PAQuitter;                    // Used for quitting apps
        private object _KeyQuitter;                             // Used for quitting apps & services
        #endregion

        #region Properties
        //public Form ActiveForm
        //{
        //    get { return _ActiveForm; }
        //    set { _ActiveForm = value; }
        //}
        public static string FreeRAM(bool FormatOutput)
        {
            string[] sSize = new string[] { " bytes", " KB", " MB", "GB", " TB" };
            byte index = 0;
            float iFree = Debug.GC(false);
            string sFree = string.Empty;

            // Unformatted
            if (!FormatOutput)
                return iFree.ToString();

            // Format
            while (iFree > 1024)
            {
                iFree = iFree / 1024;
                index++;
                if (index == 4)
                    break;
            }

            sFree = iFree.ToString();
            if (sFree.IndexOf('.') > 0)
                sFree = sFree.Substring(0, sFree.IndexOf('.') + 3);
            return sFree + sSize[index];
        }
        public DriveManager MyDrives
        {
            get { return DM; }
        }
        //public FileManager MyFiles
        //{
        //    get { return FM; }
        //}
        //public NetworkManager MyNetwork
        //{
        //    get { return NM; }
        //}
        public USBManager MyUSB
        {
            get { return usbManager; }
        }
        public UIManager MyUI
        {
            get { return UI; }
        }

        //public MenuTray SystemMenuTray
        //{
        //    get { return desktop._mnu._tray; }
        //}
        //public bool UseVirtualKeyboard
        //{
        //    get { return IM.UseVirtualKeyboard; }
        //}
        //internal Bitmap ScreenBuffer
        //{
        //    get { return _buffer; }
        //}
        #endregion

        #region Events
        public event OnModalStateChanged ModalStateChanged;

        /// <summary>
        /// Fired when modal state changes
        /// </summary>
        /// <param name="IsModalActive"></param>
        protected virtual void OnModalStateChanged(bool IsModalActive)
        {
            if (ModalStateChanged != null)
                ModalStateChanged(IsModalActive);
        }

        public event OnDateTimeChanged DateTimeChanged;

        /// <summary>
        /// Fired when date/time changes
        /// </summary>
        protected virtual void OnDateTimeChanged()
        {
            if (DateTimeChanged != null)
                DateTimeChanged();
        }

        public event OnUnhandledTouch UnhandledTouch;

        /// <summary>
        /// Fires when a touch is received but unused
        /// </summary>
        /// <param name="Down"></param>
        /// <param name="e"></param>
        protected virtual void OnUnhandledTouch(bool Down, Point e)
        {
            if (UnhandledTouch != null)
                UnhandledTouch(Down, e);
        }
        #endregion

        #region Constructor
        public Engine(Application application)
        {
            Instance = this;
            Application = application;

            UI = new UIManager();
            DM = new DriveManager();
            usbManager = new USBManager();
            //NM = new NetworkManager(this);
            //FM = new FileManager(this);

            // Restore Time Zone
            SetTimeZoneOffset(GetTimeZoneMinutes(SettingsManager.Boot.TimeZone));

            // Create Buffer
            //_buffer = new Bitmap(AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
        }
        #endregion

        #region Public methods
        public void Start(ThreadStart bootScreen, ThreadStart prepBootScreen)
        {
            if (DeviceManager.IsEmulator)
                NormalBoot();
            else
            {
                //switch (SystemUpdate.GetMode())
                //{
                //    case SystemUpdate.SystemUpdateMode.NonFormatted:
                //        if (prepBootScreen != null)
                //        {
                //            // Display IFU Prep Screen
                //            bootThread = new Thread(prepBootScreen);
                //            bootThread.Priority = ThreadPriority.Highest;
                //            bootThread.Start();
                //        }
                //        SystemUpdate.EnableBootloader();
                //        break;
                //    case SystemUpdate.SystemUpdateMode.Bootloader:
                //        throw new Exception("Invalid Boot Mode!");
                //    case SystemUpdate.SystemUpdateMode.Application:
                //        if (bootScreen != null)
                //        {
                //            // Display Boot Screen
                //            bootThread = new Thread(bootScreen);
                //            bootThread.Priority = ThreadPriority.Highest;
                //            bootThread.Start();
                //        }
                //        NormalBoot();
                //        break;
                //}
            }

            // now returns to user Main entry
        }
        //public void CalibrateScreen()
        //{
        //    if (!DeviceManager.IsEmulator)
        //    {
        //        myTouch = new Thread(CalibrationWork);
        //        myTouch.Priority = ThreadPriority.Highest;
        //        myTouch.Start();

        //        ModalBlock();
        //    }
        //}
        public void CheckForUpdates(bool inBackground)
        {
            myTouch = new Thread(UpdateWork);
            myTouch.Priority = ThreadPriority.Highest;
            _IsSave = inBackground;
            myTouch.Start();
        }
        public void DisplayDesktop()
        {
            //if (desktop == null)
            //{
            //    desktop = new Desktop(this);
            //    //Thread LS = new Thread(LoadServices);
            //    //LS.Priority = ThreadPriority.BelowNormal;
            //    //LS.Start();
            //}

            //_ActiveForm = null;
            //desktop.Activate();
        }

        //public PyxisService[] GetInstalledServices()
        //{
        //    PyxisService[] svc = new PyxisService[_services.Count];
        //    for (int i = 0; i < svc.Length; i++)
        //        svc[i] = (PyxisService)_services[i];
        //    return svc;
        //}
        //public PyxisService[] GetRunningServices()
        //{
        //    PyxisService PS;
        //    PyxisService[] holder, ret;
        //    int c = 0;
        //    int i;

        //    holder = new PyxisService[_services.Count];
        //    for (i = 0; i < _services.Count; i++)
        //    {
        //        PS = (PyxisService)_services[i];
        //        if (PS.Info.IsRunning)
        //            holder[c++] = PS;
        //    }

        //    ret = new PyxisService[c];
        //    Array.Copy(holder, ret, ret.Length);
        //    return ret;
        //}
        //public PyxisService[] GetServiceByStartMode(StartMode StartMode)
        //{
        //    PyxisService PS;
        //    PyxisService[] holder, ret;
        //    int c = 0;
        //    int i;

        //    holder = new PyxisService[_services.Count];
        //    for (i = 0; i < _services.Count; i++)
        //    {
        //        PS = (PyxisService)_services[i];
        //        if (PS.Info.StartMode == StartMode)
        //            holder[c++] = PS;
        //    }

        //    ret = new PyxisService[c];
        //    Array.Copy(holder, ret, ret.Length);
        //    return ret;
        //}
        //public PyxisService[] GetStoppedServices()
        //{
        //    PyxisService PS;
        //    PyxisService[] holder, ret;
        //    int c = 0;
        //    int i;

        //    holder = new PyxisService[_services.Count];
        //    for (i = 0; i < _services.Count; i++)
        //    {
        //        PS = (PyxisService)_services[i];
        //        if (!PS.Info.IsRunning)
        //            holder[c++] = PS;
        //    }

        //    ret = new PyxisService[c];
        //    Array.Copy(holder, ret, ret.Length);
        //    return ret;
        //}
        public bool IsServiceInstalled(string filename)
        {
            //if (!File.Exists(filename))
            //    return false;

            //string sServ = MyDrives.RootDirectory + Resources.GetString(Resources.StringResources.ServicePath);
            //if (!File.Exists(sServ))
            //    return false;

            //string[] Services = new string(UTF8Encoding.UTF8.GetChars(File.ReadAllBytes(sServ))).Split('\n');
            //string[] Parts;

            //filename = filename.ToLower();
            //for (int i = 0; i < Services.Length; i++)
            //{
            //    Parts = Services[i].Split('|');
            //    if (Parts[0].ToLower() == filename)
            //        return true;
            //}
            return false;
        }
        //public void StartService(ServiceKey ServiceKey)
        //{
        //    PyxisService ps;
        //    for (int i = 0; i < _services.Count; i++)
        //    {
        //        ps = (PyxisService)_services[i];
        //        if (ps.Key == ServiceKey)
        //        {
        //            if (!ps.Info.IsRunning)
        //            {
        //                ModalMessage MM = new ModalMessage("Starting '" + ps.Info.Name + "'...", this);
        //                MM.Start(false);
        //                _services[i] = AppLoader.StartRegisteredService(this, ps, null);
        //                MM.Stop();
        //                return;
        //            }
        //            else
        //                throw new Exception("Service is already running");
        //        }
        //    }

        //    throw new Exception("Service not found");
        //}
        //public void StopService(ServiceKey ServiceKey)
        //{
        //    PyxisService ps;
        //    for (int i = 0; i < _services.Count; i++)
        //    {
        //        ps = (PyxisService)_services[i];
        //        if (ps.Key == ServiceKey)
        //        {
        //            if (ps.Info.IsRunning)
        //            {
        //                ModalMessage MM = new ModalMessage("Stopping '" + ps.Info.Name + "'...", this);
        //                MM.Start(false);
        //                AppDomain.Unload(ps.Key.AppDomain);
        //                ps.Info.IsRunning = false;
        //                ps.Key = new ServiceKey(-1, null, ps.Key.ServicePath);
        //                _services[i] = ps;
        //                MM.Stop();
        //                return;
        //            }
        //            else
        //                throw new Exception("Service is not running");
        //        }
        //    }

        //    throw new Exception("Service not found");
        //}


        //public void ShowContextMenu(ContextMenu contextmenu, Point e)
        //{
        //    ShowContextMenu(contextmenu, e.X, (e.Y < 22) ? 22 : e.Y);
        //}
        //public void ShowContextMenu(ContextMenu contextmenu, int x, int y)
        //{
        //    if (_ActiveCM != null)
        //        RefreshScreen();

        //    _ActiveCM = contextmenu;
        //    contextmenu.Show(this, x, y);
        //}
        public void HideContextMenu()
        {
            //if (_ActiveCM == null)
            //    return;

            //_ActiveCM = null;
            //RefreshScreen();
        }

        public void RefreshScreen()
        {
            //desktop.Render(true);
        }

        //public byte[] CreateLink(string path)
        //{
        //    if (!File.Exists(path))
        //        throw new Exception("Path does not exist");

        //    return CreateLink(File.ReadAllBytes(path), path);
        //}
        //public bool InstallApplication(string filename)
        //{
        //    byte[] b;
        //    FileStream iFile, iWrite;
        //    string sDir, sAppName, sName, sPath;
        //    string sPXE = string.Empty;
        //    int iLen, iCount, i;
        //    bool bLaunchService = false;

        //    // Check file
        //    if (!File.Exists(filename))
        //    {
        //        Prompt("Could not install " + Path.GetFileName(filename) + ".\r\nFile not found.", Product, PromptType.OKOnly);
        //        return false;
        //    }

        //    // Check Magic Number
        //    iFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
        //    b = new byte[4];
        //    iFile.Read(b, 0, 4);
        //    if (b[0] != 112 || b[1] != 105 || b[2] != 101 || b[3] != 95)
        //    {
        //        Prompt("Could not install " + Path.GetFileName(filename) + ".\r\nInvalid install file.", Product, PromptType.OKOnly);
        //        return false;
        //    }

        //    // Read Application Name
        //    iFile.Read(b, 0, 1);
        //    b = new byte[b[0]];
        //    iFile.Read(b, 0, b.Length);
        //    sAppName = new string(UTF8Encoding.UTF8.GetChars(b));

        //    // Get an install directory
        //    sDir = MyDrives.RootDirectory + "apps\\" + sAppName + "\\";
        //    if (Directory.Exists(sDir))
        //    {
        //        sDir = SelectDirectory(sAppName + " - Select Install Directory");
        //        if (sDir == string.Empty)
        //            return false;
        //    }
        //    else
        //        Directory.CreateDirectory(sDir);

        //    // Install Root Files
        //    try
        //    {
        //        iFile.Read(b, 0, 1);
        //        iCount = b[0];          // Check the number of items in the directory
        //        for (i = 0; i < iCount; i++)
        //        {
        //            // Read file name
        //            iFile.Read(b, 0, 1);
        //            b = new byte[b[0]];
        //            iFile.Read(b, 0, b.Length);
        //            sName = new string(UTF8Encoding.UTF8.GetChars(b));

        //            // Watch for application
        //            if (sPXE == string.Empty && Path.GetExtension(sName.ToLower()) == ".pxe")
        //                sPXE = sDir + sName;

        //            // Get File Size
        //            b = new byte[4];
        //            iFile.Read(b, 0, 4);
        //            iLen = ((b[3] & 0xFF) << 24) + ((b[2] & 0xFF) << 16) + ((b[1] & 0xFF) << 8) + (b[0] & 0xFF);

        //            // Delete existant file
        //            if (File.Exists(sDir + sName))
        //                File.Delete(sDir + sName);

        //            // Write File
        //            b = new byte[iLen];
        //            iFile.Read(b, 0, b.Length);
        //            iWrite = new FileStream(sDir + sName, FileMode.CreateNew, FileAccess.Write);
        //            iWrite.Write(b, 0, b.Length);
        //            iWrite.Close();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Prompt("Install failed!\r\n" + e.Message, Product, PromptType.OKOnly);
        //        return false;
        //    }

        //    // Install Remaining Folders & Files
        //    try
        //    {
        //        while (true)
        //        {
        //            // Read Directory Name or EOF
        //            sName = "";
        //            iFile.Read(b, 0, 1);
        //            if (b[0] == 0)
        //                break;
        //            b = new byte[b[0]];
        //            iFile.Read(b, 0, b.Length);
        //            sName = new string(UTF8Encoding.UTF8.GetChars(b));
        //            if (sName.Substring(0, 1) == "\\")
        //                sName = sName.Substring(1);

        //            // Create Directory
        //            sPath = FileManager.NormalizeDirectory(sDir + sName);
        //            if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);

        //            // Write files in Directory
        //            iFile.Read(b, 0, 1);
        //            iCount = b[0];          // Check the number of items in the directory
        //            for (i = 0; i < iCount; i++)
        //            {
        //                // Read file name
        //                iFile.Read(b, 0, 1);
        //                b = new byte[b[0]];
        //                iFile.Read(b, 0, b.Length);
        //                sName = new string(UTF8Encoding.UTF8.GetChars(b));

        //                // Get File Size
        //                b = new byte[4];
        //                iFile.Read(b, 0, 4);
        //                iLen = ((b[3] & 0xFF) << 24) + ((b[2] & 0xFF) << 16) + ((b[1] & 0xFF) << 8) + (b[0] & 0xFF);

        //                // Delete existant file
        //                if (File.Exists(sPath + sName))
        //                    File.Delete(sPath + sName);

        //                // Write File
        //                b = new byte[iLen];
        //                iFile.Read(b, 0, b.Length);
        //                iWrite = new FileStream(sPath + sName, FileMode.CreateNew, FileAccess.Write);
        //                iWrite.Write(b, 0, b.Length);
        //                iWrite.Close();
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Prompt("Install failed!\r\n" + e.Message, Product, PromptType.OKOnly);
        //        return false;
        //    }

        //    // Create Associations
        //    try
        //    {
        //        iFile.Read(b, 0, 1);
        //        iCount = b[0];

        //        for (i = 0; i < iCount; i++)
        //        {
        //            // Read file ext
        //            iFile.Read(b, 0, 1);
        //            b = new byte[b[0]];
        //            iFile.Read(b, 0, b.Length);
        //            sName = new string(UTF8Encoding.UTF8.GetChars(b));

        //            MyFiles.AssociateType(sName, sPXE + " %1", "");
        //        }

        //        if (iCount > 0)
        //            MyFiles.SaveAssociations();

        //    }
        //    catch (Exception) { }   // Ignore errors here

        //    iFile.Close();

        //    i = AppLoader.GetPXEType(this, sPXE);

        //    // None
        //    if (i == -1)
        //    {
        //        Prompt("The PXE file does not contain a valid application or service!", Product, PromptType.OKOnly);
        //        File.Delete(filename);
        //        MyDrives.FlushFileSystems();
        //        return false;
        //    }

        //    // App Present
        //    if (i == 0 || i == 2)
        //    {
        //        if (sPXE != string.Empty)
        //        {
        //            if (Prompt("Would you like to create a shortcut for this application on the desktop?", sAppName, PromptType.YesNo) == PromptResult.Yes)
        //            {
        //                try
        //                {
        //                    b = CreateLink(sPXE);
        //                    string file = MyDrives.RootDirectory + "pyxis\\desktop\\" + PyxisAPI.StringReplace(Path.GetFileName(sPXE), Path.GetExtension(sPXE), "");
        //                    if (File.Exists(file + ".lnk"))
        //                        file = FileManager.GetFile(file);

        //                    FileStream fs = new FileStream(file + ".lnk", FileMode.CreateNew, FileAccess.Write);
        //                    fs.Write(b, 0, b.Length);
        //                    fs.Close();

        //                }
        //                catch (Exception e)
        //                {
        //                    Debug.Print("Error creating link: " + e.Message);
        //                    Prompt("Could not create link.\n" + sPXE, sAppName, PromptType.OKOnly);
        //                    return false;
        //                }
        //            }
        //        }
        //    }

        //    // Service Present
        //    if (i == 1)
        //    {
        //        if (!IsServiceInstalled(sPXE))
        //            AppLoader.RegisterService(this, (StartMode)i, sPXE, null);
        //    }

        //    Prompt(sAppName + " has been installed.", Product, PromptType.OKOnly);


        //    File.Delete(filename);
        //    MyDrives.FlushFileSystems();
        //    return true;
        //}
        //public void StartApplication(string path)
        //{
        //    StartApplication(path, Path.GetFileName(path), null);
        //}
        //public void StartApplication(string path, string[] parameters)
        //{
        //    StartApplication(path, Path.GetFileName(path), parameters);
        //}
        //public void StartApplication(string path, string title)
        //{
        //    StartApplication(path, title, null);
        //}
        //public void StartApplication(string path, string title, string[] parameters)
        //{
        //    switch (path)
        //    {
        //        case "-1":
        //            AppStore.Show(this);
        //            break;
        //        case "-2":
        //            FileFinder.Show(this);
        //            break;
        //        case "-3":
        //            PictureViewer.Show(this, null);
        //            break;
        //        case "-4":
        //            SettingsWin.Show(this);
        //            break;
        //        default:
        //            _sRelay = path;
        //            _sTitle = title;
        //            _sParams = parameters;
        //            _launcher = new Thread(ApplicationLaunchThread);
        //            _launcher.Priority = ThreadPriority.AboveNormal;
        //            _launcher.Start();
        //            break;
        //    }
        //}
        //public void QuitApp(ApplicationKey AppKey)
        //{
        //    PyxisApplication PA;

        //    // Find the right Application
        //    for (int i = 0; i < _runningApps.Count; i++)
        //    {
        //        PA = (PyxisApplication)_runningApps[i];
        //        if (PA.Key == AppKey)
        //        {
        //            _KeyQuitter = AppKey;
        //            _PAQuitter = PA;
        //            _launcher = new Thread(QuitAppSafely);
        //            _launcher.Priority = ThreadPriority.Normal;
        //            _launcher.Start();

        //            return;
        //        }
        //    }

        //    throw new Exception("Invalid Application Key");
        //}

        public void SetDateTime(DateTime NewDateTime)
        {
            if (DeviceManager.ActiveDevice != DeviceType.Emulator)
            {
                try
                {
                    RealTimeClock.SetTime(NewDateTime);
                }
                catch (Exception) { }
            }
            Utility.SetLocalTime(NewDateTime);
            OnDateTimeChanged();
        }
        public void SetTimeZoneOffset(int OffsetInMinutes)
        {
            TimeService.SetTimeZoneOffset(OffsetInMinutes);
            OnDateTimeChanged();
        }

        public void ShowAbout()
        {
            //if (DeviceManager.IsEmulator)
            //    Prompt(Product + "\nVersion: " +
            //           Version + "\nScreen Size: " + AppearanceManager.ScreenWidth + "x" + AppearanceManager.ScreenHeight +
            //           "\nCopyright: " + Copyright +
            //           "\nFree RAM: " + FreeRAM(true) +
            //           "\nCustom Heap Size: " + Configuration.Heap.CurrentCustomHeapSize + " of " + Configuration.Heap.TotalHeapSize +
            //           "\n\nSpecial Thanks: GHI Electronics",
            //           "About " + Product, PromptType.OKOnly);
            //else
            //    Prompt(Product + "\nVersion: " +
            //           Version + "\nScreen Size: " + AppearanceManager.ScreenWidth + "x" + AppearanceManager.ScreenHeight +
            //           "\nCopyright: " + Copyright +
            //           "\nFree RAM: " + FreeRAM(true) +
            //           "\nCustom Heap Size: " + Configuration.Heap.CurrentCustomHeapSize + " of " + Configuration.Heap.TotalHeapSize +
            //           "\n\nSpecial Thanks: GHI Electronics",
            //           "About " + Product, PromptType.OKOnly);
        }
        #endregion

        #region Internal Methods
        internal int GetTimeZoneMinutes(int iRes)
        {
            if (iRes == -1)
                return 0;
            if (iRes == 0)
                return -720;	// - 12hrs
            if (iRes <= 2)
                return -660;	// - 11hrs
            if (iRes == 3)
                return -600;	// - 10hrs
            if (iRes == 4)
                return -540;	// - 9hrs
            if (iRes <= 6)
                return -480;	// - 8hrs
            if (iRes <= 9)
                return -420;	// - 7hrs
            if (iRes <= 13)
                return -360;	// - 6hrs
            if (iRes <= 16)
                return -300;	// - 5hrs
            if (iRes == 17)
                return -270;	// - 4.5hrs
            if (iRes <= 22)
                return -240;	// - 4hrs
            if (iRes == 23)
                return -210;	// - 3.3hrs
            if (iRes <= 28)
                return -180;	// - 3hrs
            if (iRes <= 30)
                return -120;	// - 2hrs
            if (iRes <= 32)
                return -60;		// - 1hr
            if (iRes <= 36)
                return 0;
            if (iRes <= 42)
                return 60;		// + 1hr
            if (iRes <= 51)
                return 120;		// + 2hrs
            if (iRes <= 55)
                return 180;		// + 3hrs
            if (iRes == 56)
                return 210;		// + 3.5hrs
            if (iRes <= 61)
                return 240;		// + 4hrs
            if (iRes == 62)
                return 270;		// + 4.5hrs
            if (iRes <= 65)
                return 300;		// + 5hrs
            if (iRes <= 67)
                return 330;		// + 5.5hrs
            if (iRes == 68)
                return 345;		// + 5.75hrs
            if (iRes <= 71)
                return 360;		// + 6hrs
            if (iRes == 72)
                return 390;		// + 6.5hrs
            if (iRes <= 74)
                return 420;		// + 7hrs
            if (iRes <= 80)
                return 480;		// + 8hrs
            if (iRes <= 83)
                return 540;		// + 9hrs
            if (iRes <= 85)
                return 570;		// + 9.5hrs
            if (iRes <= 90)
                return 600;		// + 10hrs
            if (iRes <= 92)
                return 660;		// + 11hrs
            if (iRes <= 95)
                return 720;		// + 12hrs

            return 750;		// + 12.5hrs
        }
        internal void SetModalState(bool modal)
        {
            OnModalStateChanged(modal);
        }
        internal void RegisterForm(Form Form, AppDomain AD)
        {
            // Find App
            //PyxisApplication PA;
            //for (int i = 0; i < _runningApps.Count; i++)
            //{
            //    PA = (PyxisApplication)_runningApps[i];
            //    if (PA.Domain == AD)
            //    {
            //        PA.Forms.Add(Form);
            //        return;
            //    }
            //}
        }

        #endregion

        #region Private methods
        private void NormalBoot()
        {
            if (
                  AppearanceManager.SetLCDResolution()
                | AppearanceManager.SetBootLogo(null, 0, 0)//Resources.GetBitmap(Resources.BitmapResources.Train_800_480))
                | AppearanceManager.SetLCDBootupMessages(true)
                )
            {
                Util.FlushExtendedWeakReferences();
                PowerState.RebootDevice(false);
            }

            //if (!UI.IsCalibrated)
            //{
            //    if (bootThread != null && bootThread.IsAlive)
            //        bootThread.Suspend();

            //    UI.CalibrateScreen();

            //    if (bootThread != null && bootThread.IsAlive)
            //        bootThread.Resume();
            //}

            if (!DeviceManager.IsEmulator)
            {
                //MyNetwork.AutoConnect = SettingsManager.BootSettings.ConnectionType;

                //if (SettingsManager.Boot.EnableRTC)
                //    SetDateTime(Utils.GetNetworkTime());
            }

            if (bootThread != null && bootThread.IsAlive)
            {
                bootThread.Suspend();
                bootThread.Abort();
                bootThread = null;
            }

            //DisplayDesktop();
        }

        private void ModalBlock()
        {
            SetModalState(true);
            ManualResetEvent localBlocker = new ManualResetEvent(false);

            if (_activeBlock != null)
                _blockers.Add(_activeBlock);

            _activeBlock = localBlocker;

            // Wait for Result
            localBlocker.Reset();
            while (!localBlocker.WaitOne(1000, false))
            {
                ;
            }

            // Unblock
            ReleaseBlock();

            // End Modal Touch
            if (_blockers.Count == 0)
            {
                try { myTouch.Suspend(); }
                catch (Exception) { }
                myTouch = null;
            }
        }
        private void ReleaseBlock()
        {
            if (_blockers.Count > 0)
            {
                _activeBlock = (ManualResetEvent)_blockers[0];
                _blockers.Remove(_activeBlock);
            }
            else
            {
                _activeBlock = null;
            }
            SetModalState(false);
        }

        //private void CalibrationWork()
        //{
        //    // Remove Active Form (w/o rendering)
        //    Form prv = _ActiveForm;
        //    _ActiveForm = null;

        //    // Copy out the current buffer
        //    Bitmap buffer2 = new Bitmap(AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight);
        //    buffer2.DrawImage(0, 0, _buffer, 0, 0, buffer2.Width, buffer2.Height);

        //    // prepare points
        //    SettingsManager.PrepareCalibrationPoints();

        //    // tart the calibration process.
        //    TouchEventArgs e;
        //    int idx = 0;
        //    Touch.ActiveTouchPanel.StartCalibration();
        //    IM.Pause();

        //    // get Calibration Points
        //    while (true)
        //    {
        //        // draw crosshair
        //        Color background = Color.White;
        //        _buffer.DrawRectangle(Color.Black, 0, 0, 0, buffer2.Width, buffer2.Height, 0, 0, background, 0, 0, background, 0, 0, 256);
        //        DrawCrossHair(idx);
        //        _buffer.Flush();

        //        e = IM.GetTouchPoint();
        //        ++idx;
        //        SettingsManager.CalibrationPoints.TouchX[idx - 1] = (short)e.location.X;
        //        SettingsManager.CalibrationPoints.TouchY[idx - 1] = (short)e.location.Y;
        //        e = IM.GetTouchPoint(); // Don't forget about the touch up

        //        if (idx == SettingsManager.CalibrationPoints.PointCount)
        //        {
        //            // the last point has been reached, so set the calibration.
        //            SettingsManager.ApplyCalibrationPoints();
        //            SettingsManager.SaveCalibrationPoints();
        //            break;
        //        }

        //        //Thread.Sleep(1000);

        //    }

        //    // Restore Screen
        //    _ActiveForm = prv;
        //    _buffer.DrawImage(0, 0, buffer2, 0, 0, buffer2.Width, buffer2.Height);
        //    _buffer.Flush();

        //    // Restore Input State
        //    IM.Resume();
        //    if (_activeBlock != null)
        //        _activeBlock.Set();
        //}
        //private void DrawCrossHair(int idx)
        //{
        //    int x = SettingsManager.CalibrationPoints.ScreenX[idx];
        //    int y = SettingsManager.CalibrationPoints.ScreenY[idx];

        //    _buffer.DrawLine(Colors.Red, 1, x - 10, y, x - 2, y);
        //    _buffer.DrawLine(Colors.Red, 1, x + 10, y, x + 2, y);
        //    _buffer.DrawLine(Colors.Red, 1, x, y - 10, x, y - 2);
        //    _buffer.DrawLine(Colors.Red, 1, x, y + 10, x, y + 2);
        //}

        private void UpdateWork()
        {
            // Make sure we have a primary HDD
            //if (MyDrives.RootDirectory == "\\" && !DeviceManager.IsEmulator)
            //{
            //    if (!_IsSave)
            //        Prompt("Cannot updates without a drive prepared for " + Product + ".", "Alert", PromptType.OKOnly);
            //    return;
            //}

            // Make sure we're connected to the net
            //if (!MyNetwork.NetworkConnected)
            //{
            //    if (!_IsSave)
            //        Prompt("No network connection is available!", "Error", PromptType.OKOnly);
            //    return;
            //}

            //byte[] data = MyNetwork.DownloadURL("http://www.skewworks.com/updates.php?id=PyxisOS2");
            //if (data == null)
            //{
            //    if (!_IsSave) Prompt("Could not contact update server.", "Alert", PromptType.OKOnly);
            //    return;
            //}
            //else if (data.Length == 0)
            //{
            //    if (!_IsSave) Prompt("Could not locate update inFormation.", "Alert", PromptType.OKOnly);
            //    return;
            //}

            //int iDate = int.Parse(new string(Encoding.UTF8.GetChars(data)));
            //if (iDate <= PyxisAPI.BuildDate)
            //{
            //    if (!_IsSave) Prompt("Your system is up-to-date.", PyxisAPI.Product, PromptType.OKOnly);
            //    return;
            //}

            //if (_IsSave)
            //{
            //    MenuTrayIcon mti = new MenuTrayIcon(null, new Image32(Resources.GetBytes(Resources.BinaryResources.update)));
            //    mti.tapEvent += new OnTap(UpdateIcon_Tap);
            //    mti.TapHold += new OnTapHold(UpdateIcon_Tap);

            //    desktop._mnu._tray.AddIcon(mti);

            //    return;
            //}

            InstallUpdates();
        }
        private void InstallUpdates()
        {
            //string sNotes = new string(UTF8Encoding.UTF8.GetChars(MyNetwork.DownloadURL("http://www.skewworks.com/updatenotes.php?id=PyxisOS2")));
            //if (PromptDialog("Updates are available; would you like to download them now?\nDoing so will call all applications to close.", sNotes, "System Updates", PromptType.YesNo) == PromptResult.No) return;

            //// Prep Window
            //int y = (AppearanceManager.ScreenHeight / 2) - 26;
            //Form frmDownload = new Form(this, Colors.LightGray, 10, y, AppearanceManager.ScreenWidth - 20, 51, true, true, Form.WindowType.container);
            //Label lblStatus = new Label("Exiting Applications...", 8, 8);
            //Progressbar progDownload = new Progressbar(8, 28, frmDownload.Width - 16, 15);
            //frmDownload.AddChild(lblStatus);
            //frmDownload.AddChild(progDownload);
            //progDownload.Maximum = _runningApps.Count;

            //ScreenBuffer.DrawRectangle(Color.Black, 0, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Colors.Black, 0, 0, Colors.Black, 0, 0, 100);
            //ScreenBuffer.Flush();
            //ActiveForm = frmDownload;

            //// Quit Apps
            //while (_runningApps.Count > 0)
            //{
            //    PyxisApplication PA = (PyxisApplication)_runningApps[0];
            //    QuitApp(PA.Key);
            //    progDownload.Value++;
            //}

            //// Download File(s)
            //byte[] bFiles = MyNetwork.DownloadURL("http://www.skewworks.com/updatefiles.php?id=PyxisOS2");
            //string[] files = new string(UTF8Encoding.UTF8.GetChars(bFiles)).Split('|');
            //progDownload.Value = 0;
            //progDownload.Maximum = files.Length;
            //bool bDownload;
            //for (int i = 0; i < files.Length; i++)
            //{
            //    bDownload = true;
            //    if (files[i] == "ER_CONFIG" && DeviceManager.ActiveDevice != DeviceType.ChipworkX) bDownload = false;
            //    if (files[i] == "ER_FLASH" && DeviceManager.ActiveDevice != DeviceType.ChipworkX) bDownload = false;

            //    if (files[i] == "CLR.HEX" && DeviceManager.ActiveDevice != DeviceType.Cobra) bDownload = false;
            //    if (files[i] == "CLR2.HEX" && DeviceManager.ActiveDevice != DeviceType.Cobra) bDownload = false;
            //    if (files[i] == "Config.HEX" && DeviceManager.ActiveDevice != DeviceType.Cobra) bDownload = false;

            //    if (bDownload)
            //    {
            //        progDownload.Value = 0;
            //        lblStatus.Text = "Downloading '" + files[i] + "'";
            //        Debug.Print(">>" + files[i] + MyNetwork.DownloadFile("http://www.skewworks.com/downloads/updates/pyxis2/" + files[i], MyDrives.RootDirectory + "\\pyxis\\system\\" + files[i], progDownload, 5120));
            //        Debug.GC(true);
            //    }
            //}

            //// Prompt and restart
            //Prompt(PyxisAPI.Product + " must now reboot to apply updates.", "System Updates", PromptType.OKOnly);
            //SystemUpdate.AccessBootloader();
        }

        #endregion

        #region Event handlers
        private void UpdateIcon_Tap(object sender, Point e)
        {
            //ContextMenu CM = new ContextMenu();
            //ContextMenuItem cmiInstall = new ContextMenuItem("Install Updates");
            //cmiInstall.tapEvent += new GUI.Controls.MenuItemTap((object sender2) => InstallUpdates());
            //CM.Add(cmiInstall);
            //ShowContextMenu(CM, e);
        }


        #endregion
    }
}
