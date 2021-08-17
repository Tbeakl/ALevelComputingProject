using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace ComputingProject.PatternGeneration.Voronoi
{
	public class Voronoi
    {

        List<Vector2> places;
        List<Region> regions;
		Region backgroundRegion;
        Vector2 regionSize;
        int bytesPerPixel;
        int[,] grid;
        float gridpartSize;
		/// <summary>
		/// This loops through every possible pair of points
		/// </summary>
		/// <param name="points">The points to be looped through</param>
		/// <returns>The shortest distance between the given points</returns>
        private static float bruteForce(List<Vector2> points)
        {
            float min = float.MaxValue;
            for (int i = 0; i < points.Count; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    if (Vector2.Distance(points[i], points[j]) < min) min = Vector2.Distance(points[i], points[j]);
                }
            }
            return min;
        }
		/// <summary>
		/// This implements a divide and conquer algorithm to find the smallest distance between two points 
		/// </summary>
		/// <param name="points">This is the list of points to be searched</param>
		/// <param name="firstTime"></param>
		/// <returns>The shroest distance between any two points</returns>
		public static float calculateRadius(List<Vector2> points, bool firstTime)
        {
            if (points.Count <= 3) return bruteForce(points);
            //This puts the list in order of x then if two share the same x coordinate then by their y position
            if(firstTime) points = points.OrderBy(p => p.X).ToList();
            List<Vector2> leftSide = points.GetRange(0, points.Count / 2);
            List<Vector2> rightSide = points.GetRange(points.Count / 2, points.Count / 2);
            if (leftSide.Count + rightSide.Count != points.Count)
            {
                rightSide.Add(points[points.Count - 1]);
            }
            float minimumDistanceLeft = calculateRadius(leftSide,false);
            float minimumDistanceRight = calculateRadius(rightSide,false);
            float dist = (float)Math.Min(minimumDistanceLeft, minimumDistanceRight);
            List<Vector2> Strip = new List<Vector2>();
            Strip.Add(rightSide[0]);
            int addAmount = 1;
            bool leftRunOut = false;
            bool rightRunOut = false;
            while (!(leftRunOut && rightRunOut))
            {
                if (addAmount >= points.Count / 2)
                {
                    leftRunOut = true;
                    rightRunOut = true;
                }
                else
                {
                    if (!rightRunOut && points[(points.Count / 2) + addAmount].X - Strip[0].X < dist) Strip.Add(points[(points.Count / 2) + addAmount]);
                    else rightRunOut = true;
                    if (!leftRunOut && Strip[0].X - points[(points.Count / 2) - addAmount].X < dist) Strip.Add(points[(points.Count / 2) - addAmount]);
                    else leftRunOut = true;
                    addAmount++;
                }

            }
            Strip = Strip.OrderBy(p => p.Y).ToList();
            float distLR = float.MaxValue;
            //Need to see if something in the strip is of shorter distance
            if (Strip.Count == 1) return dist;
            for (int i = 0; i < Strip.Count; i++)
            {
                for (int j = i+1; j < Strip.Count && (Strip[j].Y -Strip[i].Y) < dist; j++)
                {
                    if (Vector2.DistanceSquared(Strip[i], Strip[j]) < distLR) distLR = Vector2.DistanceSquared(Strip[i], Strip[j]);
                }
            }
            distLR = (float)Math.Sqrt(distLR);
            dist = Math.Min(distLR, dist);
            return dist;

        }
        public Voronoi(List<Vector2> vertices, Vector2 RegionSize, float Radius, int BytesPerPixel)
        {
            gridpartSize = Radius / (float)Math.Sqrt(2);
            grid = new int[(int)Math.Ceiling(RegionSize.X / gridpartSize), (int)Math.Ceiling(RegionSize.Y / gridpartSize)];
            places = vertices;
            fillGrid();
            regionSize = RegionSize;
            bytesPerPixel = BytesPerPixel;
            regions = new List<Region>();
            for (int i = 0; i < places.Count; i++)
            {
                regions.Add(new Region(i));
            }
			backgroundRegion = new Region(-1);
        }

        private (float, int, float) CheckCells(int x, int y, int searchDistance, float p, Vector2 point)
        {
            int nearestPoint = -1;
            float nearestDistance = float.MaxValue;
			float secondNearestDistance = float.MaxValue;
            for (int i = x - searchDistance; i <= x + searchDistance; i++)
            {
                for (int j = y - searchDistance; j <= y + searchDistance; j++)
                {
                    if (i < 0 || i >= grid.GetLength(0) || j < 0 || j >= grid.GetLength(1))
                    {
                        //It has fallen out of the image
                    }
                    else if ((i != x + searchDistance && i != x - searchDistance) && (j != y + searchDistance && j != y - searchDistance))
                    {

                    }
                    else if (grid[i, j] == -1)
                    {

                    }
                    else
                    {
                        //It has found a cell with a point in it
                        float distanceToPoint = (float)Math.Pow((double)Math.Pow((double)Math.Abs(point.X - places[grid[i, j]].X), (double)p) + (double)Math.Pow((double)Math.Abs(point.Y - places[grid[i, j]].Y), (double)p), 1d / (double)p);
                        if (distanceToPoint < nearestDistance)
                        {
							secondNearestDistance = nearestDistance;
							nearestDistance = distanceToPoint;
                            nearestPoint = grid[i, j];
                        }
						else if(distanceToPoint < secondNearestDistance)
						{
							secondNearestDistance = distanceToPoint;
						}
                    }
                }
            }

            return (nearestDistance, nearestPoint, secondNearestDistance);
        }

        public (List<Region>, Region) MakeRegions(float borderWidth, float p, bool randomlyGenerated)
        {
            for (int x = 0; x < regionSize.X; x++)
            {
                for (int y = 0; y < regionSize.Y; y++)
                {
                    if (!randomlyGenerated)
                    {
                        int gridX = (int)((float)x / gridpartSize);
                        int gridY = (int)((float)y / gridpartSize);
                        //I need to search around until I have found the nearest site
                        int foundCellIndex = -1;
                        float distanceToCell = float.MaxValue;
                        float secondShortestDistance = float.MaxValue;
                        int maxdistanceAdd = int.MaxValue;
                        for (int addAmount = 0; addAmount <= maxdistanceAdd; addAmount++)
                        {
                            (float, int, float) curSearchData = CheckCells(gridX, gridY, addAmount, p, new Vector2(x, y));
                            if (curSearchData.Item2 == -1)
                            {
                                //This means no site has been found in this cell so just go out another row to look again
                            }
                            else
                            {
                                if (curSearchData.Item1 < distanceToCell)
                                {
                                    //This means that the new site which had been found is closer to the point
                                    secondShortestDistance = distanceToCell;
                                    if (curSearchData.Item3 < secondShortestDistance) secondShortestDistance = curSearchData.Item3;
                                    foundCellIndex = curSearchData.Item2;
                                    distanceToCell = curSearchData.Item1;
                                    int amountRequiredLeft = (int)Math.Abs(Math.Floor((float)(x - distanceToCell - borderWidth) / gridpartSize) - gridX);
                                    int amountRequiredUp = (int)Math.Abs(Math.Floor((float)(y - distanceToCell - borderWidth) / gridpartSize) - gridY);
                                    int amountRequiredRight = (int)Math.Abs(Math.Ceiling((float)(x + distanceToCell + borderWidth) / gridpartSize) - gridX);
                                    int amountRequiredDown = (int)Math.Abs(Math.Ceiling((float)(y + distanceToCell + borderWidth) / gridpartSize) - gridY);
                                    maxdistanceAdd = Math.Max(Math.Max(amountRequiredDown, amountRequiredUp), Math.Max(amountRequiredLeft, amountRequiredRight));
                                }
                                else if (curSearchData.Item1 < secondShortestDistance)
                                {
                                    secondShortestDistance = curSearchData.Item1;
                                }
                            }
                        }
                        if (secondShortestDistance - distanceToCell <= borderWidth && borderWidth != 0)
                        {
                            //This place needs to be added to the background region
                            backgroundRegion.pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
                            backgroundRegion.Size++;
                        }
                        else
                        {
                            //At this point the nearest site has been found so this point needs to be added to its region --> Probably need to switch x and y back
                            regions[foundCellIndex].pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
                            regions[foundCellIndex].Size++;
                        }
                    }
                    else
                    {
                        //Simple Loop through all the points which exist and find the one with the shortest disance to the point
                        float shortestDistance = float.MaxValue;
                        float secondShortestDistance = float.MaxValue;
                        int shortestDistanceIndex = -1;
                        for (int i = 0; i < places.Count; i++)
                        {
                            float dist = (float)Math.Pow(Math.Pow(Math.Abs(places[i].X - x), p) + Math.Pow(Math.Abs(places[i].Y - y), p), 1f / p);
                            if(dist < shortestDistance)
                            {
                                secondShortestDistance = shortestDistance;
                                shortestDistance = dist;
                                shortestDistanceIndex = i;
                            }
                            else if(dist < secondShortestDistance)
                            {
                                secondShortestDistance = dist;
                            }
                        }

                        if(secondShortestDistance - shortestDistance <= borderWidth && borderWidth > 0)
                        {
                            backgroundRegion.pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
                            backgroundRegion.Size++;
                        }
                        else
                        {
                            regions[shortestDistanceIndex].pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
                            regions[shortestDistanceIndex].Size++;
                        }
                    }
                }
            }

            return (regions, backgroundRegion);
        }
		//Need to change to background which would now be included in actaul regions because of the point removed
        public (List<Region>, Region) RemoveRegion(int index, float p, float borderWidth)
        {
			//I need to calculate the extremites of this region so that I can then alter it so that the new points 
			int minX = int.MaxValue;
			int maxX = int.MinValue;
			int minY = int.MaxValue;
			int maxY = int.MinValue;
			for (int i = 0; i < regions[index].Size; i++)
			{
				int y = regions[index].pixelIndexes[i] / (int)(regionSize.X * bytesPerPixel);
				int x = (regions[index].pixelIndexes[i] % (int)(regionSize.X * bytesPerPixel)) / bytesPerPixel;
				minY = Math.Min(y, minY);
				maxY = Math.Max(y, maxY);
				minX = Math.Min(x, minX);
				maxX = Math.Max(x, maxX);
			}
			minX -= (int)Math.Ceiling(borderWidth);
			maxX += (int)Math.Ceiling(borderWidth);
			minY -= (int)Math.Ceiling(borderWidth);
			maxY += (int)Math.Ceiling(borderWidth);
			Region regionToRemove = regions[index];
            Vector2 pointRemoved = places[index];
            regions.RemoveAt(index);
            places.RemoveAt(index);
            for (int i = 0; i < regions.Count; i++)
            {
                regions[i].basePoint = i;
            }
            fillGrid();
			//Now I need to loop through all of these positions and check if they are in the background if they are then I should remove them from the background and add them back into reconsideration
			for (int x = minX; x <= maxX; x++)
			{
				for (int y = minY; y <= maxY; y++)
				{
					int pixelIndex = x * bytesPerPixel + (int) (y * bytesPerPixel * regionSize.X);
					if (backgroundRegion.pixelIndexes.Contains(pixelIndex))
					{
						regionToRemove.pixelIndexes.Add(pixelIndex);
						regionToRemove.Size++;
						backgroundRegion.pixelIndexes.Remove(pixelIndex);
						backgroundRegion.Size--;
					}
				}
			}
			//Now I need to recalculate all of the pixels in the region so that they fall in new regions
			for (int i = 0; i < regionToRemove.Size; i++)
            {
                int y = regionToRemove.pixelIndexes[i] / (int)(regionSize.X * bytesPerPixel);
                int x = (regionToRemove.pixelIndexes[i] % (int)(regionSize.X * bytesPerPixel)) / bytesPerPixel;

                int gridX = (int)((float)x / gridpartSize);
                int gridY = (int)((float)y / gridpartSize);
				//I need to search around until I have found the nearest site
				int foundCellIndex = -1;
				float distanceToCell = float.MaxValue;
				float secondShortestDistance = float.MaxValue;
				int maxdistanceAdd = int.MaxValue;
				for (int addAmount = 0; addAmount <= maxdistanceAdd; addAmount++)
				{
					(float, int, float) curSearchData = CheckCells(gridX, gridY, addAmount, p, new Vector2(x, y));
					if (curSearchData.Item2 == -1)
					{
						//This means no site has been found in this cell so just go out another row to look again
					}
					else
					{
						if (curSearchData.Item1 < distanceToCell)
						{
							//This means that the new site which had been found is closer to the point
							secondShortestDistance = distanceToCell;
							if (curSearchData.Item3 < secondShortestDistance) secondShortestDistance = curSearchData.Item3;
							foundCellIndex = curSearchData.Item2;
							distanceToCell = curSearchData.Item1;
							int amountRequiredLeft = (int)Math.Abs(Math.Floor((float)(x - distanceToCell - borderWidth) / gridpartSize) - gridX);
							int amountRequiredUp = (int)Math.Abs(Math.Floor((float)(y - distanceToCell - borderWidth) / gridpartSize) - gridY);
							int amountRequiredRight = (int)Math.Abs(Math.Ceiling((float)(x + distanceToCell + borderWidth) / gridpartSize) - gridX);
							int amountRequiredDown = (int)Math.Abs(Math.Ceiling((float)(y + distanceToCell + borderWidth) / gridpartSize) - gridY);
							maxdistanceAdd = Math.Max(Math.Max(amountRequiredDown, amountRequiredUp), Math.Max(amountRequiredLeft, amountRequiredRight));
						}
						else if (curSearchData.Item1 < secondShortestDistance)
						{
							secondShortestDistance = curSearchData.Item1;
						}
					}
				}
				if (secondShortestDistance - distanceToCell <= borderWidth)
				{
					//This place needs to be added to the background region
					backgroundRegion.pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
					backgroundRegion.Size = backgroundRegion.Size + 1;
				}
				else
				{
					//At this point the nearest site has been found so this point needs to be added to its region --> Probably need to switch x and y back
					regions[foundCellIndex].pixelIndexes.Add((y * (int)regionSize.X * bytesPerPixel) + x * bytesPerPixel);
					regions[foundCellIndex].Size = regions[foundCellIndex].Size + 1;
				}
			}
            return (regions, backgroundRegion);
        }

        private int[,] fillGrid()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = -1;
                }
            }
            for (int i = 0; i < places.Count; i++)
            {
                grid[(int)(places[i].X / gridpartSize), (int)(places[i].Y / gridpartSize)] = i;
            }
            return grid;
        }
    }

    public class Region
    {
        public Color regionColor;
        public List<int> pixelIndexes;
        public int Size;
        public int basePoint;
        public Region(int index)
        {
            basePoint = index;
            regionColor = Color.Black;
            Size = 0;
            pixelIndexes = new List<int>();
        }
    }
}