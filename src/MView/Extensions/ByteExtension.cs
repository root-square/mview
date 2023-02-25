using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Extensions
{
    public static class ByteExtension
    {
        public static string ByteArrayToString(this byte[] hexBytes)
        {
            string result = string.Empty;

            foreach (byte c in hexBytes)
                result += c.ToString("x2").ToUpper();

            return result;
        }

        public static byte ReadByte(this byte[] array, int offset)
        {
            byte[] skipped = array.Skip(offset).ToArray();
            return skipped[0];
        }

        public static byte[] ReadByte(this byte[] array, int offset, int length)
        {
            byte[] skipped = array.Skip(offset).ToArray();
            byte[] result = new byte[length];

            Array.Copy(skipped, 0, result, 0, length);

            return result;
        }

        public static byte[] ReadUInt32LE(this byte[] array, int offset)
        {
            byte[] skipped = array.Skip(offset).ToArray();
            byte[] result = new byte[4];

            Array.Copy(skipped, 0, result, 0, 4);
            Array.Reverse(result);

            return result;
        }

        public static byte[] ReadUInt32BE(this byte[] array, int offset)
        {
            byte[] skipped = array.Skip(offset).ToArray();
            byte[] result = new byte[4];

            Array.Copy(skipped, 0, result, 0, 4);
            Array.Sort(result);

            return result;
        }
    }
}
