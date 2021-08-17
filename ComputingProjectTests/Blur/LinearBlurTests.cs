﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class LinearBlurTests
    {
        [TestMethod()]
        public void BlurTest()
        {
            Bitmap orgBitmap = new Bitmap(Image.FromFile("LargeLandscape.jpg"));
            BitmapData orgBitmapData = orgBitmap.LockBits(new System.Drawing.Rectangle(0, 0, orgBitmap.Width, orgBitmap.Height), ImageLockMode.ReadWrite, orgBitmap.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(orgBitmap.PixelFormat) / 8;
            int byteCount = orgBitmapData.Stride * orgBitmap.Height;
            
            byte[] pixels = new byte[byteCount];
            
            IntPtr ptrFirstPixel = orgBitmapData.Scan0;
            
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            IBlur testingObject = new LinearBlur();
            pixels = testingObject.Blur(pixels, orgBitmapData, 50, 0, new int[1, 1], new Rectangle(), false, false);
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            orgBitmap.UnlockBits(orgBitmapData);     
            orgBitmap.Save("LargeLandscapeMean50.png");
        }
    }
}