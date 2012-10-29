using Microsoft.SPOT.Input;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class PageFileBrowser : BasePage
    {
        #region Fields
        private FileBrowser fb;
        #endregion

        #region Constructor
        public PageFileBrowser(FileBrowser.StorageType deviceType)
        {
            fb = new FileBrowser(deviceType, Program.FontRegular);
            Child = fb;
            Buttons.Focus(fb);
        }
        #endregion

        #region Public methods
        public void Update()
        {
            fb.Update();
        }
        public override void Exit()
        {
            if (fb != null)
                fb.Dispose();

            base.Exit();
        }
        #endregion
    }
}
