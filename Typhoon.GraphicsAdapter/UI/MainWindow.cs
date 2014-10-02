using GHI.Premium.System;
using GHI.Premium.USBHost;
using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.USB;
using Microsoft.SPOT;
using System;
using Typhoon.Server;
using Typhoon.Server.Hardware;

namespace Typhoon.GraphicsAdapter.UI
{
    public class MainWindow : Window
    {
        #region Fields
        private Panel rootPanel;
        private Control hub;

        private HubHome hubHome;
        private HubSettings hubSettings;


        private Button btnHome;

        private Button btnPower;
        private MultiImage iconPowerState;
        
        private Image iconUSBMouse;
        private Image iconUSBKeyboard;
        private Image iconUSBMassSrorage;
        
        private MultiImage iconNetwork;

        private MultiImage iconMainBooster1State;
        private Level lvlMainBooster1Current;
        private Label lblMainBooster1Current;

        private MultiImage iconMainBooster2State;
        private Level lvlMainBooster2Current;
        private Label lblMainBooster2Current;

        private MultiImage iconProgBoosterState;
        private Level lvlProgBoosterCurrent;
        private Label lblProgBoosterCurrent;

        private Label lblClock;
        #endregion

        #region Properties
        public HubHome HubHome
        {
            get
            {
                if (hubHome == null)
                {
                    hubHome = new HubHome();
                    hubHome.ShortcutClick += new EventHandler(hubHome_ShortcutClick);
                }

                return hubHome;
            }
        }
        public HubSettings HubSettings
        {
            get
            {
                if (hubSettings == null)
                    hubSettings = new HubSettings();

                return hubSettings;
            }
        }
        #endregion

        #region Constructor
        public MainWindow()
        {
            Beeper.PlaySound(Beeper.SoundID.Click);

            InitUI();

            USBDeviceManager.USBDeviceInserted += new USBH_DeviceConnectionEventHandler(USBDevice_Inserted);
            USBDeviceManager.USBDeviceRemoved += new USBH_DeviceConnectionEventHandler(USBDevice_Removed);
            //Program.Engine.SDCardInserted += new InsertEventHandler(SDCard_Inserted_Removed);
            //Program.Engine.SDCardRemoved += new EjectEventHandler(SDCard_Inserted_Removed);
            //Model.PropertyChanged += new PropertyChangedEventHandler(Model_PropertyChanged);
            //Model.BoosterPropertyChanged += new PropertyChangedEventHandler(Booster_PropertyChanged);

            //new Thread(delegate()
            //{
            //    while (true)
            //    {
            //        RTCTimer_Tick();
            //        Thread.Sleep(1000);
            //    }
            //}).Start();

            Debug.GC(true);
        }
        #endregion

        #region Public methods
        public void SetHub(Control page)
        {
            if (hub != null)
            {
                if (hub is Hub)
                    (hub as Hub).Exit();
                Children.Remove(hub);
                //hub.Visible = false;
            }

            hub = page;
            btnHome.Visible = hub != hubHome;

            hub.Visible = true;
            Children.Add(hub);
        }
        #endregion

        #region Private methods
        private void InitUI()
        {
            Background = new SolidColorBrush(Color.Black);
            Children.Add(InitStatusBar());
            SetHub(HubHome);
        }
        private Control InitStatusBar()
        {
            Panel statusPanel = new Panel(0, Height - 32, Width, 32);
            statusPanel.Background = new ImageBrush(Program.ToolbarBackground) { Opacity = Program.ToolbarOpacity, Stretch = Stretch.Fill };

            // left part
            btnHome = new Button(0, 0, 70, 32) { Foreground = new ImageBrush(Program.ImageHome) };//, new Image(Program.ImageHome, 22));
            btnHome.Border = null;
            btnHome.Click += new EventHandler(btnHome_Click);
            statusPanel.Children.Add(btnHome);

            // right part
            int x = statusPanel.Width - 400;

            iconUSBMassSrorage = new Image(x, 0, Program.ButtonLargeIconSize, Program.ButtonLargeIconSize, Program.ImageDrive);
            iconUSBMassSrorage.Visible = USBDeviceManager.GetCount(USBH_DeviceType.MassStorage) != 0;
            statusPanel.Children.Add(iconUSBMassSrorage);
            x += iconUSBMassSrorage.Width + 4;

            iconUSBMouse = new Image(x, 0, Program.ButtonLargeIconSize, Program.ButtonLargeIconSize, Program.ImageMouse);
            iconUSBMouse.Visible = USBDeviceManager.GetCount(USBH_DeviceType.Mouse) != 0;
            statusPanel.Children.Add(iconUSBMouse);
            x += iconUSBMouse.Width + 4;

            iconUSBKeyboard = new Image(x, 0, Program.ButtonLargeIconSize, Program.ButtonLargeIconSize, Program.ImageKeyboard);
            iconUSBKeyboard.Visible = USBDeviceManager.GetCount(USBH_DeviceType.Keyboard) != 0;
            statusPanel.Children.Add(iconUSBKeyboard);
            x += iconUSBKeyboard.Width + 4;

            iconNetwork = new MultiImage(x, 0, Program.ButtonLargeIconSize, Program.ButtonLargeIconSize);
            iconNetwork["NetworkOn"] = new ImageBrush(Program.ImageNetworkOn);
            iconNetwork["NetworkOff"] = new ImageBrush(Program.ImageNetworkOff);
            iconNetwork.ActiveBrushID = "NetworkOff";
            statusPanel.Children.Add(iconNetwork);
            x += iconNetwork.Width + 8;

            statusPanel.Children.Add(InitBoosterUIBlock(ref x, out iconMainBooster1State, out lblMainBooster1Current, out lvlMainBooster1Current));
            statusPanel.Children.Add(InitBoosterUIBlock(ref x, out iconMainBooster2State, out lblMainBooster2Current, out lvlMainBooster2Current));
            statusPanel.Children.Add(InitBoosterUIBlock(ref x, out iconProgBoosterState, out lblProgBoosterCurrent, out lvlProgBoosterCurrent));

            x += 4;
            btnPower = new Button(x, 0, 70, Program.ButtonLargeIconSize);
            btnPower.Foreground = new ImageBrush(Program.ImagePowerOff);
            btnPower.Border = null;
            btnPower.Click += new EventHandler(btnPower_Click);
            statusPanel.Children.Add(btnPower);
            x += btnPower.Width + 10;

            lblClock = new Label(x, 9, Program.FontRegular, "00:00:00") { ForeColor = Program.LabelTextColor };
            statusPanel.Children.Add(lblClock);

            return statusPanel;
        }
        private Control InitBoosterUIBlock(ref int x, out MultiImage miconBoosterState, out Label lblBoosterCurrent, out Level lvlBoosterCurrent)
        {
            Panel panelBooster = new Panel(x, 0, 30, 32);

            miconBoosterState = new MultiImage(9, 0, 12, 12);
            miconBoosterState["LedGray"] = new ImageBrush(Program.ImageLedGray);
            miconBoosterState["LedGreen"] = new ImageBrush(Program.ImageLedGreen);
            miconBoosterState["LedRed"] = new ImageBrush(Program.ImageLedRed);
            miconBoosterState.ActiveBrushID = "LedGray";
            panelBooster.Children.Add(miconBoosterState);

            lvlBoosterCurrent = new Level(0, 13, 30, 10, Orientation.Horizontal, 10);
            lvlBoosterCurrent.Foreground = new LinearGradientBrush(Color.LimeGreen, Color.Black);
            panelBooster.Children.Add(lvlBoosterCurrent);

            lblBoosterCurrent = new Label(0, 24, Program.FontRegular, "0000 mA") { ForeColor = Program.LabelTextColor };//, Program.LabelTextColor);
            //lblBoosterCurrent.Width = 45;
            //panelBooster.Children.Add(lblBoosterCurrent);

            x += 30 + 8;

            return panelBooster;
        }
        
        private void UpdateFileBrowser()
        {
            //Dispatcher.BeginInvoke((DispatcherOperationCallback)(delegate(object param)
            //{
            //    foreach (Tab tab in tasks.Children)
            //        if (tab.Content != null && tab.Content is PageFileBrowser)
            //        {
            //            (tab.Content as PageFileBrowser).Update();
            //            break;
            //        }

            //    return null;
            //}), null);
        }
        #endregion

        #region Event handlers

        #region Own events
        private void RTCTimer_Tick()
        {
            DateTime dt = DateTime.Now;

            string hour = (dt.Hour < 10) ? "0" + dt.Hour.ToString() : dt.Hour.ToString();
            string minute = (dt.Minute < 10) ? "0" + dt.Minute.ToString() : dt.Minute.ToString();
            string second = (dt.Second < 10) ? "0" + dt.Second.ToString() : dt.Second.ToString();
            string pm = "";// (dt.Hour < 12) ? "AM" : "PM";
            string result = hour + ":" + minute + ":" + second + " " + pm;

            lblClock.Text = result;
        }
        private void btnPower_Click(object sender, EventArgs e)
        {
            Model.IsPowerOn = !Model.IsPowerOn;
        }
        private void btnHome_Click(object sender, EventArgs e)
        {
            SetHub(HubHome);
        }
        private void hubHome_ShortcutClick(object sender, EventArgs e)
        {
            switch ((sender as ToolButton).Name)
            {
                case "scDatabase":

                    break;
                case "scOperation":

                    break;
                case "scTopology":

                    break;
                case "scSettings":
                    SetHub(HubSettings);
                    break;

                default: break;
            }
        }
        #endregion

        #region Model events
        private void USBDevice_Inserted(USBH_Device device)
        {
            switch (device.TYPE)
            {
                case USBH_DeviceType.MassStorage:
                    iconUSBMassSrorage.Visible = true;
                    UpdateFileBrowser();
                    break;
                case USBH_DeviceType.Keyboard:
                    iconUSBKeyboard.Visible = true;
                    //((USBH_Keyboard)Model.USBDevices.Keyboard[0]).KeyUp += OnKeyUp;
                    break;
                case USBH_DeviceType.Mouse:
                    iconUSBMouse.Visible = true;
                    // HandleMouse(device);
                    break;
                default:
                    break;
            }
        }
        private void USBDevice_Removed(USBH_Device device)
        {
            switch (device.TYPE)
            {
                case USBH_DeviceType.MassStorage:
                    iconUSBMassSrorage.Visible = false;
                    UpdateFileBrowser();
                    break;
                case USBH_DeviceType.Keyboard:
                    iconUSBKeyboard.Visible = false;
                    //((USBH_Keyboard)Model.USBDevices.Keyboard[0]).KeyUp -= OnKeyUp;
                    break;
                case USBH_DeviceType.Mouse:
                    iconUSBMouse.Visible = false;
                    //UnhandleMouse();
                    break;
                default:
                    break;
            }
        }
        //private void SDCard_Inserted_Removed(object sender, MediaEventArgs e)
        //{
        //    UpdateFileBrowser();
        //}

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.Property)
            {
                case "IsPowerOn":
                    btnPower.Foreground = new ImageBrush((bool)e.NewValue ? Program.ImagePowerOn : Program.ImagePowerOff);
                    break;
                default: break;
            }
        }
        private void Booster_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MultiImage iconBoosterState = null;
            Label lblBoosterCurrent = null;
            Level lvlBoosterCurrent = null;
            if (sender == Model.MainBooster)
            {
                iconBoosterState = iconMainBooster1State;
                lblBoosterCurrent = lblMainBooster1Current;
                lvlBoosterCurrent = lvlMainBooster1Current;
            }
            else if (sender == Model.ProgBooster)
            {
                iconBoosterState = iconProgBoosterState;
                lblBoosterCurrent = lblProgBoosterCurrent;
                lvlBoosterCurrent = lvlProgBoosterCurrent;
            }

            switch (e.Property)
            {
                case "IsActive":
                    iconBoosterState.ActiveBrushID = (bool)e.NewValue ? "LedGreen" : "LedGray";
                    break;
                case "IsOverloaded":
                    if ((bool)e.NewValue)
                        iconBoosterState.ActiveBrushID = "LedRed";
                    break;
                case "Current":
                    int current = (int)e.NewValue;
                    //int percent = current * 100 / Model.Options.MainBridgeCurrentThreshould;

                    //lblBoosterCurrent.Text = current.ToString() + " mA";
                    //lvlBoosterCurrent.Value = percent;
                    break;
                default: break;
            }
        }
        #endregion

        #endregion 
    }
}
