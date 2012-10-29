using System.Windows.Controls;

namespace Typhoon.CommandStation.UI.Pages
{
    public partial class pageDatabase : Page
    {
        #region Constructor
        public pageDatabase()
        {
            InitializeComponent();

            if (App.Model != null)
                DataContext = App.Model;

            pagerItemTypes.ListBox.SelectedIndex = 1; // set locos tab active
        }
        #endregion
    }
}
