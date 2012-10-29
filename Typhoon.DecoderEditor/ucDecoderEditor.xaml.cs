using System.Windows;
using System.Windows.Controls;
using Typhoon.Decoders;

namespace Typhoon.DecoderEditor
{
    public partial class ucDecoderEditor : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty DecoderProperty = DependencyProperty.Register("Decoder", typeof(Decoder), typeof(ucDecoderEditor), new PropertyMetadata(null));
        public Decoder Decoder
        {
            get { return (Decoder)GetValue(DecoderProperty); }
            set { SetValue(DecoderProperty, value); }
        }
        #endregion

        public ucDecoderEditor()
        {
            InitializeComponent();
        }
    }
}
