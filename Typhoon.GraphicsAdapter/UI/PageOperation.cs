using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageOperation : BasePage
    {
        #region Fields
        private BackgroundStackPanel toolbar;
        private ListBox lbThrottles;
        #endregion

        #region Constructor
        public PageOperation()
        {
            //Text txt;
            //Button btn;

            StackPanel panel = new StackPanel(Orientation.Vertical);
            //BackgroundStackPanel panel = new BackgroundStackPanel(Orientation.Vertical);
            //panel.Background = Program.PanelBackground;
            //panel.Opacity = Program.PanelOpacity;
            Child = panel;

            InitToolbar();
            panel.Children.Add(toolbar);
            InitListbox();
            panel.Children.Add(lbThrottles);



            #region Loco
            //StackPanel pnlLoco = new StackPanel(Orientation.Horizontal);

            //cbLocomotive = new UpDownComboBox(Program.FontRegular, 254);
            //cbLocomotive.ButtonDown.BackgroundImage = cbLocomotive.ButtonUp.BackgroundImage = Resources.GetBitmap(Resources.BitmapResources.Bar);
            //foreach (LayoutItem loco in Model.LayoutItems[LayoutItemType.Locomotive])
            //    cbLocomotive.Items.Add(loco);
            //cbLocomotive.SelectedIdx = 0;
            //pnlLoco.Children.Add(cbLocomotive);

            //btn = new Button(Program.FontRegular, "  ?  ", null, Color.White);
            //btn.VerticalAlignment = VerticalAlignment.Center;
            //btn.BackgroundImage = Program.ButtonBackground;
            //pnlLoco.Children.Add(btn);

            //btn = new Button(Program.FontRegular, " X ", null, Color.White);
            ////btn.Image = Resources.GetBitmap(Resources.BitmapResources.Delete);
            ////btn.ImageSize = 16;
            //btn.VerticalAlignment = VerticalAlignment.Center;
            //btn.BackgroundImage = Program.ButtonBackground;
            //pnlLoco.Children.Add(btn);

            //btn = new Button(Program.FontRegular, " + ", null, Color.White);
            ////btn.Image = Resources.GetBitmap(Resources.BitmapResources.Add);
            ////btn.ImageSize = 16;
            //btn.VerticalAlignment = VerticalAlignment.Center;
            //btn.BackgroundImage = Program.ButtonBackground;
            //pnlLoco.Children.Add(btn);

            //panel.Children.Add(pnlLoco);
            #endregion

            //StackPanel pnlOperation = new StackPanel(Orientation.Horizontal);
            //panel.Children.Add(pnlOperation);

            #region Info
            //BackgroundStackPanel pnlInfo = new BackgroundStackPanel(Orientation.Vertical);
            //pnlInfo.Background = null;// Program.PanelBackground;
            //pnlInfo.Opacity = Program.PanelOpacity;
            ////pnlInfo.Border = new Pen(Colors.Gray);
            //pnlInfo.Width = 100;
            //pnlInfo.Height = 125;
            //pnlInfo.SetMargin(0, 3, 0, 3);
            //pnlOperation.Children.Add(pnlInfo);

            //txt = new Text(Program.FontRegular, "Адрес:");
            //txt.ForeColor = Program.TextColor;
            //lblAddress = new Text(Program.FontRegular, "3");
            //lblAddress.ForeColor = Program.TextColor;
            //pnlInfo.Children.Add(new ParameterValue(txt, lblAddress));

            //txt = new Text(Program.FontRegular, "Формат:");
            //txt.ForeColor = Program.TextColor;
            //lblSpeedSteps = new Text(Program.FontRegular, "DCC 128");
            //lblSpeedSteps.ForeColor = Program.TextColor;
            //pnlInfo.Children.Add(new ParameterValue(txt, lblSpeedSteps));
            #endregion

            #region Functions
            //int btnSize = 24;
            //StackPanel pnlFunctions = new StackPanel(Orientation.Horizontal);
            //pnlFunctions.HorizontalAlignment = HorizontalAlignment.Center;
            //panel.Children.Add(pnlFunctions);
            //for (int i = 0; i < 14; i++)
            //{
            //    btn = new Button(Program.FontRegular, "F" + i, null, Program.ButtonTextColor);
            //    btn.VerticalAlignment = VerticalAlignment.Center;
            //    btn.BackgroundImage = Program.ButtonBackground;
            //    btn.Width = btn.Height = btnSize;
            //    pnlFunctions.Children.Add(btn);
            //}

            //pnlFunctions = new StackPanel(Orientation.Horizontal);
            //pnlFunctions.HorizontalAlignment = HorizontalAlignment.Center;
            //panel.Children.Add(pnlFunctions);
            //for (int i = 14; i < 29; i++)
            //{
            //    btn = new Button(Program.FontRegular, "F" + i, null, Program.ButtonTextColor);
            //    btn.VerticalAlignment = VerticalAlignment.Center;
            //    btn.BackgroundImage = Program.ButtonBackground;
            //    btn.Width = btn.Height = btnSize;
            //    pnlFunctions.Children.Add(btn);
            //}
            #endregion
        }
        #endregion

        #region Private methods
        private void InitToolbar()
        {
            toolbar = new BackgroundStackPanel(Orientation.Horizontal);
            toolbar.Background = Program.ButtonBackground;
            toolbar.Opacity = Program.ToolbarOpacity;
            toolbar.HorizontalAlignment = HorizontalAlignment.Right;
            toolbar.SetMargin(0, 3, 3, 3);

            Button btn;

            btn = new Button(Program.FontRegular, "+", Resources.GetBitmap(Resources.BitmapResources.Add), Program.ButtonTextColor);
            btn.ImageSize = Program.ButtonIconSize;
            btn.VerticalAlignment = VerticalAlignment.Center;
            btn.Clicked += new EventHandler(btnAdd_Clicked);
            toolbar.Children.Add(btn);
        }
        private void InitListbox()
        {
            lbThrottles = new ListBox();
        }
        #endregion

        #region Event handlers
        private void btnAdd_Clicked(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
