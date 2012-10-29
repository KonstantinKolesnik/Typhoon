using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using Typhoon.Core;
using Typhoon.Layouts;
using Typhoon.Localization;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucLayoutItemCommonPropertiesEditor : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty LayoutItemProperty = DependencyProperty.Register("LayoutItem", typeof(LayoutItem), typeof(ucLayoutItemCommonPropertiesEditor), new PropertyMetadata(null));
        public LayoutItem LayoutItem
        {
            get { return (LayoutItem)GetValue(LayoutItemProperty); }
            set { SetValue(LayoutItemProperty, value); }
        }
        #endregion

        #region Constructor
        public ucLayoutItemCommonPropertiesEditor()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(RoutedCommands.AddLayoutItemImage, AddLayoutItemImage_Executed, AddLayoutItemImage_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.DeleteLayoutItemImage, DeleteLayoutItemImage_Executed, DeleteLayoutItemImage_CanExecute));
        }
        #endregion

        #region Routed Commands
        private void AddLayoutItemImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LayoutItem != null;
        }
        private void AddLayoutItemImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = LanguageDictionary.Current.Translate<string>("ImageFilesFilter", "Text", "Image files|*.jpg;*.gif;*.bmp;*.png;*.tiff|All files|*.*");
            if (dlgOpen.ShowDialog() == true)
            {
                try
                {
                    imgLoco.Source = Helpers.BitmapSourceFromImage(System.Drawing.Image.FromFile(dlgOpen.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteLayoutItemImage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LayoutItem != null && LayoutItem.Image != null;
        }
        private void DeleteLayoutItemImage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LayoutItem.Image = null;
        }
        #endregion
    }
}
