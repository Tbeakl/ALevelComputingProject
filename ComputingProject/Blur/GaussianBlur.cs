using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputingProject.Blur
{
	public class GaussianBlur : IBlur
    {
        /// <summary>
        /// This blurs the inputted pixels using a Gaussian Blur
        /// </summary>
        /// <params>These are explained in IBlur</params>
        /// <returns>Explained in IBlur</returns>
        public byte[] Blur(byte[] originalPixels, BitmapData baseData, float radius, float power, int[,] kernelImage, Rectangle area, bool isEllipse, bool isInverted)
        {
            //This creates the basic arrays for the data to be stored in when being manipulated, final pixels in the array returned.  startPixels and midPixels are float arrays because they
            //need to store the values once they have been through the gamma correction which could well involve storing values such as 255 put to the power 4 which can quickly exceed to values of
            //bytes, aswell with the gaussian fucntion fractional parts need to be stored which is not possible with ints
            byte[] finalPixels = new byte[originalPixels.Length];
            float[] startPixels = new float[originalPixels.Length];
            for (int i = 0; i < originalPixels.Length; i++)
            {
                //This applies the gamma correction
                startPixels[i] = (float)Math.Pow(originalPixels[i], power);
            }
            float[] midPixels = new float[originalPixels.Length];
            //This region gets certain variables out of the BitmapData to be used through out the bluring process
            #region BitmapDataExtraction
            int bytesPerPixel = Bitmap.GetPixelFormatSize(baseData.PixelFormat) / 8;
            int heightInPixels = baseData.Height;
            int widthInBytes = baseData.Width * bytesPerPixel;
            #endregion
            //This generates a 1D kernel which has a width of 6 standard deviations away from a central point.  This means it goes out 3 standard deviations
            //away from the central point, in theory the kernel should cover the whole image but that would be very computationally expensive and when you go
            //3 standard deviations away you contain 99.7% of the information
            float[] kernel = new float[(int)(6 * radius) + 1];
            float kernelSum = 0f;
            //This fills in the kernel values using the 1D gaussian function and sums up all of these values so that it can be normalised after the kernel has been applied
            for (int i = (int)(-3 * radius); i < (int)(3*radius); i++)
            {
                kernel[i + (int)(3 * radius)] = gaussianDistribution(i, radius);
                kernelSum += kernel[i + (int)(3 * radius)];
            }

            //This performs the horizontal blurring pass on the image using the 1D gaussian function
            for (int x = 0; x < baseData.Width; x++)
            {
                for (int y = 0; y < baseData.Height; y++)
                {
                    //These variables contian the values for the current pixel, the amount missed says how much of the kernel's value was outside of the image
                    float amountMissed = 0f;
                    float redSum = 0f;
                    float blueSum = 0f;
                    float greenSum = 0f;
                    float alphaSum = 0f;
                    //This loops through the surrounding horizontal pixels, up to 3 standard deviations away from the pixel we are intrested in blurring,
                    //adding on the values to the sums where appropriate
                    for (int i = (int)(-3 * radius); i < (int)(3 * radius); i++)
                    {
                        if (x + i < 0 || x + i >= baseData.Width) amountMissed += kernel[i + (int)(3 * radius)];
                        else
                        {
                            blueSum += kernel[i + (int)(3 * radius)] * startPixels[y * baseData.Stride + x * bytesPerPixel + i * bytesPerPixel];
                            greenSum += kernel[i + (int)(3 * radius)] * startPixels[y * baseData.Stride + x * bytesPerPixel + 1 + i * bytesPerPixel];
                            redSum += kernel[i + (int)(3 * radius)] * startPixels[y * baseData.Stride + x * bytesPerPixel + 2 + i * bytesPerPixel];
                            alphaSum += kernel[i + (int)(3 * radius)] * startPixels[y * baseData.Stride + x * bytesPerPixel + 3 + i * bytesPerPixel];
                        }
                    }
                    //This stores the value of red, green and blue to the midPixels array, by taking the sum of how much each colour has been summed and dividing it by how much
                    //it has been multiplied by from the kernel
                    midPixels[y * baseData.Stride + x * bytesPerPixel] = blueSum / (kernelSum - amountMissed);
                    midPixels[y * baseData.Stride + x * bytesPerPixel + 1] = greenSum / (kernelSum - amountMissed);
                    midPixels[y * baseData.Stride + x * bytesPerPixel + 2] = redSum / (kernelSum - amountMissed);
                    midPixels[y * baseData.Stride + x * bytesPerPixel + 3] = alphaSum / (kernelSum - amountMissed);
                }
            }

            //This performs the vertical blurring pass on the image using the 1D Gaussian function and only puts to correct pixels into the final pixel array
            for (int x = 0; x < baseData.Width; x++)
            {
                for (int y = 0; y < baseData.Height; y++)
                {
					if ((x < area.topLeft.X || x > area.bottomRight.X || y < area.topLeft.Y || y > area.bottomRight.Y) && !isInverted)
					{
						//It does not fall into the bounds of the area so do not blur it
						finalPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
					}
					else if (isInverted && !(x < area.topLeft.X || x > area.bottomRight.X || y < area.topLeft.Y || y > area.bottomRight.Y) && !isEllipse)
					{
						//This means that it is inverted so inside the area is not blurred
						finalPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
						finalPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
					}
					else
					{
                        //These variables contian the values for the current pixel, the amount missed says how much of the kernel's value was outside of the image
                        float amountMissed = 0f;
						float redSum = 0f;
						float blueSum = 0f;
						float greenSum = 0f;
						float alphaSum = 0f;
                        //This loops through the surrounding horizontal pixels, up to 3 standard deviations away from the pixel we are intrested in blurring,
                        //adding on the values to the sums where appropriate
                        for (int i = (int)(-3 * radius); i < (int)(3 * radius); i++)
						{
							if (y + i < 0 || y + i >= baseData.Height) amountMissed += kernel[i + (int)(3 * radius)];
							else
							{
								blueSum += kernel[i + (int)(3 * radius)] * midPixels[y * baseData.Stride + x * bytesPerPixel + i * baseData.Stride];
								greenSum += kernel[i + (int)(3 * radius)] * midPixels[y * baseData.Stride + x * bytesPerPixel + 1 + i * baseData.Stride];
								redSum += kernel[i + (int)(3 * radius)] * midPixels[y * baseData.Stride + x * bytesPerPixel + 2 + i * baseData.Stride];
								alphaSum += kernel[i + (int)(3 * radius)] * midPixels[y * baseData.Stride + x * bytesPerPixel + 3 + i * baseData.Stride];
							}
						}
                        #region Checking if in the Ellipse
                        //This calculates the relative coordinates of the current pixel relative to the area being blurred with (0,0) being
                        //in the middle of the area.  This then allows the use of the standard Cartesian form of the Ellipse equation to check
                        //if it falls inside the ellipse.
                        int relativeX = (int)(x - area.topLeft.X)-(int)((area.bottomRight.X - area.topLeft.X)/2 );
                        int relativeY = (int)(y - area.topLeft.Y)-(int)((area.bottomRight.Y - area.topLeft.Y) / 2);
                        bool inEllipse = ((Math.Pow(relativeX, 2) / Math.Pow((area.bottomRight.X - area.topLeft.X) / 2, 2)) + (Math.Pow(relativeY, 2) / Math.Pow((area.bottomRight.Y - area.topLeft.Y) / 2, 2))) <= 1;
                        #endregion
                        //This checks if the area is an ellipse if it falls outside of the ellipse and if it is not inverted, it stores
                        //the orignal pixel to that point
                        if (isEllipse && !inEllipse && !isInverted)
						{
							finalPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
						}
                        //This checks if the area is an ellipse and if the current pixels falls within the ellipse and checks to see if the
                        //area is inverted if it is then it stores the original pixel data at that point
                        else if (isEllipse && inEllipse && isInverted)
						{
							finalPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
							finalPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
						}
						else
						{
                            //This means that the pixel needs to have the value of the blur stored there because it has not been got been 
                            //written too priorly, the calculated value from the sum is put to power of 1/the gamme correction power to invert the gamma correction so that
                            //the values of the pixels fall within the acceptable range
                            finalPixels[y * baseData.Stride + x * bytesPerPixel] = (byte)Math.Pow(blueSum / (kernelSum - amountMissed), 1d / power);
							finalPixels[y * baseData.Stride + x * bytesPerPixel + 1] = (byte)Math.Pow(greenSum / (kernelSum - amountMissed), 1d / power);
							finalPixels[y * baseData.Stride + x * bytesPerPixel + 2] = (byte)Math.Pow(redSum / (kernelSum - amountMissed), 1d / power);
							finalPixels[y * baseData.Stride + x * bytesPerPixel + 3] = (byte)Math.Pow(alphaSum / (kernelSum - amountMissed), 1d / power);
						}
					}
                }
            }
            //This returns the blurred image back to the program which called it
            return finalPixels;
        }

        /// <summary>
        /// This returns the value of the 1D gaussian function at the specifed x coordiate with the specified standard deviation
        /// </summary>
        /// <param name="x">This is the x cooridnate at which we want to find the value</param>
        /// <param name="standardDeviation">This is the standard deviation of the gaussian function to be used when returning the value</param>
        /// <returns>The value of the specified gaussian distribution at the specified x coordinate</returns>
        private float gaussianDistribution(int x, float standardDeviation)
        {
            return ((float)Math.Pow(standardDeviation * Math.Sqrt(2 * Math.PI),-1)) * (float)Math.Exp(-(Math.Pow(x, 2) / (2 * Math.Pow(standardDeviation, 2))));
        }
    }
}
