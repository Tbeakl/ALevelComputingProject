using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace ComputingProject.Blur
{
    /// <summary>
    /// This is just a structure which contains two vector2's representing the top left and bottom right of the area specified.  It is used to specify regions of pixels in images 
    /// </summary>
    public struct Rectangle
    {
        public Vector2 topLeft;
        public Vector2 bottomRight;

        public Rectangle(Vector2 TopLeft, Vector2 BottomRight)
        {
            topLeft = TopLeft;
            bottomRight = BottomRight;
        }
    }

    public class BokehBlur : IBlur
    {
        /// <summary>
        /// This blurs the inputted pixels using a Bokeh Blur
        /// </summary>
        /// <params>These are explained in IBlur</params>
        /// <returns>Explained in IBlur</returns>
        public byte[] Blur(byte[] originalPixels, BitmapData baseData, float radius, float power, int[,] kernelImage, Rectangle area, bool isEllipse, bool isInverted)
        {
			//This rounds radius to the nearest integer because it needs to be an integer
			radius = (float)Math.Round(radius);
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
			//This creates the basic summed area tables and the individual colour arrays, and populates them
			#region SummedAreaTableCreation
			SummedAreaTable redSumTable = new SummedAreaTable();
            SummedAreaTable blueSumTable = new SummedAreaTable();
            SummedAreaTable greenSumTable = new SummedAreaTable();
            SummedAreaTable alphaSumTable = new SummedAreaTable();
            int[] blueValues = new int[baseData.Width * baseData.Height];
            int[] greenValues = new int[baseData.Width * baseData.Height];
            int[] redValues = new int[baseData.Width * baseData.Height];
            int[] alphaValues = new int[baseData.Width * baseData.Height];
            for (int i = 0; i < originalPixels.Length; i += bytesPerPixel)
            {
                blueValues[i / bytesPerPixel] = startPixels[i];
                greenValues[i / bytesPerPixel] = startPixels[i + 1];
                redValues[i / bytesPerPixel] = startPixels[i + 2];
                alphaValues[i / bytesPerPixel] = startPixels[i + 3];
            }
            blueSumTable.fillTable(blueValues, baseData.Width);
            greenSumTable.fillTable(greenValues, baseData.Width);
            redSumTable.fillTable(redValues, baseData.Width);
            alphaSumTable.fillTable(alphaValues, baseData.Width);
			#endregion
			//This region is where the kernel is made and broken up into rectangles for efficient blurring
			#region KernelCreation
			//This creates a 2d array of the size large enough to just contain the circle of the bokeh blur, and then it is filled with a filled circle of 1's where the kernel should take the pixel information from
			int[,] kernel = new int[2 * (int)radius + 1, 2 * (int)radius + 1];
            for (int x = -(int)radius; x <= radius; x++)
            {
                for (int y = -(int)radius; y <= radius; y++)
                {
                    if (Math.Pow(x, 2) + Math.Pow(y, 2) <= Math.Pow(radius + 0.5f, 2))
                    {
                        kernel[x + (int)radius, y + (int)radius] = 1;
                    }
                    else
                    {
                        kernel[x + (int)radius, y + (int)radius] = 0;
                    }
                }
            }
			/*  This can be used in debugging to see the array in which the circle is drawn
            for (int i = 0; i < kernel.GetLength(0); i++)
            {
                for (int y = 0; y < kernel.GetLength(1); y++)
                {
                    Console.Write(kernel[i, y] + "  ");
                }
                Console.WriteLine();
            } */

			//This region is where the cirlce is broken up into constituent rectangles for efficient blurring 
            #region BreakUpKernel
			//This contains a list of all the rectangles of the broken up kernel
            List<Rectangle> kernelBrokenUp = new List<Rectangle>();
			//This is used to see if we need to do more checks when deciding to create rectangles
            bool finishedBreakUp = false;
            //Need to break down the circle into a minimal number of rectangles which I believe to be the number of vertical stripes
			//This loops y coordinates in the top left of the image, we only need to do the top left quarter because of the symmetry of a circle
            for (int y = -(int)radius; y <= 0; y++)
            {
				//If the breaking up of the kernel has finished then there is no need to carry on looking for opportuity to break up the kernel into rectangles
                if (!finishedBreakUp)
                {
					//This stores the coordinates of the top left of the rectangle we are interested in
                    Vector2 TopLeftLeftRect = new Vector2();
                    bool doneY = false;
					//This loops through all the x pixels on that y value
					for (int x = -(int)radius; x <= 0; x++)
                    {
						//This checks to see if on the top row at this point there is a 1, if there is then we know we have entered the central rectangle so it needs to be created and we can calucate the bottom right from symmetry
                        if (y + radius == 0)
                        {
                            if (kernel[x + (int)radius, y + (int)radius] == 1)
                            {
                                if (kernel[x + (int)radius - 1, y + (int)radius] == 0)
                                {
                                    //This is the top left corner of the central rectangle
                                    Vector2 topLeft = new Vector2(x, y);
                                    Vector2 bottomRight = new Vector2(-x, -y);
                                    kernelBrokenUp.Add(new Rectangle(topLeft, bottomRight));
                                }
                            }
                        }
                        else
                        {
							//This breaks up the kernel using the symmtry around the centre point and we know that 
                            if (kernel[x + (int)radius, y + (int)radius] == 1 && !doneY)
                            {
                                if (TopLeftLeftRect == new Vector2())
                                {
                                    //It must be the top left place because none have been found before it
                                    TopLeftLeftRect = new Vector2(x, y);
                                }
								//This will get to true when you have entered the region of the next rectangle because the pixel above it is a 1
                                if (kernel[x + (int)radius, y + (int)radius - 1] == 1)
                                {
                                    int farRight = x - 1;
									//Due to this being on the right of the kernel you can generate the two rectangles because of the reflection vertically and you know the y coordinates of the top and bottom because of the horizontal symmetry
                                    kernelBrokenUp.Add(new Rectangle(TopLeftLeftRect, new Vector2(farRight, -y)));
                                    kernelBrokenUp.Add(new Rectangle(new Vector2(-farRight, y), new Vector2(-TopLeftLeftRect.X, -y)));
                                    if (TopLeftLeftRect.X == -radius) finishedBreakUp = true;
                                    doneY = true;
                                }
                            }
                        }
                    }
                }
            }
			#endregion
			#endregion
			//These two loops loop through all of the image performing the blur on the relevant pixels
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
						//Need to calculate area within the image
						int areaInImage = 0;
						//This loops through all the constituent rectangles of the image
						for (int i = 0; i < kernelBrokenUp.Count; i++)
						{
							//This calculates where that part of the kernel fall on the image
							int inImageTopLeftX = (int)kernelBrokenUp[i].topLeft.X + x;
							int inImageTopLeftY = (int)kernelBrokenUp[i].topLeft.Y + y;
							int inImageBottomRightX = (int)kernelBrokenUp[i].bottomRight.X + x;
							int inImageBottomRightY = (int)kernelBrokenUp[i].bottomRight.Y + y;
							//This checks if the rectangle falls completely out of the image, if it does then no further action is required
							if (inImageBottomRightX < 0 || inImageTopLeftX >= baseData.Width)
							{

							}
							else
							{
								//This makes sure that the top left of the rectangle falls inside of the image
								if (inImageTopLeftX < 0) inImageTopLeftX = 0;
								if (inImageTopLeftY < 0) inImageTopLeftY = 0;

								//This makes sure that the bottom right of the rectangle falls inside of the image
								if (inImageBottomRightX >= baseData.Width) inImageBottomRightX = baseData.Width - 1;
								if (inImageBottomRightY >= baseData.Height) inImageBottomRightY = baseData.Height - 1;
								//This then adds on the area which that rectangle takes up to the the total area covered in the image by the kernel
								areaInImage += ((inImageBottomRightX - inImageTopLeftX) + 1) * ((inImageBottomRightY - inImageTopLeftY) + 1);
							}
						}
						//These store the sums of the colours for that pixels during the blurring process
						int redSum = 0;
						int blueSum = 0;
						int greenSum = 0;
						int alphaSum = 0;
						//This loops through all of the kernel parts and adds the sums of the colour which falls in each rectangle
						for (int i = 0; i < kernelBrokenUp.Count; i++)
						{
							redSum += redSumTable.AreaSum((int)kernelBrokenUp[i].topLeft.X + x, (int)kernelBrokenUp[i].topLeft.Y + y, (int)kernelBrokenUp[i].bottomRight.X + x, (int)kernelBrokenUp[i].bottomRight.Y + y);
							blueSum += blueSumTable.AreaSum((int)kernelBrokenUp[i].topLeft.X + x, (int)kernelBrokenUp[i].topLeft.Y + y, (int)kernelBrokenUp[i].bottomRight.X + x, (int)kernelBrokenUp[i].bottomRight.Y + y);
							greenSum += greenSumTable.AreaSum((int)kernelBrokenUp[i].topLeft.X + x, (int)kernelBrokenUp[i].topLeft.Y + y, (int)kernelBrokenUp[i].bottomRight.X + x, (int)kernelBrokenUp[i].bottomRight.Y + y);
							alphaSum += alphaSumTable.AreaSum((int)kernelBrokenUp[i].topLeft.X + x, (int)kernelBrokenUp[i].topLeft.Y + y, (int)kernelBrokenUp[i].bottomRight.X + x, (int)kernelBrokenUp[i].bottomRight.Y + y);
						}
						//Need to check it falls out of the bounds of the Ellipse
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
							//This means that the pixel needs to have the value of the blur stored there because it has not been got been 
							//written too priorly
							newPixels[y * widthInBytes + x * bytesPerPixel] = (byte)Math.Pow((blueSum / areaInImage), 1d / power);
							newPixels[y * widthInBytes + x * bytesPerPixel + 1] = (byte)Math.Pow((greenSum / areaInImage), 1d / power);
							newPixels[y * widthInBytes + x * bytesPerPixel + 2] = (byte)Math.Pow((redSum / areaInImage), 1d / power);
							newPixels[y * widthInBytes + x * bytesPerPixel + 3] = (byte)Math.Pow((alphaSum / areaInImage), 1d / power);
						}
					}
				}
            }
			//This returns the new pixels back to the function which called it
			return newPixels;
        }   
    }
}