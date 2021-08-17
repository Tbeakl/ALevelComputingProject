using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.Resizing
{
    public class NearestNeighbourRoundD : IResize
    {
		/// <summary>
		/// This functions enlarges the image using the nearest neighbour method where when deciding which pixel to look at all of the rounding will be down to the nearest integer
		/// </summary>
		/// <param name="originalPixels">This is explained in the interface</param>
		/// <param name="baseData">This is explained in the interface</param>
		/// <param name="newPixels">This is explained in the interface</param>
		/// <param name="newData">This is explained in the interface</param>
		/// <returns></returns>
		public byte[] Resize(byte[] originalPixels, BitmapData baseData, byte[] newPixels, BitmapData newData)
        {
            //This region gets certain variables out of the baseData to be used through out the resizing process
            #region BitmapDataExtraction
            int bytesPerPixel = Bitmap.GetPixelFormatSize(baseData.PixelFormat) / 8;
            int orgHeightInPixels = baseData.Height;
            int orgWidthInBytes = baseData.Width * bytesPerPixel;
            #endregion

            //This region gets certain variables out of the newData to be used through out the resizing process
            #region BitmapDataExtraction
            int newHeightInPixels = newData.Height;
            int newWidthInBytes = newData.Width * bytesPerPixel;
            #endregion
            //This region is where the image is actually resized according to the given algorithm
            #region ActualResizing
            //Calculate Scaling Factors Both Horizontal and Vertical
            float horizontalScale = (float)newWidthInBytes / (float)orgWidthInBytes;
            float verticalScale = (float)newHeightInPixels / (float)orgHeightInPixels;

            for (int y = 0; y < newHeightInPixels; y++)
            {
                //Calculates how far through the pixel array the current line is
                int currentLine = y * newData.Stride;
                //Loop through each of the pixels on that line
                for (int x = 0; x < newWidthInBytes; x += bytesPerPixel)
                {
                    //This calculates the original pixel that ocresponds to this new pixel when resizing the image, alwasy rounds down
                    int orgY = (int)Math.Floor(y / verticalScale);
                    int orgX = (int)Math.Floor((x / bytesPerPixel) / horizontalScale);
                    //This then sets the new pixel to that same colour
                    newPixels[currentLine + x] = originalPixels[(orgY * baseData.Stride) + (orgX * bytesPerPixel)];
                    newPixels[currentLine + x + 1] = originalPixels[(orgY * baseData.Stride) + (orgX * bytesPerPixel) + 1];
                    newPixels[currentLine + x + 2] = originalPixels[(orgY * baseData.Stride) + (orgX * bytesPerPixel) + 2];
                    //This checks to see if their is an alpha component and if their is then it sets it to what the alpha value of the orginal image was
                    if (bytesPerPixel >= 4) newPixels[currentLine + x + 3] = originalPixels[(orgY * baseData.Stride) + (orgX * bytesPerPixel) + 3];
                }
            }
            #endregion
			//This returns the enlarged images pixel array
            return newPixels;
        }


    }
}