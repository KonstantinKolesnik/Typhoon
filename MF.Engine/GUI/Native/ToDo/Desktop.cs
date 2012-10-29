using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;
using System.Collections;
using MF.Engine.Managers;
using System.IO;
using GHIElectronics.NETMF.System;
using MF.Engine.Utilities;

namespace MF.Engine.GUI
{
    internal class Desktop : Container
    {
        #region Fields
        private Engine engine;

        internal Color _bkgColor = Colors.LightGray;            // Background Color
        internal Bitmap _bkgImage = null;                       // Background Image
        private ScaleMode _bkgScale = ScaleMode.Normal;         // Background Image Scaling

        //private bool _appstore;                                 // Display App Store Icon
        //private bool _filefinder;                               // Display File Finder Icon
        //private bool _pictureviewer;                            // Display Picture Viewer Icon
        //private bool _settings;                                 // Display Settings Icon

        //internal MenuBar _mnu;                                  // System MenuBar
        //private MenuItem mnuDesktop;                            // Desktop Menu
        //internal MenuItem mnuRun;                               // Pyxis Run Menu Item
        //internal MenuItem mnuForce;                             // Pyxis Force Quit Menu Item
        //private MenuItem mnuDSep;                               // Desktop separator
        //private MenuItem mnuPage;                               // Desktop page select

        //private ArrayList _icons = new ArrayList();             // Array of Icons waiting to be rendered to the buffer
        //private Bitmap _icoBuffer = null;                       // Buffer of Icon images used to speed up rendering

        //private int _hSpacing;                                  // Horizontal Icon Spacing
        //private int _vSpacing;                                  // Vertical Icon Spacing
        //private int _pages;                                     // # of Desktop Pages needed
        //private int _curPage;                                   // Current Desktop Page
        //private int _IPP;                                       // Icons Per Page

        //private DesktopIcon _ActiveIcon;                        // Actively selected icon
        //private ContextMenu _IconCM;                            // Icon ContextMenu
        #endregion



        #region Constructor
        public Desktop(Engine engine)
        {
            this.engine = engine;

            // Hook Events
            engine.MyDrives.DriveAdded += new OnDriveAdded(MyDrives_DriveAdded);
            engine.MyDrives.DriveRemoved += new OnDriveRemoved(MyDrives_DriveRemoved);

            // Calculate Icon Spacing
            //int IPR = (int)System.Math.Floor(AppearanceManager.ScreenWidth / 80);
            //_hSpacing = (int)System.Math.Floor((AppearanceManager.ScreenWidth - (IPR * 80)) / (IPR + 1));
            //int IPC = (int)System.Math.Floor((AppearanceManager.ScreenHeight - 24) / 74);
            //_vSpacing = (int)System.Math.Floor(((AppearanceManager.ScreenHeight - 24) - (IPC * 74)) / (IPC + 1));
            //_IPP = IPR * IPC;

            // Prep Desktop
            CreateMenus();
            DetectPrimaryHDD();
            if (SettingsManager.Boot.CheckUpdates)
                engine.CheckForUpdates(true);
        }
        #endregion

        #region Public Methods
        public void Activate()
        {
            //if (_mnu.count < 2)
            //{
            //    _mnu.add(mnuDesktop);
            //}
            //else
            //{
            //    if (_mnu.GetItemAt(1) != mnuDesktop)
            //    {
            //        _mnu.remove(1);
            //        _mnu.add(mnuDesktop);
            //    }
            //}

            //if (engine.MyDrives.RootDirectory != "\\")
            //    LoadDesktopIcons();

            Render();
        }
        #endregion

        #region Private Methods
        private void CreateMenus()
        {
            //// SYSTEM MENU
            //_mnu = new MenuBar(this, API, (DeviceManager.ActiveDevice == DeviceType.ChipworkX) ? true : false);
            //_mnu.Suspended = true;

            //MenuItem mnuMain = new MenuItem(PyxisAPI.Product);

            //MenuItem mnuAbout = new MenuItem("About " + PyxisAPI.Product);
            //mnuAbout.tapEvent += new MenuItemTap((object sender) => API.ShowAbout());
            //mnuMain.AddItem(mnuAbout);

            //MenuItem mnuUpdate = new MenuItem("Software Update");
            //mnuUpdate.tapEvent += new MenuItemTap((object sender) => API.CheckForUpdates(false));
            //mnuMain.AddItem(mnuUpdate);

            //MenuItem mnusSepApps = new MenuItem("-");
            //mnuMain.AddItem(mnusSepApps);

            //if (DeviceManager.ActiveDevice == DeviceType.ChipworkX)
            //{
            //    MenuItem mnuMount = new MenuItem((API.MyDrives.SDMounted) ? "Unmount SD" : "Mount SD");
            //    mnuMount.tapEvent += new MenuItemTap((object sender) => SDHandler(mnuMount));
            //    mnuMain.AddItem(mnuMount);
            //}

            //MenuItem mnuSysTools = new MenuItem("System Tools");

            //MenuItem mnuAS = new MenuItem("App Market");
            //mnuAS.tapEvent += new MenuItemTap((object sender) => AppStore.Show(API));
            //mnuSysTools.AddItem(mnuAS);

            //MenuItem mnuFF = new MenuItem("File Finder");
            //mnuFF.tapEvent += new MenuItemTap((object sender) => FileFinder.Show(API));
            //mnuSysTools.AddItem(mnuFF);

            //MenuItem mnuPV = new MenuItem("Picture Viewer");
            //mnuPV.tapEvent += new MenuItemTap((object sender) => PictureViewer.Show(API, null));
            //mnuSysTools.AddItem(mnuPV);

            //MenuItem mnuSettings = new MenuItem("Settings");
            //mnuSettings.tapEvent += new MenuItemTap((object sender) => SettingsWin.Show(API));
            //mnuSysTools.AddItem(mnuSettings);

            //mnuSysTools.AddItem(new MenuItem("-"));

            //MenuItem mnuCalibrate = new MenuItem("Calibrate LCD");
            //mnuCalibrate.tapEvent += new MenuItemTap((object sender) => API.CalibrateScreen());
            //mnuSysTools.AddItem(mnuCalibrate);

            //MenuItem mnuDesk = new MenuItem("Show Desktop");
            //mnuDesk.tapEvent += new MenuItemTap((object target) => API.DisplayDesktop());
            //mnuMain.AddItem(mnuDesk);

            //mnuMain.AddItem(mnuSysTools);
            //mnuMain.AddItem(new MenuItem("-"));

            //mnuRun = new MenuItem("Switch To");
            //mnuRun.Enabled = false;
            //mnuMain.AddItem(mnuRun);

            //mnuForce = new MenuItem("Force Quit");
            //mnuForce.Enabled = false;
            //mnuMain.AddItem(mnuForce);

            //_mnu.add(mnuMain);
            //_mnu.Suspended = false;
            //_mnu._buffer = null;

            //// DESKTOP MENU
            //mnuDesktop = new MenuItem("Desktop");
            //MenuItem mnuRefresh = new MenuItem("Refresh");
            //mnuRefresh.tapEvent += new MenuItemTap((object sender) => LoadDesktopIcons());
            //mnuDesktop.AddItem(mnuRefresh);
            //mnuDSep = new MenuItem("-");
            //mnuDSep.Visible = false;
            //mnuPage = new MenuItem("Show Page");
            //mnuPage.Visible = false;
            //mnuDesktop.AddItem(mnuDSep);
            //mnuDesktop.AddItem(mnuPage);

            //_mnu.add(mnuMain);

            //// ICON CONTEXT MENU
            //ContextMenuItem cmiOpen = new ContextMenuItem("Run");
            //cmiOpen.tapEvent += new GUI.Controls.MenuItemTap(RunIcon);

            //ContextMenuItem cmiSep = new ContextMenuItem("-");

            //ContextMenuItem cmiRename = new ContextMenuItem("Rename");

            //ContextMenuItem cmiDelete = new ContextMenuItem("Delete");
            //cmiDelete.tapEvent += new GUI.Controls.MenuItemTap(DeleteIcon);

            //_IconCM = new ContextMenu(new ContextMenuItem[] { cmiOpen, cmiSep, cmiRename, cmiDelete });
        }
        private void DetectPrimaryHDD()
        {
            //bool needsLoad = (engine.MyDrives.RootDirectory == "\\");

            //Drive[] drives = engine.MyDrives.AvailableDrives;

            //for (int i = 0; i < drives.Length; i++)
            //{
            //    if (IsOSInstalled(drives[i].RootName + "\\"))
            //    {
            //        engine.MyDrives.RootDirectory = FileManager.NormalizeDirectory(drives[i].RootName);
            //        if (needsLoad)
            //        {
            //            if (File.Exists(engine.MyDrives.RootDirectory + "bootloaderdemand.txt"))
            //                SystemUpdate.AccessBootloader();
            //            LoadDefaultDrive();
            //        }
            //        return;
            //    }
            //}

            //if (needsLoad)
            //{
            //    if (drives.Length > 0)
            //    {
            //        if (engine.Prompt("You have " + drives.Length + " drives available.\nWould you like to prepare a drive for Pyxis?", "Question", PromptType.YesNo) == PromptResult.Yes)
            //        {
            //            if (drives.Length == 1)
            //            {
            //                if (engine.PrepHDD(engine.MyDrives.AvailableDrives[0]))
            //                {
            //                    DetectPrimaryHDD();
            //                    Render();
            //                    return;
            //                }
            //                else
            //                {
            //                    engine.Prompt("Could not prepare HDD.\nMake sure there is sufficient room and the drive is not write protected.", "Error", PromptType.OKOnly);
            //                }
            //            }
            //            else
            //            {
            //                string myDrive = engine.SelectDrive();
            //                int index = -1;
            //                for (int i = 0; i < engine.MyDrives.DriveRoots.Length; i++)
            //                {
            //                    if (engine.MyDrives.DriveRoots[i] + "\\" == myDrive)
            //                    {
            //                        index = i;
            //                        break;
            //                    }
            //                }

            //                if (engine.PrepHDD(engine.MyDrives.AvailableDrives[0]))
            //                {
            //                    DetectPrimaryHDD();
            //                    Render();
            //                    return;
            //                }
            //                else
            //                {
            //                    engine.Prompt("Could not prepare HDD.\nMake sure there is sufficient room and the drive is not write protected.", "Error", PromptType.OKOnly);
            //                }

            //            }
            //        }
            //    }
            //    LoadDefaultDrive();
            //}
        }
        private void LoadDefaultDrive()
        {
            //engine.MyFiles.LoadAssociations();

            //if (engine.MyDrives.RootDirectory == "\\")
            //{
            //    // Set Default Desktop
            //    _bkgImage = new Bitmap(EmbeddedResourceManager.GetWallpaperByName("greenstripes"), Bitmap.BitmapImageType.Jpeg);
            //    LoadDesktopIcons();

            //    // Update Menu Items
            //    //mnuSwitch.Enabled = false;
            //    mnuRun.Enabled = false;
            //    mnuForce.Enabled = false;
            //}
            //else
            //{
            //    LoadDesktopSettings();
            //    LoadDesktopIcons();

            //    //mnuSwitch.Enabled = true;
            //}
        }
        private void LoadDesktopIcons()
        {
            //int x = _hSpacing;
            //int y = 24 + _vSpacing;
            //bool bVisible = true;
            //MenuItem mnu;
            //DesktopIcon DI;

            //_icoBuffer = null;
            //_icons.Clear();
            //mnuPage.ClearItems();
            //mnuPage.Visible = false;
            //mnuDSep.Visible = false;
            //mnu = new MenuItem("Page 1");
            //mnu.tapEvent += new MenuItemTap((object sender) => LoadDesktopPage(1));
            //mnuPage.AddItem(mnu);
            //_curPage = 1;
            //_pages = 1;

            //// App Store Icon
            //if (_appstore)
            //{
            //    DI = new DesktopIcon(API, "App Market", "-1", EmbeddedResourceManager.GetImage32BytesByName("AppStore"), x, y, 76, 69, true);
            //    _icons.Add(DI);
            //    x += _hSpacing + 80;
            //}

            //// File Finder Icon
            //if (_filefinder)
            //{
            //    DI = new DesktopIcon(API, "File Finder", "-2", EmbeddedResourceManager.GetImage32BytesByName("FileFinder"), x, y, 76, 69, true);
            //    _icons.Add(DI);
            //    x += _hSpacing + 80;
            //}

            //// Picture Viewer Icon
            //if (_pictureviewer)
            //{
            //    DI = new DesktopIcon(API, "Picture Viewer", "-3", EmbeddedResourceManager.GetImage32BytesByName("PictureViewer"), x, y, 76, 69, true);
            //    _icons.Add(DI);
            //    x += _hSpacing + 80;
            //}

            //// Settings Icon
            //if (_settings)
            //{
            //    DI = new DesktopIcon(API, "Settings", "-4", EmbeddedResourceManager.GetImage32BytesByName("Settings"), x, y, 76, 69, true);
            //    _icons.Add(DI);
            //    x += _hSpacing + 80;
            //}

            //// Check overflow
            //if (x + 78 >= AppearanceManager.ScreenWidth)
            //{
            //    x = _hSpacing;
            //    y += 74 + _vSpacing;
            //}

            //if (API.MyDrives.RootDirectory != "\\")
            //{
            //    string[] Files = Directory.GetFiles(API.MyDrives.RootDirectory + "pyxis\\desktop\\");
            //    for (int j = 0; j < Files.Length; j++)
            //    {
            //        if (Path.GetExtension(Files[j]).ToLower() == ".lnk")
            //        {
            //            try
            //            {
            //                DI = new DesktopIcon(API, Files[j], x, y, 76, 69, bVisible);
            //                DI.TapHold += new OnTapHold(IconTapHold);
            //                _icons.Add(DI);
            //                x += _hSpacing + 80;
            //                if (x + 78 >= AppearanceManager.ScreenWidth)
            //                {
            //                    x = _hSpacing;
            //                    y += 74 + _vSpacing;
            //                    if (y + 74 + _vSpacing >= AppearanceManager.ScreenHeight)
            //                    {
            //                        bVisible = false;
            //                        x = _hSpacing;
            //                        y = 24 + _vSpacing;

            //                        _pages++;
            //                        mnu = new MenuItem("Page " + _pages);
            //                        mnu.tapEvent += new MenuItemTap((object sender) => LoadDesktopPage(_pages));
            //                        mnuPage.Visible = true;
            //                        mnuDSep.Visible = true;
            //                        mnuPage.AddItem(mnu);

            //                    }
            //                }
            //            }
            //            catch (Exception)
            //            { }
            //        }
            //    }
            //}
        }
        private void LoadDesktopPage(int index)
        {
            //int startIndex = (_curPage - 1) * _IPP;
            //int i;
            //DesktopIcon DI;

            //// We're already here
            //if (index == _curPage) return;

            //// Hide Current Page
            //for (i = startIndex; i < startIndex + _IPP; i++)
            //{
            //    DI = (DesktopIcon)_icons[i];
            //    DI.Visible = false;
            //    if (i == _icons.Count - 1) break;
            //}

            //// Show New Page
            //startIndex = (index - 1) * _IPP;
            //for (i = startIndex; i < startIndex + _IPP; i++)
            //{
            //    DI = (DesktopIcon)_icons[i];
            //    DI.Visible = true;
            //    if (i == _icons.Count - 1) break;
            //}

            //// Update Buffer
            //_curPage = index;
            //_icoBuffer = null;
            //Render(true);
        }
        private void LoadDesktopSettings()
        {
            //engine.ScreenBuffer.DrawRectangle(Colors.Black, 1, 0, 0, AppearanceManager.ScreenWidth, AppearanceManager.ScreenHeight, 0, 0, Colors.Black, 0, 0, Colors.Black, 0, 0, 256);
            //int tY = (AppearanceManager.ScreenHeight / 2) - (FontManager.ArialBold.Height / 2);
            //engine.ScreenBuffer.DrawTextInRect("...Loading Desktop...", 0, tY, AppearanceManager.ScreenWidth, FontManager.ArialBold.Height, Bitmap.DT_AlignmentCenter, Colors.White, FontManager.ArialBold);
            //engine.ScreenBuffer.Flush();

            //_mnu._buffer = null;
            //_icoBuffer = null;


            //// Read system settings
            //XMLReaderEX xr = new XMLReaderEX(API.MyDrives.RootDirectory + "\\pyxis\\system\\desktop.Xml");
            //XMLNodeEX node = (XMLNodeEX)xr.Nodes[0];
            //node = node.NodeByName("Appearance");
            //string sPaper = node.NodeByName("Wallpaper").value;
            //if (sPaper.IndexOf("\\") < 0) sPaper = API.MyDrives.RootDirectory + "pyxis\\wallpapers\\" + sPaper;
            //_appstore = (node.NodeByName("ShowAppStore").value == "1") ? true : false;
            //_filefinder = (node.NodeByName("ShowFileFinder").value == "1") ? true : false;
            //_settings = (node.NodeByName("ShowSettings").value == "1") ? true : false;
            //_pictureviewer = (node.NodeByName("ShowPictureViewer").value == "1") ? true : false;
            //string[] vals = node.NodeByName("Backcolor").value.Split(',');
            //_bkgColor = ColorUtility.ColorFromRGB(byte.Parse(vals[0]), byte.Parse(vals[1]), byte.Parse(vals[2]));
            //_bkgScale = (Skewworks.Pyxis.ScaleMode)(int.Parse(node.NodeByName("WallpaperMode").value));

            //// Set wallpaper
            //try
            //{
            //    if (sPaper.IndexOf("\\") < 0) sPaper = API.MyDrives.RootDirectory + "\\pyxis\\wallpapers\\" + sPaper;
            //    FileStream iFile = new FileStream(sPaper, FileMode.Open);
            //    byte[] b = new byte[iFile.Length];
            //    iFile.Read(b, 0, b.Length);
            //    iFile.Close();
            //    _bkgImage = Utils.ImageFromBytes(b);
            //}
            //catch (Exception) { }

            //Render();
        }
        private bool IsOSInstalled(string DeviceRoot)
        {
            // Check Directories
            string[] paths = new string[] { "apps\\", "documents\\", "documents\\temp\\", "pictures\\", "pyxis\\", "pyxis\\desktop\\", "pyxis\\fonts", "pyxis\\wallpapers\\", "pyxis\\system\\" };
            for (int i = 0; i < paths.Length; i++)
                if (!Directory.Exists(DeviceRoot + paths[i]))
                    return false;

            // Check Files
            string[] files = new string[] { "pyxis\\system\\desktop.Xml" };
            for (int i = 0; i < files.Length; i++)
                if (!File.Exists(DeviceRoot + files[i]))
                    return false;

            // We're good
            return true;
        }
        //private void SDHandler(MenuItem mnuMount)
        //{
        //    if (engine.MyDrives.SDMounted)
        //    {
        //        engine.MyDrives.UnmountSD();
        //        mnuMount.Text = "Mount SD";
        //    }
        //    else
        //    {
        //        if (engine.MyDrives.MountSD())
        //        {
        //            mnuMount.Text = "Unmount SD";
        //        }
        //        else
        //        {
        //            engine.Prompt("Could not mount SD", Engine.Product, PromptType.OKOnly);
        //        }
        //    }
        //}
        #endregion

        #region Event handlers
        private void MyDrives_DriveAdded(string root)
        {
            if (engine.MyDrives.RootDirectory == "\\")
            {
                DetectPrimaryHDD();
                Render();
            }
        }
        private void MyDrives_DriveRemoved(string root)
        {
            if (engine.MyDrives.RootDirectory == root)
            {
                engine.MyDrives.RootDirectory = "\\";
                DetectPrimaryHDD();
                Render();
            }
        }



        #endregion


    }
}
