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
    public class UniformGeneratorTests
    {
        [TestMethod()]
        public void GeneratePointsTest()
        {
            IPointGenerator pointGenerator = new UniformGenerator();
            List<Vector2> points = pointGenerator.GeneratePoints(new Vector2(5000, 5000), new Random(),15f, 7f);
            Bitmap plane = new Bitmap(5000, 5000);
            for (int i = 0; i < points.Count; i++)
            {
                plane.SetPixel((int)points[i].X, (int)points[i].Y, Color.Red);
            }
            plane.Save("UniformGeneratorDisplay.png");
        }
    }
}