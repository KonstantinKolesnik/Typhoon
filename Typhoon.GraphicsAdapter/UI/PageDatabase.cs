using System.Collections;
using Microsoft.SPOT;
using Microsoft.SPOT.Input;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Typhoon.Layouts;
using Typhoon.MF.Layouts;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageDatabase : BasePage
    {
        #region Fields
        private QuaziMessageBox dlg;

        private BackgroundStackPanel toolbar;
        private WrapPanel filter;
        private ListBox lbItems;
        private Button btnAdd;
        private Button btnClear;
        private Button btnImport;
        private Checkbox chbLocomotives;
        private Checkbox chbConsists;
        private Checkbox chbTurnouts;
        private Checkbox chbSignals;
        private Checkbox chbTurntables;
        private Checkbox chbAccessoryGroups;

        private int x, y;
        #endregion

        #region Constructor
        public PageDatabase()
        {
            StackPanel panel = new StackPanel(Orientation.Vertical);
            Child = panel;

            InitToolbar();
            panel.Children.Add(toolbar);
            InitListbox();
            panel.Children.Add(lbItems);

            PopulateList(null);

            Model.Layout.Modified += new Layout.ModifiedEventHandler(Layout_Modified);
        }
        #endregion

        #region Private methods
        private void InitToolbar()
        {
            toolbar = new BackgroundStackPanel(Orientation.Horizontal);
            toolbar.Background = Program.ToolbarBackground;
            toolbar.Opacity = Program.ToolbarOpacity;

            btnAdd = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Add), Program.ButtonTextColor);
            btnAdd.ImageSize = Program.ButtonIconSize;
            btnAdd.VerticalAlignment = VerticalAlignment.Center;
            btnAdd.Clicked += new EventHandler(btnAdd_Clicked);
            toolbar.Children.Add(btnAdd);

            btnImport = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Down), Program.ButtonTextColor);
            btnImport.ImageSize = Program.ButtonIconSize;
            btnImport.VerticalAlignment = VerticalAlignment.Center;
            btnImport.Clicked += new EventHandler(btnImport_Clicked);
            toolbar.Children.Add(btnImport);

            btnClear = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Clear), Program.ButtonTextColor);
            btnClear.ImageSize = Program.ButtonIconSize;
            btnClear.VerticalAlignment = VerticalAlignment.Center;
            btnClear.Clicked += new EventHandler(btnClear_Clicked);
            toolbar.Children.Add(btnClear);
            
            

            filter = new WrapPanel(Orientation.Horizontal);
            filter.VerticalAlignment = VerticalAlignment.Center;
            toolbar.Children.Add(filter);
            
            chbLocomotives = new Checkbox(Program.FontRegular, "Локомотивы", true);
            chbLocomotives.VerticalAlignment = VerticalAlignment.Center;
            chbLocomotives.SetMargin(2, 0, 2, 0);
            chbLocomotives.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbLocomotives);

            chbConsists = new Checkbox(Program.FontRegular, "Составы", true);
            chbConsists.VerticalAlignment = VerticalAlignment.Center;
            chbConsists.SetMargin(2, 0, 2, 0);
            chbConsists.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbConsists);

            chbTurnouts = new Checkbox(Program.FontRegular, "Стрелки", true);
            chbTurnouts.VerticalAlignment = VerticalAlignment.Center;
            chbTurnouts.SetMargin(2, 0, 2, 0);
            chbTurnouts.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbTurnouts);

            chbSignals = new Checkbox(Program.FontRegular, "Сигналы", true);
            chbSignals.VerticalAlignment = VerticalAlignment.Center;
            chbSignals.SetMargin(2, 0, 2, 0);
            chbSignals.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbSignals);

            chbTurntables = new Checkbox(Program.FontRegular, "Поворотные круги", true);
            chbTurntables.VerticalAlignment = VerticalAlignment.Center;
            chbTurntables.SetMargin(2, 0, 2, 0);
            chbTurntables.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbTurntables);

            chbAccessoryGroups = new Checkbox(Program.FontRegular, "Группы аксессуаров", true);
            chbAccessoryGroups.VerticalAlignment = VerticalAlignment.Center;
            chbAccessoryGroups.SetMargin(2, 0, 2, 0);
            chbAccessoryGroups.IsCheckedChanged += new PropertyChangedEventHandler(Filters_IsCheckedChanged);
            filter.Children.Add(chbAccessoryGroups);
        }
        private void InitListbox()
        {
            lbItems = new ListBox();
            lbItems.Background = new SolidColorBrush(Color.White);
            lbItems.ScrollingStyle = ScrollingStyle.LineByLine;
            ((ScrollViewer)lbItems.Child).LineHeight = 10;
            lbItems.Child.Width = SystemMetrics.ScreenWidth;
            //lbItems.Child.Height = Height - toolbar.ActualHeight;
            lbItems.Child.VerticalAlignment = VerticalAlignment.Stretch;

            lbItems.TouchDown += new TouchEventHandler(lbItems_TouchDown);
            lbItems.TouchMove += new TouchEventHandler(lbItems_TouchMove);
            lbItems.TouchUp += new TouchEventHandler(lbItems_TouchUp);

            lbItems.TouchGestureStart += new TouchGestureEventHandler(lbItems_TouchGestureStart);
            lbItems.TouchGestureChanged += new TouchGestureEventHandler(lbItems_TouchGestureChanged);
            lbItems.TouchGestureEnd += new TouchGestureEventHandler(lbItems_TouchGestureEnd);
        }

        private void PopulateList(LayoutItem layoutItemToSelect)
        {
            ArrayList filter = GetFilter();
            lbItems.Items.Clear();
            foreach (LayoutItem layoutItem in Model.Layout.Items)
            {
                if (IsInFilter(layoutItem.Type, filter))
                {
                    ucLayoutItemInfo lbItem = CreateListBoxItem(layoutItem);
                    lbItems.Items.Add(lbItem);
                    lbItems.Items.Add(new SeparatorListBoxItem(Colors.LightGray));

                    if (layoutItemToSelect != null && layoutItemToSelect == layoutItem)
                    {
                        lbItems.SelectedItem = lbItem;
                        lbItems.ScrollIntoView(lbItem);
                    }
                }
            }
            lbItems.Invalidate();
        }
        private ArrayList GetFilter()
        {
            ArrayList filter = new ArrayList();
            if (chbLocomotives.IsChecked)
                filter.Add(LayoutItemType.Locomotive);
            if (chbConsists.IsChecked)
                filter.Add(LayoutItemType.Consist);
            if (chbTurnouts.IsChecked)
                filter.Add(LayoutItemType.Turnout);
            if (chbSignals.IsChecked)
                filter.Add(LayoutItemType.Signal);
            if (chbTurntables.IsChecked)
                filter.Add(LayoutItemType.Turntable);
            if (chbAccessoryGroups.IsChecked)
                filter.Add(LayoutItemType.AccessoryGroup);

            return filter;
        }
        private bool IsInFilter(LayoutItemType type, ArrayList types)
        {
            foreach (LayoutItemType t in types)
                if (t == type)
                    return true;

            return false;
        }
        private ucLayoutItemInfo CreateListBoxItem(LayoutItem layoutItem)
        {
            ucLayoutItemInfo lbItem = new ucLayoutItemInfo();
            lbItem.LayoutItem = layoutItem;
            lbItem.SelectionBackgroundImage = Program.ButtonBackground;
            lbItem.Opacity = 10;

            lbItem.TouchDown += new TouchEventHandler(Item_TouchDown);
            lbItem.EditWanted += new EventHandler(Item_EditWanted);
            lbItem.DeleteWanted += new EventHandler(Item_DeleteWanted);
            lbItem.ExportWanted += new EventHandler(Item_ExportWanted);

            return lbItem;
        }
        #endregion

        #region Event handlers
        private void Layout_Modified(object sender, Layout.ModifiedAction action, LayoutItem layoutItem)
        {
            switch (action)
            {
                case Layout.ModifiedAction.ItemAdded:
                    ArrayList filter = GetFilter();
                    if (IsInFilter(layoutItem.Type, filter))
                    {
                        ucLayoutItemInfo lbItem = CreateListBoxItem(layoutItem);
                        lbItems.Items.Add(lbItem);
                        lbItems.Items.Add(new SeparatorListBoxItem(Colors.LightGray));
                        lbItems.SelectedItem = lbItem;
                        lbItems.ScrollIntoView(lbItem);
                    }
                    break;
            }
        }

        private void Filters_IsCheckedChanged(object sender, PropertyChangedEventArgs e)
        {
            ucLayoutItemInfo item = lbItems.SelectedItem as ucLayoutItemInfo;
            PopulateList(item != null ? item.LayoutItem : null);
        }

        private void lbItems_TouchDown(object sender, TouchEventArgs e)
        {
            TouchCapture.Capture(lbItems, CaptureMode.Element);
            e.GetPosition(lbItems, 0, out x, out y);
        }
        private void lbItems_TouchMove(object sender, TouchEventArgs e)
        {
            if (TouchCapture.Captured == lbItems)
            {
                int y_old = y;
                e.GetPosition(lbItems, 0, out x, out y);

                int dy = y - y_old;
                bool down = dy < 0;
                dy = System.Math.Abs(dy);
                while (dy > 0)
                {
                    if (down)
                        ((ScrollViewer)lbItems.Child).LineDown();
                    else
                        ((ScrollViewer)lbItems.Child).LineUp();
                    dy--;
                }
            }
        }
        private void lbItems_TouchUp(object sender, TouchEventArgs e)
        {
            TouchCapture.Capture(lbItems, CaptureMode.None);
        }
        private void lbItems_TouchGestureStart(object sender, TouchGestureEventArgs e)
        {

        }
        private void lbItems_TouchGestureChanged(object sender, TouchGestureEventArgs e)
        {
            ushort a = e.Arguments;
        }
        private void lbItems_TouchGestureEnd(object sender, TouchGestureEventArgs e)
        {

        }

        private void Item_TouchDown(object sender, TouchEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            if (lbItems.SelectedItem != item)
                lbItems.SelectedItem = item;
        }
        private void Item_EditWanted(object sender, EventArgs e)
        {
            ucLayoutItemInfo lbItem = sender as ucLayoutItemInfo;
            //switch (lbItem.LayoutItem.Type)
            //{
            //    case LayoutItemType.Locomotive:
            //        Desktop.AddPage(new PageLocomotiveProperties(lbItem.LayoutItem as Locomotive));
            //        break;



            //}
        }
        private void Item_DeleteWanted(object sender, EventArgs e)
        {
            MessageBox dlg = new MessageBox(
                Program.FontRegular,
                Program.MessageBoxWidth,
                "Удалить выбранный элемент?",
                MessageBoxButton.YesNo,
                Program.ButtonBackground,
                Program.ButtonTextColor,
                Program.LabelTextColor);

            if (dlg.ShowDialog() == MessageBoxResult.Yes)
            {
                // check if used !!!



                ucLayoutItemInfo lbItem = sender as ucLayoutItemInfo;
                lbItems.Items.Remove(lbItem);
                Model.Layout.Items.Remove(lbItem.LayoutItem);
            }
        }

        private void Item_ExportWanted(object sender, EventArgs e)
        {

        }
        
        private void btnAdd_Clicked(object sender, EventArgs e)
        {
            //MessageBox dlg = new MessageBox(
            //    Program.FontRegular,
            //    Program.MessageBoxWidth,
            //    "Test",
            //    MessageBoxButton.OK,
            //    Program.ButtonBackground,
            //    Program.ButtonTextColor,
            //    Program.TextColor
            //    );
            //dlg.ShowDialog();



            dlg = new QuaziMessageBox(
                Program.FontRegular,
                Program.MessageBoxWidth,
                "Test",
                MessageBoxButton.OK,
                Program.ButtonBackground,
                Program.ButtonTextColor,
                Program.LabelTextColor,
                dlg_Closed
                );
            //Desktop.AddModalElement(dlg);
            (Parent as Panel).Children.Add(dlg);
            dlg.ShowDialog();

            //Desktop.AddPage(new PageNewLayoutItem());

            //Locomotive loco = new Locomotive();
            //Model.LayoutItems.Add(loco);

        }
        private void btnClear_Clicked(object sender, EventArgs e)
        {
            // TODO: ask!!!

            lbItems.Items.Clear();
            Model.Layout.Items.Clear();
        }
        private void btnImport_Clicked(object sender, EventArgs e)
        {
        }
        private void dlg_Closed(object sender, EventArgs e)
        {
            //Desktop.RemoveModalElement(dlg);
            (Parent as Panel).Children.Add(dlg);
        }

        #endregion
    }
}
