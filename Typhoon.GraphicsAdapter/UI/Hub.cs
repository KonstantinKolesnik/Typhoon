using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using MFE.LCD;

namespace Typhoon.GraphicsAdapter.UI
{
    public abstract class Hub : Panel
    {
        #region Fields
        protected Panel Toolbar;
        protected Panel ContentHolder;
        #endregion

        public Hub()
            : base(0, 0, LCDManager.ScreenWidth, LCDManager.ScreenHeight - 32)
        {
            InitUI();
        }

        private void InitUI()
        {
            Children.Add(Toolbar = InitToolBar());
            Children.Add(ContentHolder = InitContentHolder());
        }
        private Panel InitToolBar()
        {
            ImageBrush brush = new ImageBrush(Program.ToolbarBackground);
            brush.Opacity = Program.ToolbarOpacity;
            brush.Stretch = Stretch.Fill;

            return new Panel(0, 0, Width, 32) { Background = brush };
        }
        private Panel InitContentHolder()
        {
            return new Panel(0, 32, Width, Height - 32);
        }

        public virtual void Exit()
        {
        }
    }
}
