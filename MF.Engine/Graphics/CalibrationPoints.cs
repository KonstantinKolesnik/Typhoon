using System;

namespace MF.Engine.Graphics
{
    [Serializable]
    public sealed class CalibrationPoints
    {
        public short[] ScreenX;
        public short[] ScreenY;
        public short[] TouchX;
        public short[] TouchY;

        public int PointCount
        {
            get { return ScreenX.Length == 0 ? 0 : ScreenX.Length; }
        }
    }
}
