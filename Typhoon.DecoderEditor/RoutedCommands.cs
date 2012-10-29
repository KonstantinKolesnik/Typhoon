using System.Windows.Input;

namespace Typhoon.DecoderEditor
{
    public class RoutedCommands
    {
        #region Fields
        private static RoutedCommand cmdNewDecoder = new RoutedCommand("NewDecoder", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdOpenDecoder = new RoutedCommand("OpenDecoder", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdSaveDecoder = new RoutedCommand("SaveDecoder", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdSaveAsDecoder = new RoutedCommand("SaveAsDecoder", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdPrintDecoder = new RoutedCommand("PrintDecoder", typeof(RoutedCommands), new InputGestureCollection());
        private static RoutedCommand cmdEmailDecoder = new RoutedCommand("EmailDecoder", typeof(RoutedCommands), new InputGestureCollection());
        #endregion

        #region Constructor
        static RoutedCommands()
        {
            cmdNewDecoder.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl + N"));
        }
        #endregion

        #region Properties
        public static RoutedCommand NewDecoder
        {
            get { return cmdNewDecoder; }
        }
        public static RoutedCommand OpenDecoder
        {
            get { return cmdOpenDecoder; }
        }
        public static RoutedCommand SaveDecoder
        {
            get { return cmdSaveDecoder; }
        }
        public static RoutedCommand SaveAsDecoder
        {
            get { return cmdSaveAsDecoder; }
        }
        public static RoutedCommand PrintDecoder
        {
            get { return cmdPrintDecoder; }
        }
        public static RoutedCommand EmailDecoder
        {
            get { return cmdEmailDecoder; }
        }
        #endregion
    }
}
