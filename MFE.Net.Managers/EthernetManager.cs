using System.Threading;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;

namespace MFE.Net.Managers
{
    public class EthernetManager : INetworkManager
    {
        #region Fields
        private ManualResetEvent blocker = null;
        private PWM portNetworkLED = null;
        #endregion

        #region Events
        public event EventHandler Started;
        public event EventHandler Stopped;
        #endregion

        #region Constructor
        public EthernetManager(PWM.Pin pinNetworkStatusLED)
        {
            blocker = new ManualResetEvent(false);
            portNetworkLED = new PWM(pinNetworkStatusLED);
            
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
        }
        #endregion

        #region Public methods
        public void Start()
        {
            portNetworkLED.Set(1, 50); // blink LED with 1 Hz

            if (!Ethernet.IsEnabled)
                Ethernet.Enable();

            if (!Ethernet.IsCableConnected)
            {
                blocker.Reset();
                while (!blocker.WaitOne(500, false))
                {
                    if (Ethernet.IsCableConnected)
                        break;
                }
            }

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

                    portNetworkLED.Set(true);
                    if (Started != null)
                        Started(this, EventArgs.Empty);

                    return;
                }
            }

            portNetworkLED.Set(false);
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
            if (e.IsAvailable)
            {
                if (Ethernet.IsCableConnected)
                    blocker.Set();
            }
            else
            {
                blocker.Set();
                if (Stopped != null)
                    Stopped(this, EventArgs.Empty);

                Start();
            }
        }
        #endregion
    }
}
