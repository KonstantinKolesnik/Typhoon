using System.Windows;
using Microsoft.Phone.Controls;

namespace Typhoon.WPClient
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //    App.ViewModel.LoadData();
        }
    }
}