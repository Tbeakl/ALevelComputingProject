using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ComputingProject.Blur
{
    public class LinearBlur : IBlur
    {
        /// <summary>
        /// This blurs the inputted pixels using a mean blur
        /// </summary>
        /// <params>These are explained in the IBlur interface</params>
        /// <returns>Explained in IBlur</returns>
        public byte[] Blur(byte[] originalPixels, BitmapData baseData, float radius, float power, int[,] kernelImage, Rectangle area, bool isEllipse, bool isInverted)
        {
			//This rounds radius to the nearest integer because it needs to be an integer
			radius = (float)Math.Round(radius);
			//Creates a blank byte array to store the manipulated pixels as it goes along
			byte[] newPixels = new byte[originalPixels.Length];
			//This creates a blank starting array which contians the information of the original pixels array but after the gamma correction has been performed
            int[] startPixels = new int[originalPixels.Length];
            for (int i = 0; i < originalPixels.Length; i++)
            {
                startPixels[i] = (int)Math.Pow(originalPixels[i], power);
            }
            ///For most blurs at this point a kernel would be created but this is not required for a linear
            ///blur due to each pixel being weighted equally in the evaluation but a integer is required for the number
            ///of pixels in the kernel
            int kernelSize = (int)Math.Pow((2 * (int)Math.Round(radius) + 1), 2);
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
            //These two loops loop through all of the image performing the blur on the relevant pixels
            for (int x = 0; x < baseData.Width; x++)
            {
                for (int y = 0; y < baseData.Height; y++)
                {
                    if ((x < area.topLeft.X || x > area.bottomRight.X || y < area.topLeft.Y || y > area.bottomRight.Y) && !isInverted)
                    {
                        //It does not fall into the bounds of the area so do not blur it and the area is not inverted
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
                        //This means it falls within the rough area to be blurred, i.e. if it was to blur a rectangle then it is right but if it is an
                        //ellipse then it might not be blurred
                        //The next part calculates how many pixels the blur will take into account
                        int widthInside = 2 * (int)radius + 1;
                        int heightInside = 2 * (int)radius + 1;
                        //This checks to see if the kernel falls out of the image on both the top and bottom, if it does then the kernel size is changed to the height of the image
                        if (y + radius >= baseData.Height && y - radius <= 0)
                        {
                            heightInside = baseData.Height;
                        }
                        //This checks to see if the kernel falls off the top of the image if it does then its height is changed appropriately
                        else if (y - radius <= 0)
                        {
                            heightInside = y + (int)radius + 1;
                        }
                        //This checks to see if the kernel falls off the bottom of the image if it does tehn its height is changed appropriately
                        else if (y + radius >= baseData.Height)
                        {
                            heightInside = baseData.Height - (y - (int)radius);
                        }
                        //This does the same as above but for the width of the image
                        if (x + radius >= baseData.Width && x - radius <= 0)
                        {
                            widthInside = baseData.Width;
                        }
                        else if (x - radius <= 0)
                        {
                            widthInside = x + (int)radius + 1;
                        }
                        else if (x + radius >= baseData.Width)
                        {
                            widthInside = baseData.Width - (x - (int)radius);
                        }
                        //This is the numjber of the kernel which fall in the image
                        int numberOfPixelsKernel = widthInside * heightInside;
                        //This calculates the sum of the area in the image
                        int redValue = redSumTable.AreaSum(x - (int)radius, y - (int)radius, x + (int)radius, y + (int)radius) / numberOfPixelsKernel;
                        int greenValue = greenSumTable.AreaSum(x - (int)radius, y - (int)radius, x + (int)radius, y + (int)radius) / numberOfPixelsKernel;
                        int blueValue = blueSumTable.AreaSum(x - (int)radius, y - (int)radius, x + (int)radius, y + (int)radius) / numberOfPixelsKernel;
                        int alphaValue = alphaSumTable.AreaSum(x - (int)radius, y - (int)radius, x + (int)radius, y + (int)radius) / numberOfPixelsKernel;
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
                            newPixels[y * widthInBytes + x * bytesPerPixel] = (byte)Math.Pow(blueValue, 1d / power);
                            newPixels[y * widthInBytes + x * bytesPerPixel + 1] = (byte)Math.Pow(greenValue, 1d / power);
                            newPixels[y * widthInBytes + x * bytesPerPixel + 2] = (byte)Math.Pow(redValue, 1d / power);
                            newPixels[y * widthInBytes + x * bytesPerPixel + 3] = (byte)Math.Pow(alphaValue, 1d / power);
                        }
                    }
                }
            }
            //This returns the new pixels back to the function which called it
            return newPixels;

        }
    }

    /// <summary>
    /// This class represents the data structure of a summed area table which is used in some of the blurs
    /// </summary>
    public class SummedAreaTable
    {
        private int[,] table;
        /// <summary>
        /// This is the operation which you call when you need to first generate the summed area table
        /// </summary>
        /// <param name="inputData">This is a basic 1d integer array where each value at each each index just reperesents the value 
        /// at that point, it is not a sum of any area</param>
        /// <param name="width">This is the width of the 2d array which the 1d array represents, used to slice up the input data array</param>
        public void fillTable(int[] inputData, int width)
        {
            //This initizilises the table to the correct width and height 
            table = new int[width, inputData.Length / width];
            //This puts in the top left value because it does not need to sum any other values
            table[0, 0] = inputData[0];
            //Makes the top row of the table
            for (int i = 1; i < width; i++)
            {
                table[i, 0] = inputData[i] + table[i - 1, 0];
            }
            //Makes the left column of the table
            for (int i = width; i <= inputData.Length - width; i += width)
            {
                table[0, i / width] = inputData[i] + table[0, (i / width) - 1];
            }

            //Fills in the rest of the table by looping down each column in turn so that the sum value can be caluculated because if you started
            //in the wrong place then you would not be able to generate the sum easily by using the previously calculated sums
            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < inputData.Length / width; y++)
                {
                    table[x, y] = table[x - 1, y] + table[x, y - 1] - table[x - 1, y - 1] + inputData[y * width + x];
                }
            }
        }
        /// <summary>
        /// (x1,y1) are the coordinates of the point in the top left of the area
        /// (x2,y2) are the coordinates of the point in the bottom right of the area
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns>This returns the sum of area of the original data within the specified area</returns>
        public int AreaSum(int x1, int y1, int x2, int y2)
        {
            //These check if (x2,y2) falls out of the bottom/right of the image, if they do then they are changed to the bottom/right row/column
            if (x2 >= table.GetLength(0)) x2 = table.GetLength(0) - 1;
            if (y2 >= table.GetLength(1)) y2 = table.GetLength(1) - 1;
            //This checks if the bottom right pixel falls out of the top/left of the image, if it does then we know that the sum will be zero
            if (x2 < 0 || y2 < 0) return 0;
            //These check if (x1,y1) falls out of the bottom/right of the image if they do then we know that the sum is zero
            if (x1 >= table.GetLength(0)) return 0;
            if (y1 >= table.GetLength(1)) return 0; //Have not checked 
            //This checks if the (x1,y1) falls out of the topleft corner if it does then we just need to return the value found at (x2,y2) 
            if (x1 <= 0 && y1 <= 0)
            {
                return table[x2, y2];
            }
            //This checks if the (x1,y1) falls out of the left side of the image if it does then you need to do the value (x2,y2) minus the value
            //directly above it at the y coordinate of (x1,y1)
            if (x1 <= 0)
            {
                return table[x2, y2] - table[x2, y1 - 1];
            }
            //This checks if (x1,y1) falls out of the top of the image if it does then you need to do the value (x2,y2) minus the value
            //directly to its left at the x coordinate of (x1,y1)_
            if (y1 <= 0)
            {
                return table[x2, y2] - table[x1 - 1, y2];
            }
            //This is used because both the topleft and bottom right corners fall in the image so you need to apply the standard operation to calculate the area sum
            return table[x2, y2] - table[x2, y1 - 1] - table[x1 - 1, y2] + table[x1 - 1, y1 - 1];
        }
    }

}