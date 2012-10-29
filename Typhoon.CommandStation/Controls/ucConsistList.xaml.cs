using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Data;
using Typhoon.Layouts;
using Typhoon.Layouts.LayoutItems;
using Typhoon.Localization;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucConsistList : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(ucConsistList), new PropertyMetadata(null, new PropertyChangedCallback(OnLayoutChanged)));
        private static void OnLayoutChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucConsistList uc = (ucConsistList)sender;
            uc.SetFilter();
        }
        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static DependencyProperty ConsistProperty = DependencyProperty.Register("Consist", typeof(Consist), typeof(ucConsistList), new PropertyMetadata(null));
        public Consist Consist
        {
            get { return (Consist)GetValue(ConsistProperty); }
            set { SetValue(ConsistProperty, value); }
        }
        #endregion

        #region Constructor
        public ucConsistList()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(RoutedCommands.AddConsist, AddConsist_Executed, AddConsist_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.DeleteLayoutItem, DeleteLayoutItem_Executed, DeleteLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ImportLayoutItem, ImportLayoutItem_Executed, ImportLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ExportLayoutItem, ExportLayoutItem_Executed, ExportLayoutItem_CanExecute));
        }
        #endregion

        #region Private methods
        private void SetFilter()
        {
            gvConsists.FilterDescriptors.Clear();
            gvConsists.FilterDescriptors.Add(new FilterDescriptor("Type", FilterOperator.IsEqualTo, LayoutItemType.Consist));
        }
        #endregion

        #region Routed Commands
        private void AddConsist_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Layout != null;
        }
        private void AddConsist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Consist consist = new Consist();
            Layout.Items.Add(consist);
            Consist = consist;
        }

        private void DeleteLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Consist != null;
        }
        private void DeleteLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string s = LanguageDictionary.Current.Translate<string>("DeleteSelectedItem", "Text", "Delete selected item?");
            if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (ConsistItem item in Consist.Items)
                    if (item.LocomotiveID != Guid.Empty)
                        foreach (Locomotive loco in App.Model.Layout.Locomotives)
                            if (loco.ID == item.LocomotiveID)
                                loco.IsUsedInConsist = false;

                Layout.Items.Remove(Consist);
            }
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
            e.CanExecute = Consist != null;
        }
        private void ExportLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        #endregion
    }
}
