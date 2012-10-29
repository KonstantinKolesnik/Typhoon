using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Typhoon.MF.Layouts;
using Typhoon.MF.Layouts.LayoutItems;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageLocomotiveProperties : BasePage
    {
        #region Fields
        private Locomotive locomotive;
        
        private TabControl tabs;
        private Text txtName;
        private UpDownComboBox cbProtocol;

        #endregion

        #region Constructor
        public PageLocomotiveProperties(Locomotive locomotive)
        {
            this.locomotive = locomotive;

            StackPanel panel = new StackPanel(Orientation.Vertical);
            Child = panel;

            tabs = new TabControl();
            tabs.AddTab(CreateTabCommon());
            tabs.AddTab(CreateTabFunctions());
            panel.Children.Add(tabs);

            StackPanel panelBottom = new StackPanel(Orientation.Horizontal);
            panelBottom.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Children.Add(panelBottom);

            Button btnSave = new Button(Program.FontRegular, "Сохранить", Resources.GetBitmap(Resources.BitmapResources.Save), Program.ButtonTextColor);
            btnSave.SetMargin(2);
            btnSave.VerticalAlignment = VerticalAlignment.Bottom;
            btnSave.HorizontalAlignment = HorizontalAlignment.Center;
            btnSave.Background = Program.ButtonBackground;
            btnSave.Clicked += new EventHandler(btnSave_Clicked);
            panelBottom.Children.Add(btnSave);

            tabs.Height = panel.Parent.Height - btnSave.Height - 8;

            ucWriteBlock writeBlock = new ucWriteBlock();
            writeBlock.VerticalAlignment = VerticalAlignment.Bottom;
            writeBlock.HorizontalAlignment = HorizontalAlignment.Center;
            writeBlock.SetMargin(5, 0, 0, 0);
            writeBlock.WriteClicked += new EventHandler(writeBlock_WriteClicked);
            panelBottom.Children.Add(writeBlock);

            LoadValues();
        }
        #endregion

        #region Event handlers
        private void btnSave_Clicked(object sender, EventArgs e)
        {
            SaveValues();
        }
        private void writeBlock_WriteClicked(object sender, EventArgs e)
        {
            TrackType track = (sender as ucWriteBlock).TrackType;



        }
        #endregion

        #region Private methods
        private Tab CreateTabCommon()
        {
            Tab tab = new Tab(Program.FontRegular, "tabCommon", "Общие", null);
            tab.Background = Program.ButtonBackground;
            tab.ForeColor = Program.LabelTextColor;
            tab.IsSelected = true;

            StackPanel panel = new StackPanel(Orientation.Vertical);
            panel.SetMargin(3);

            //BackgroundStackPanel panel = new BackgroundStackPanel(Orientation.Vertical);
            //panel.Background = Program.PanelBackground;
            //panel.Opacity = Program.PanelOpacity;
            //panel.Height = 150;
            //panel.SetMargin(3);

            tab.Content = panel;

            Text txt;
            StackPanel p;

            txt = new Text(Program.FontRegular, "Имя:");
            txt.ForeColor = Program.LabelTextColor;
            
            p = new StackPanel(Orientation.Horizontal);
            
            txtName = new Text(Program.FontRegular, "");
            txtName.ForeColor = Program.LabelTextColor;
            txtName.VerticalAlignment = VerticalAlignment.Center;
            p.Children.Add(txtName);

            Button btnKeyboard = new Button(Program.FontRegular, " ... ", null, Program.ButtonTextColor);
            btnKeyboard.SetMargin(2, 0, 0, 0);
            btnKeyboard.VerticalAlignment = VerticalAlignment.Center;
            btnKeyboard.Background = Program.ButtonBackground;
            //btnKeyboard.Clicked += new EventHandler(btnSave_Clicked);
            p.Children.Add(btnKeyboard);



            panel.Children.Add(new ParameterValue(txt, p));

            txt = new Text(Program.FontRegular, "Протокол:");
            txt.ForeColor = Program.LabelTextColor;
            cbProtocol = new UpDownComboBox(Program.FontRegular, 120);
            cbProtocol.ButtonDown.Background = cbProtocol.ButtonUp.Background = Program.ButtonBackground;
            cbProtocol.Items.Add("DCC 14");
            cbProtocol.Items.Add("DCC 28");
            cbProtocol.Items.Add("DCC 128");
            cbProtocol.Items.Add("Selectrix");
            cbProtocol.Items.Add("Motorola 14");
            cbProtocol.Items.Add("Motorola 27");
            cbProtocol.Items.Add("Motorola 28");
            cbProtocol.Items.Add("Motorola Fx 14");
            panel.Children.Add(new ParameterValue(txt, cbProtocol));

            txt = new Text(Program.FontRegular, "Адрес:");
            txt.ForeColor = Program.LabelTextColor;
            UpDownComboBox cbProgCurrentThreshold = new UpDownComboBox(Program.FontRegular, 90);
            cbProgCurrentThreshold.ButtonDown.Background = cbProgCurrentThreshold.ButtonUp.Background = Program.ButtonBackground;
            cbProgCurrentThreshold.Items.Add(3);
            panel.Children.Add(new ParameterValue(txt, cbProgCurrentThreshold));

            //txt = new Text(Program.FontRegular, "Выкл. при коротком замыкании во внешн. бустерах:");
            //txt.ForeColor = Program.TextColor;
            //txt.TextWrap = true;
            //chbShutdownOnExternalShortCircuit = new Checkbox(Program.FontRegular);
            //panel.Children.Add(new ParameterValue(txt, chbShutdownOnExternalShortCircuit));

            //txt = new Text(Program.FontRegular, "Калибровка экрана:");
            //txt.ForeColor = Program.TextColor;
            //Button btnCalibrate = new Button(Program.FontRegular, "Калибровать", Resources.GetBitmap(Resources.BitmapResources.Calibrate), Colors.White);
            //btnCalibrate.ImageSize = 16;
            //btnCalibrate.Width = 90;
            //btnCalibrate.BackgroundImage = Program.ButtonBackground;
            //btnCalibrate.Clicked += new EventHandler(btnCalibrate_Clicked);
            //panel.Children.Add(new ParameterValue(txt, btnCalibrate));



            return tab;
        }
        private Tab CreateTabFunctions()
        {
            Tab tab = new Tab(Program.FontRegular, "tabFunctions", "Функции", null);
            tab.Background = Program.ButtonBackground;
            tab.ForeColor = Program.LabelTextColor;

            StackPanel panel = new StackPanel(Orientation.Vertical);
            panel.SetMargin(3);
            tab.Content = panel;






            return tab;
        }


        private void LoadValues()
        {
            txtName.TextContent = locomotive.Name;
            cbProtocol.SelectedIdx = (int)locomotive.Protocol;



        }
        private void SaveValues()
        {
            locomotive.Name = txtName.TextContent;
            locomotive.Protocol = (ProtocolType)cbProtocol.SelectedIdx;

            
            
            Model.Layout.Save();
        }
        #endregion
    }
}
