
namespace Typhoon.Decoders
{
    public class DecoderParameterProgramUnit
    {
        #region Fields
        private uint cv;
        private int bitPosition = -1;
        private byte? value = null;
        #endregion

        #region Properties
        public uint CV
        {
            get { return cv; }
        }
        public int BitPosition
        {
            get { return bitPosition; }
        }
        public byte? Value
        {
            get { return value; }
            set { this.value = value; }
        }
        #endregion

        #region Constructors
        public DecoderParameterProgramUnit(uint cv, byte? value)
            : this(cv, -1, value)
        {
        }
        public DecoderParameterProgramUnit(uint cv, int bitPosition, byte? value)
        {
            this.cv = cv;
            this.bitPosition = bitPosition;
            this.value = value;
        }
        #endregion
    }
}
