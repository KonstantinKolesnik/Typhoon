
namespace Typhoon.Decoders
{
    public class DecoderReference
    {
        #region Fields
        private string fileName = null;
        private string manufacturer = null;
        private string model = null;
        #endregion

        #region Properties
        public string FileName
        {
            get { return fileName; }
        }
        public string Manufacturer
        {
            get { return manufacturer; }
        }
        public string Model
        {
            get { return model; }
        }
        #endregion

        #region Constructor
        public DecoderReference(string fileName, string manufacturer, string model)
        {
            this.fileName = fileName;
            this.manufacturer = manufacturer;
            this.model = model;
        }
        #endregion
    }
}
