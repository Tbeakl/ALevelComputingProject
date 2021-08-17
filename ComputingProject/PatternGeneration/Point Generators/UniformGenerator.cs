using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.PatternGeneration.Point_Generators
{
    public class UniformGenerator : IPointGenerator
    {
		/// <summary>
		/// This generates a uniform grid of points, they can have offset between the rows
		/// </summary>
		/// 	/// <param name="RegionSize">Explained in the interface</param>
		/// <param name="getRandom">Not used</param>
		/// <param name="parameter1">This is the offset between the rows and between the points on 1 row</param>
		/// <param name="parameter2">This is the offset between the rows</param>
		/// <returns></returns>
		public List<Vector2> GeneratePoints(Vector2 RegionSize, Random getRandom, float parameter1 = 0, float parameter2 = 0)
        {
			//This contains all of the generated points
            List<Vector2> allPoints = new List<Vector2>();
			//This loops through every row which the points will be placed on in the image
            for (int row = 0; row < Math.Ceiling(RegionSize.Y / parameter1); row++)
            {
				//This calculates where it should start on that row by using the offset and the row
                //If row multiplied by offset is greater than or equal to length between points then start as near to the edge as possible
                float startingLocation = (row * parameter2) % parameter1;
				//This loops through all the points which need to be put on that row and inserts them into the list of all the points with the correct x and y coordinates
                for (int col = 0; col < Math.Ceiling((RegionSize.X - startingLocation) / parameter1); col++)
                {
                    allPoints.Add(new Vector2((startingLocation + col * parameter1), (row * parameter1)));
                }
            }
			//This returns the generated points
            return allPoints;
        }
    }
}