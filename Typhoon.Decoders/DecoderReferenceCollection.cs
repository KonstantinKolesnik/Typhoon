using System.Collections.ObjectModel;
using System.IO;

namespace Typhoon.Decoders
{
    public class DecoderReferenceCollection : ObservableCollection<DecoderReference>
    {
        public DecoderReferenceCollection(string folderName, DecoderType type)
        {
            string[] files = Directory.GetFiles(folderName, "*.decoder", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                Decoder decoder = new Decoder();
                if (decoder.LoadFromFile(fileName))
                {
                    if (type == DecoderType.Any || decoder.Type == type)
                        Add(new DecoderReference(fileName, decoder.Manufacturer, decoder.Model));
                }
            }
        }
    }
}
