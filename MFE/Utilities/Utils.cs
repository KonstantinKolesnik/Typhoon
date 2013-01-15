using System;
//using System.Net;
//using System.Net.Sockets;
using System.Text;
using Microsoft.SPOT;
//using Microsoft.SPOT.Presentation;
//using Microsoft.SPOT.Input;

namespace MFE.Utilities
{
    public static class Utils
    {
        static readonly char[] hchars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        //public static int TextWidth(string text, Font font)
        //{
        //    int width = 0;

        //    char[] chars = text.ToCharArray();
        //    for (int i = 0; i < chars.Length; i++)
        //        width += font.CharWidth(chars[i]);

        //    return width;
        //}
        public static bool IsStringNullOrEmpty(string str)
        {
            return str == null || str == "" || str == String.Empty || str.Length == 0;
        }
        public static string StringReplace(string Source, string ToFind, string ReplaceWith)
        {
            int i;
            int iStart = 0;

            if (ToFind == string.Empty)
                return Source;

            while (true)
            {
                i = Source.IndexOf(ToFind, iStart);
                if (i < 0) break;

                if (i > 0)
                {
                    Source = Source.Substring(0, i) + ReplaceWith + Source.Substring(i + ToFind.Length);
                }
                else
                {
                    Source = ReplaceWith + Source.Substring(i + ToFind.Length);
                }

                iStart = i + ReplaceWith.Length;
            }
            return Source;
        }
        public static bool IsWithinRectangle(int x, int y, int rectWidth, int rectHeight)
        {
            return x >= 0 && x < rectWidth && y >= 0 && y < rectHeight;
        }
        //public static bool ContainsPoint(this UIElement el, TouchEventArgs e)
        //{
        //    return el.ContainsPoint(e.Touches[0].X, e.Touches[0].Y);
        //}


        public static string Quotate(string s)
        {
            return "'" + s + "'";
        }


        //public static DateTime GetNetworkTime()//double timeZone)
        //{
        //    byte[] ip = { 129, 6, 15, 28 }; //time-a.nist.gov            
        //    IPEndPoint ep = new IPEndPoint(new IPAddress(ip), 123);
        //    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //    s.Connect(ep);

        //    byte[] ntpData = new byte[48]; // RFC 2030
        //    ntpData[0] = 0x1B;
        //    for (int i = 1; i < 48; i++)
        //        ntpData[i] = 0;

        //    s.Send(ntpData);
        //    s.Receive(ntpData);

        //    byte offsetTransmitTime = 40;
        //    ulong intpart = 0;
        //    ulong fractpart = 0;
        //    for (int i = 0; i <= 3; i++)
        //        intpart = 256 * intpart + ntpData[offsetTransmitTime + i];

        //    for (int i = 4; i <= 7; i++)
        //        fractpart = 256 * fractpart + ntpData[offsetTransmitTime + i];

        //    ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);

        //    s.Close();

        //    TimeSpan timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
        //    DateTime dateTime = new DateTime(1900, 1, 1);
        //    dateTime += timeSpan;

        //    return dateTime;//.AddHours(timeZone);
        //}

        public static string ToBase64String(byte[] value)
        {
            return ConvertBase64My.ToBase64String(value);
        }
        public static string ToBase64String(string value)
        {
            return ConvertBase64My.ToBase64String(Encoding.UTF8.GetBytes(value));
        }
        public static string ToBase64String(Guid value)
        {
            return ConvertBase64My.ToBase64String(value.ToByteArray());
        }

        public static byte[] FromBase64StringToBytes(string s)
        {
            return ConvertBase64My.FromBase64String(s);
        }
        public static string FromBase64StringToString(string s)
        {
            return new string(Encoding.UTF8.GetChars(ConvertBase64My.FromBase64String(s)));
        }
        public static Guid FromBase64StringToGuid(string s)
        {
            return new Guid(ConvertBase64My.FromBase64String(s));
        }

        //public static Bitmap ImageFromBytes(byte[] data)
        //{
        //    if (data[0] == 0xFF && data[1] == 0xD8)// JPEG
        //        return new Bitmap(data, Bitmap.BitmapImageType.Jpeg);
        //    if (data[0] == 0x42 && data[1] == 0x4D)// BMP
        //        return new Bitmap(data, Bitmap.BitmapImageType.Bmp);
        //    if (data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x38)// GIF
        //        return new Bitmap(data, Bitmap.BitmapImageType.Gif);

        //    return null;
        //}

        public static string byteToHex(byte input)
        {
            return new string(new char[] { hchars[input >> 4], hchars[input & 0x0F] });
        }
        public static string bytesToHex(byte[] input)
        {
            string r = "";
            foreach (byte b in input)
            {
                r += byteToHex(b);
            }
            return r;
        }

        public static string FreeRAM(bool formatOutput)
        {
            uint iFree = Debug.GC(false);

            if (!formatOutput)
                return iFree.ToString();
            else
                return FormatSize(iFree);
        }

        public static string FormatSize(uint size)
        {
            byte index = 0;
            while (size > 1024)
            {
                size = size / 1024;
                index++;
                if (index == 4)
                    break;
            }

            string[] sSize = new string[] { " bytes", " KB", " MB", "GB", " TB" };
            string sFree = size.ToString();
            if (sFree.IndexOf('.') > 0)
                sFree = sFree.Substring(0, sFree.IndexOf('.') + 3);
            return sFree + sSize[index];
        }
        public static string FormatSize(long size)
        {
            byte index = 0;
            while (size > 1024)
            {
                size = size / 1024;
                index++;
                if (index == 4)
                    break;
            }

            string[] sSize = new string[] { " bytes", " KB", " MB", "GB", " TB" };
            string sFree = size.ToString();
            if (sFree.IndexOf('.') > 0)
                sFree = sFree.Substring(0, sFree.IndexOf('.') + 3);
            return sFree + sSize[index];
        }

    }

    public static class ConvertBase64My
    {

        /// <summary>
        /// Conversion array from 6 bit of value into base64 encoded character.
        /// </summary>
        
        /* old array */
        //static char[] s_rgchBase64Encoding = new char[]
        //{
        //   'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', /* 12 */
        //   'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', /* 24 */
        //   'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', /* 36 */
        //   'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', /* 48 */
        //   'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', /* 60 */
        //   '8', '9', '!', '*'            /* 64 */
        //};

        /* new array */
        static char[] s_rgchBase64Encoding = new char[]
        {
           'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', /* 12 */
           'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', /* 24 */
           'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', /* 36 */
           'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', /* 48 */
           'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', /* 60 */
           '8', '9', '+', '/'            /* 64 */
        };


        static byte[] s_rgbBase64Decode = new byte[]
        {
            // Note we also accept ! and + interchangably.
            // Note we also accept * and / interchangably.
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*   0 -   7 */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*   8 -  15 */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*  16 -  23 */
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*  24 -  31 */
            0x00, 0x3E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*  32 -  39 */
            0x00, 0x00, 0x3f, 0x3e, 0x00, 0x00, 0x00, 0x3f, /*  40 -  47 */
            0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, /*  48 -  55 */
            0x3c, 0x3d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, /*  56 -  63 */
            0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, /*  64 -  71 */
            0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, /*  72 -  79 */
            0x0f, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, /*  80 -  87 */
            0x17, 0x18, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, /*  88 -  95 */
            0x00, 0x1a, 0x1b, 0x1c, 0x1d, 0x1e, 0x1f, 0x20, /*  96 - 103 */
            0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, /* 104 - 111 */
            0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f, 0x30, /* 112 - 119 */
            0x31, 0x32, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00  /* 120 - 127 */
        };

        private const int CCH_B64_IN_QUARTET = 4;
        private const int CB_B64_OUT_TRIO = 3;

        static private int GetBase64EncodedLength(int binaryLen)
        {
            return (((binaryLen / 3) + (((binaryLen % 3) != 0) ? 1 : 0)) * 4);

        }

        /// <summary>
        /// Converts an array of 8-bit unsigned integers to its equivalent String representation encoded with base 64 digits.
        /// </summary>
        /// <param name="inArray">An array of 8-bit unsigned integers. </param>
        /// <returns>The String representation, in base 64, of the contents of inArray.</returns>
        public static string ToBase64String(byte[] inArray)
        {
            if (inArray == null)
            {
                throw new ArgumentNullException();
            }

            // Create array of characters with appropriate length.
            int inArrayLen = inArray.Length;
            int outArrayLen = GetBase64EncodedLength(inArrayLen);
            char[] outArray = new char[outArrayLen];

            /* encoding starts from end of string */

            /*
            ** Convert the input buffer bytes through the encoding table and
            ** out into the output buffer.
            */
            int iInputEnd = (outArrayLen / CCH_B64_IN_QUARTET - 1) * CB_B64_OUT_TRIO;
            int iInput = 0, iOutput = 0;
            byte uc0 = 0, uc1 = 0, uc2 = 0;
            // Loop is for all trios except of last one.
            for (; iInput < iInputEnd; iInput += CB_B64_OUT_TRIO, iOutput += CCH_B64_IN_QUARTET)
            {
                uc0 = inArray[iInput];
                uc1 = inArray[iInput + 1];
                uc2 = inArray[iInput + 2];
                // Writes data to output character array.
                outArray[iOutput] = s_rgchBase64Encoding[uc0 >> 2];
                outArray[iOutput + 1] = s_rgchBase64Encoding[((uc0 << 4) & 0x30) | ((uc1 >> 4) & 0xf)];
                outArray[iOutput + 2] = s_rgchBase64Encoding[((uc1 << 2) & 0x3c) | ((uc2 >> 6) & 0x3)];
                outArray[iOutput + 3] = s_rgchBase64Encoding[uc2 & 0x3f];
            }

            // Now we process the last trio of bytes. This trio might be incomplete and thus require special handling.
            // This code could be incorporated into main "for" loop, but hte code would be slower becuase of extra 2 "if"
            uc0 = inArray[iInput];
            uc1 = ((iInput + 1) < inArrayLen) ? inArray[iInput + 1] : (byte)0;
            uc2 = ((iInput + 2) < inArrayLen) ? inArray[iInput + 2] : (byte)0;
            // Writes data to output character array.
            outArray[iOutput] = s_rgchBase64Encoding[uc0 >> 2];
            outArray[iOutput + 1] = s_rgchBase64Encoding[((uc0 << 4) & 0x30) | ((uc1 >> 4) & 0xf)];
            outArray[iOutput + 2] = s_rgchBase64Encoding[((uc1 << 2) & 0x3c) | ((uc2 >> 6) & 0x3)];
            outArray[iOutput + 3] = s_rgchBase64Encoding[uc2 & 0x3f];

            switch (inArrayLen % CB_B64_OUT_TRIO)
            {
                /*
                ** One byte out of three, add padding and fall through
                */
                case 1:
                    outArray[outArrayLen - 2] = '=';
                    goto case 2;
                /*
                ** Two bytes out of three, add padding.
                */
                case 2:
                    outArray[outArrayLen - 1] = '=';
                    break;
            }

            // Creates string out of character array and return it.
            return new string(outArray);
        }

        /// <summary>
        /// Converts the specified String, which encodes binary data as base 64 digits, to an equivalent 8-bit unsigned integer array.
        /// </summary>
        /// <param name="inString">Base64 encoded string to convert</param>
        /// <returns>An array of 8-bit unsigned integers equivalent to s.</returns>
        /// <remarks>s is composed of base 64 digits, white space characters, and trailing padding characters.
        /// The base 64 digits in ascending order from zero are the uppercase characters 'A' to 'Z',
        /// lowercase characters 'a' to 'z', numerals '0' to '9', and the symbols '+' and '/'.
        /// An arbitrary number of white space characters can appear in s because all white space characters are ignored.
        /// The valueless character, '=', is used for trailing padding. The end of s can consist of zero, one, or two padding characters.
        /// </remarks>
        public static byte[] FromBase64String(string inString)
        {

            if (inString == null)
            {
                throw new ArgumentNullException();
            }

            // Checks that length of string is multiple of 4
            int inLength = inString.Length;
            if (inLength % CCH_B64_IN_QUARTET != 0)
            {
                throw new ArgumentException("Encoded string length should be multiple of 4");
            }

            // Maximum buffer size needed.
            int outCurPos = (((inLength + (CCH_B64_IN_QUARTET - 1)) / CCH_B64_IN_QUARTET) * CB_B64_OUT_TRIO);
            if (inString[inLength - 1] == '=')
            {   // If the last was "=" - it means last byte was padded/
                --outCurPos;
                // If one more '=' - two bytes were actually padded.
                if (inString[inLength - 2] == '=')
                {
                    --outCurPos;
                }
            }

            // Output array.
            byte[] retArray = new byte[outCurPos];
            // Array of 4 bytes - temporary.
            byte[] rgbOutput = new byte[CCH_B64_IN_QUARTET];
            // Loops over each 4 bytes quartet.
            for (int inCurPos = inLength;
                 inCurPos > 0;
                 inCurPos -= CCH_B64_IN_QUARTET)
            {
                int ibDest = 0;
                for (; ibDest < CB_B64_OUT_TRIO + 1; ibDest++)
                {
                    int ichGet = inCurPos + ibDest - CCH_B64_IN_QUARTET;
                    // Equal sign can be only at the end and maximum of 2
                    if (inString[ichGet] == '=')
                    {
                        if (ibDest < 2 || inCurPos != inLength)
                        {
                            throw new ArgumentException("Invalid base64 encoded string");
                        }
                        break;
                    }

                    // Applies decoding table to the character.
                    rgbOutput[ibDest] = s_rgbBase64Decode[inString[ichGet]];
                }

                // Convert 4 bytes in rgbOutput, each having 6 bits into three bytes in final data.
                switch (ibDest)
                {
                    default:
                        retArray[--outCurPos] = (byte)(((rgbOutput[2] & 0x03) << 6) | rgbOutput[3]);
                        goto case 3;

                    case 3:
                        retArray[--outCurPos] = (byte)(((rgbOutput[1] & 0x0F) << 4) | (((rgbOutput[2]) & 0x3C) >> 2));
                        goto case 2;

                    case 2:
                        retArray[--outCurPos] = (byte)(((rgbOutput[0]) << 2) | (((rgbOutput[1]) & 0x30) >> 4));
                        break;
                }
            }

            return retArray;
        }
    }
}
