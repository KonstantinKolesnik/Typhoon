using System.Windows;
using System.Windows.Controls;
using Typhoon.Layouts.LayoutItems;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucConsistEditor : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty ConsistProperty = DependencyProperty.Register("Consist", typeof(Consist), typeof(ucConsistEditor), new PropertyMetadata(null));
        public Consist Consist
        {
            get { return (Consist)GetValue(ConsistProperty); }
            set { SetValue(ConsistProperty, value); }
        }
        #endregion

        #region Constructor
        public ucConsistEditor()
        {
            InitializeComponent();
        }
        #endregion
    }
}
