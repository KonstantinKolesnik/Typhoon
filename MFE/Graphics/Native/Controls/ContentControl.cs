
namespace MFE.Graphics.Native.Controls
{
    public abstract class ContentControl : Control
    {
        public UIElement Child
        {
            get { return LogicalChildren.Count > 0 ? LogicalChildren[0] : null; }
            set
            {
                LogicalChildren.Clear();
                LogicalChildren.Add(value);
            }
        }

        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            desiredWidth = desiredHeight = 0;

            UIElement child = Child;
            if (child != null)
            {
                child.Measure(availableWidth, availableHeight);
                child.GetDesiredSize(out desiredWidth, out desiredHeight);
            }
        }
    }
}
