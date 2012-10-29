using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Typhoon.MF.Layouts;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class ucWriteBlock : StackPanel
    {
        #region Fields
        private UpDownComboBox cbTrackType;
        #endregion

        #region Properties
        public TrackType TrackType
        {
            get { return (TrackType)cbTrackType.SelectedIdx; }
        }
        #endregion

        #region Events
        public event EventHandler WriteClicked;
        #endregion

        #region Constructor
        public ucWriteBlock()
        {
            Orientation = Orientation.Horizontal;

            Button btnWrite = new Button(Program.FontRegular, "Записать", null, Program.ButtonTextColor);
            btnWrite.SetMargin(2);
            btnWrite.VerticalAlignment = VerticalAlignment.Center;
            btnWrite.Background = Program.ButtonBackground;
            btnWrite.Clicked += new EventHandler(btnWrite_Clicked);
            Children.Add(btnWrite);

            cbTrackType = new UpDownComboBox(Program.FontRegular, 130);
            cbTrackType.ButtonDown.Background = cbTrackType.ButtonUp.Background = Program.ButtonBackground;
            cbTrackType.VerticalAlignment = VerticalAlignment.Center;
            cbTrackType.Items.Add("На главном");
            cbTrackType.Items.Add("На программном");
            cbTrackType.SelectedIdx = 1;
            Children.Add(cbTrackType);
        }
        #endregion

        #region Event handlers
        private void btnWrite_Clicked(object sender, EventArgs e)
        {
            if (WriteClicked != null)
                WriteClicked(this, EventArgs.Empty);
        }
        #endregion
    }
}
