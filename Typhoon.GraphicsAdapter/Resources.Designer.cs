//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17626
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Typhoon.GraphicsAdapter
{
    
    internal partial class Resources
    {
        private static System.Resources.ResourceManager manager;
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if ((Resources.manager == null))
                {
                    Resources.manager = new System.Resources.ResourceManager("Typhoon.GraphicsAdapter.Resources", typeof(Resources).Assembly);
                }
                return Resources.manager;
            }
        }
        internal static Microsoft.SPOT.Bitmap GetBitmap(Resources.BitmapResources id)
        {
            return ((Microsoft.SPOT.Bitmap)(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        internal static Microsoft.SPOT.Font GetFont(Resources.FontResources id)
        {
            return ((Microsoft.SPOT.Font)(Microsoft.SPOT.ResourceUtility.GetObject(ResourceManager, id)));
        }
        [System.SerializableAttribute()]
        internal enum BitmapResources : short
        {
            Settings_48 = -31826,
            Home = -29342,
            PowerOn = -26768,
            LedRed = -20757,
            Drive = -19534,
            LedGreen = -14597,
            LedGray = -13686,
            NetworkOff = -12807,
            Layout = -1120,
            PowerOff = 760,
            Back = 1076,
            Keyboard = 3946,
            Database = 6837,
            ButtonBackground = 7118,
            Train_800_480 = 8792,
            Settings = 10758,
            Bar = 16851,
            Mouse = 26648,
            Operation = 29388,
            NetworkOn = 30065,
        }
        [System.SerializableAttribute()]
        internal enum FontResources : short
        {
            CourierNew_12 = -22404,
            CourierNew_10 = -22402,
            CourierNew_9 = 25746,
            LucidaSansUnicode_8 = 30591,
        }
    }
}