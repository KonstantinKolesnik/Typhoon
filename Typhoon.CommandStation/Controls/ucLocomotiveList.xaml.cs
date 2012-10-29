using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Data;
using Typhoon.Layouts;
using Typhoon.Layouts.LayoutItems;
using Typhoon.Localization;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucLocomotiveList : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(ucLocomotiveList), new PropertyMetadata(null, new PropertyChangedCallback(OnLayoutChanged)));
        private static void OnLayoutChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucLocomotiveList uc = (ucLocomotiveList)sender;
            uc.SetFilter();
        }
        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static DependencyProperty LocomotiveProperty = DependencyProperty.Register("Locomotive", typeof(Locomotive), typeof(ucLocomotiveList), new PropertyMetadata(null));
        public Locomotive Locomotive
        {
            get { return (Locomotive)GetValue(LocomotiveProperty); }
            set { SetValue(LocomotiveProperty, value); }
        }
        #endregion

        #region Constructor
        public ucLocomotiveList()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(RoutedCommands.AddLocomotive, AddLocomotive_Executed, AddLocomotive_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.DeleteLayoutItem, DeleteLayoutItem_Executed, DeleteLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ImportLayoutItem, ImportLayoutItem_Executed, ImportLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ExportLayoutItem, ExportLayoutItem_Executed, ExportLayoutItem_CanExecute));
        }
        #endregion

        #region Private methods
        private void SetFilter()
        {
            gvLocomotives.FilterDescriptors.Clear();
            gvLocomotives.FilterDescriptors.Add(new FilterDescriptor("Type", FilterOperator.IsEqualTo, LayoutItemType.Locomotive));
        }
        #endregion

        #region Routed Commands
        private void AddLocomotive_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Layout != null;
        }
        private void AddLocomotive_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Locomotive loco = new Locomotive();
            Layout.Items.Add(loco);
            Locomotive = loco;
        }

        private void DeleteLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Locomotive != null && !Locomotive.IsUsedInConsist;
        }
        private void DeleteLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string s = LanguageDictionary.Current.Translate<string>("DeleteSelectedItem", "Text", "Delete selected item?");
            if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Layout.Items.Remove(Locomotive);
        }

        private void ImportLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Layout != null;
        }
        private void ImportLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void ExportLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Locomotive != null;
        }
        private void ExportLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        #endregion
    }
}
