using System.Windows.Input;

namespace Typhoon.CommandStation
{
    public class RoutedCommands
    {
        #region Fields
        private static RoutedCommand cmdNewLayout = new RoutedCommand("NewLayot", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdOpenLayout = new RoutedCommand("OpenLayout", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdSaveLayout = new RoutedCommand("SaveLayout", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdSaveAsLayout = new RoutedCommand("SaveAsLayout", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdPrintLayout = new RoutedCommand("PrintLayout", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdEmailLayout = new RoutedCommand("EmailLayout", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdImportLayout = new RoutedCommand("ImportLayot", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdExportLayout = new RoutedCommand("ExportLayot", typeof(RoutedCommands), new InputGestureCollection());
        
        private static RoutedCommand cmdStationPower = new RoutedCommand("StationPower", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdStationOptions = new RoutedCommand("StationOptions", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdStationRailcom = new RoutedCommand("StationRailcom", typeof(RoutedCommands), new InputGestureCollection());

        private static RoutedCommand cmdAllBrake = new RoutedCommand("AllBrake", typeof(RoutedCommands));
        private static RoutedCommand cmdAllStop = new RoutedCommand("AllStop", typeof(RoutedCommands));
        private static RoutedCommand cmdAllReset = new RoutedCommand("AllReset", typeof(RoutedCommands));



        private static RoutedCommand cmdAddLayoutItemImage = new RoutedCommand("AddLayoutItemImage", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdDeleteLayoutItemImage = new RoutedCommand("DeleteLayoutItemImage", typeof(RoutedCommands), new InputGestureCollection());
        
        private static RoutedCommand cmdDeleteLayoutItem = new RoutedCommand("DeleteLayoutItem", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdImportLayoutItem = new RoutedCommand("ImportLayoutItem", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdExportLayoutItem = new RoutedCommand("ExportLayoutItem", typeof(RoutedCommands), new InputGestureCollection());

        private static RoutedCommand cmdAddLocomotive = new RoutedCommand("AddLocomotive", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdAddConsist = new RoutedCommand("AddConsist", typeof(RoutedCommands), new InputGestureCollection());
        
        private static RoutedCommand cmdAddConsistItem = new RoutedCommand("AddConsistItem", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdDeleteConsistItem = new RoutedCommand("DeleteConsistItem", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdConsistItemUp = new RoutedCommand("ConsistItemUp", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdConsistItemDown = new RoutedCommand("ConsistItemDown", typeof(RoutedCommands), new InputGestureCollection());

        private static RoutedCommand cmdStop = new RoutedCommand("Stop", typeof(RoutedCommands));
        private static RoutedCommand cmdBrake = new RoutedCommand("Brake", typeof(RoutedCommands));

        private static RoutedCommand cmdWriteDecoderParameter = new RoutedCommand("WriteDecoderParameter", typeof(RoutedCommands));
        private static RoutedCommand cmdReadDecoderParameter = new RoutedCommand("ReadDecoderParameter", typeof(RoutedCommands));
        private static RoutedCommand cmdResetDecoder = new RoutedCommand("ResetDecoder", typeof(RoutedCommands));
        #endregion

        #region Constructor
        static RoutedCommands()
        {
            cmdNewLayout.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl + N"));



        }
        #endregion

        #region Properties
        public static RoutedCommand NewLayout
        {
            get { return cmdNewLayout; }
        }
        public static RoutedCommand OpenLayout
        {
            get { return cmdOpenLayout; }
        }
        public static RoutedCommand SaveLayout
        {
            get { return cmdSaveLayout; }
        }
        public static RoutedCommand SaveAsLayout
        {
            get { return cmdSaveAsLayout; }
        }
        public static RoutedCommand PrintLayout
        {
            get { return cmdPrintLayout; }
        }
        public static RoutedCommand EmailLayout
        {
            get { return cmdEmailLayout; }
        }
        public static RoutedCommand ImportLayout
        {
            get { return cmdImportLayout; }
        }
        public static RoutedCommand ExportLayout
        {
            get { return cmdExportLayout; }
        }

        public static RoutedCommand StationPower
        {
            get { return cmdStationPower; }
        }
        public static RoutedCommand StationOptions
        {
            get { return cmdStationOptions; }
        }
        public static RoutedCommand StationRailcom
        {
            get { return cmdStationRailcom; }
        }

        public static RoutedCommand AllBrake
        {
            get { return cmdAllBrake; }
        }
        public static RoutedCommand AllStop
        {
            get { return cmdAllStop; }
        }
        public static RoutedCommand AllReset
        {
            get { return cmdAllReset; }
        }

        public static RoutedCommand AddLayoutItemImage
        {
            get { return cmdAddLayoutItemImage; }
        }
        public static RoutedCommand DeleteLayoutItemImage
        {
            get { return cmdDeleteLayoutItemImage; }
        }

        public static RoutedCommand DeleteLayoutItem
        {
            get { return cmdDeleteLayoutItem; }
        }
        public static RoutedCommand ImportLayoutItem
        {
            get { return cmdImportLayoutItem; }
        }
        public static RoutedCommand ExportLayoutItem
        {
            get { return cmdExportLayoutItem; }
        }

        public static RoutedCommand AddLocomotive
        {
            get { return cmdAddLocomotive; }
        }
        public static RoutedCommand AddConsist
        {
            get { return cmdAddConsist; }
        }
        
        public static RoutedCommand AddConsistItem
        {
            get { return cmdAddConsistItem; }
        }
        public static RoutedCommand DeleteConsistItem
        {
            get { return cmdDeleteConsistItem; }
        }
        public static RoutedCommand ConsistItemUp
        {
            get { return cmdConsistItemUp; }
        }
        public static RoutedCommand ConsistItemDown
        {
            get { return cmdConsistItemDown; }
        }



        public static RoutedCommand Stop
        {
            get { return cmdStop; }
        }
        public static RoutedCommand Brake
        {
            get { return cmdBrake; }
        }

        public static RoutedCommand WriteDecoderParameter
        {
            get { return cmdWriteDecoderParameter; }
        }
        public static RoutedCommand ReadDecoderParameter
        {
            get { return cmdReadDecoderParameter; }
        }
        public static RoutedCommand ResetDecoder
        {
            get { return cmdResetDecoder; }
        }
        #endregion
    }
}
