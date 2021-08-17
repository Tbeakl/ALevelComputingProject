using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputingProject.Resizing
{
	public class BiLinear : IResize
    {
		/// <summary>
		/// This enlarges the image using the bilinear algorithm
		/// </summary>
		/// <param name="originalPixels">Explained in the interface</param>
		/// <param name="baseData">Explained in the interface</param>
		/// <param name="newPixels">Explained in the interface</param>
		/// <param name="newData">Explained in the interface</param>
		/// <returns>This returns the image after it has been bilinearly enlarged</returns>
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

            #region Acutal Resizing
            //Calculate Scaling Factors Both Horizontal and Vertical
            float horizontalScale = (float)newData.Width / (float)baseData.Width;
            float verticalScale = (float)newHeightInPixels / (float)orgHeightInPixels;
			//This loops through each line of the image
            for (int y = 0; y < newHeightInPixels; y++)
            {
                //Calculates how far through the pixel array the current line is
                int currentLine = y * newData.Stride;
                //Loop through each of the pixels on that line
                for (int x = 0; x < newWidthInBytes; x += bytesPerPixel)
                {
                    //The ideal cooridnate we are looking for
                    float idealX = (x/bytesPerPixel) / horizontalScale;
                    float idealY = y / verticalScale;
                    //This gets the actuall cooridantes near the pixel where we can get a deffinant value
                    float leftX = (int)Math.Floor(idealX);
                    float rightX = (int)Math.Ceiling(idealX);

                    float aboveY = (int)Math.Floor(idealY);
                    float belowY = (int)Math.Ceiling(idealY);
                    //Checks to make sure the pixel we are looking for is outside the image
                    if (belowY >= baseData.Height) belowY--;
                    if (rightX >= baseData.Width) rightX--;
                    for (int partThroughPixel = 0; partThroughPixel < bytesPerPixel; partThroughPixel++)
                    {
                        //This gets the value for this colour (R,G,B,A) from the 4 pixels which surround the ideal pixel
                        byte topLeft = originalPixels[ (int)(aboveY * baseData.Stride) + (int)(leftX * bytesPerPixel) + partThroughPixel];
                        byte topRight = originalPixels[(int)(aboveY * baseData.Stride) + (int)(rightX * bytesPerPixel) + partThroughPixel];
                        byte bottomLeft = originalPixels[(int)(belowY * baseData.Stride) + (int)(leftX * bytesPerPixel) + partThroughPixel];
                        byte bottomRight = originalPixels[(int)(belowY * baseData.Stride) + (int)(rightX * bytesPerPixel) + partThroughPixel];
                        //This calculates the linear interpolation between the two top pixels and the bottom two pixels
                        float valueAbove = leftX == rightX ? topLeft : (((1 - Math.Abs(idealX - leftX)) * topLeft) + (((1 - Math.Abs(rightX - idealX)) * topRight)));
                        float valueBelow = leftX == rightX ? bottomLeft : (((1 - Math.Abs(idealX - leftX)) * bottomLeft) + (((1 - Math.Abs(rightX - idealX)) * bottomRight)));
                        //This linearly interpolates between the two linear value interpolated from below and above
                        float value = aboveY == belowY ? valueAbove : (((1 - Math.Abs(idealY - aboveY)) * valueAbove) + ((Math.Abs(idealY - aboveY)) * valueBelow));
                        //This sets the colour of the current pixel to the previously calculated 
                        newPixels[currentLine + x + partThroughPixel] = (byte)value;
                    }
                   
                }
            }

			#endregion
			return newPixels;
        }
    }
}