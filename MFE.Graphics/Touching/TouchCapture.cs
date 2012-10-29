using System;
using MFE.Graphics.Controls;

namespace MFE.Graphics.Touching
{
    public static class TouchCapture
    {
        private static Control capturedCtrl = null;

        public static Control Captured
        {
            get { return capturedCtrl; }
        }

        public static void Capture(Control element)
        {
            if (element == null)
                throw new ArgumentNullException();

            capturedCtrl = element;
        }
        public static void ReleaseCapture()
        {
            capturedCtrl = null;
        }
    }
}
