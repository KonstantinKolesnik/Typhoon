using System.Windows;
using System.Windows.Controls;

namespace Typhoon.CommandStation.Controls
{
    public partial class ucPager : UserControl
    {
        #region DependencyProperties
        public static DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ItemCollection), typeof(ucPager), new PropertyMetadata(null));
        public ItemCollection Items
        {
            get { return (ItemCollection)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }
        public static DependencyProperty ListBoxProperty = DependencyProperty.Register("ListBox", typeof(ListBox), typeof(ucPager), new PropertyMetadata(null));
        public ListBox ListBox
        {
            get { return (ListBox)GetValue(ListBoxProperty); }
            set { SetValue(ListBoxProperty, value); }
        }
        #endregion

        public ucPager()
        {
            InitializeComponent();

            Items = lbListBox.Items;
            ListBox = lbListBox;
        }

        private void ButtonLeft_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedIndex <= 0)
                ListBox.SelectedIndex = ListBox.Items.Count - 1;
            else
                ListBox.SelectedIndex--;
        }
        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.SelectedIndex == ListBox.Items.Count - 1)
                ListBox.SelectedIndex = 0;
            else
                ListBox.SelectedIndex++;
        }

    }
}
