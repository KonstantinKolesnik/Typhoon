using System;
using GHIElectronics.NETMF.System;
using Microsoft.SPOT;

namespace MF.Engine.Managers
{
    public enum NetworkConnectionType
    {
        None = 0,
        DHCP = 1,
        StaticIP = 2,
    }

    public static class SettingsManager
    {
        #region Settings classes
        [Serializable]
        public sealed class BootSettings
        {
            #region Fields
            // Fields must be serializable.
            public NetworkConnectionType ConnectionType;
            public string IPAddress;
            public string GatewayAddress;
            public string SubnetMask;
            public bool CheckUpdates;
            public bool EnableRTC;
            public byte Volume;
            public byte TrebleAmp;
            public byte TrebleLimit;
            public byte BassAmp;
            public byte BassLimit;
            public int TimeZone;
            #endregion

            #region Constructor
            public BootSettings(NetworkConnectionType ConnectionType, string IPAddress, string GatewayAddress, string SubnetMask, bool CheckUpdates, bool EnableRTC, byte Volume, byte TrebleAmp, byte TrebleLimit, byte BassAmp, byte BassLimit, int TimeZone)
            {
                this.ConnectionType = ConnectionType;
                this.IPAddress = IPAddress;
                this.GatewayAddress = GatewayAddress;
                this.SubnetMask = SubnetMask;
                this.CheckUpdates = CheckUpdates;
                this.EnableRTC = EnableRTC;
                this.Volume = Volume;
                this.TrebleAmp = TrebleAmp;
                this.TrebleLimit = TrebleLimit;
                this.BassAmp = BassAmp;
                this.BassLimit = BassLimit;
                this.TimeZone = TimeZone;
            }
            #endregion
        }
        [Serializable]
        public sealed class SystemUpdateSettings
        {
            #region Fields
            // Fields must be serializable.
            public bool NeedsUpdate;
            public string FilePath;
            public int LastResult;
            #endregion

            #region Constructor
            public SystemUpdateSettings(bool RequireUpdate, string FileLocation, int iResult)
            {
                this.NeedsUpdate = RequireUpdate;
                this.FilePath = FileLocation;
                this.LastResult = iResult;
            }
            #endregion
        }
        #endregion

        #region Fields
        private class BootSettingsID { }
        private static ExtendedWeakReference ewrBootSettings;
        private static BootSettings bootSettings;

        private class SystemUpdateSettingsID { }
        private static ExtendedWeakReference ewrSystemUpdateSettings;
        private static SystemUpdateSettings systemUpdateSettings;
        #endregion

        #region Properties
        public static BootSettings Boot
        {
            get { return bootSettings; }
        }
        public static SystemUpdateSettings SystemUpdate
        {
            get { return systemUpdateSettings; }
        }
        #endregion

        #region Constructor
        static SettingsManager()
        {
            LoadBootSettings();
            LoadUpdateSettings();
        }
        #endregion

        #region Public Methods
        public static void SaveBootSettings()
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));

            ewrBootSettings.Target = bootSettings;
            Util.FlushExtendedWeakReferences();
        }
        public static void SaveSystemUpdateSettings()
        {
            if (AppDomain.CurrentDomain.FriendlyName != "default")
                throw new Exception(Resources.GetString(Resources.StringResources.DefaultDomainError));

            ewrSystemUpdateSettings.Target = systemUpdateSettings;
            Util.FlushExtendedWeakReferences();
        }
        #endregion

        #region Private Methods
        private static void LoadBootSettings()
        {
            ewrBootSettings = ExtendedWeakReference.RecoverOrCreate(typeof(BootSettingsID), 0, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);
            ewrBootSettings.Priority = (int)ExtendedWeakReference.PriorityLevel.System;
            bootSettings = (BootSettings)ewrBootSettings.Target;

            if (bootSettings == null)
            {
                bootSettings = new BootSettings(NetworkConnectionType.DHCP, "", "", "", true, false, 100, 0, 0, 0, 0, -1);
                ewrBootSettings.Target = bootSettings;
            }
        }
        private static void LoadUpdateSettings()
        {
            ewrSystemUpdateSettings = ExtendedWeakReference.RecoverOrCreate(typeof(SystemUpdateSettingsID), 0, ExtendedWeakReference.c_SurvivePowerdown | ExtendedWeakReference.c_SurviveBoot);
            ewrSystemUpdateSettings.Priority = (int)ExtendedWeakReference.PriorityLevel.System;
            systemUpdateSettings = (SystemUpdateSettings)ewrSystemUpdateSettings.Target;
        }
        #endregion
    }
}
