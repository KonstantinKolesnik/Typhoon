using System.Reflection;
using System.Windows.Controls;

namespace Typhoon.CommandStation.UI.Pages
{
    public partial class pageMenu : Page
    {
        public pageMenu()
        {
            InitializeComponent();
        }

        private void page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
