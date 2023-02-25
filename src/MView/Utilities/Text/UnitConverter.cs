using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities.Text
{
    public static class UnitConverter
    {
        /// <summary>
        /// Gets the string for the file size.
        /// </summary>
        /// <param name="length">The size of file(in byte).</param>
        /// <returns>The file size string.</returns>
        public static string GetFileSizeString(long length)
        {
            double byteCount = length;

            string size = "0 Byte";

            if (byteCount >= 1099511627776.0)
                size = string.Format("{0:##.##}", byteCount / 1099511627776.0) + " TB";
            else if (byteCount >= 1073741824.0)
                size = string.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = string.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = string.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Byte";

            return size;
        }
    }
}
