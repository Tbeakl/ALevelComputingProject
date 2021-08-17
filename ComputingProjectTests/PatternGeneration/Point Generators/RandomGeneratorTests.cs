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
    public class RandomGeneratorTests
    {
        [TestMethod()]
        public void GeneratePointsTest()
        {
            IPointGenerator pointGenerator = new RandomGenerator();
            List<Vector2> points = pointGenerator.GeneratePoints(new Vector2(500, 500), new Random(),2500);
            Bitmap plane = new Bitmap(500, 500);
            for (int i = 0; i < points.Count; i++)
            {
                plane.SetPixel((int)points[i].X, (int)points[i].Y, Color.Red);
            }
            plane.Save("RandomGeneratorDisplay.png");
        }
    }
}