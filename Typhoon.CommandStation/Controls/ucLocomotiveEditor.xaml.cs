using System.Windows;
using System.Windows.Controls;
using Typhoon.Layouts.LayoutItems;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucLocomotiveEditor : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty LocomotiveProperty = DependencyProperty.Register("Locomotive", typeof(Locomotive), typeof(ucLocomotiveEditor), new PropertyMetadata(null));
        public Locomotive Locomotive
        {
            get { return (Locomotive)GetValue(LocomotiveProperty); }
            set { SetValue(LocomotiveProperty, value); }
        }
        #endregion

        #region Constructor
        public ucLocomotiveEditor()
        {
            InitializeComponent();
        }
        #endregion
    }
}
