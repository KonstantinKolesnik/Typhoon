using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Typhoon.Layouts.LayoutItems;
using Typhoon.Localization;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucConsistItemList : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty ConsistProperty = DependencyProperty.Register("Consist", typeof(Consist), typeof(ucConsistItemList), new PropertyMetadata(null));
        public Consist Consist
        {
            get { return (Consist)GetValue(ConsistProperty); }
            set { SetValue(ConsistProperty, value); }
        }

        public static DependencyProperty ConsistItemProperty = DependencyProperty.Register("ConsistItem", typeof(ConsistItem), typeof(ucConsistItemList), new PropertyMetadata(null));
        public ConsistItem ConsistItem
        {
            get { return (ConsistItem)GetValue(ConsistItemProperty); }
            set { SetValue(ConsistItemProperty, value); }
        }
        #endregion

        #region Constructor
        public ucConsistItemList()
        {
            InitializeComponent();

            if (App.Model != null)
                DataContext = App.Model;

            CommandBindings.Add(new CommandBinding(RoutedCommands.AddConsistItem, AddConsistItem_Executed, AddConsistItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.DeleteConsistItem, DeleteConsistItem_Executed, DeleteConsistItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ConsistItemUp, ConsistItemUp_Executed, ConsistItemUp_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ConsistItemDown, ConsistItemDown_Executed, ConsistItemDown_CanExecute));
        }
        #endregion

        #region Event handlers
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (Locomotive loco in e.RemovedItems)
                loco.IsUsedInConsist = false;
            foreach (Locomotive loco in e.AddedItems)
                loco.IsUsedInConsist = true;
        }
        #endregion

        #region Routed Commands
        private void AddConsistItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Consist != null;
        }
        private void AddConsistItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConsistItem item = new ConsistItem();
            Consist.Items.Add(item);
            ConsistItem = item;
        }

        private void DeleteConsistItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ConsistItem != null;
        }
        private void DeleteConsistItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string s = LanguageDictionary.Current.Translate<string>("DeleteSelectedItem", "Text", "Delete selected item?");
            if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (ConsistItem.LocomotiveID != Guid.Empty)
                    foreach (Locomotive loco in App.Model.Layout.Locomotives)
                        if (loco.ID == ConsistItem.LocomotiveID)
                            loco.IsUsedInConsist = false;

                Consist.Items.Remove(ConsistItem);
            }
        }

        private void ConsistItemUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ConsistItem != null && Consist.Items.IndexOf(ConsistItem) > 0;
        }
        private void ConsistItemUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConsistItem item = ConsistItem;
            int idx = Consist.Items.IndexOf(item);
            Consist.Items.Move(idx, idx - 1);
            ConsistItem = item;
        }

        private void ConsistItemDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ConsistItem != null && Consist.Items.IndexOf(ConsistItem) < Consist.Items.Count - 1;
        }
        private void ConsistItemDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ConsistItem item = ConsistItem;
            int idx = Consist.Items.IndexOf(item);
            Consist.Items.Move(idx, idx + 1);
            ConsistItem = item;
        }
        #endregion
    }
}
