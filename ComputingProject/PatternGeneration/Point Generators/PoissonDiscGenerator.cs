using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.PatternGeneration.Point_Generators
{
    /// <summary>
    /// Based on algorithm described in the paper Fast Poisson Disk Sampling in Arbitrary Dimensions
    /// Robert Bridson
    /// University of British Columbia
    /// </summary>
    public class PoissonDiscGenerator : IPointGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegionSize">This is the size of the area where the points can be placed</param>
        /// <param name="parameter1">Sampling Radius</param>
        /// <param name="parameter2">Number until rejection</param>
        /// <returns></returns>
        public List<Vector2> GeneratePoints(Vector2 RegionSize, Random getRandom, float parameter1 = 0, float parameter2 = 0)
        {
            //This calculates the side length of the cells in the background grid so that their diagonal is equal to
            //the radius of not placing points in, then it generates a grid of this size cells across the entire plane
            double gridpartSize = parameter1 / Math.Sqrt(2);
            int[,] Grid = new int[(int)Math.Ceiling(RegionSize.X / gridpartSize), (int)Math.Ceiling(RegionSize.Y / gridpartSize)];
            //This sets all of the index's of the grid to -1 which indicates that no point is inside that cell
            for (int i = 0; i < Grid.GetLength(0); i++)
            {
                for (int j = 0; j < Grid.GetLength(1); j++)
                {
                    Grid[i, j] = -1;
                }
            }
            //These 2 lists contain: PossiblePoints contains all the points which still need to have the points generated around them
            //and allPoints contains all of the points which are present in the plane
            List<Vector2> possiblePoints = new List<Vector2>();
            List<Vector2> allPoints = new List<Vector2>();
            //This generates a starting point randomly acorss the entire grid because there are no other points to avoid
            Vector2 firstPoint = new Vector2((float)getRandom.NextDouble() * RegionSize.X, (float)getRandom.NextDouble() * RegionSize.Y);
            possiblePoints.Add(firstPoint);
            allPoints.Add(firstPoint);
            //This sets the cell to 0 indicating that point 0 is inside of it for future lookup
            Grid[(int)(possiblePoints[0].X /gridpartSize), (int)(possiblePoints[0].Y / gridpartSize)] = 0;
            //This starts the index count for keeping track of which point is being placed next
            int index = 1;
            while (possiblePoints.Count != 0)
            {
                //This picks a random point to generate new points around, and then removes the point which they 
                //are being generated around from the list containing points to generate round
                Vector2 curPoint = possiblePoints[getRandom.Next(possiblePoints.Count)];
                possiblePoints.Remove(curPoint);
                //This loops round the number of times specified in trying to generate new points
                for (int i = 0; i < parameter2; i++)
                {
                    //This generates a random angle bewteen 0 and 2π, and of random length away from the current point is between 1 and 2 radii
                    double angle = 2 * Math.PI * getRandom.NextDouble();
                    double length = parameter1 + parameter1 * getRandom.NextDouble();
                    //The next stage checks that the new point can be placed
                    bool placeable = true;
                    //This comes up with new point by using the angle and length with triginometry
                    Vector2 newPoint = new Vector2(curPoint.X + (float)length * (float)Math.Cos(angle),curPoint.Y + (float)length * (float)Math.Sin(angle));
                    //Console.WriteLine("New Point: X: " + newPoint.X + " Y: " + newPoint.Y + ", Point Away From: X: " + curPoint.X + " Y: " + curPoint.Y +" Distance Apart: " + Vector2.Distance(newPoint,curPoint));
                    //This checks that the new points falls within the region size
                    if (newPoint.X < 0 || newPoint.X > RegionSize.X || newPoint.Y < 0 || newPoint.Y > RegionSize.Y)
                    {
                        placeable = false;
                    }
                    else
                    {
                        //This calculates which cell on the grid the current point is then it add 2 to both of the value so that they are the 
                        //top value where a near point could be 
                        int xWidth = (int)(newPoint.X / gridpartSize) + 2;
                        int yHeight = (int)(newPoint.Y / gridpartSize) + 2;
                        //This loops through the cells 5 * 5 around the cell which the new point is in
                        for (int x = (int)(newPoint.X / gridpartSize) - 2 ; x <= xWidth; x++)
                        {
                            for (int y = (int)(newPoint.Y / gridpartSize) - 2; y <= yHeight; y++)
                            {
                                //This checks that the cell we are looking for actually is within the grid
                                if (x < 0 || x >= Grid.GetLength(0) || y < 0 || y >= Grid.GetLength(1))
                                {
                                    placeable = false;
                                }
                                else
                                {
                                    //This checks to see if the cell underconsideration actually contains another point, if it does then the distance
                                    //between that point and the new point are calculated if it less than the radius then the new point is not able to be 
                                    //placed
                                    if(Grid[x,y] != -1)
                                    {
                                        Vector2 nearPoint = allPoints[Grid[x, y]];
                                        if (Vector2.Distance(nearPoint, newPoint) < parameter1) placeable = false;
                                    }
                                }
                            }
                        }
                    }
                    //This checks to see that the new point can actaully be placed
                    if (placeable)
                    {
                        //This sets the cell in which it is contained equal to its index in the allPoints list, then the index is icrememnted
                        //the new point added to the list of all points and the list containing the points to have new points generated around
                        Grid[(int)(newPoint.X/gridpartSize), (int)(newPoint.Y/gridpartSize)] = index;
                        index++;
                        possiblePoints.Add(newPoint);
                        allPoints.Add(newPoint);
                    }
                }
               /*
			    *THIS IS JUST CODE WHICH WAS USED WHEN DEBUGGING
				* Bitmap drawing = new Bitmap((int)RegionSize.X, (int)RegionSize.Y);
                for (int i = 0; i < allPoints.Count; i++)
                {
                    drawing.SetPixel((int)allPoints[i].X, (int)allPoints[i].Y, Color.Red);               
                }
                drawing.Save("PoissonRandomGeneratorDisplay"+index+".png"); */
            }
            //This returns all of the suitable points in a list to the main function
            return allPoints;
        }
    }
}
