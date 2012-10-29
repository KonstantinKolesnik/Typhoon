using MFE.Graphics.Controls;
using MFE.Graphics.Media;

namespace MFE.Demo.Demos
{
    class CheckboxDemo : Panel
    {
        public CheckboxDemo()
            : base(0, 0, 0, 0)
        {
            Checkbox chb;
            int size = 20;

            chb = new Checkbox(20, 20, size, size, true);
            Children.Add(chb);
            chb = new Checkbox(50, 20, size, size);
            Children.Add(chb);

            chb = new Checkbox(20, 50, size, size, true);
            chb.BackgroundUnchecked = new SolidColorBrush(Color.BurlyWood);
            chb.BackgroundChecked = new SolidColorBrush(Color.DarkBlue);
            Children.Add(chb);
            chb = new Checkbox(50, 50, size, size);
            chb.BackgroundUnchecked = new SolidColorBrush(Color.BurlyWood);
            chb.BackgroundChecked = new SolidColorBrush(Color.DarkBlue);
            Children.Add(chb);

            chb = new Checkbox(20, 80, size, size, true);
            chb.BackgroundUnchecked = new LinearGradientBrush(Color.BurlyWood, Color.Black);
            chb.BackgroundChecked = new LinearGradientBrush(Color.DarkBlue, Color.Cornsilk);
            Children.Add(chb);
            chb = new Checkbox(50, 80, size, size);
            chb.BackgroundUnchecked = new LinearGradientBrush(Color.BurlyWood, Color.Black);
            chb.BackgroundChecked = new LinearGradientBrush(Color.DarkBlue, Color.Cornsilk);
            Children.Add(chb);

            chb = new Checkbox(20, 110, size + 10, size + 10, true);
            chb.BackgroundUnchecked = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.PowerOff));
            chb.BackgroundChecked = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.PowerOn));
            chb.Border = null;
            Children.Add(chb);
            chb = new Checkbox(60, 110, size + 10, size + 10);
            chb.BackgroundUnchecked = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.PowerOff));
            chb.BackgroundChecked = new ImageBrush(Resources.GetBitmap(Resources.BitmapResources.PowerOn));
            chb.Border = null;
            Children.Add(chb);
        }
    }
}
