using System;
using System.Collections;
using System.IO;
using System.Threading;
using GHI.Premium.IO;
using GHI.Premium.System;
using GHI.Premium.USBHost;
using MFE.Device;
using Microsoft.SPOT.IO;
using Microsoft.SPOT;

namespace MFE.Storage
{
    public static class DriveManager
    {
        #region Fields
        private static ArrayList drives = new ArrayList();
        private static PersistentStorage sd;
        private static PersistentStorage flash;
        #endregion

        #region Properties
        public static Drive[] Drives
        {
            get
            {
                Drive[] drvs = new Drive[drives.Count];
                for (int i = 0; i < drives.Count; i++)
                    drvs[i] = (Drive)drives[i];
                return drvs;
            }
        }
        public static string[] Roots
        {
            get
            {
                Drive[] drvs = Drives;

                string[] roots = new string[drvs.Length];
                for (int i = 0; i < drvs.Length; i++)
                    roots[i] = drvs[i].RootName;

                return roots;
            }
        }
        public static bool IsSDMounted
        {
            get { return sd != null; }
        }
        public static bool IsFlashMounted
        {
            get { return flash != null; }
        }
        #endregion

        #region Events
        public static event DriveEventHandler DriveAdded;
        public static event DriveEventHandler DriveRemoved;
        #endregion

        #region Constructor
        static DriveManager()
        {
            if (DeviceManager.IsEmulator)
            {
                VolumeInfo emulatedRoot = VolumeInfo.GetVolumes()[0];
                emulatedRoot.Format(0);

                Drive drive = new Drive();
                drive.Formatted = emulatedRoot.IsFormatted;
                drive.RootName = emulatedRoot.RootDirectory;
                drive.VolumeName = emulatedRoot.Name;
                drive.VolumeInfo = emulatedRoot;
                drives.Add(drive);

                if (DriveAdded != null)
                    DriveAdded(emulatedRoot.RootDirectory);
            }
            else
            {
                try
                {
                    USBHostController.DeviceConnectedEvent += new USBH_DeviceConnectionEventHandler(USBDevice_Connected);
                }
                catch (Exception) { }

                RemovableMedia.Insert += RemovableMedia_Inserted;
                RemovableMedia.Eject += RemovableMedia_Ejected;
                new Thread(SDWatcher) { Priority = ThreadPriority.BelowNormal }.Start();

                try
                {
                    flash = new PersistentStorage("NAND");
                    flash.MountFileSystem();
                    //string rootDirectory = VolumeInfo.GetVolumes()[0].RootDirectory;
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }
            }
        }
        #endregion

        #region Public Methods
        public static void FlushFileSystems()
        {
            for (int i = 0; i < drives.Count; i++)
                ((Drive)drives[i]).VolumeInfo.FlushAll();
        }
        public static void SaveToSD(byte[] data, string fileName)
        {
            if (IsSDMounted && data != null && data.Length > 0)
            {
                using (FileStream fs = File.OpenWrite(fileName))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Flush();
                }
            }
        }
        public static byte[] LoadFromSD(string fileName)
        {
            byte[] data = null;

            if (DriveManager.IsSDMounted && File.Exists(fileName))
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                }
            }

            return data;
        }
        #endregion

        #region Private Methods
        private static void SDWatcher()
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
        private static void USBDevice_Connected(USBH_Device device)
        {
            if (device.TYPE == USBH_DeviceType.MassStorage)
            {
                Drive drive = new Drive();
                drive.Device = device;
                try
                {
                    drive.ps = new PersistentStorage(device);
                    drive.ps.MountFileSystem();
                    drives.Add(drive); // Add drive to Array
                }
                catch (Exception)// e)
                {
                    //Debug.Print("couldn't mount!\n" + e.Message);
                }
            }
        }
        private static void RemovableMedia_Inserted(object sender, MediaEventArgs e)
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

            if (DriveAdded != null)
                DriveAdded(e.Volume.RootDirectory);
        }
        private static void RemovableMedia_Ejected(object sender, MediaEventArgs e)
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

                    if (DriveRemoved != null)
                        DriveRemoved(e.Volume.RootDirectory);

                    return;
                }
            }
        }
        #endregion
    }
}
