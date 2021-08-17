using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.Blur
{
    public class ShapeBlur : IBlur
    {
		/// <summary>
		/// This blurs the inputted image using the kernel specified in the kernelImage array
		/// </summary>
		/// <returns>Explained in IBlur</returns>
        public byte[] Blur(byte[] originalPixels, BitmapData baseData, float radius, float power, int[,] kernelImage, Rectangle area, bool isEllipse, bool isInverted)
        {
			//This creates a blank byte array to store the manipulated pixel data in
			byte[] newPixels = new byte[originalPixels.Length];
			//This creates a blank starting array which contians the information of the original pixels array but after the gamma correction has been performed
			int[] startPixels = new int[originalPixels.Length];
            for (int i = 0; i < originalPixels.Length; i++)
            {
                startPixels[i] = (int)Math.Pow(originalPixels[i], power);
            }
			//This region gets certain variables out of the BitmapData to be used through out the bluring process
			#region BitmapDataExtraction
			int bytesPerPixel = Bitmap.GetPixelFormatSize(baseData.PixelFormat) / 8;
            int heightInPixels = baseData.Height;
            int widthInBytes = baseData.Width * bytesPerPixel;
            #endregion

            //These tell me how far each way from a source pixel I need to check in the blurring process
            int halfKernelWidth = kernelImage.GetLength(0) / 2;
            int halfKernelHeight = kernelImage.GetLength(1) / 2;
            //This loops through the entire image applying the kernel on each pixel
            for (int x = 0; x < baseData.Width; x++)
            {
				for (int y = 0; y < baseData.Height; y++)
				{
					if ((x < area.topLeft.X || x > area.bottomRight.X || y < area.topLeft.Y || y > area.bottomRight.Y) && !isInverted)
					{
						//It does not fall into the bounds of the area so do not blur it
						newPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
						newPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
						newPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
						newPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
					}
					else if (isInverted && !(x < area.topLeft.X || x > area.bottomRight.X || y < area.topLeft.Y || y > area.bottomRight.Y) && !isEllipse)
					{
						//This means that it is inverted so inside the area is not blurred
						newPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
						newPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
						newPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
						newPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
					}
					else
					{
						//These store information about how much red etc should be included in the given pixel
						long redSum = 0;
						long greenSum = 0;
						long blueSum = 0;
						long alphaSum = 0;
						long kernelSum = 0;
						//This loops through all of the kernel checking if that position falls inside of the image if it does then adding it to the kernel sum and adding it times the colour of the image to the respective sums
						for (int i = 0; i < kernelImage.GetLength(0); i++)
						{
							for (int j = 0; j < kernelImage.GetLength(1); j++)
							{
								if (x + i - halfKernelWidth < 0 || x + i - halfKernelWidth >= baseData.Width || y + j - halfKernelHeight < 0 || y + j - halfKernelHeight >= baseData.Height)
								{
									//This pixel underconsideration does not fall within the image

								}
								else
								{
									//The pixel actually falls on the image so I need to add the sums up
									kernelSum += kernelImage[i, j];
									redSum += kernelImage[i, j] * startPixels[x * bytesPerPixel + y * baseData.Stride + (i - halfKernelWidth) * bytesPerPixel + (j - halfKernelHeight) * baseData.Stride];
									greenSum += kernelImage[i, j] * startPixels[x * bytesPerPixel + y * baseData.Stride + (i - halfKernelWidth) * bytesPerPixel + (j - halfKernelHeight) * baseData.Stride + 1];
									blueSum += kernelImage[i, j] * startPixels[x * bytesPerPixel + y * baseData.Stride + (i - halfKernelWidth) * bytesPerPixel + (j - halfKernelHeight) * baseData.Stride + 2];
									alphaSum += kernelImage[i, j] * startPixels[x * bytesPerPixel + y * baseData.Stride + (i - halfKernelWidth) * bytesPerPixel + (j - halfKernelHeight) * baseData.Stride + 3];
								}
							}
						}
						//This calculates the colour value that needs to be stored at each pixel and the gamma correction is undid
						if(kernelSum != 0)
						{
							redSum /= kernelSum;
							greenSum /= kernelSum;
							blueSum /= kernelSum;
							alphaSum /= kernelSum;
						}

						redSum = (long)Math.Pow(redSum, 1d / power);
						greenSum = (long)Math.Pow(greenSum, 1d / power);
						blueSum = (long)Math.Pow(blueSum, 1d / power);
						alphaSum = (long)Math.Pow(alphaSum, 1d / power);
						//This checks that all of the values attempting to be stored are resamble and if they are not then it is set to value nearest it
						if (redSum < 0) redSum = 0;
						else if (redSum > 255) redSum = 255;
						if (blueSum < 0) blueSum = 0;
						else if (blueSum > 255) blueSum = 255;
						if (greenSum < 0) greenSum = 0;
						else if (greenSum > 255) greenSum = 255;
						if (alphaSum < 0) alphaSum = 0;
						else if (alphaSum > 255) alphaSum = 255;
						#region Checking if in the Ellipse
						//This calculates the relative coordinates of the current pixel relative to the area being blurred with (0,0) being
						//in the middle of the area.  This then allows the use of the standard Cartesian form of the Ellipse equation to check
						//if it falls inside the ellipse.
						int relativeX = (int)(x - area.topLeft.X) - (int)((area.bottomRight.X - area.topLeft.X) / 2);
                        int relativeY = (int)(y - area.topLeft.Y) - (int)((area.bottomRight.Y - area.topLeft.Y) / 2);
                        bool inEllipse = ((Math.Pow(relativeX, 2) / Math.Pow((area.bottomRight.X - area.topLeft.X) / 2, 2)) + (Math.Pow(relativeY, 2) / Math.Pow((area.bottomRight.Y - area.topLeft.Y) / 2, 2))) <= 1;
						#endregion
						//This checks if the area is an ellipse if it falls outside of the ellipse and if it is not inverted, it stores
						//the orignal pixel to that point
						if (isEllipse && !inEllipse && !isInverted)
						{
							newPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
							newPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
							newPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
							newPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
						}
						//This checks if the area is an ellipse and if the current pixels falls within the ellipse and checks to see if the
						//area is inverted if it is then it stores the original pixel data at that point
						else if (isEllipse && inEllipse && isInverted)
						{
							newPixels[y * widthInBytes + x * bytesPerPixel] = originalPixels[y * widthInBytes + x * bytesPerPixel];
							newPixels[y * widthInBytes + x * bytesPerPixel + 1] = originalPixels[y * widthInBytes + x * bytesPerPixel + 1];
							newPixels[y * widthInBytes + x * bytesPerPixel + 2] = originalPixels[y * widthInBytes + x * bytesPerPixel + 2];
							newPixels[y * widthInBytes + x * bytesPerPixel + 3] = originalPixels[y * widthInBytes + x * bytesPerPixel + 3];
						}
						else
						{
								newPixels[x * bytesPerPixel + y * baseData.Stride] = (byte)redSum;
								newPixels[x * bytesPerPixel + y * baseData.Stride + 1] = (byte)greenSum;
								newPixels[x * bytesPerPixel + y * baseData.Stride + 2] = (byte)blueSum;
								newPixels[x * bytesPerPixel + y * baseData.Stride + 3] = (byte)alphaSum;
						}
					}
				}
            }

            return newPixels;
        }
    }
}
