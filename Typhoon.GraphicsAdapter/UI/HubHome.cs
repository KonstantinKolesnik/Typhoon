using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT;

namespace Typhoon.GraphicsAdapter.UI
{
    public class HubHome : Panel
    {
        public HubHome()
            : base(0, 0, LCDManager.ScreenWidth, LCDManager.ScreenHeight - 32)
        {
            InitUI();
        }

        public event EventHandler ShortcutClick;

        private void InitUI()
        {
            ImageBrush brush = new ImageBrush(Program.Background);
            //brush.Opacity = 150;// 70;
            brush.Stretch = Stretch.Fill;
            Background = brush;

            #region Title
            //TextFlow textTitle = new TextFlow();
            //textTitle.HorizontalAlignment = HorizontalAlignment.Left;
            //textTitle.TextAlignment = TextAlignment.Left;
            //textTitle.SetMargin(50,150,0,10);
            //Children.Add(textTitle);

            //textTitle.TextRuns.Add("Typhoon", Program.FontCourierNew12, Colors.White);
            //textTitle.TextRuns.Add(" " + Program.Info.Version, Program.FontRegular, Program.LabelTextColor);
            ////textTitle.TextRuns.Add(TextRun.EndOfLine);
            ////textTitle.TextRuns.Add("© Konstantin Kolesnik", Program.FontRegular, Colors.White);
            #endregion

            #region Menu
            //StackPanel panelMenu = new StackPanel(Orientation.Horizontal);
            //panelMenu.HorizontalAlignment = HorizontalAlignment.Right;
            //panelMenu.SetMargin(0, 10, 0, 0);
            //Children.Add(panelMenu);

            int x = 200;
            CreateToolButton(ref x, "scDatabase", "База данных", Program.ImageDatabase);
            CreateToolButton(ref x, "scOperation", "Управление", Program.ImageOperation);
            CreateToolButton(ref x, "scTopology", "Топология", Program.ImageLayout);
            CreateToolButton(ref x, "scSettings", "Настройки", Program.ImageSettings);
            #endregion
        }
        private void CreateToolButton(ref int x, string name, string text, Bitmap bitmap)
        {
            ToolButton btn = new ToolButton(x, 250, Program.ShortcutIconSize, Program.ShortcutIconSize)
            {
                Name = name,
                Foreground = new ImageBrush(bitmap)
            };
            //btn.Foreground.Opacity = 200;
            btn.Click += new EventHandler(ToolButton_Click);
            Children.Add(btn);

            TextBlock lbl = new TextBlock(x, btn.Area.Bottom, Program.ShortcutIconSize, 20, Program.FontCourierNew10, text)
            {
                ForeColor = Program.ButtonTextColor,
                TextAlignment = TextAlignment.Center,
                TextVerticalAlignment = VerticalAlignment.Top
            };
            Children.Add(lbl);

            x += btn.Width + 10;
        }

        private void ToolButton_Click(object sender, EventArgs e)
        {
            if (ShortcutClick != null)
                ShortcutClick(sender, e);
        }
    }
}
