using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Typhoon.CommandStation.Operators;
using Typhoon.Decoders;
using Typhoon.Layouts.LayoutItems;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucLocomotiveOperator : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty LocomotiveProperty = DependencyProperty.Register("Locomotive", typeof(Locomotive), typeof(ucLocomotiveOperator), new PropertyMetadata(null, new PropertyChangedCallback(OnLocomotiveChanged)));
        private static void OnLocomotiveChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucLocomotiveOperator uc = (ucLocomotiveOperator)sender;
            uc.LocomotiveOperator.Decoder = uc.Locomotive != null ? uc.Locomotive.DecoderMobile : null;
            if (uc.LocomotiveOperator.Decoder != null)
                uc.Locomotive.DecoderMobile.PropertyChanged += uc.Decoder_PropertyChanged;

            uc.SetGaugeScale();
        }
        public Locomotive Locomotive
        {
            get { return (Locomotive)GetValue(LocomotiveProperty); }
            set { SetValue(LocomotiveProperty, value); }
        }

        public static DependencyProperty LocomotiveOperatorProperty = DependencyProperty.Register("LocomotiveOperator", typeof(LocomotiveOperator), typeof(ucLocomotiveOperator), new PropertyMetadata(null, new PropertyChangedCallback(OnLocomotiveOperatorChanged)));
        private static void OnLocomotiveOperatorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ucLocomotiveOperator uc = (ucLocomotiveOperator)sender;
            if (uc.LocomotiveOperator != null & uc.LocomotiveOperator.Decoder != null)
                uc.LocomotiveOperator.Decoder.PropertyChanged += uc.Decoder_PropertyChanged;

            uc.SetGaugeScale();
        }
        public LocomotiveOperator LocomotiveOperator
        {
            get { return (LocomotiveOperator)GetValue(LocomotiveOperatorProperty); }
            set { SetValue(LocomotiveOperatorProperty, value); }
        }
        #endregion

        #region Constructor
        public ucLocomotiveOperator()
        {
            InitializeComponent();

            LocomotiveOperator = new LocomotiveOperator();

            CommandBindings.Add(new CommandBinding(RoutedCommands.Stop, Stop_Executed, Stop_CanExecute));
            CommandBindings.Add(new CommandBinding(RoutedCommands.Brake, Brake_Executed, Brake_CanExecute));
        }
        #endregion

        #region Event handlers
        private void Decoder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LocomotiveSpeedSteps")
                SetGaugeScale();
        }
        private void ButtonDirection_Click(object sender, RoutedEventArgs e)
        {
            LocomotiveOperator.ToggleDirection();
        }
        //private void ButtonLight_Click(object sender, RoutedEventArgs e)
        //{
        //    LocomotiveOperator.Light = !LocomotiveOperator.Light;
        //}
        #endregion

        #region Private methods
        private void SetGaugeScale()
        {
            //if (Locomotive != null && Locomotive.DecoderMobile != null)
            //{
            //    double max = (double)Locomotive.DecoderMobile.LocomotiveSpeedSteps;

            //    radialRange1.Min = 0;
            //    radialRange2.Min = 0;
            //    radialRange3.Min = 0;

            //    radialRange3.Max = max;
            //    radialRange2.Max = radialRange3.Min = max * 2 / 3.0;
            //    radialRange1.Max = radialRange2.Min = max * 1 / 3.0;

            //    switch (Locomotive.DecoderMobile.LocomotiveSpeedSteps)
            //    {
            //        case SpeedSteps.Speed14: speedometerScale.MajorTicks = 14; break;
            //        case SpeedSteps.Speed28: speedometerScale.MajorTicks = 14; break;
            //        case SpeedSteps.Speed128: speedometerScale.MajorTicks = 20; break;
            //    }
            //}
        }
        #endregion

        #region Routed commands
        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LocomotiveOperator.IsOperable;
            IsEnabled = e.CanExecute;
        }
        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LocomotiveOperator.Stop();
        }

        private void Brake_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LocomotiveOperator.IsOperable;
        }
        private void Brake_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LocomotiveOperator.Brake();
        }
        #endregion
    }
}
