using System.Drawing.Imaging;

namespace ComputingProject.Blur
{
    /// <summary>
    /// This is the enum used in the main program to store which type of blur is going to be used.
    /// </summary>
    enum BlurringType
    {
        Mean,
        Gaussian,
        Bokeh,
        Shape,
        NotSelected
    }

    public interface IBlur
    {
        /// <summary>
        /// This is the interface which all the blurs must implement so that the main program can use them
        /// </summary>
        /// <param name="originalPixels">This byte array contains all of the colour information of the original image stored Blue Green Red Alpha so each pixel takes up a block of 4 pixels</param>
        /// <param name="baseData">This contains the metadata of the original metadata of the image</param>
        /// <param name="radius">This specifies how large the kernel should be, if it is not sent as an array to the blur</param>
        /// <param name="power">This specifies the power to be used in the gamma correction process</param>
        /// <param name="kernelImage">This is an integer array contianing the values of kernel if it has been input as an image</param>
        /// <param name="area">This specifies the area of the image to be blurred</param>
        /// <param name="isEllipse">The tells the blur if the area being blurred is an ellipse</param>
        /// <param name="isInverted">This says if the area specified should be the area blurred or the area not blurred</param>
        /// <returns>It returns the blurred image's pixel data as a byte array in the same form as the original pixel data</returns>
        byte[] Blur(byte[] originalPixels, BitmapData baseData, float radius, float power, int[,] kernelImage, Rectangle area, bool isEllipse, bool isInverted);
    }
}
