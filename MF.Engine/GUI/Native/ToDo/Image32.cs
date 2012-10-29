using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation.Media;

namespace MF.Engine.GUI
{
    public class Image32
    {
        #region Fields
        private Bitmap bmp;
        private ColorEX[] pixels;
        #endregion

        #region Properties
        public int Width
        {
            get { return bmp.Width; }
        }
        public int Height
        {
            get { return bmp.Height; }
        }
        #endregion

        #region Constructor
        public Image32(byte[] bytes)
        {
            //if (bytes[0] != 80 || bytes[1] != 50 || bytes[2] != 73)
            //    new Exception("Invalid image format");

            long lPixels = BytesToLong(bytes[3], bytes[4], bytes[5], bytes[6]);
            pixels = new ColorEX[lPixels];

            long lIndex = 7;
            ColorEX c;
            for (long l = 0; l < pixels.Length; l++)
            {
                c = new ColorEX();
                c.X = BytesToInt(bytes[lIndex], bytes[lIndex + 1]);
                lIndex += 2;
                c.Y = BytesToInt(bytes[lIndex], bytes[lIndex + 1]);
                lIndex += 2;
                c.Alpha = (ushort)(bytes[lIndex++] + 1);
                c.Color = ColorUtility.ColorFromRGB(bytes[lIndex], bytes[lIndex + 1], bytes[lIndex + 2]);
                lIndex += 3;
                pixels[l] = c;
            }

            byte[] bBMP = new byte[bytes.Length - lIndex];
            Array.Copy(bytes, (int)lIndex, bBMP, 0, bBMP.Length);
            bmp = new Bitmap(bBMP, Bitmap.BitmapImageType.Bmp);
            
            //bmp.MakeTransparent(ColorUtility.ColorFromRGB(255, 0, 255));
            bmp.MakeTransparent(bmp.GetPixel(0, 0));
        }
        #endregion

        #region Public methods
        public void Draw(Bitmap bmp, int x, int y)
        {
            ColorEX c;
            int i = 0;

            bmp.DrawImage(x, y, this.bmp, 0, 0, this.bmp.Width, this.bmp.Height);
            while (i < pixels.Length)
            {
                c = pixels[i++];
                bmp.DrawRectangle(Color.Black, 0, c.X + x, c.Y + y, 1, 1, 0, 0, c.Color, 0, 0, c.Color, 0, 0, c.Alpha);
            }
        }
        #endregion

        #region Private methods
        private int BytesToLong(byte b1, byte b2, byte b3, byte b4)
        {
            return ((b4 & 0xFF) << 24) + ((b3 & 0xFF) << 16) + ((b2 & 0xFF) << 8) + (b1 & 0xFF);
        }
        private int BytesToInt(byte b1, byte b2)
        {
            return ((b2 & 0xFF) << 8) + (b1 & 0xFF);
        }
        #endregion
    }
}
