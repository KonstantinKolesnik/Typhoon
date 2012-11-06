using System;
using System.Net.Sockets;
using System.Threading;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;

namespace MFE.Net.Managers
{
    public class WiFiManager : INetworkManager
    {
        #region Fields
        private ManualResetEvent blocker = null;
        private PWM portNetworkLED = null;
        private string ssid;
        private string password;
        
        // ChipworkX Developement System V1.5 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        //private SPI.SPI_module _spi = SPI.SPI_module.SPI2; /*SPI bus*/
        //private Cpu.Pin _cs = ChipworkX.Pin.PC9; /*ChipSelect*/
        //private Cpu.Pin _extInt = ChipworkX.Pin.PA19; /*External Interrupt*/
        //private Cpu.Pin _reset = ChipworkX.Pin.PC8; /*Reset*/
        /*-------------------------------------------------------------------*/
        // EMX Developement System V1.3 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        //private SPI.SPI_module _spi = SPI.SPI_module.SPI1; /*SPI bus*/
        //private Cpu.Pin _cs = EMX.Pin.IO2; /*ChipSelect*/
        //private Cpu.Pin _extInt = EMX.Pin.IO26; /*External Interrupt*/
        //private Cpu.Pin _reset = EMX.Pin.IO3; /*Reset*/
        /*-------------------------------------------------------------------*/
        // FEZ Cobra OEM board V1.2 or V1.3 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
        private SPI.SPI_module _spi = SPI.SPI_module.SPI2; /*SPI bus*/
        private Cpu.Pin _cs = (Cpu.Pin)2; /*ChipSelect*/
        private Cpu.Pin _extInt = (Cpu.Pin)26; /*External Interrupt*/
        private Cpu.Pin _reset = (Cpu.Pin)3; /*Reset*/
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public WiFiManager(PWM.Pin pinNetworkStatusLED, string ssid, string password)
        {
            blocker = new ManualResetEvent(false);
            portNetworkLED = new PWM(pinNetworkStatusLED);

            this.ssid = ssid;
            this.password = password;

            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            portNetworkLED.Set(1, 50); // blink LED with 1 Hz

            if (EnableWiFi())
            {
                //WiFiNetworkInfo ni = Scan();
                WiFiNetworkInfo ni = Scan2();

                if (Connect(ni) && EnableDHCP())
                {
                    portNetworkLED.Set(true);
                    if (Started != null)
                        Started(this, EventArgs.Empty);

                    //Debug.Print("Test DNS");
                    //try
                    //{
                    //    IPHostEntry myIP = Dns.GetHostEntry("www.ghielectronics.com");
                    //    if (myIP != null)
                    //        Debug.Print(myIP.HostName + ": " + myIP.AddressList[0].ToString());
                    //}
                    //catch (SocketException e)
                    //{
                    //    Debug.Print("Faild to Get the host entry of the FQN from DNS server!");
                    //    if (e.ErrorCode == 11003)
                    //        Debug.Print("Re-Enable the module.");
                    //    throw;
                    //}

                    return;
                }
            }

            portNetworkLED.Set(false);



            //if (!WiFi.IsLinkConnected)
            //{
            //    blocker.Reset();
            //    while (!blocker.WaitOne(5000, false))
            //    {
            //        if (!WiFi.IsLinkConnected)
            //        {
            //            //Debug.Print("Waiting for Network");
            //        }
            //        else
            //            break;
            //    }
            //}
        }

        public void OnBeforeMessage()
        {
            portNetworkLED.Set(false);
        }
        public void OnAfterMessage()
        {
            portNetworkLED.Set(true);
        }
        #endregion

        #region Event handlers
        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            //Debug.Print("Network Availability Event Triggered");
            if (e.IsAvailable)
            {
                if (WiFi.IsEnabled) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (WiFi.IsLinkConnected)
                    {
                        blocker.Set();
                        //Debug.Print("WiFi connection was established!");
                    }
            }
            else
            {
                if (WiFi.IsEnabled) // make sure that the event is fired by WiFi interface, not other networking interface.
                    if (!WiFi.IsLinkConnected)
                    {
                        blocker.Set();
                        //Debug.Print("WiFi connection was dropped or disconnected!");
                        if (Stopped != null)
                            Stopped(this, EventArgs.Empty);

                        Start();
                    }
            }
        }
        #endregion

        #region Private methods
        private bool EnableWiFi()
        {
            try
            {
                //Thread.Sleep(2000);
                WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible, _spi, _cs, _extInt, _reset);
            }
            catch (WiFi.WiFiException e)
            {
                switch (e.errorCode)
                {
                    case WiFi.WiFiException.ErrorCode.HardwareFirmwareVersionMismatch:
                        WiFi.UpdateFirmware(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible, _spi, _cs, _extInt, _reset);
                        WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible, _spi, _cs, _extInt, _reset);
                        break;
                    case WiFi.WiFiException.ErrorCode.HardwareCommunicationFailure:
                    case WiFi.WiFiException.ErrorCode.HardwareNotEnabled:
                    case WiFi.WiFiException.ErrorCode.HardwareCommunicationTimeout:
                        //Debug.Print("Error Message: " + e.ErrorMsg);
                        //Debug.Print("Check WiFi module hardware connections and SPI/signals configurations.");
                        WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible, _spi, _cs, _extInt, _reset);
                        break;
                    default:
                        //Debug.Print("Error Message: " + e.ErrorMsg);
                        return false;
                }
            }
            catch (Exception e)
            {
                //Debug.Print("Error Message: " + e.Message);
                return false;
            }
            
            if (WiFi.IsEnabled)
            {
                //Debug.Print("\nEnabled successfully!\nAt this point, the on-board LED on RS9110_N_11_21_1_Compatible module is ON.\n");
                return true;
            }

            return false;
        }
        private WiFiNetworkInfo Scan()
        {
            WiFiNetworkInfo myNi = null;
            while (myNi == null)
            {
                // scan for all networks:
                WiFiNetworkInfo[] nis = null;
                while (nis == null)
                {
                    Thread.Sleep(500);
                    Debug.Print("Searching for WiFi access points...");
                    nis = WiFi.Scan();
                }

                // output networks info:
                Debug.Print("Found " + nis.Length.ToString() + " network(s):");
                foreach (WiFiNetworkInfo ni in nis)
                {
                    Debug.Print("-----------------------------------------------------");
                    Debug.Print(WiFiNetworkInfoToString(ni));
                }
                Debug.Print("-----------------------------------------------------");

                // check for required SSID:
                Debug.Print("Check for required SSID...");
                foreach (WiFiNetworkInfo ni in nis)
                    if (string.Compare(ni.SSID, ssid) == 0)
                    {
                        myNi = ni;
                        break;
                    }
            }

            return myNi;
        }
        private WiFiNetworkInfo Scan2()
        {
            // scan for network with required SSID:
            WiFiNetworkInfo[] nis = WiFi.Scan(ssid);
            while (nis == null)
            {
                Thread.Sleep(500);
                //Debug.Print("Searching for WiFi access point with required SSID...\n");
                nis = WiFi.Scan(ssid);
            }
            //Debug.Print(WiFiNetworkInfoToString(nis[0]));
            return nis[0];
        }
        private bool Connect(WiFiNetworkInfo ni)
        {
            //Debug.Print("Connecting to " + ni.SSID + "...");
            blocker.Reset();
            try
            {
                WiFi.Join(ni, password);
            }
            catch (WiFi.WiFiException e)
            {
                switch (e.errorCode)
                {
                    case WiFi.WiFiException.ErrorCode.AuthenticationFailed: Debug.Print("AuthenticationFailed"); break;
                    default: Debug.Print(e.errorCode.ToString()); break;
                }
                //Debug.Print("Error Message: " + e.ErrorMsg);
                return false;
            }
            //Debug.Print("Done connecting...\n");
            blocker.WaitOne();
            //Debug.Print("We got NetworkAvailable event. WiFi link is ready!\n");
            return true;
        }
        private bool EnableDHCP()
        {
            //Debug.Print("Enable DHCP...\n");
            try
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();

                #region DHCP Code (dynamic IP)
                if (!nis[0].IsDhcpEnabled)
                    nis[0].EnableDhcp(); // This function is blocking
                else
                    nis[0].RenewDhcpLease(); // This function is blocking
                #endregion

                #region Static IP code
                // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
                //netif[0].EnableStaticIP("192.168.137.2", "255.255.255.0", "192.168.137.1");
                //netif[0].EnableStaticDns(new string[] { "10.1.10.1" });
                #endregion

                //Debug.Print("Network settings:");
                //Debug.Print("IP Address: " + nis[0].IPAddress);
                //Debug.Print("Subnet Mask: " + nis[0].SubnetMask);
                //Debug.Print("Default Getway: " + nis[0].GatewayAddress);
                //Debug.Print("DNS Server: " + nis[0].DnsAddresses[0]);

                return true;
            }
            catch (SocketException e)
            {
                //Debug.Print("DHCP faild");
                if (e.ErrorCode == 11003)
                    Debug.Print("Re-Enable the module.");
            }

            return false;
        }
        private void Disconnect()
        {
            //Debug.Print("Disconnect WiFi link.");
            blocker.Reset();
            WiFi.Disconnect();
            blocker.WaitOne();
            Thread.Sleep(1000);
        }
        
        private static class Auxiliary
        {
            /// <summary>
            /// Convert Byte to HEX string.
            /// </summary>
            /// <param name="number">number</param>
            /// <returns>HEX in a string</returns>
            public static string ByteToHex(byte number)
            {
                string hex = "0123456789ABCDEF";
                return new string(new char[] { hex[(number & 0xF0) >> 4], hex[number & 0x0F] });
            }
        }
        private static string WiFiNetworkInfoToString(WiFiNetworkInfo info)
        {
            string str;
            str = "SSID: " + info.SSID + "\n";
            str += "Channel Number: " + info.ChannelNumber + "\n";
            str += "RSSI: -" + info.RSSI + "dB" + "\n";
            str += "Security Mode: ";
            switch (info.SecMode)
            {
                case SecurityMode.Open: str += "Open"; break;
                case SecurityMode.WEP: str += "WEP"; break;
                case SecurityMode.WPA: str += "WPA"; break;
                case SecurityMode.WPA2: str += "WPA2"; break;
            }
            str += "\n";
            str += "Network Type: ";
            switch (info.networkType)
            {
                case NetworkType.AccessPoint: str += "Access Point"; break;
                case NetworkType.AdHoc: str += "AdHoc"; break;
            }
            str += "\n";
            str += "BS MAC: " + Auxiliary.ByteToHex(info.PhysicalAddress[0]) + "-"
                                + Auxiliary.ByteToHex(info.PhysicalAddress[1]) + "-"
                                + Auxiliary.ByteToHex(info.PhysicalAddress[2]) + "-"
                                + Auxiliary.ByteToHex(info.PhysicalAddress[3]) + "-"
                                + Auxiliary.ByteToHex(info.PhysicalAddress[4]) + "-"
                                + Auxiliary.ByteToHex(info.PhysicalAddress[5]) + "\n";
            return str;
        }
        #endregion
    }
}
