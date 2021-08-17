using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputingProject.Resizing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ComputingProject.Resizing.Tests
{
    [TestClass()]
    public class BiCubicTests
    {
        [TestMethod()]
        public void ResizeTest()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("SmallLandscape.jpg"));
            Bitmap progResizedBitmap = new Bitmap(880, 604);
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
            BiCubic testingObject = new BiCubic();
            newpixels = testingObject.Resize(pixels, orgBitmapData, newpixels, progResizedBitmapData);
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            orgBitmap.UnlockBits(orgBitmapData);
            Marshal.Copy(newpixels, 0, ptrNewFirstPixel, newpixels.Length);
            // Console.Write("Hello");
            progResizedBitmap.UnlockBits(progResizedBitmapData);
            progResizedBitmap.Save("SmallLandscapeBiCubic.png");
        }

        [TestMethod()]
        public void ToCSVBlue()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("SmallLandscape.jpg"));
            BitmapData orgBitmapData = orgBitmap.LockBits(new System.Drawing.Rectangle(0, 0, orgBitmap.Width, orgBitmap.Height), ImageLockMode.ReadWrite, orgBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(orgBitmap.PixelFormat) / 8;
            int byteCount = orgBitmapData.Stride * orgBitmap.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = orgBitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = orgBitmapData.Height;
            int widthInBytes = orgBitmapData.Width * bytesPerPixel;
            string FileInfo = "";
            for (int y = 0; y < heightInPixels; y++)
            {
                for (int x = 0; x < widthInBytes; x += bytesPerPixel)
                {
                    FileInfo += (pixels[y * orgBitmapData.Stride + x]).ToString() + ",";
                }
                FileInfo += "\n";
            }

            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            System.IO.File.WriteAllText("smallLandscapeBlue.csv", FileInfo);
            orgBitmap.UnlockBits(orgBitmapData);
        }
    }
}