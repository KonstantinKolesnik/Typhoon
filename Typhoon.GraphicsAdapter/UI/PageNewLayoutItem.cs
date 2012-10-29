using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageNewLayoutItem : BasePage
    {
        #region Fields
        private RadioButtonGroup itemTypes;
        #endregion

        #region Constructor
        public PageNewLayoutItem()
        {
            //BackgroundStackPanel panel = new BackgroundStackPanel(Orientation.Vertical);
            //panel.Background = Program.PanelBackground;
            //panel.Opacity = Program.PanelOpacity;

            StackPanel panel = new StackPanel(Orientation.Vertical);
            Child = panel;

            itemTypes = new RadioButtonGroup(Orientation.Vertical);
            itemTypes.VerticalAlignment = VerticalAlignment.Center;
            itemTypes.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Children.Add(itemTypes);

            Font font = Program.FontRegular;
            itemTypes.AddButton(new RadioButton(font, "Локомотив"));
            itemTypes.AddButton(new RadioButton(font, "Состав"));
            itemTypes.AddButton(new RadioButton(font, "Стрелка"));
            itemTypes.AddButton(new RadioButton(font, "Сигнал"));
            itemTypes.AddButton(new RadioButton(font, "Поворотный круг"));
            itemTypes.AddButton(new RadioButton(font, "Группа аксессуаров"));
            itemTypes.SelectedIndex = 0;
            //itemTypes.Children[1].SetMargin(0, 0, 0, 15);
            //itemTypes.Children[1].HorizontalAlignment = HorizontalAlignment.Left;

            Button btnOK = new Button(Program.FontRegular, "Добавить", Resources.GetBitmap(Resources.BitmapResources.OK), Program.ButtonTextColor);
            btnOK.SetMargin(2);
            btnOK.VerticalAlignment = VerticalAlignment.Bottom;
            btnOK.HorizontalAlignment = HorizontalAlignment.Center;
            btnOK.Background = Program.ButtonBackground;
            btnOK.Clicked += new EventHandler(btnOK_Clicked);
            panel.Children.Add(btnOK);

            itemTypes.Height = panel.Parent.Height - btnOK.Height - 8;
        }
        #endregion

        #region Event handlers
        private void btnOK_Clicked(object sender, EventArgs e)
        {
            //Desktop.RemoveLastPage();
            //switch (itemTypes.SelectedIndex)
            //{
            //    case 0: // Locomotives
            //        Locomotive loco = new Locomotive();
            //        Model.LayoutItems.Add(loco);
            //        Desktop.AddPage(new PageLocomotiveProperties(loco));
            //        break;




            //}
        }
        #endregion

    }
}
