using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ComputingProject.PatternGeneration.Point_Generators
{
	/// <summary>
	/// This is the enum used in the main program to store which type of point generator is going to be used.
	/// </summary>
	enum PointGeneratorType
    {
        Random,
        Uniform,
        PoissonDisc,
        NonSelected
    }

    public interface IPointGenerator
    {
		/// <summary>
		///  This is the interface which all the point generators must implement to be used by the main program
		/// </summary>
		/// <param name="RegionSize">This is the size of the area where the points can be placed</param>
		/// <param name="getRandom">This is the random number generator to be used in the point generating process to allow reproducible results</param>
		/// <param name="parameter1">Changes depending on what is implementing this interface</param>
		/// <param name="parameter2">Changes depending on what is implementing this interface</param>
		/// <returns></returns>
		List<Vector2> GeneratePoints( Vector2 RegionSize, Random getRandom, float parameter1 = 0f, float parameter2 = 0f);

    }
}
