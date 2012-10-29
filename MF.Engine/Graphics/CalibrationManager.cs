using System;
using Microsoft.SPOT;

namespace MF.Engine.Graphics
{
    public class CalibrationManager : MarshalByRefObject
    {
        #region Fields
        private int idx = 0;

        private class CalibrationPointsID { }
        private static ExtendedWeakReference ewrCalibrationPoints;
        private static CalibrationPoints calibrationPoints = null;
        #endregion

    }
}
