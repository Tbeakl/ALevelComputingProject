using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.PatternGeneration.Point_Generators
{
    public class RandomGenerator : IPointGenerator
    {
        /// <summary>
        /// This generates a set of point which are randomly distributed accross the plain by the use of a uniform random number generator
        /// </summary>
        /// <param name="parameter1">This is the number of points to generate</param>
        /// <param name="parameter2">This is not used</param>
        /// <param name="RegionSize">Explained in the interface</param>
		/// <param name="getRandom">Explained in the interface</param>
        /// <returns></returns>
        public List<Vector2> GeneratePoints(Vector2 RegionSize, Random getRandom, float parameter1, float parameter2)
        {
            //This is the list of vectors for where each point is
            List<Vector2> points = new List<Vector2>();
            //This checks that the number of points can fit in the given region
            if ((int)parameter1 > ((int)RegionSize.X * (int)RegionSize.Y)) parameter1 = (int)RegionSize.X * (int)RegionSize.Y;
            //This creates a boolean array to be able to quickly check whether a point is already where a point is trying to be placed
            bool[,] pointPos = new bool[(int)RegionSize.X, (int)RegionSize.Y];
			//This loop creates the number of points specified
            for (int i = 0; i < (int)parameter1; i++)
            {
				//This generates a point at a random location
                Vector2 curPos = new Vector2((float)RegionSize.X * (float)getRandom.NextDouble(), (float)RegionSize.Y * (float)getRandom.NextDouble());
				//This checks if a point is already at that location when it will be displayed, this is so that all of the points can be seen, it is is not to be displayed then the loop is iterated one more time
                if (pointPos[(int)curPos.X, (int)curPos.Y]) i--;
                else
                {
					//This adds the point to the list of all the points, and makes it so another point cannot be placed on the same pixel as it
                    pointPos[(int)curPos.X, (int)curPos.Y] = true;
                    points.Add(curPos);
                }
            }
			//This returns the list of all the points
            return points;
        }
    }
}