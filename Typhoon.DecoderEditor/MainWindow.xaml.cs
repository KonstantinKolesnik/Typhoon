using System.ComponentModel;
using System.Windows;
using Typhoon.Localization;

namespace Typhoon.DecoderEditor
{
    public partial class MainWindow : Window
    {
        #region Dependency properties
        public static DependencyProperty ModelProperty = DependencyProperty.Register("Model", typeof(Model), typeof(MainWindow), new PropertyMetadata(null));
        public Model Model
        {
            get { return (Model)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }
        #endregion

        #region Constructor
        public MainWindow()
        {
            Model = App.Model;
            CommandBindings.AddRange(Model.CommandBindings);

            InitializeComponent();
        }
        #endregion

        #region Event handlers
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Model.Options.Maximized)
                WindowState = WindowState.Maximized;
        }
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            string s = LanguageDictionary.Current.Translate<string>("ExitProgram", "Text", "Exit program?");
            e.Cancel = (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No);
            if (!e.Cancel)
            {
                Model.Options.Maximized = WindowState == WindowState.Maximized;
                Model.Dispose();
            }
        }
        #endregion
    }
}
