using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputingProject.Resizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ComputingProject.Resizing.Tests
{
    [TestClass()]
    public class NearestNeighbourRoundDTests
    {
        [TestMethod()]
        public void ResizeTest()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("LittleImageOrg.png"));
            Bitmap progResizedBitmap = new Bitmap(100, 100);
            Bitmap PropBitmap = new Bitmap(Image.FromFile("LittleImage55NearestNeighbourUD.png"));
            BitmapData orgBitmapData = orgBitmap.LockBits(new System.Drawing.Rectangle(0, 0, orgBitmap.Width, orgBitmap.Height), ImageLockMode.ReadWrite, orgBitmap.PixelFormat);
            BitmapData progResizedBitmapData = progResizedBitmap.LockBits(new System.Drawing.Rectangle(0, 0, progResizedBitmap.Width, progResizedBitmap.Height), ImageLockMode.ReadWrite, progResizedBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(orgBitmap.PixelFormat) / 8;
            int byteCount = orgBitmapData.Stride * orgBitmap.Height;
            int byteNewCount = progResizedBitmapData.Stride * progResizedBitmapData.Height;
            byte[] pixels = new byte[byteCount];
            byte[] newpixels = new byte[byteNewCount];
            IntPtr ptrFirstPixel = orgBitmapData.Scan0;
            IntPtr ptrNewFirstPixel = progResizedBitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            Marshal.Copy(ptrNewFirstPixel, newpixels, 0, newpixels.Length);
            NearestNeighbourRoundUandD testingObject = new NearestNeighbourRoundUandD();
            newpixels = testingObject.Resize(pixels, orgBitmapData, newpixels, progResizedBitmapData);
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            orgBitmap.UnlockBits(orgBitmapData);
            Marshal.Copy(newpixels, 0, ptrNewFirstPixel, newpixels.Length);
            // Console.Write("Hello");
            progResizedBitmap.UnlockBits(progResizedBitmapData);

            /*    for (int y = 0; y < progResizedBitmap.Height; y++)
                {
                    for (int x = 0; x < progResizedBitmap.Width; x++)
                    {
                        Console.WriteLine("Pixel (" + x + ", " + y + ") has the colours R: " + progResizedBitmap.GetPixel(x, y).R + "G: " + progResizedBitmap.GetPixel(x, y).G + "B: " + progResizedBitmap.GetPixel(x, y).B);
                    }
                } */
            // throw new Exception("The green value of the pixel at 1,1 is " + progResizedBitmap.GetPixel(1,1).G);
            progResizedBitmap.Save("ErrorDPBitmap.png");
            /*for (int i = 0; i < PropBitmap.Height; i++)
            {
                for (int j = 0; j < PropBitmap.Width; j++)
                {
                    if (PropBitmap.GetPixel(i, j) != progResizedBitmap.GetPixel(i, j))
                    {
                        progResizedBitmap.Save("ErrorBitmap.png");
                        Assert.Fail();
                    }
                }
            } */

        }
    }
}