using System;
using Microsoft.SPOT.Hardware;

namespace MF.Engine.GUI
{
    public class FPSCounter
    {
        #region Fields
        private int frames;
        private double framesPerSecond;
        private int accumMsec;
        private int interval = 1000;
        private TimeSpan ts;
        private DateTime dt;
        #endregion

        #region Properties
        /// <summary>Frames per second</summary>
        public double FPS
        {
            get { return framesPerSecond; }
        }
        /// <summary>Updating interval</summary>
        //public int Interval
        //{
        //    get { return interval; }
        //    set
        //    {
        //        if (value > 0)
        //            interval = value;
        //    }
        //}
        #endregion

        #region Constructor
        public FPSCounter()
        {
            ts = Utility.GetMachineTime();
            dt = DateTime.Now;
        }
        #endregion

        public void Update()
        {
            //TimeSpan t = Utility.GetMachineTime() - ts;

            //framesPerSecond = 1 / ((ts.Seconds * 1000 + t.Milliseconds) * 0.001);

            //ts = t;
            DateTime d = DateTime.Now;
            ts = d - dt;
            dt = d;
            framesPerSecond = (double)ts.Ticks / (double)TimeSpan.TicksPerSecond;

            //accumMsec += ts.Seconds * 1000 + ts.Milliseconds;
            //if (accumMsec >= interval)
            //{
            //    framesPerSecond = frames * 1000 / interval;
            //    frames = 0;
            //    accumMsec = 0;
            //    ts = Utility.GetMachineTime();
            //}
            //else
            //    ++frames;

        }
    }
}
