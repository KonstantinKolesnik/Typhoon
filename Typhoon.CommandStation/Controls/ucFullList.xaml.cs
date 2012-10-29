using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Typhoon.Layouts;
using Typhoon.Localization;
using Microsoft.Win32;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucFullList : UserControl
    {
        private OpenFileDialog dlgOpen = new OpenFileDialog();

        #region DependencyProperties
        public static DependencyProperty LayoutProperty = DependencyProperty.Register("Layout", typeof(Layout), typeof(ucFullList), new PropertyMetadata(null));
        public Layout Layout
        {
            get { return (Layout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static DependencyProperty LayoutItemProperty = DependencyProperty.Register("LayoutItem", typeof(LayoutItem), typeof(ucFullList), new PropertyMetadata(null));
        public LayoutItem LayoutItem
        {
            get { return (LayoutItem)GetValue(LayoutItemProperty); }
            set { SetValue(LayoutItemProperty, value); }
        }
        #endregion

        #region Constructor
        public ucFullList()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(RoutedCommands.DeleteLayoutItem, DeleteLayoutItem_Executed, DeleteLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ImportLayoutItem, ImportLayoutItem_Executed, ImportLayoutItem_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ExportLayoutItem, ExportLayoutItem_Executed, ExportLayoutItem_CanExecute));
        }
        #endregion

        #region Routed Commands
        private void DeleteLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LayoutItem != null;
        }
        private void DeleteLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string s = LanguageDictionary.Current.Translate<string>("DeleteSelectedItem", "Text", "Delete selected item?");
            if (MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Layout.Items.Remove(LayoutItem);
        }

        private void ImportLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Layout != null;
        }
        private void ImportLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //if (dlgOpen.ShowDialog() == true)
            //{
            //    Layout.Import(dlgOpen.FileName);
            //}
        }

        private void ExportLayoutItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LayoutItem != null;
        }
        private void ExportLayoutItem_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        #endregion
    }
}
