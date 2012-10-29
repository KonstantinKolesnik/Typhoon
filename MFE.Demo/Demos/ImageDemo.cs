using MFE.Graphics.Controls;
using MFE.Graphics.Media;
using Microsoft.SPOT;

namespace MFE.Demo.Demos
{
    class ImageDemo : Panel
    {
        public ImageDemo()
            : base(0, 0, 0, 0)
        {
            Image img;
            int size = 128;
            Bitmap bmp = Resources.GetBitmap(Resources.BitmapResources.Operation);

            img = new Image(20, 10, size, size, bmp);
            Children.Add(img);

            img = new Image(150, 10, size+100, size, bmp);
            Children.Add(img);


            img = new Image(20, 150, size, size, bmp) { Border = new Pen(Color.Red, 2) };
            Children.Add(img);

            img = new Image(150, 150, size, size, bmp) { Opacity = 80 };
            Children.Add(img);

            img = new Image(280, 150, size, size, bmp) { Opacity = 150, Background = new LinearGradientBrush(Color.LimeGreen, Color.CornflowerBlue) { Opacity = 80 } };
            Children.Add(img);
        }
    }
}
