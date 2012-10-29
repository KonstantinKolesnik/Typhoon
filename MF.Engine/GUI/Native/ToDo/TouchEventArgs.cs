
namespace MF.Engine.GUI
{
    internal struct TouchEventArgs
    {
        public Point location;
        public int type;

        public TouchEventArgs(Point e, int type)
        {
            this.location = e;
            this.type = type;
        }
    }
}
