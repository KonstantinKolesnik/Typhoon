//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18010
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MFETest
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
                    Resources.manager = new System.Resources.ResourceManager("MFETest.Resources", typeof(Resources).Assembly);
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
            Home = -29342,
            Drive = -19534,
            Background_800_600 = 3700,
            Keyboard = 3946,
            GHILogo = 5830,
            Database = 6837,
            ButtonBackground = 7118,
            Settings = 10758,
            Bar = 16851,
            Mouse = 26648,
            Operation = 29388,
        }
        [System.SerializableAttribute()]
        internal enum FontResources : short
        {
            CourierNew_10 = -22402,
        }
    }
}
