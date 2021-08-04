using System;
using System.IO;
using System.Text;

namespace Novatek.Core
{
    class Streams
    {
        public static bool Seek(Stream stream, long offset)
        {
            try
            {
                long length = stream.Length;
                long current = stream.Position;

                if ((current + offset) <= length)
                {
                    stream.Seek(offset, SeekOrigin.Current);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                //MessageBox.Show(e.Message);
                return false;
            }
        }

        public static uint ReadUInt(Stream stream, bool bigEndian = false)
        {
            var rawBytes = new byte[4];
            stream.Read(rawBytes, 0, 4);

            if (!BitConverter.IsLittleEndian || bigEndian)
            {
                Array.Reverse(rawBytes);
            }

            return BitConverter.ToUInt32(rawBytes, 0);
        }

        public static float ReadFloat(Stream stream, bool bigEndian = false)
        {
            var rawBytes = new byte[4];
            stream.Read(rawBytes, 0, 4);

            if (!BitConverter.IsLittleEndian || bigEndian)
            {
                Array.Reverse(rawBytes);
            }

            return BitConverter.ToSingle(rawBytes, 0);
        }

        public static string ReadString(Stream stream, int byteLength, bool bigEndian = false)
        {
            var rawBytes = new byte[byteLength];
            stream.Read(rawBytes, 0, byteLength);

            if (!BitConverter.IsLittleEndian || bigEndian)
            {
                Array.Reverse(rawBytes);
            }

            var encoding = Encoding.UTF8;
            return encoding.GetString(rawBytes);
        }
    }
}
