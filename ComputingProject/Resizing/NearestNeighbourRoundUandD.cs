using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.Resizing
{
    public class NearestNeighbourRoundUandD : IResize
    {
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
            float horizontalScale = (float) newWidthInBytes / (float)orgWidthInBytes;
            float verticalScale = (float) newHeightInPixels / (float)orgHeightInPixels;

            for (int y = 0; y < newHeightInPixels; y++)
            {
                //Calculates how far through the pixel array the current line is
                int currentLine = y * newData.Stride;
                //Loop through each of the pixels on that line
                for (int x = 0; x < newWidthInBytes; x += bytesPerPixel)
                {
                    //This calculates the original pixel that ocresponds to this new pixel when resizing the image, rounds up and down
                    int orgY = (int) Math.Round( y / verticalScale );
                    int orgX = (int) Math.Round((x / bytesPerPixel) / horizontalScale);
                    //Checks to see if the calculated pixels fall out of the image, this can only be by a maximum of 1 so it takes 1 off
                    if (orgY >= orgHeightInPixels) orgY--;
                    if (orgX >= (orgWidthInBytes / bytesPerPixel)) orgX--;
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
