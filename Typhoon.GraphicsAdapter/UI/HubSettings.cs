//using Microsoft.SPOT;
//using Microsoft.SPOT.Presentation;
//using Microsoft.SPOT.Presentation.Controls;
//using Microsoft.SPOT.Presentation.Media;
//using MFE.Presentation.Controls;
using MFE.Graphics.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class HubSettings : Hub
    {
        #region Fields
        //private TabControl tabs;

        //private UpDownComboBox cbLanguage;
        //private UpDownComboBox cbMainCurrentThreshold;
        //private UpDownComboBox cbProgCurrentThreshold;
        //private Checkbox chbShutdownOnExternalShortCircuit;

        #endregion

        #region Constructor
        public HubSettings()
        {
            Toolbar.Height = 48;
            Image icon = new Image(0, 0, 48, 48, Resources.GetBitmap(Resources.BitmapResources.Settings_48));
            Toolbar.Children.Add(icon);

            Label lbl = new Label(icon.Width + 4, 10, Program.FontCourierNew12, "Настройки") { ForeColor = Program.LabelTextColor };
            Toolbar.Children.Add(lbl);


            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));
            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));
            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));
            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));
            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));
            //RightToolbar.Children.Add(new ToolButton("", new Icon(Program.ImageHome, 22)));

            //StackPanel panel = new StackPanel(Orientation.Vertical);
            ////Root = panel;

            //tabs = new TabControl();
            //panel.Children.Add(tabs);

            //Button btnSave = new Button(Program.FontRegular, "Сохранить", null, Program.ButtonTextColor);
            //btnSave.SetMargin(2);
            //btnSave.VerticalAlignment = VerticalAlignment.Bottom;
            //btnSave.HorizontalAlignment = HorizontalAlignment.Center;
            //btnSave.Background = Program.ButtonBackground;
            //btnSave.Clicked += new EventHandler(btnSave_Clicked);
            //panel.Children.Add(btnSave);

            ////tabs.Height = panel.Parent.Height - btnSave.ImageSize - 10;
            //tabs.VerticalAlignment = VerticalAlignment.Stretch;
            //tabs.HorizontalAlignment = HorizontalAlignment.Stretch;
            
            
            
            ////----------------------------------------
            //tabs.AddTab(CreateTabCommon());
            //tabs.AddTab(CreateTabBoosters());
            //// train operations
            //// accessories & routes
            //// data formats
            //// access control
            ////--------------------------

            //LoadValues();
        }
        #endregion

        #region Private methods
        //private Tab CreateTabCommon()
        //{
        //    Tab tab = new Tab(Program.FontRegular, "tabCommon", "Общие", null);
        //    tab.Background = Program.ButtonBackground;
        //    tab.ForeColor = Program.LabelTextColor;
        //    tab.IsSelected = true;

        //    StackPanel panel = new StackPanel(Orientation.Vertical);
        //    panel.SetMargin(3);
        //    tab.Content = panel;

        //    Text txt;

        //    txt = new Text(Program.FontRegular, "Язык:");
        //    txt.ForeColor = Program.LabelTextColor;
        //    cbLanguage = new UpDownComboBox(Program.FontRegular, 90);
        //    cbLanguage.ButtonDown.Background = cbLanguage.ButtonUp.Background = Program.ButtonBackground;
        //    cbLanguage.Items.Add("Русский");
        //    //cbLanguage.Items.Add("English");
        //    panel.Children.Add(new ParameterValue(txt, cbLanguage));

        //    txt = new Text(Program.FontRegular, "Калибровка экрана:");
        //    txt.ForeColor = Program.LabelTextColor;
        //    Button btnCalibrate = new Button(Program.FontRegular, "Калибровать", null, Program.ButtonTextColor);
        //    btnCalibrate.ImageSize = Program.ButtonIconSize;
        //    btnCalibrate.Width = 90;
        //    btnCalibrate.Background = Program.ButtonBackground;
        //    btnCalibrate.Clicked += new EventHandler(btnCalibrate_Clicked);
        //    panel.Children.Add(new ParameterValue(txt, btnCalibrate));



        //    return tab;
        //}
        //private Tab CreateTabBoosters()
        //{
        //    Tab tab = new Tab(Program.FontRegular, "tabBoosters", "Бустеры", null);
        //    tab.Background = Program.ButtonBackground;
        //    tab.ForeColor = Program.LabelTextColor;
            
        //    StackPanel panel = new StackPanel(Orientation.Vertical);
        //    panel.SetMargin(3);
        //    tab.Content = panel;

        //    Text txt;

        //    txt = new Text(Program.FontRegular, "Максимальный ток главного пути, mA:");
        //    txt.ForeColor = Program.LabelTextColor;
        //    cbMainCurrentThreshold = new UpDownComboBox(Program.FontRegular, 90);
        //    cbMainCurrentThreshold.ButtonDown.Background = cbMainCurrentThreshold.ButtonUp.Background = Program.ButtonBackground;
        //    cbMainCurrentThreshold.Items.Add(1000);
        //    cbMainCurrentThreshold.Items.Add(2000);
        //    cbMainCurrentThreshold.Items.Add(3000);
        //    cbMainCurrentThreshold.Items.Add(4000);
        //    panel.Children.Add(new ParameterValue(txt, cbMainCurrentThreshold));

        //    txt = new Text(Program.FontRegular, "Максимальный ток программного пути, mA:");
        //    txt.ForeColor = Program.LabelTextColor;
        //    cbProgCurrentThreshold = new UpDownComboBox(Program.FontRegular, 90);
        //    cbProgCurrentThreshold.ButtonDown.Background = cbProgCurrentThreshold.ButtonUp.Background = Program.ButtonBackground;
        //    cbProgCurrentThreshold.Items.Add(500);
        //    cbProgCurrentThreshold.Items.Add(600);
        //    cbProgCurrentThreshold.Items.Add(700);
        //    cbProgCurrentThreshold.Items.Add(800);
        //    cbProgCurrentThreshold.Items.Add(900);
        //    cbProgCurrentThreshold.Items.Add(1000);
        //    panel.Children.Add(new ParameterValue(txt, cbProgCurrentThreshold));

        //    txt = new Text(Program.FontRegular, "Выкл. при коротком замыкании во внешн. бустерах:");
        //    txt.ForeColor = Program.LabelTextColor;
        //    txt.TextWrap = true;
        //    chbShutdownOnExternalShortCircuit = new Checkbox(Program.FontRegular);
        //    panel.Children.Add(new ParameterValue(txt, chbShutdownOnExternalShortCircuit));

        //    return tab;
        //}


        //private void LoadValues()
        //{
        //    for (int i = 0; i < cbLanguage.Items.Count; i++)
        //    {
        //        if ((string)cbLanguage.Items[i] == Model.Options.Language)
        //        {
        //            cbLanguage.SelectedIdx = i;
        //            break;
        //        }
        //    }
        //    for (int i = 0; i < cbMainCurrentThreshold.Items.Count; i++)
        //    {
        //        if ((int)cbMainCurrentThreshold.Items[i] == Model.Options.MainBridgeCurrentThreshould)
        //        {
        //            cbMainCurrentThreshold.SelectedIdx = i;
        //            break;
        //        }
        //    }
        //    for (int i = 0; i < cbProgCurrentThreshold.Items.Count; i++)
        //    {
        //        if ((int)cbProgCurrentThreshold.Items[i] == Model.Options.ProgBridgeCurrentThreshould)
        //        {
        //            cbProgCurrentThreshold.SelectedIdx = i;
        //            break;
        //        }
        //    }
        //    //chbShutdownOnExternalShortCircuit.IsChecked = Model.Options.ShutdownOnExternalShortCircuit;




        //}
        //private void SaveValues()
        //{
        //    Model.Options.Language = (string)cbLanguage.Items[cbLanguage.SelectedIdx];
        //    Model.Options.MainBridgeCurrentThreshould = (int)cbMainCurrentThreshold.Items[cbMainCurrentThreshold.SelectedIdx];
        //    Model.Options.ProgBridgeCurrentThreshould = (int)cbProgCurrentThreshold.Items[cbProgCurrentThreshold.SelectedIdx];
        //    //Model.Options.ShutdownOnExternalShortCircuit = chbShutdownOnExternalShortCircuit.IsChecked;
            
            
            
            
        //    Model.Options.Save();
        //}
        #endregion

        #region Event handlers
        //private void btnCalibrate_Clicked(object sender, EventArgs e)
        //{
        //    Program.Instance.CalibrateScreen();
        //}
        //private void btnSave_Clicked(object sender, EventArgs e)
        //{
        //    SaveValues();
        //}
        #endregion
    }
}
