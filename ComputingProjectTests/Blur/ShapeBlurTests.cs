using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputingProject.Blur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ComputingProject.Blur.Tests
{
    [TestClass()]
    public class ShapeBlurTests
    {
        [TestMethod()]
        public void BlurTest()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("LargeLandscape.jpg"));
            Bitmap kernelImage = new Bitmap(Image.FromFile("Kernel.bmp"));
            int[,] kernelArray = new int[kernelImage.Width, kernelImage.Height];
            for (int i = 0; i < kernelImage.Width; i++)
            {
                for (int j = 0; j < kernelImage.Height; j++)
                {
                    kernelArray[i, j] = kernelImage.GetPixel(i, j).R - kernelImage.GetPixel(i, j).G;
                }
            }
            BitmapData orgBitmapData = orgBitmap.LockBits(new System.Drawing.Rectangle(0, 0, orgBitmap.Width, orgBitmap.Height), ImageLockMode.ReadWrite, orgBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(orgBitmap.PixelFormat) / 8;
            int byteCount = orgBitmapData.Stride * orgBitmap.Height;

            byte[] pixels = new byte[byteCount];

            IntPtr ptrFirstPixel = orgBitmapData.Scan0;

            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            IBlur testingObject = new ShapeBlur();
            pixels = testingObject.Blur(pixels, orgBitmapData, 15, 0, kernelArray, new Rectangle(), false, false);
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            orgBitmap.UnlockBits(orgBitmapData);
            orgBitmap.Save("LargeLandscapeShapeBlur.png");
        }
    }
}