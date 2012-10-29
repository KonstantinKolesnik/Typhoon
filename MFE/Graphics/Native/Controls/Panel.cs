
namespace MFE.Graphics.Native.Controls
{
    public class Panel : UIElement
    {
        public UIElementCollection Children
        {
            get { return LogicalChildren; }
        }

        protected override void MeasureOverride(int availableWidth, int availableHeight, out int desiredWidth, out int desiredHeight)
        {
            int childDesiredWidth, childDesiredHeight;
            desiredWidth = desiredHeight = 0;

            UIElementCollection children = LogicalChildren;
            if (children != null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    UIElement child = children[i];
                    child.Measure(availableWidth, availableHeight);

                    child.GetDesiredSize(out childDesiredWidth, out childDesiredHeight);

                    desiredWidth = System.Math.Max(desiredWidth, childDesiredWidth);
                    desiredHeight = System.Math.Max(desiredHeight, childDesiredHeight);
                }
            }
        }
    }
}
