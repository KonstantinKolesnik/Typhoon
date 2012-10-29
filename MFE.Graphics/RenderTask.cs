using MFE.Graphics.Controls;
using MFE.Graphics.Geometry;

namespace MFE.Graphics
{
    class RenderTask
    {
        public Control Control;
        public Rect DirtyArea;

        public RenderTask(Control control, Rect dirtyArea)
        {
            Control = control;
            DirtyArea = dirtyArea;
        }
    }
}
