using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputingProject.Resizing
{
	public class BiCubic : IResize
    {
		/// <summary>
		/// This enlarges the image using the bicubic algorithm
		/// </summary>
		/// <param name="originalPixels">Explained in the interface</param>
		/// <param name="baseData">Explained in the interface</param>
		/// <param name="newPixels">Explained in the interface</param>
		/// <param name="newData">Explained in the interface</param>
		/// <returns>This returns the image after it has been bicubicly enlarged</returns>
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

            for (int y = 0; y < newHeightInPixels; y++)
            {
                //Calculates how far through the pixel array the current line is
                int currentLine = y * newData.Stride;
                //Loop through each of the pixels on that line
                for (int x = 0; x < newWidthInBytes; x += bytesPerPixel)
                {
                    //The ideal cooridnate we are looking for
                    float idealX = (x / bytesPerPixel) / horizontalScale;
                    float idealY = y / verticalScale;
                    //This gets the actuall cooridantes near the pixel where we can get a deffinant value
                    float leftOneX = (int)Math.Floor(idealX);
                    float leftTwoX = leftOneX - 1;
                    float rightOneX = leftOneX + 1;
                    float rightTwoX = rightOneX + 1;

                    float aboveOneY = (int)Math.Floor(idealY);
                    float aboveTwoY = aboveOneY - 1;
                    float belowOneY = (int)Math.Ceiling(idealY);
                    float belowTwoY = belowOneY + 1;
                    //Checks to make sure the pixel we are looking for is outside the image
                    if (belowOneY >= baseData.Height) belowOneY--;
                    if (belowTwoY >= baseData.Height) belowTwoY = baseData.Height - 1;

                    if (rightOneX >= baseData.Width) rightOneX--;
                    if (rightTwoX >= baseData.Width) rightTwoX = baseData.Width - 1;
                    if (leftTwoX < 0) leftTwoX = 0;
                    if (aboveTwoY < 0) aboveTwoY = 0;
					//This loops through each colour when being interpolated
                    for (int partThroughPixel = 0; partThroughPixel < bytesPerPixel; partThroughPixel++)
                    {
                        //This gets the value for this colour (R,G,B,A) from the 4 * 4 of pixels which surround the ideal pixel
                        byte p00 = originalPixels[(int)(aboveTwoY * baseData.Stride) + (int)(leftTwoX * bytesPerPixel) + partThroughPixel];
                        byte p01 = originalPixels[(int)(aboveTwoY * baseData.Stride) + (int)(leftOneX * bytesPerPixel) + partThroughPixel];
                        byte p02 = originalPixels[(int)(aboveTwoY * baseData.Stride) + (int)(rightOneX * bytesPerPixel) + partThroughPixel];
                        byte p03 = originalPixels[(int)(aboveTwoY * baseData.Stride) + (int)(rightTwoX * bytesPerPixel) + partThroughPixel];

                        byte p10 = originalPixels[(int)(aboveOneY * baseData.Stride) + (int)(leftTwoX * bytesPerPixel) + partThroughPixel];
                        byte p11 = originalPixels[(int)(aboveOneY * baseData.Stride) + (int)(leftOneX * bytesPerPixel) + partThroughPixel];
                        byte p12 = originalPixels[(int)(aboveOneY * baseData.Stride) + (int)(rightOneX * bytesPerPixel) + partThroughPixel];
                        byte p13 = originalPixels[(int)(aboveOneY * baseData.Stride) + (int)(rightTwoX * bytesPerPixel) + partThroughPixel];

                        byte p20 = originalPixels[(int)(belowOneY * baseData.Stride) + (int)(leftTwoX * bytesPerPixel) + partThroughPixel];
                        byte p21 = originalPixels[(int)(belowOneY * baseData.Stride) + (int)(leftOneX * bytesPerPixel) + partThroughPixel];
                        byte p22 = originalPixels[(int)(belowOneY * baseData.Stride) + (int)(rightOneX * bytesPerPixel) + partThroughPixel];
                        byte p23 = originalPixels[(int)(belowOneY * baseData.Stride) + (int)(rightTwoX * bytesPerPixel) + partThroughPixel];

                        byte p30 = originalPixels[(int)(belowTwoY * baseData.Stride) + (int)(leftTwoX * bytesPerPixel) + partThroughPixel];
                        byte p31 = originalPixels[(int)(belowTwoY * baseData.Stride) + (int)(leftOneX * bytesPerPixel) + partThroughPixel];
                        byte p32 = originalPixels[(int)(belowTwoY * baseData.Stride) + (int)(rightOneX * bytesPerPixel) + partThroughPixel];
                        byte p33 = originalPixels[(int)(belowTwoY * baseData.Stride) + (int)(rightTwoX * bytesPerPixel) + partThroughPixel];

						//This calculates the interploated value along each row of pixels
                        float topLine =  OneDCubicInterpolateValue((float)p00,(float) p01,(float) p02, (float) p03, (idealX - (float)leftOneX));
                        float aboveLine = OneDCubicInterpolateValue(p10, p11, p12, p13, (idealX - (float)leftOneX));
                        float belowLine = OneDCubicInterpolateValue(p20, p21, p22, p23, (idealX - (float)leftOneX));
                        float bottomLine = OneDCubicInterpolateValue(p30, p31, p32, p33, (idealX - (float)leftOneX));
						//This then gets the value if you interpolate between the previously found values from the interpolations along the rows
                        float value = OneDCubicInterpolateValue(topLine, aboveLine, belowLine, bottomLine, (idealY - (float)aboveOneY));

                        //This sets the colour of the current pixel to the previously calculated
                        newPixels[currentLine + x + partThroughPixel] = value > 255 ? (byte) 255 : value < 0 ? (byte) 0 : (byte)value;
                    }
                }
            }

            #endregion

            return newPixels;
        }
		/// <summary>
		/// This performs a cubic interpolation in one dimension
		/// </summary>
		/// <param name="p0">Point at x = -1</param>
		/// <param name="p1">Point at x = 0</param>
		/// <param name="p2">Point at x = 1</param>
		/// <param name="p3">Point at x = 2</param>
		/// <param name="x">This is the x value at which you want the interpolated value to be returned from</param>
		/// <returns>The correct value for the interpolation at the specified x value</returns>
		private static float OneDCubicInterpolateValue(float p0, float p1, float p2, float p3, float x) 
        {
            //Between 0 and 1, p0 is at -1, p1 is at 0, p2 is at 1, p3 is at 2

            //This performs the bicubic interpolation in one dimension fomula from https://www.paulinternet.nl/?page=bicubic

            float a = -0.5f*p0 + 1.5f*p1 - 1.5f*p2 + 0.5f*p3;
            float b = p0 - 2.5f*p1 + 2f*p2 - 0.5f*p3;
            float c = -0.5f*p0+0.5f*p2;
            float d = p1;

            return (a * (float)Math.Pow((double)x, 3) + b *(float) Math.Pow((double)x, 2) + c * x + d);
        }
    }


}