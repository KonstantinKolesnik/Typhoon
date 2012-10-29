using System;
using GHIElectronics.NETMF.IO;
using GHIElectronics.NETMF.USBHost;
using Microsoft.SPOT.IO;

namespace MFE.Storage
{
    /// <summary>
    /// SD card or USB Hard Drive
    /// </summary>
    [Serializable]
    public struct Drive
    {
        public PersistentStorage ps;
        public bool Formatted;
        public string VolumeName;
        public string RootName;
        public VolumeInfo VolumeInfo;
        public USBH_Device Device;
    }
}
