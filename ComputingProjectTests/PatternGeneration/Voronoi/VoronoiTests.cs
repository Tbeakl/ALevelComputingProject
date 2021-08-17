using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;
using ComputingProject.PatternGeneration.Point_Generators;

namespace ComputingProject.PatternGeneration.Voronoi.Tests
{
	[TestClass()]
    public class VoronoiTests
    {
        [TestMethod()]
        public void MakeRegionsTest()
        {
            Vector2 Size = new Vector2(500,500);
            float radius = 15f;
			float borderWidth = 2f;
            float p = 1f;
            Bitmap newfile = new Bitmap((int)Size.X,(int)Size.Y);
            BitmapData oldbitmapData = newfile.LockBits(new Rectangle(0, 0, newfile.Width, newfile.Height), ImageLockMode.ReadWrite, newfile.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(newfile.PixelFormat) / 8;
            int byteCount = oldbitmapData.Stride * newfile.Height;
            byte[] pixels = new byte[byteCount];
            IntPtr ptrFirstPixel = oldbitmapData.Scan0;
            Marshal.Copy(ptrFirstPixel, pixels, 0, pixels.Length);
            int heightInPixels = oldbitmapData.Height;
            int widthInBytes = oldbitmapData.Width * bytesPerPixel;
            IPointGenerator pointGenerator = new PoissonDiscGenerator();
            List<Vector2> points = pointGenerator.GeneratePoints(Size, new Random(),radius, 30f);
            Voronoi voronoiGenerator = new Voronoi(points, Size, Voronoi.calculateRadius(points,true), bytesPerPixel);
            (List<Region>,Region) regions = voronoiGenerator.MakeRegions(borderWidth, p, true);
            Random getRandom = new Random(1);
            for (int i = 0; i < regions.Item1.Count; i++)
            {     
                Color ranCol = new Color();
                ranCol = Color.FromArgb(getRandom.Next(255), getRandom.Next(255), getRandom.Next(255));
                for (int j = 0; j < regions.Item1[i].Size; j++)
                {
                    pixels[regions.Item1[i].pixelIndexes[j]] = ranCol.R;
                    pixels[regions.Item1[i].pixelIndexes[j] + 1] = ranCol.G;
                    pixels[regions.Item1[i].pixelIndexes[j] + 2] = ranCol.B;
                    pixels[regions.Item1[i].pixelIndexes[j] + 3] = 255;
                }
            }
			for (int i = 0; i < regions.Item2.Size; i++)
			{
				pixels[regions.Item2.pixelIndexes[i]] = 0;
				pixels[regions.Item2.pixelIndexes[i] + 1] = 0;
				pixels[regions.Item2.pixelIndexes[i] + 2] = 0;
				pixels[regions.Item2.pixelIndexes[i] + 3] = 255;
			}
            Marshal.Copy(pixels, 0, ptrFirstPixel, pixels.Length);
            newfile.UnlockBits(oldbitmapData);
            for (int i = 0; i < points.Count; i++)
            {
                newfile.SetPixel((int)points[i].X, (int)points[i].Y, Color.Red);
            }
            newfile.Save("VoronoiBeforeRemovingTest.png", ImageFormat.Png);
			regions = voronoiGenerator.RemoveRegion(getRandom.Next(regions.Item1.Count), p, borderWidth);
			getRandom = null;
			getRandom = new Random(1);
			Bitmap newNewFile = new Bitmap((int)Size.X, (int)Size.Y); ;
			BitmapData newbitmapData = newNewFile.LockBits(new System.Drawing.Rectangle(0, 0, newNewFile.Width, newNewFile.Height), ImageLockMode.ReadWrite, newNewFile.PixelFormat);
			byte[] newpixels = new byte[byteCount];
			IntPtr ptrNewFirstPixel = newbitmapData.Scan0;
			Marshal.Copy(ptrNewFirstPixel, newpixels, 0, newpixels.Length);
			for (int i = 0; i < regions.Item1.Count; i++)
			{
				Color ranCol = new Color();
				ranCol = Color.FromArgb(getRandom.Next(255), getRandom.Next(255), getRandom.Next(255));
				for (int j = 0; j < regions.Item1[i].Size; j++)
				{
					newpixels[regions.Item1[i].pixelIndexes[j]] = ranCol.R;
					newpixels[regions.Item1[i].pixelIndexes[j] + 1] = ranCol.G;
					newpixels[regions.Item1[i].pixelIndexes[j] + 2] = ranCol.B;
					newpixels[regions.Item1[i].pixelIndexes[j] + 3] = 255;
				}
			}
			for (int i = 0; i < regions.Item2.Size; i++)
			{
				newpixels[regions.Item2.pixelIndexes[i]] = 0;
				newpixels[regions.Item2.pixelIndexes[i] + 1] = 0;
				newpixels[regions.Item2.pixelIndexes[i] + 2] = 0;
				newpixels[regions.Item2.pixelIndexes[i] + 3] = 255;
			}
			//Console.WriteLine(newpixels[4096]);
			Marshal.Copy(newpixels, 0, ptrNewFirstPixel, newpixels.Length);
			newNewFile.UnlockBits(newbitmapData);
			Console.WriteLine(newNewFile.GetPixel(25, 25).GetHue());
			for (int i = 0; i < points.Count; i++)
			{
				newNewFile.SetPixel((int)points[i].X, (int)points[i].Y, Color.Red);
			}
			newNewFile.Save("VoronoiAfterRemoving.png", ImageFormat.Png);
		}
    }
}