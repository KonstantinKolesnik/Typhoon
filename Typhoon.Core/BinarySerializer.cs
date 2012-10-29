using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Typhoon.Core
{
    public static class BinarySerializer
    {
        public static Object FromFile(string FileName, bool compressed)
        {
            if (File.Exists(FileName))
            {
                Object obj;
                using (Stream file = File.Open(FileName, FileMode.Open))
                {
                    obj = FromStream(file, compressed);
                }
                return obj;
            }
            return null;
        }
        public static Object FromStream(Stream stream, bool compressed)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (compressed)
                {
                    using (MemoryStream temp = new MemoryStream())
                    {
                        DecompressStream(stream, temp);
                        return bf.Deserialize(temp);
                    }
                }
                else
                    return bf.Deserialize(stream);
            }
            catch (SerializationException)
            {
                //MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public static Object FromByteArray(byte[] buffer, bool compressed)
        {
            Object temp;
            using (MemoryStream stream = new MemoryStream(buffer))
                temp = FromStream(stream, compressed);
            return temp;
        }

        public static void ToFile(Object obj, string fileName, bool compressed)
        {
            using (Stream file = File.Open(fileName, FileMode.Create))
                ToStream(obj, file, compressed);
        }
        public static void ToStream(Object obj, Stream stream, bool compressed)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (compressed)
                {
                    MemoryStream temp = new MemoryStream();
                    bf.Serialize(temp, obj);
                    CompressStream(temp, stream);
                }
                else
                    bf.Serialize(stream, obj);
            }
            catch (SerializationException)
            {
                //MessageBox.Show(e.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public static byte[] ToByteArray(Object obj, bool compressed)
        {
            byte[] temp;
            using (MemoryStream stream = new MemoryStream())
            {
                ToStream(obj, stream, compressed);
                stream.Capacity = (int)stream.Length;
                temp = stream.GetBuffer();
            }
            return temp;
        }

        private static void CompressStream(Stream src, Stream dst)
        {
            src.Position = 0;
            using (DeflateStream dstream = new DeflateStream(dst, CompressionMode.Compress))
            {
                int read = src.ReadByte();
                while (read != -1)
                {
                    dstream.WriteByte((byte)read);
                    read = src.ReadByte();
                }
            }
        }
        private static void DecompressStream(Stream src, Stream dst)
        {
            using (DeflateStream dstream = new DeflateStream(src, CompressionMode.Decompress, false))
            {
                int read = dstream.ReadByte();
                while (read != -1)
                {
                    dst.WriteByte((byte)read);
                    read = dstream.ReadByte();
                }
            }
            dst.Position = 0;
        }
    }
}
