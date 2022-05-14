using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace MView.Utilities
{
    public static class ColoringHelper
    {
        public static void GetAverageColor(Stream stream, out byte red, out byte green, out byte blue)
        {
            Bitmap bm = (Bitmap)Image.FromStream(stream);

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = srcData.Stride;

            IntPtr Scan0 = srcData.Scan0;

            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            int width = bm.Width;
            int height = bm.Height;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int color = 0; color < 3; color++)
                        {
                            int idx = (y * stride) + (x * bppModifier) + color;

                            totals[color] += p[idx];
                        }
                    }
                }
            }

            red = (byte)(totals[0] / (width * height));
            green = (byte)(totals[1] / (width * height));
            blue = (byte)(totals[2] / (width * height));
        }

        public static void RGBToHSV(int red, int green, int blue, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(red, Math.Max(green, blue));
            int min = Math.Min(red, Math.Min(green, blue));

            hue = System.Drawing.Color.FromArgb(red, green, blue).GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }
    }
}
