using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Typhoon.Core
{
    public static class Helpers
    {
        public static string ImageToString(Image img)
        {
            if (img == null)
                return "";

            MemoryStream s = new MemoryStream();
            img.Save(s, ImageFormat.Bmp);
            return Encoding.Unicode.GetString(s.GetBuffer());
        }
        public static Image ImageFromString(string s)
        {
            return (String.IsNullOrEmpty(s) ? null : Image.FromStream(new MemoryStream(Encoding.Unicode.GetBytes(s))));
        }
        public static BitmapSource BitmapSourceFromImage(Image img)
        {
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, ImageFormat.Png);
            PngBitmapDecoder decoder = new PngBitmapDecoder(memStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        public static Bitmap BitmapFromBitmapSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            };
            return bitmap;
        }
        public static MemoryStream BitmapSourceToStream(BitmapSource bitmapsource)
        {
            MemoryStream s = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bitmapsource));
            enc.Save(s);
            return s;
        }

        public static string GetEnumValueName<TEnum>(TEnum value)
        {
            return Enum.GetName(typeof(TEnum), value);
        }
        public static string[] GetEnumValueNames<TEnum>()
        {
            return Enum.GetNames(typeof(TEnum));
        }
        public static TEnum GetEnumValue<TEnum>(string valueName)
        {
            Array values = Enum.GetValues(typeof(TEnum));
            foreach (TEnum value in values)
            {
                if (Enum.GetName(typeof(TEnum), value) == valueName)
                    return value;
            }
            throw new Exception("Unknown enum value name");
        }
        //public static TEnum GetEnumValue<TEnum>(int value)
        //{
        //    Array values = Enum.GetValues(typeof(TEnum));
        //    foreach (TEnum val in values)
        //    {
        //        if ((int)val == value)
        //            return val;
        //    }
        //    throw new Exception("Unknown enum value int");
        //}

        public static List<bool> ByteToBits(byte value)
        {
            int val = value;
            List<bool> bits = new List<bool>(8);

            for (uint i = 0; i < 8; i++)
            {
                bits.Add((val & 1) == 1);
                val = val >> 1;
            }

            return bits;
        }
        public static byte BitsToByte(List<bool> bits)
        {
            byte value = 0;
            for (int i = 0; i < 8; i++)
                value += (byte)((bits[i] ? 1 : 0) * Math.Pow(2, i));

            return value;
        }
        public static byte BitsToByte(List<byte> bits)
        {
            byte value = 0;
            for (int i = 0; i < 8; i++)
                value += (byte)(bits[i] * Math.Pow(2, i));

            return value;
        }

        public static void ExtendedAddressToCV17_18(uint address, out byte cv17, out byte cv18)
        {
            /*
            First convert the locomotive address to a 16 digit binary number.
            Then, change the left two digits to 11.
            Finally, split the number into two blocks of 8, converting each back to decimal.
            The left block goes in CV17, the right in CV18.
                    
            Example: Loco 2537 -> binary 0000 1001 1110 1001 -> 
            change left two digits 1100 1001 1110 1001 -> 
            split this into two 8 digit numbers,
            the left is CV17 (1100 1001) = 201, and the right CV18 (1110 1001) = 233.
            */
            cv18 = (byte)(address & 0x00FF);
            cv17 = (byte)((address >> 8) & 0x00FF);
            cv17 |= 0xC0;
        }
        public static uint CV17_18ToExtendedAddress(byte cv17, byte cv18)
        {
            if (cv17 < 192 || cv17 > 231)
                throw new ArgumentOutOfRangeException("CV#17 out of range");
            cv17 &= 0x3F; // remove msb 11 from cv17
            return (uint)(((cv17 << 8) & 0xFF00) + cv18);
        }

        public static string ToBase64String(byte[] value)
        {
            return Convert.ToBase64String(value);
        }
        public static string ToBase64String(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
        public static string ToBase64String(Guid value)
        {
            return Convert.ToBase64String(value.ToByteArray());
        }

        public static byte[] FromBase64StringToBytes(string s)
        {
            return Convert.FromBase64String(s);
        }
        public static string FromBase64StringToString(string s)
        {
            return new string(Encoding.UTF8.GetChars(Convert.FromBase64String(s)));
        }
        public static Guid FromBase64StringToGuid(string s)
        {
            return new Guid(Convert.FromBase64String(s));
        }
    }
}
