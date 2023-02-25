using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Extensions
{
    public static class StringExtensions
    {
        public static byte HexToByte(this string hex)
        {
            return Convert.ToByte(hex, 16);
        }

        public static byte[] HexToBytes(this string hex)
        {
            byte[] convert = new byte[hex.Length / 2];

            int length = convert.Length;
            for (int i = 0; i < length; i++)
            {
                convert[i] = Convert.ToByte(hex.Substring(i * 2), 16);
            }

            return convert;
        }

        public static IEnumerable<string> SplitInParts(this string text, int partLength)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            if (partLength <= 0)
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));

            for (var i = 0; i < text.Length; i += partLength)
                yield return text.Substring(i, Math.Min(partLength, text.Length - i));
        }

        public static string[] SplitByString(this string text, string seperator)
        {
            return text.Split(new string[1] { seperator }, StringSplitOptions.None);
        }
    }
}
