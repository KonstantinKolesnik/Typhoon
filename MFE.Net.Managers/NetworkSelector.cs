using System;
using System.Threading;
using GHIElectronics.NETMF.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT.Net.NetworkInformation;
using GHIElectronics.NETMF.Hardware;

namespace Typhoon.MF.Net
{
    public class NetworkSelector
    {
        static public ManualResetEvent NetworkAvailablityBlocking = null;
        static public bool ethernet_event = false;
        static public bool ethernet_last_status = false;
        static public bool network_is_read = false;

        public static bool SelectNetwork(NetworkType type)
        {
            switch (type)
            {
                case NetworkType.Ethernet: return StartEthernet();
                case NetworkType.WiFi: return StartWiFi();
                case NetworkType.AdHocHost: return StartAdHocHost();
                case NetworkType.PPP: return StartPPP();
                default: return false;
            }
        }

        private static bool StartEthernet()
        {
            if (!Ethernet.IsEnabled)
                Ethernet.Enable();

            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
            
            NetworkAvailablityBlocking = new ManualResetEvent(false);
            if (!Ethernet.IsCableConnected)
            {
                Debug.Print("Cable is not connected!");
                NetworkAvailablityBlocking.Reset();
                while (!NetworkAvailablityBlocking.WaitOne(1000, false))
                {
                    if (!Ethernet.IsCableConnected)
                    {
                        Debug.Print("Cable is not connected!");
                        Debug.Print("Still waiting.");
                    }
                    else
                        break;
                }
            }
            Debug.Print("Ethernet cable is connected!");
            Debug.Print("Enable DHCP");
            
            
            NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in nis)
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    #region DHCP Code (dynamic IP)
                    if (!ni.IsDhcpEnabled)
                        ni.EnableDhcp();
                    else
                        ni.RenewDhcpLease();
                    #endregion

                    #region Static IP code
                    // Uncomment the following line if you want to use a static IP address, and comment out the DHCP code region above
                    //string ip = "192.168.0.150";
                    //string subnet = "255.255.255.0";
                    //string gateway = "192.168.0.1";
                    ////byte[] mac = ni.PhysicalAddress;// { 0x00, 0x26, 0x1C, 0x7B, 0x29, 0xE8 };

                    //ni.EnableStaticIP(ip, subnet, gateway);
                    //ni.EnableStaticDns(gateway);
                    #endregion

                    return true;
                }
            }

            return false;
        }
        private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable)
            {
                if (Ethernet.IsCableConnected)
                {
                    if (ethernet_last_status != true)
                    {
                        ethernet_last_status = true;
                        NetworkAvailablityBlocking.Set();
                    }
                }
            }
            else
            {
                if (!WiFi.IsLinkConnected)
                {
                    if (ethernet_last_status != false)
                    {
                        ethernet_last_status = false;
                        network_is_read = false;
                    }
                }
            }
        }
        //---------------------------------------------
        private static bool StartAdHocHost()
        {
            //FEZ Cobra OEM board V1.2 or V1.3 UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
            SPI.SPI_module _spi = SPI.SPI_module.SPI2; /*SPI bus*/
            Cpu.Pin _cs = EMX.Pin.IO2; /*ChipSelect*/
            Cpu.Pin _ExtInt = EMX.Pin.IO26; /*External Interrupt*/
            Cpu.Pin _reset = EMX.Pin.IO3; /*Reset*/

            try
            {
                //ChipworkX Developement System UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
                WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible,
                            _spi,/*SPI bus*/
                            _cs,  /*ChipSelect*/
                            _ExtInt, /*External Interrupt*/
                            _reset); /*Reset*/
            }
            catch (WiFi.WiFiException e)
            {
                if (e.errorCode == WiFi.WiFiException.ErrorCode.HardwareCommunicationFailure ||
                    e.errorCode == WiFi.WiFiException.ErrorCode.HardwareFirmwareVersionMismatch)
                {
                    //ChipworkX Developement System UEXT header with WiFi RS21 Module: P/N:GHI-WIFIEXP2-298
                    WiFi.UpdateFirmware(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible,
                                        _spi,/*SPI bus*/
                                        _cs,  /*ChipSelect*/
                                        _ExtInt, /*External Interrupt*/
                                        _reset); /*Reset*/
                    WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible,
                                 _spi,/*SPI bus*/
                                 _cs,  /*ChipSelect*/
                                 _ExtInt, /*External Interrupt*/
                                 _reset); /*Reset*/
                }
                else if (e.errorCode == WiFi.WiFiException.ErrorCode.HardwareCommunicationFailure)
                {
                    Debug.Print("Error Message: " + e.ErrorMsg);
                    Debug.Print("Check WiFi module hardware connections and SPI/signals configurations.");
                    throw;
                }
            }

            if (WiFi.IsEnabled)
                Debug.Print("Enabled Successfully. At this point, the on-board LED on RS9110_N_11_21_1_Compatible module is ON.");
            else
                throw new Exception();

            // This event handler checks netowrk avaialblty (Ethernet, WiFi and PPP).
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged2);

            WiFi.StartAdHocHost("Typhoon", SecurityMode.Open, "", 10);
            //WiFi.StartAdHocHost("Ad-Hoc Test WEP", SecurityMode.WEP, "A1CEF53456A1CEF53456AF34DB", 10); // 128bit key (26 Hex numbers)
            //WiFi.StartAdHocHost("Ad-Hoc Test WEP", SecurityMode.WEP, "A1CEF53456A", 10); // 64bit key (10 Hex numbers)
            Debug.Print("Join the other peer of Ad-Hoc connection");
            NetworkInterface[] netif = NetworkInterface.GetAllNetworkInterfaces();
            netif[0].EnableStaticIP("192.168.137.2", "255.255.255.0", "0.0.0.0");
            Debug.Print("Set the other peer's IP to 192.168.137.1");
            Debug.Print("Try to ping this IP from the other peer 192.168.137.2");
            //Thread.Sleep(10000);
            //Debug.Print("Disconnect WiFi link.");
            //WiFi.Disconnect();
            //Debug.Print("Disable WiFi interface");
            //WiFi.Disable();
            //Debug.Print("The end of test");

            return true;
        }
        private static void NetworkChange_NetworkAvailabilityChanged2(object sender, NetworkAvailabilityEventArgs e)
        {
            Debug.Print("Network Availability Event Triggered");
            if (e.IsAvailable)
            {
                if (WiFi.IsEnabled)// Make sure that the event is fired by WiFi interface, not other networking interface.
                    if (WiFi.IsLinkConnected)
                    {
                        Debug.Print("Ad-Hoc Host started.");
                    }
            }
            else
            {
                if (WiFi.IsEnabled)// Make sure that the event is fired by WiFi interface, not other networking interface.
                    if (!WiFi.IsLinkConnected)
                    {
                        Debug.Print("WiFi connection was dropped or disconnected!");
                    }
            }
        }
        //---------------------------------------------
        private static bool StartWiFi()
        {
            WiFi.Enable(WiFi.HardwareModule.RS9110_N_11_21_1_Compatible, SPI.SPI_module.SPI1, (Cpu.Pin)2, (Cpu.Pin)26, (Cpu.Pin)3);
            NetworkInterface[] networks = NetworkInterface.GetAllNetworkInterfaces();
            Wireless80211 WiFiSettings = null;
            for (int index = 0; index < networks.Length; ++index)
            {
                if (networks[index] is Wireless80211)
                {
                    WiFiSettings = (Wireless80211)networks[index];
                    //Debug.Print("Found network: " + WiFiSettings.Ssid.ToString());
                }
            }
            
            WiFiSettings.Ssid = "yourSSID";
            WiFiSettings.PassPhrase = "yourPassphrase";
            WiFiSettings.Encryption = Wireless80211.EncryptionType.WPA;
            Wireless80211.SaveConfiguration(new Wireless80211[] { WiFiSettings }, false);

            ManualResetEvent _networkAvailabilityBlocking = new ManualResetEvent(false);
            if (!WiFi.IsLinkConnected)
            {
                _networkAvailabilityBlocking.Reset();
                while (!_networkAvailabilityBlocking.WaitOne(5000, false))
                {
                    if (!WiFi.IsLinkConnected)
                    {
                        //Debug.Print("Waiting for Network");
                    }
                    else
                        break;
                }
            }
            //Debug.Print("Enable DHCP");
            try
            {
                if (!WiFiSettings.IsDhcpEnabled)
                    WiFiSettings.EnableDhcp(); // This function is blocking
                else
                    WiFiSettings.RenewDhcpLease(); // This function is blocking
            }
            catch
            {
                //Debug.Print("DHCP Failed");
            }

            return false;
        }
        private static bool StartPPP()
        {
            return false;
        }
    }
}
