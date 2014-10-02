using GHI.Premium.IO;
using GHI.Premium.System;
using GHI.Premium.USBHost;
using Microsoft.SPOT.IO;
using System;
using System.Collections;
using System.Threading;

namespace MF.Engine.Managers
{
    /// <summary>
    /// SD or USB Hard Drive
    /// </summary>
    [Serializable]
    public struct Drive
    {
        public PersistentStorage ps;
        public bool Formatted;
        public string VolumeName;
        public string RootName;
        public USBH_Device Device;
        public VolumeInfo VolumeInfo;
    }

    [Serializable]
    public delegate void OnDriveAdded(string Root);

    [Serializable]
    public delegate void OnDriveRemoved(string Root);

    public class DriveManager : MarshalByRefObject
    {
        #region Fields
        private ArrayList drives = new ArrayList();
        private PersistentStorage sd;
        private string root = "\\";                     // Path to Root of Primary HDD
        #endregion

        #region Properties
        public Drive[] AvailableDrives
        {
            get
            {
                Drive[] drvs = new Drive[drives.Count];
                for (int i = 0; i < drives.Count; i++)
                    drvs[i] = (Drive)drives[i];
                return drvs;
            }
        }
        public string[] DriveRoots
        {
            get
            {
                string[] roots = new string[drives.Count];
                for (int i = 0; i < drives.Count; i++)
                    roots[i] = ((Drive)drives[i]).RootName;
                return roots;
            }
        }

        public string RootDirectory
        {
            get { return root; }
            set
            {
                if (AppDomain.CurrentDomain.FriendlyName != "default")
                    throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));

                root = value;
            }
        }

        public bool SDMounted
        {
            get { return sd != null; }
        }
        #endregion

        #region Events
        public event OnDriveAdded DriveAdded;
        protected virtual void OnDriveAdded(string Root)
        {
            if (DriveAdded != null)
                DriveAdded(Root);
        }

        public event OnDriveRemoved DriveRemoved;
        protected virtual void OnDriveRemoved(string Root)
        {
            if (DriveRemoved != null)
                DriveRemoved(Root);
        }
        #endregion

        #region Constructor
        public DriveManager()
        {
            if (DeviceManager.ActiveDevice == DeviceType.Emulator)
            {
                VolumeInfo emulatedRoot = VolumeInfo.GetVolumes()[0];
                emulatedRoot.Format(0);

                Drive drive = new Drive();
                drive.Formatted = emulatedRoot.IsFormatted;
                drive.RootName = emulatedRoot.RootDirectory;
                drive.VolumeName = emulatedRoot.Name;
                drive.VolumeInfo = emulatedRoot;
                drives.Add(drive);

                OnDriveAdded(emulatedRoot.RootDirectory);
            }
            else
            {
                try
                {
                    USBHostController.DeviceConnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Connected);
                }
                catch (Exception) { }

                RemovableMedia.Insert += RemovableMedia_Insert;
                RemovableMedia.Eject += RemovableMedia_Eject;

                Thread sdWatcher = new Thread(SDMountThread);
                sdWatcher.Priority = ThreadPriority.AboveNormal;
                sdWatcher.Start();
            }
        }
        #endregion

        #region Public Methids
        public void FlushFileSystems()
        {
            for (int i = 0; i < drives.Count; i++)
                ((Drive)drives[i]).VolumeInfo.FlushAll();
        }
        #endregion

        #region Private Methods
        private void SDMountThread()
        {
            const int POLL_TIME = 500; // check every 500 millisecond
            bool sdExists;

            while (true)
            {
                try // If SD card was removed while mounting, it may throw exceptions
                {
                    sdExists = PersistentStorage.DetectSDCard();

                    // make sure it is fully inserted and stable
                    if (sdExists)
                    {
                        Thread.Sleep(50);
                        sdExists = PersistentStorage.DetectSDCard();
                    }

                    if (sdExists && sd == null)
                    {
                        sd = new PersistentStorage("SD");
                        sd.MountFileSystem();
                    }
                    else if (!sdExists && sd != null)
                    {
                        sd.UnmountFileSystem();
                        sd.Dispose();
                        sd = null;
                    }
                }
                catch
                {
                    if (sd != null)
                    {
                        sd.Dispose();
                        sd = null;
                    }
                }

                Thread.Sleep(POLL_TIME);
            }
        }
        #endregion

        #region Event handlers
        private void USBDevice_Connected(USBH_Device device)
        {
            if (device.TYPE == USBH_DeviceType.MassStorage)
            {
                Drive drive = new Drive();
                drive.Device = device;
                try
                {
                    drive.ps = new PersistentStorage(device);
                    drive.ps.MountFileSystem();
                }
                catch (Exception)// e)
                {
                    //Debug.Print("couldn't mount!\n" + e.Message);
                    return;
                }
                drives.Add(drive);                        // Add drive to Array
            }
        }
        private void RemovableMedia_Insert(object sender, MediaEventArgs e)
        {
            // Because of how this is called we have no choice
            // But to assume this is always the last mounted PS

            if (e.Volume.RootDirectory != "\\SD") // USB
            {
                try
                {
                    Drive drive = (Drive)drives[drives.Count - 1];
                    drive.Formatted = e.Volume.IsFormatted;
                    drive.RootName = e.Volume.RootDirectory;
                    drive.VolumeName = e.Volume.Name;
                    drive.VolumeInfo = e.Volume;
                    drives[drives.Count - 1] = drive;
                }
                catch (Exception) { }
            }
            else
            {
                Drive drive = new Drive();
                drive.VolumeInfo = e.Volume;
                drive.Formatted = e.Volume.IsFormatted;
                drive.RootName = e.Volume.RootDirectory;
                drive.VolumeName = e.Volume.Name;
                drive.ps = sd;
                drives.Add(drive);
            }

            // Attempt to get the drive name
            //try
            //{
            //    string sFile = @e.Volume.RootDirectory + "\\volume.info";
            //    if (File.Exists(sFile))
            //    {
            //        Debug.Print("Found volume.info for " + e.Volume.RootDirectory);
            //        byte[] b = new byte[new FileInfo(sFile).Length];
            //        FileStream iFile = new FileStream(sFile, FileMode.Open, FileAccess.Read);
            //        iFile.Read(b, 0, b.Length);
            //        iFile.Close();
            //        Debug.Print(new string(System.Text.UTF8Encoding.UTF8.GetChars(b)));
            //    }
            //}
            //catch (Exception) { }

            OnDriveAdded(e.Volume.RootDirectory);
        }
        private void RemovableMedia_Eject(object sender, MediaEventArgs e)
        {
            Drive drive;
            for (int i = 0; i < drives.Count; i++)
            {
                drive = (Drive)drives[i];
                if (drive.RootName == e.Volume.RootDirectory)
                {
                    drives.Remove(drive);
                    if (drive.Device != null)
                        drive.ps.UnmountFileSystem();

                    OnDriveRemoved(e.Volume.RootDirectory);
                    return;
                }
            }
        }
        #endregion
    }
}
