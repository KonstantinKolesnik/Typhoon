using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Typhoon.MF.Layouts;
using Typhoon.MF.Presentation.Controls;

namespace Typhoon.GraphicsAdapter.UI
{
    public class ucLayoutItemInfo : HighlightableListBoxItem
    {
        #region Fields
        private LayoutItem layoutItem;
        private Text txtName;
        private Text txtDescription;

        private Button btnEdit;
        private Button btnDelete;
        private Button btnExport;
        #endregion

        #region Properties
        public LayoutItem LayoutItem
        {
            get { return layoutItem; }
            set
            {
                if (layoutItem != value)
                {
                    layoutItem = value;
                    layoutItem.PropertyChanged += new PropertyChangedEventHandler(LayoutItem_PropertyChanged);
                    UpdateInfo();
                }
            }
        }
        #endregion

        #region Events
        public event EventHandler EditWanted;
        public event EventHandler DeleteWanted;
        public event EventHandler ExportWanted;
        #endregion

        #region Constructor
        public ucLayoutItemInfo()
        {
            StackPanel leftPanel = new StackPanel(Orientation.Horizontal);
            BackgroundStackPanel toolbar = new BackgroundStackPanel(Orientation.Horizontal);
            ParameterValue pairPanel = new ParameterValue(leftPanel, toolbar);
            Child = pairPanel;

            InitToolbar(toolbar);
            InitCommonControls(leftPanel);
            InitSpecificControls(leftPanel);
        }
        #endregion

        #region Event handlers
        private void LayoutItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateInfo();
        }
        private void btnEdit_Clicked(object sender, EventArgs e)
        {
            if (EditWanted != null)
                EditWanted(this, EventArgs.Empty);
        }
        private void btnDelete_Clicked(object sender, EventArgs e)
        {
            if (DeleteWanted != null)
                DeleteWanted(this, EventArgs.Empty);
        }
        private void btnExport_Clicked(object sender, EventArgs e)
        {
            if (ExportWanted != null)
                ExportWanted(this, EventArgs.Empty);
        }
        #endregion

        #region Private methods
        private void InitCommonControls(StackPanel leftPanel)
        {
            //Checkbox chbSelection = new Checkbox(Program.FontRegular, "", false);
            //chbSelection.SetMargin(2, 0, 0, 0);
            //chbSelection.VerticalAlignment = VerticalAlignment.Center;
            //leftPanel.Children.Add(chbSelection);

            StackPanel pnl = new StackPanel(Orientation.Vertical);
            leftPanel.Children.Add(pnl);

            txtName = new Text(Program.FontCourierNew9, "");
            txtName.ForeColor = Colors.Gray;// Program.TextColor;
            txtName.VerticalAlignment = VerticalAlignment.Center;
            txtName.TextWrap = true;
            txtName.SetMargin(3, 3, 3, 0);
            pnl.Children.Add(txtName);

            txtDescription = new Text(Program.FontRegular, "");
            //txtDescription.ForeColor = Program.TextColor;
            txtDescription.VerticalAlignment = VerticalAlignment.Center;
            txtDescription.TextWrap = false;
            txtDescription.SetMargin(3, 0, 3, 2);
            pnl.Children.Add(txtDescription);
        }
        private void InitSpecificControls(StackPanel leftPanel)
        {
            StackPanel pnl;

            #region 1
            pnl = new StackPanel(Orientation.Vertical);
            leftPanel.Children.Add(pnl);



            #endregion

        }
        private void InitToolbar(BackgroundStackPanel toolbar)
        {
            toolbar.Background = Program.ToolbarBackground;
            //toolbar.Opacity = 210;// Program.ToolbarOpacity;
            toolbar.SetMargin(0, 0, 2, 0);

            btnEdit = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Edit), Program.ButtonTextColor);
            btnEdit.ImageSize = Program.ButtonIconSize;
            btnEdit.VerticalAlignment = VerticalAlignment.Center;
            btnEdit.Clicked += new EventHandler(btnEdit_Clicked);
            toolbar.Children.Add(btnEdit);

            btnDelete = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Delete), Program.ButtonTextColor);
            btnDelete.ImageSize = Program.ButtonIconSize;
            btnDelete.VerticalAlignment = VerticalAlignment.Center;
            btnDelete.Clicked += new EventHandler(btnDelete_Clicked);
            toolbar.Children.Add(btnDelete);

            btnExport = new Button(Program.FontRegular, "", Resources.GetBitmap(Resources.BitmapResources.Up), Program.ButtonTextColor);
            btnExport.ImageSize = Program.ButtonIconSize;
            btnExport.VerticalAlignment = VerticalAlignment.Center;
            btnExport.Clicked += new EventHandler(btnExport_Clicked);
            toolbar.Children.Add(btnExport);
        }

        private void UpdateInfo()
        {
            txtName.TextContent = layoutItem.Name;
            txtDescription.TextContent = layoutItem.Description;

            UpdateSpecificInfo();
        }
        private void UpdateSpecificInfo()
        {
        }
        #endregion
    }
}
