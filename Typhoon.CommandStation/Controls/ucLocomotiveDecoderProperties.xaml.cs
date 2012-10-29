using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Typhoon.Decoders;
using Typhoon.Localization;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucLocomotiveDecoderProperties : UserControl
    {
        #region Fields
        private bool assignDecoder = true;
        #endregion

        #region DependencyProperties
        public static DependencyProperty DecoderProperty = DependencyProperty.Register("Decoder", typeof(Decoder), typeof(ucLocomotiveDecoderProperties), new PropertyMetadata(null, new PropertyChangedCallback(OnDecoderChanged)));
        private static void OnDecoderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucLocomotiveDecoderProperties uc = (ucLocomotiveDecoderProperties)sender;
            uc.CheckDecoder();
        }
        public Decoder Decoder
        {
            get { return (Decoder)GetValue(DecoderProperty); }
            set { SetValue(DecoderProperty, value); }
        }
        #endregion

        #region Constructor
        public ucLocomotiveDecoderProperties()
        {
            InitializeComponent();

            cbSpeedSteps.ItemsSource = Enum.GetValues(typeof(SpeedSteps));
            if (App.Model != null)
                cbDecoders.ItemsSource = App.Model.LocomotiveDecoderReferenceCollection;
        }
        #endregion

        #region Private methods
        private void CheckDecoder()
        {
            assignDecoder = false;
            int idx = -1;
            if (Decoder != null)
            {
                foreach (DecoderReference dref in cbDecoders.Items)
                    if (dref.Manufacturer == Decoder.Manufacturer && dref.Model == Decoder.Model)
                    {
                        idx = cbDecoders.Items.IndexOf(dref);
                        break;
                    }
            }
            cbDecoders.SelectedIndex = idx;
            assignDecoder = true;
        }
        private void cbDecoders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (assignDecoder)
            {
                DecoderReference dref = cbDecoders.SelectedItem as DecoderReference;
                if (dref == null)
                    Decoder = null;
                else
                {
                    Decoder decoder = new Decoder();
                    if (decoder.LoadFromFile(dref.FileName))
                        Decoder = decoder;
                }
            }
        }
        private void btnImportDecoder_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = LanguageDictionary.Current.Translate<string>("DecoderFilesFilter", "Text", "Decoder files|*.decoder|All files|*.*");
            if (dlgOpen.ShowDialog() == true)
            {
                try
                {
                    Decoder d = new Decoder();
                    if (d.LoadFromFile(dlgOpen.FileName))
                        Decoder = d;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void btnExportDecoder_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgSave = new SaveFileDialog();
            dlgSave.Filter = LanguageDictionary.Current.Translate<string>("DecoderFilesFilter", "Text", "Decoder files|*.decoder|All files|*.*");
            if (dlgSave.ShowDialog() == true)
            {
                try
                {
                    Decoder.SaveToFile(dlgSave.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, App.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        #endregion
    }
}
