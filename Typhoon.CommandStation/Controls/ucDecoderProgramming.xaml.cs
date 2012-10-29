using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Data;
using Typhoon.Decoders;
using Typhoon.Localization;
using Typhoon.NMRA;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucDecoderProgramming : UserControl
    {
        #region Fields
        //private readonly Station station;
        #endregion

        #region DependencyProperties
        public static DependencyProperty DecoderProperty = DependencyProperty.Register("Decoder", typeof(Decoder), typeof(ucDecoderProgramming), new PropertyMetadata(null, new PropertyChangedCallback(OnDecoderChanged)));
        private static void OnDecoderChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucDecoderProgramming uc = (ucDecoderProgramming)sender;
            uc.SetGroupping();
        }
        public Decoder Decoder
        {
            get { return (Decoder)GetValue(DecoderProperty); }
            set { SetValue(DecoderProperty, value); }
        }
        #endregion

        #region Constructor
        public ucDecoderProgramming()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(RoutedCommands.WriteDecoderParameter, WriteDecoderParameter_Executed, WriteDecoderParameter_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ReadDecoderParameter, ReadDecoderParameter_Executed, ReadDecoderParameter_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.ResetDecoder, ResetDecoder_Executed, ResetDecoder_CanExecute));

            cbProgramMode.ItemsSource = Enum.GetValues(typeof(NMRAProgramMode));
            cbProgramMode.SelectedValue = NMRAProgramMode.ServiceDirect;

            //if (App.Model != null && App.Model.Station != null)
            //    station = App.Model.Station;
        }
        #endregion

        #region Private methods
        private void SetGroupping()
        {
            gvParameters.GroupDescriptors.Clear();

            GroupDescriptor descriptor = new GroupDescriptor();
            descriptor.Member = "Group";
            gvParameters.GroupDescriptors.Add(descriptor);
        }
        #endregion

        #region Routed Commands
        private void WriteDecoderParameter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //DecoderParameter param = e.Parameter as DecoderParameter;
            
            //bool canExecute =
            //    station != null && station.Connected && Decoder != null && param != null && !param.ReadOnly && param.Value.HasValue &&
            //    ((station.MainTrackActive && (NMRAProgramMode)cbProgramMode.SelectedValue == NMRAProgramMode.POM) || (NMRAProgramMode)cbProgramMode.SelectedValue != NMRAProgramMode.POM)
            //    ;
            
            //// forbid addressware programming; and some cv if page/register mode!!!!!!
            //if (canExecute &&
            //    (NMRAProgramMode)cbProgramMode.SelectedValue == NMRAProgramMode.POM &&
            //    (param.IsPrimaryAddress || param.IsExtendedAddress || param.IsUseExtendedAddress))
            //    canExecute = false;

            //e.CanExecute = canExecute;
        }
        private void WriteDecoderParameter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //busyIndicator.IsBusy = true;
            //station.WriteLocoDecoderParameter(e.Parameter as DecoderParameter, (NMRAProgramMode)cbProgramMode.SelectedValue, Decoder.LocomotiveAddress);
            //busyIndicator.IsBusy = false;
        }

        private void ReadDecoderParameter_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute =
            //    station != null &&
            //    station.Connected &&
            //    Decoder != null &&
            //    (e.Parameter as DecoderParameter) != null &&
            //    (NMRAProgramMode)cbProgramMode.SelectedValue != NMRAProgramMode.POM;
            //    ;
        }
        private void ReadDecoderParameter_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //busyIndicator.IsBusy = true;
            //station.ReadLocoDecoderParameter(e.Parameter as DecoderParameter, (NMRAProgramMode)cbProgramMode.SelectedValue, Decoder.LocomotiveAddress);
            //busyIndicator.IsBusy = false;
        }

        private void ResetDecoder_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute =
            //    station != null && station.Connected && Decoder != null &&
            //    ((station.MainTrackActive && (NMRAProgramMode)cbProgramMode.SelectedValue == NMRAProgramMode.POM) || (NMRAProgramMode)cbProgramMode.SelectedValue != NMRAProgramMode.POM)
            //    ;
        }
        private void ResetDecoder_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //string s = LanguageDictionary.Current.Translate<string>("ResetDecoder", "Text", "Reset decoder to factory defaults?");
            //MessageBoxResult res = MessageBox.Show(s, App.Name, MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (res == MessageBoxResult.Yes)
            //{
            //    busyIndicator.IsBusy = true;
            //    station.ResetLocoDecoder((NMRAProgramMode)cbProgramMode.SelectedValue, Decoder.LocomotiveAddress, Decoder);
            //    busyIndicator.IsBusy = false;
            //}
        }
        #endregion
    }
}
