using Microsoft.VisualStudio.TestTools.UnitTesting;
using ComputingProject.PatternGeneration.Point_Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Drawing;

namespace ComputingProject.PatternGeneration.Point_Generators.Tests
{
    [TestClass()]
    public class PoissonDiscGeneratorTests
    {
        [TestMethod()]
        public void GeneratePointsTest()
        {
            IPointGenerator pointGenerator = new PoissonDiscGenerator();
            List<Vector2> points = pointGenerator.GeneratePoints(new Vector2(200, 200),new Random(), 15, 30);
            Bitmap plane = new Bitmap(200, 200);
            float red = 255f;
            float blue = 0f;
            float difference = 255f / (float)points.Count;
			for (int x = 0; x < 200; x++)
			{
				for (int y = 0; y < 200; y++)
				{
					plane.SetPixel(x, y, Color.Black);
				}
			}
            for (int i = 0; i < points.Count; i++)
            {
                plane.SetPixel((int)points[i].X, (int)points[i].Y, Color.White);
                red -= difference;
                blue += difference;
            }
            plane.Save("UnblurredStars.png");
        }
    }
}