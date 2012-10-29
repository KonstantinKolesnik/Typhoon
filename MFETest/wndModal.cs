using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;
using Microsoft.SPOT;

namespace MFETest
{
    class wndModal : Window
    {


        public wndModal(int x, int y, int width, int height)
            : base(x, y, width, height)
        {
            Width = 300;
            Height = 200;

            // maybe to move this to ModalWindow class?
            X = (LCDManager.ScreenWidth - Width) / 2;
            Y = (LCDManager.ScreenHeight - Height) / 2;

            Background = new SolidColorBrush(Color.Orange);

            ToolButton btn = new ToolButton(10, 10, 40, 24);
            btn.Click += delegate(object sender, EventArgs e)
            {
                Close();
            };
            Children.Add(btn);
        }
    }
}
