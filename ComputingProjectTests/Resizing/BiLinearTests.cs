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
    public class BiLinearTests
    {
        [TestMethod()]
        public void ResizeTest()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("SmallLandscape.jpg"));
            Bitmap progResizedBitmap = new Bitmap(440, 302);
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
            BiLinear testingObject = new BiLinear();
            newpixels = testingObject.Resize(pixels, orgBitmapData, newpixels, progResizedBitmapData);
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            orgBitmap.UnlockBits(orgBitmapData);
            Marshal.Copy(newpixels, 0, ptrNewFirstPixel, newpixels.Length);
            // Console.Write("Hello");
            progResizedBitmap.UnlockBits(progResizedBitmapData);
            progResizedBitmap.Save("ErrorLinRedBitmap.png");
        }
    }
}