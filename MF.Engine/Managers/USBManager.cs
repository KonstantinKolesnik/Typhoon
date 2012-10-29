using System;
using System.Collections;
using GHIElectronics.NETMF.USBHost;

namespace MF.Engine.Managers
{
    public class USBManager : MarshalByRefObject
    {
        #region Fields
        private ArrayList deviceList;
        #endregion

        #region Properties
        public int Count
        {
            get { return deviceList.Count; }
        }
        #endregion

        #region Events
        public event USBH_DeviceConnectionEventHandler USBDeviceInserted;
        public event USBH_DeviceConnectionEventHandler USBDeviceRemoved;
        #endregion

        #region Constructor
        public USBManager()
        {
            deviceList = new ArrayList();

            if (!DeviceManager.IsEmulator)
            {
                try
                {
                    USBHostController.DeviceConnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Connected);
                    USBHostController.DeviceDisconnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Disconnected);
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region Public methods
        public int GetCount(USBH_DeviceType type)
        {
            int n = 0;
            foreach (USBH_Device device in deviceList)
                if (device.TYPE == type)
                    n++;

            return n;
        }
        #endregion

        #region Event handlers
        private void USBDevice_Connected(USBH_Device device)
        {
            deviceList.Add(device);

            if (USBDeviceInserted != null)
                USBDeviceInserted(device);
        }
        private void USBDevice_Disconnected(USBH_Device device)
        {
            foreach (USBH_Device dev in deviceList)
            {
                if (dev.ID == device.ID)
                {
                    deviceList.Remove(dev);
                    break;
                }
            }

            if (USBDeviceRemoved != null)
                USBDeviceRemoved(device);
        }
        #endregion
    }
}
