using Telerik.Windows.Controls;
using Typhoon.Localization;

namespace Typhoon.Core
{
    public class TelerikLocalizationManager : LocalizationManager
    {
        public override string GetStringOverride(string key)
        {
            return LanguageDictionary.Current.Translate<string>(key, "Text", base.GetStringOverride(key));
        }
    }
}
