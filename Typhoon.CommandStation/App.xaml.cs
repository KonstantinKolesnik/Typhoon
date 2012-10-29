using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using Telerik.Windows.Controls;
using Typhoon.Core;
using Typhoon.Localization;

namespace Typhoon.CommandStation
{
    public partial class App : Application
    {
        #region Fields
        private static Model model = null;
        #endregion

        #region Properties
        public static string StartupPath
        {
            get { return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); }
        }
        public static string Name
        {
            get { return LanguageDictionary.Current.Translate<string>("Typhoon", "Text", "Typhoon"); }
        }
        public static Model Model
        {
            get { return model; }
        }
        #endregion

        //public static void DoEvents()//this Application application)
        //{
        //    DispatcherFrame frame = new DispatcherFrame();
        //    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new ExitFrameHandler(frm => frm.Continue = false), frame);
        //    Dispatcher.PushFrame(frame);
        //}
        //private delegate void ExitFrameHandler(DispatcherFrame frame);

        //public static void DoEvents2()//this Application application)
        //{
        //    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new VoidHandler(() => { }));
        //}
        //private delegate void VoidHandler();

        protected override void OnStartup(StartupEventArgs e)
        {
            //StyleManager.ApplicationTheme = new Windows7Theme();
            //StyleManager.ApplicationTheme = new SummerTheme();
            StyleManager.ApplicationTheme = new Office_BlackTheme();
            //StyleManager.ApplicationTheme = new TransparentTheme();
            //StyleManager.ApplicationTheme = new MetroTheme();

            SetLanguage();

            // in Windows Vista, you'll need to run the application as an administrator.            
            //string extensionDescription = "LayoutDocument";
            //FileRegistrationHelper.SetFileAssociation(".layout", "Typhoon" + "." + extensionDescription);

            model = new Model();

            base.OnStartup(e);
            //if (e.Args.Length != 0)
            //    RoutedCommands.OpenLayout.Execute(e.Args[0], null);
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            model.Dispose();
        }

        private void SetLanguage()
        {
            string[] files = Directory.GetFiles(StartupPath + @"\Languages\", "*.xml", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                string ciName = Path.GetFileNameWithoutExtension(fileName).ToLower();
                LanguageDictionary.RegisterDictionary(CultureInfo.GetCultureInfo(ciName), new XmlLanguageDictionary(fileName));
            }
            LanguageContext.Instance.Culture = CultureInfo.GetCultureInfo(CultureInfo.CurrentCulture.Parent.TwoLetterISOLanguageName.ToLower());

            LocalizationManager.Manager = new TelerikLocalizationManager();
        }
    }
}
