using System.IO;
using System.Threading;

namespace Typhoon.Server.Hardware
{
    public static class MP3Control
    {
        private static Thread thread = null;
        private static bool done;
        private static FileStream file;
        private static object source;
        private static byte[] buffer = new byte[2 * 1024];

        public static void SetSource(object src)
        {
            source = src;
        }
        public static bool InUse()
        {
            return thread == null ? false : thread.IsAlive;
        }
        public static void Play()
        {
            Stop();
            //VS1053.Initialize();

            done = false;
            thread = new Thread(Loop);
            thread.Start();
        }
        public static void Stop()
        {
            done = true;
            //VS1053.Shutdown();
        }

        private static void Loop()
        {
            if (source is string)
            {
            //    file = File.Open((string)source, FileMode.Open, FileAccess.Read);
            //    int count = file.Read(buffer, 0, buffer.Length);
            //    while (count > 0 && !done)
            //    {
            //        VS1053.SendData(buffer);
            //        count = file.Read(buffer, 0, buffer.Length);
            //    }
            //    file.Close();

            }
            //else if (source is Resources.BinaryResources)
            //{
            //    VS1053.SendData(Resources.GetBytes((Resources.BinaryResources)source));
            //}
        }
    }
}
