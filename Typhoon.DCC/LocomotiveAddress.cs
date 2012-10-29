using System.Collections;

namespace Typhoon.DCC
{
    public class LocomotiveAddress
    {
        #region Fields
        private int address = 3;
        private bool isLong = false;
        #endregion

        #region Properties
        public int Address
        {
            get { return address; }
            set { address = value; }
        }
        public bool Long
        {
            get { return isLong; }
            set { isLong = value; }
        }
        #endregion

        #region Constructors
        //public LocomotiveAddress()
        //{
        //}
        public LocomotiveAddress(int address, bool isLong)
        {
            Address = address;
            Long = isLong;
        }
        #endregion

        #region Public methods
        public ArrayList GetBytes()
        {
            ArrayList bytes = new ArrayList();
            if (!isLong) // short address
            {
                if (address >= 1 && address <= DCC.LocoShortAddressMax)
                    bytes.Add((byte)(address & 0x7F)); // 7F = 01111111
            }
            else // long address
            {
                if (address >= 0 && address <= 10239)
                {
                    bytes.Add((byte)(192 + ((address / 256) & 0x3F))); // 192 = 11000000, 3F = 00111111
                    bytes.Add((byte)(address & 0xFF));
                }
            }
            return bytes;
        }
        #endregion
    }
}
