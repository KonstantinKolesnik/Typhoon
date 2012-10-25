
namespace Typhoon.NMRA
{
    public class LocomotiveAddress
    {
        private uint address = 3;
        private bool isLong = false;

        public uint Address
        {
            get { return address; }
            set { address = value; }
        }
        public bool Long
        {
            get { return isLong; }
            set { isLong = value; }
        }

        public LocomotiveAddress()
        {
        }
        public LocomotiveAddress(uint address, bool isLong)
        {
            Address = address;
            Long = isLong;
        }
    }
}
