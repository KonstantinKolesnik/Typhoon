using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Typhoon.GraphicsAdapter.Hardware;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageShortcuts : BasePage
    {
        #region Constructor
        public PageShortcuts()
        {
            InitShortcuts();
        }
        #endregion

        #region Private methods
        private void InitShortcuts()
        {
            Font font = Program.FontCourierNew9;

            WrapPanel grid = new WrapPanel();
            grid.Orientation = Orientation.Horizontal;
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.ItemWidth = 100;// 150;
            Child = grid;

            //grid.Children.Add(new Shortcut("scDatabase", "База данных", Resources.GetBitmap(Resources.BitmapResources.About), font, Program.ShortcutIconSize));
            //grid.Children.Add(new Shortcut("scOperation", "Управление", Resources.GetBitmap(Resources.BitmapResources.Operation), font, Program.ShortcutIconSize));
            //grid.Children.Add(new Shortcut("scTopology", "Топология", Resources.GetBitmap(Resources.BitmapResources.About), font, Program.ShortcutIconSize));
            //grid.Children.Add(new Shortcut("scSettings", "Настройки", Resources.GetBitmap(Resources.BitmapResources.Settings), font, Program.ShortcutIconSize));

            foreach (Shortcut sc in grid.Children)
            {
                sc.SetMargin(3, 2, 3, 2);
                sc.ForeColor = Program.ButtonTextColor;
                //sc.BackgroundImage = Program.ButtonBackground;
                //sc.BackgroundImage = Resources.GetBitmap(Resources.BitmapResources.ButtonPlaceHolder);
                //sc.Opacity = 180;
                sc.Clicked += new EventHandler(Shortcut_Clicked);
            }
        }
        #endregion

        #region Event handlers
        private void Shortcut_Clicked(object sender, EventArgs e)
        {
            //switch ((sender as Shortcut).Name)
            //{
            //    case "scImport": Desktop.AddPage(new PageFileBrowser(FileBrowser.StorageType.USB)); break;
            //    case "scExport": Desktop.AddPage(new PageFileBrowser(FileBrowser.StorageType.USB)); break;
            //    case "scDatabase": Desktop.AddPage(new PageDatabase()); break;
            //    case "scTopology": Desktop.AddPage(new PageTopology()); break;
            //    case "scOperation": Desktop.AddPage(new PageOperation()); break;
            //    case "scSettings": Desktop.AddPage(new PageSettings()); break;
            //    case "scInformation": Desktop.AddPage(new PageAbout()); break;
            //    default: break;
            //}

            //NoiseMaker.MakeNoise(NoiseMaker.ID_OPEN);
        }
        #endregion
    }
}
