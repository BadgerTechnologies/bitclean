using System.Drawing;

/* /Systems/bitclean/imageops.cs
 * Contains algorithms for parsing through the image's pixels, replacing floor
 colored pixels with white pixels (for standardization), populating the pixel
 array, and converting RGB colors to integers.
 */

namespace bitclean
{
    /// <summary>
    /// Image operations.
    /// </summary>
    public static class ImageOperations
    {
        /// <summary>
        /// Changes Magenta floor colored pixels from Cloud Compare into white
        /// </summary>
        /// <param name="bmp">Bmp.</param>
		public static void ParseImage(ref Bitmap bmp)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				for (int x = 0; x < bmp.Width; x++)
				{
                    // System.Console.WriteLine(bmp.GetPixel(x, y).ToString());
					// if the current pixel is the FLOOR (magenta) color, set it to white
					if (bmp.GetPixel(x, y) == Constants.FLOOR)
						bmp.SetPixel(x, y, Constants.WHITE);
				}
			}
		}

        /// <summary>
        /// Populates the pixel array.
        /// </summary>
        /// <param name="bmp">Bmp.</param>
        /// <param name="pixels">Pixels.</param>
        public static void PopulatePixelArray(Bitmap bmp, ref Pixel[] pixels)
        {
            int i = 0;
            if (pixels.Length != bmp.Size.Height * bmp.Size.Width) {
                System.Console.WriteLine("pixel array size not equal to bmp size");
                return;
            }


            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color p = bmp.GetPixel(x, y);
                    pixels[i].id = i;
                    pixels[i].value = ColToInt(p);
                    pixels[i].r = p.R;
                    pixels[i].g = p.G;
                    pixels[i].b = p.B;
                    pixels[i].selected = false;

                    i++;
                }
            }
        }

        /// <summary>
        /// Pushs the pixels in the pixel array to the image.
        /// </summary>
        /// <param name="bmp">Bmp.</param>
        /// <param name="pixels">Pixels.</param>
        public static void PushPixelsToImage(Bitmap bmp, Pixel[] pixels)
		{
			// update bmp with pixels stored in pixel array
			int i = 0;
			for (int y = 0; y < bmp.Height; y++) {
				for (int x = 0; x < bmp.Width; x++) {
					Color toargb = Color.FromArgb(pixels[i].r, pixels[i].g, pixels[i].b);
					bmp.SetPixel(x, y, toargb);
					i++;
				}
			}
		}

        /// <summary>
        /// Converts a Color (rgb) to an integer representation (short). Look at
        /// the documentation for details on this.
        /// </summary>
        /// <returns>The to int.</returns>
        /// <param name="p">P.</param>
		public static short ColToInt(Color p)
		{
            short numcolor = 0;

            if (p == Constants.WHITE)
                numcolor = Constants.INT_WHITE;
            else if (p == Constants.OBJ_TAG)
                numcolor = Constants.INT_OBJ_TAG;
            else if (p.R == 255)
                numcolor = (short)(255 + (255 - p.G) + 255 + 1);
            else if (p.B == 0)
                numcolor = (short)(p.R + 255 + 1);
            else
                numcolor = (short)((255 - p.B) + 1);

            return numcolor;
        }
    }
}