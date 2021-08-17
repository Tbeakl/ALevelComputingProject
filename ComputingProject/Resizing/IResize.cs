using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputingProject.Resizing
{
	/// <summary>
	/// This is the enum used in the main program to store which type of enlargement is going to be used.
	/// </summary>
	enum ResizingType
    {
        NearestNeighbourDown,
        NearestNeighbourUandD,
        BiLinear,
        BiCubic,
        NotSelected
    }
	/// <summary>
	/// This is the interface which all the enlargers must implement to be used by the main program
	/// </summary>
	interface IResize
    {
		/// <summary>
		/// This is the function where the actual resizing will be done
		/// </summary>
		/// <param name="originalPixels">This byte array contains all of the colour information of the original image stored Blue Green Red Alpha so each pixel takes up a block of 4 pixels</param>
		/// <param name="baseData">This contains the metadata of the original metadata of the image</param>
		/// <param name="newPixels">This byte array is the correct size for the enlarged image to be stored in, in the same format as the in the originalPixels arrray</param>
		/// <param name="newData">This contians the metadata of the enlarged image</param>
		/// <returns>It returns the pixel information of the enlarged imagae after the enlargement has taken place</returns>
		byte[] Resize(byte[] originalPixels, BitmapData baseData, byte[] newPixels, BitmapData newData);
    }
}
