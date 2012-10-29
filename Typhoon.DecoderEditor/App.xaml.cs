using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using Telerik.Windows.Controls;
using Typhoon.Core;
using Typhoon.Localization;

namespace Typhoon.DecoderEditor
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
            get { return LanguageDictionary.Current.Translate<string>("Typhoon", "Text", "Typhoon") + " : " + LanguageDictionary.Current.Translate<string>("DecoderEditor", "Text", "Decoder Editor"); }
        }
        public static Model Model
        {
            get { return model; }
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            //StyleManager.ApplicationTheme = new Windows7Theme();
            //StyleManager.ApplicationTheme = new SummerTheme();
            StyleManager.ApplicationTheme = new Office_BlackTheme();
            //StyleManager.ApplicationTheme = new TransparentTheme();

            SetLanguage();
            model = new Model();
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
