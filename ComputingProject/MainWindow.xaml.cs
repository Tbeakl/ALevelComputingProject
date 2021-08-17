using ComputingProject.Blur;
using ComputingProject.PatternGeneration.Point_Generators;
using ComputingProject.PatternGeneration.Voronoi;
using ComputingProject.Resizing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Region = ComputingProject.PatternGeneration.Voronoi.Region;

namespace ComputingProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PictureCanvas.Children.Add(areaEllipse);
            PictureCanvas.Children.Add(areaRectangle);
            areaEllipse.IsEnabled = true;
            areaEllipse.Height = areaHeight;
            areaEllipse.Width = areaWidth;
            areaEllipse.Fill = Brushes.Red;

            areaRectangle.IsEnabled = true;
            areaRectangle.Height = areaHeight;
            areaRectangle.Width = areaWidth;
            areaRectangle.Fill = Brushes.Red;

            Canvas.SetLeft(areaEllipse, distanceOffLeft);
            Canvas.SetBottom(areaEllipse, distanceOffBottom);
            Canvas.SetLeft(areaRectangle, distanceOffLeft);
            Canvas.SetBottom(areaRectangle, distanceOffBottom);
        }
        //Copied from stack overflow
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        ResizingType resizingMethod = ResizingType.NotSelected; 
        
        Bitmap inputImage;
        string inputImagePath;
        bool currentlyLockedInput = false;
        string inputImageFileName;
        Bitmap outputImage;
        bool currentlyLockedOutput = false;
        #region Enlarging
        private void Enlargring_InputFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if(currentlyLockedInput)
            {
                MessageBox.Show("Please wait for the image to be finished processed", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                // Create OpenFileDialog 
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                // Set filter for file extension and default file extension 
                dlg.DefaultExt = ".png";
                dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
                // Display OpenFileDialog by calling ShowDialog method 
                bool? result = dlg.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    string[] pathParts = filename.Split('\\');
                    inputImageFileName = pathParts[pathParts.Length - 1];
                    inputImagePath = "";
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        inputImagePath += pathParts[i] + "\\";
                    }
                    Enlarging_InputFileName_Textbox.Text = filename;
                    inputImage = new Bitmap(Bitmap.FromFile(inputImagePath + inputImageFileName));

                    Enlarging_InputFileWidth_Textbox.Text = inputImage.Width.ToString();
                    Enlarging_InputFileHeight_Textbox.Text = inputImage.Height.ToString();
                }
                Enlarging_ShowOrginal_Button_Click(this, new RoutedEventArgs());
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("There was an error with the file you selected.", "Selected File Issue", MessageBoxButton.OK, MessageBoxImage.Error);
                inputImageFileName = "";
                inputImagePath = "";
                Enlarging_InputFileName_Textbox.Text = "Example.png";
                Enlarging_InputFileWidth_Textbox.Text = "File Width";
                Enlarging_InputFileHeight_Textbox.Text = "File Height";
            }
        }
       
        private void Enlarging_ShowOrginal_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayInputImage();
        }

        private void Enlarging_ShowNew_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayOutputImage();
        }

        private void Englarging_Method_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox sentCombo = sender as ComboBox;
            if (sentCombo.SelectedIndex == 0) resizingMethod = ResizingType.NearestNeighbourDown;
            else if (sentCombo.SelectedIndex == 1) resizingMethod = ResizingType.NearestNeighbourUandD;
            else if (sentCombo.SelectedIndex == 2) resizingMethod = ResizingType.BiLinear;
            else if (sentCombo.SelectedIndex == 3) resizingMethod = ResizingType.BiCubic;
        }

        private void Enlarging_StartEnglarging_Click(object sender, RoutedEventArgs e)
        {
            bool scaleFactor = (bool)Enlarging_ScaleFactor_RadioButton.IsChecked;
            string ScaleFactorVal = Enlarging_ScaleFactor_Textbox.Text;
            string newWidth = Enlarging_OutputFileWidth_Textbox.Text;
            string newHeight = Enlarging_OutputFileHeight_Textbox.Text;
            string outputFileName = OutputFilenameTextBox.Text;
            Task.Run(() => Enlarging(scaleFactor, ScaleFactorVal, newWidth, newHeight, outputFileName));
        }

        private void Enlarging(bool IsScale, string ScaleFactorVal, string NewWidth, string NewHeight, string OutputFilename)
        {
            if (currentlyLockedInput || currentlyLockedOutput)
            {
                MessageBox.Show("Either the input or output image is not currently available", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (inputImage == null)
            {
                MessageBox.Show("There is not selected input image", "Input Image", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (resizingMethod == ResizingType.NotSelected)
            {
                MessageBox.Show("There is no method for resizing selected", "Enlarging Method", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int newWidth = 0;
            int newHeight = 0;
            if (IsScale)
            {
                //This means that it is by scale factor and not by specifying
                float scaleFactor = 1;
                if (!float.TryParse(ScaleFactorVal, out scaleFactor))
                {
                    MessageBox.Show("The entered scale factor is not a valid number", "Scale Factor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if(scaleFactor <= 1)
                {
                    MessageBox.Show("The entered scale factor is not a valid number", "Scale Factor Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    scaleFactor = Math.Abs(scaleFactor);
                    newWidth = (int)(scaleFactor * inputImage.Width);
                    newHeight = (int)(scaleFactor * inputImage.Height);
                }
            }
            else
            {
                if (!int.TryParse(NewWidth, out newWidth))
                {
                    MessageBox.Show("The entered width is not a valid number", "Output Width Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (newWidth < inputImage.Width)
                {
                    MessageBox.Show("The entered width is not a valid number", "Output Width Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!int.TryParse(NewHeight, out newHeight))
                {
                    MessageBox.Show("The entered height is not a valid number", "Output Height Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (newHeight < inputImage.Height)
                {
                    MessageBox.Show("The entered height is not a valid number", "Output Height Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            IResize Enlarger = new NearestNeighbourRoundD();
            if (resizingMethod == ResizingType.NearestNeighbourDown) Enlarger = new NearestNeighbourRoundD();
            else if (resizingMethod == ResizingType.NearestNeighbourUandD) Enlarger = new NearestNeighbourRoundUandD();
            else if (resizingMethod == ResizingType.BiLinear) Enlarger = new BiLinear();
            else if (resizingMethod == ResizingType.BiCubic) Enlarger = new BiCubic();

            //Now we need to get all the data out of the bitmap to be passed to the resizer
            currentlyLockedInput = true;
            //Generate the required data from the input image
            BitmapData inputFileData = inputImage.LockBits(new System.Drawing.Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, inputImage.PixelFormat);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(inputImage.PixelFormat) / 8;
            int inputByteCount = inputFileData.Stride * inputImage.Height;
            byte[] inputPixels = new byte[inputByteCount];
            IntPtr firstInputPixelPtr = inputFileData.Scan0;
            Marshal.Copy(firstInputPixelPtr, inputPixels, 0, inputPixels.Length);

            currentlyLockedOutput = true;
            //Generate the required data for the output image
            outputImage = new Bitmap(newWidth, newHeight);
            BitmapData outputFileData = outputImage.LockBits(new System.Drawing.Rectangle(0, 0, newWidth, newHeight), ImageLockMode.ReadWrite, inputImage.PixelFormat);
            int outputByteCount = outputFileData.Stride * outputImage.Height;
            byte[] outputPixels = new byte[outputByteCount];
            IntPtr firstOutputPixelPtr = outputFileData.Scan0;

            Enlarger.Resize(inputPixels, inputFileData, outputPixels, outputFileData);

            Marshal.Copy(inputPixels, 0, firstInputPixelPtr, inputPixels.Length);
            inputImage.UnlockBits(inputFileData);
            currentlyLockedInput = false;
            Marshal.Copy(outputPixels, 0, firstOutputPixelPtr, outputPixels.Length);
            outputImage.UnlockBits(outputFileData);
            try
            {
                if (OutputFilename == "")
                {
                    //They have not set a file to save the image to so we will just save it to a default file
                    outputImage.Save(inputImagePath + "example.png", ImageFormat.Png);
                }
                else
                {
                    outputImage.Save(inputImagePath + OutputFilename);
                }
            }
            catch
            {
                MessageBox.Show("There was an issue saving the output image", "Output Image Saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            currentlyLockedOutput = false;
            //From StackOverflow https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            this.Dispatcher.Invoke(() => { Enlarging_ShowNew_Button_Click(this, new RoutedEventArgs()); });
        }

        #endregion

        #region Voronoi
        PointGeneratorType generatorType = PointGeneratorType.NonSelected;
        IPointGenerator generator;
        Vector2 areaSize;
        List<Vector2> points;
        float radius;
		Voronoi voronoiPattern;
		(List<Region>, Region) regions;
		const int bytesPerPixel = 4;
        bool pointsGeneratedRandomly = false;
        private void Voronoi_PointGenerator_Button_Click(object sender, RoutedEventArgs e)
        {
            int width = 0;
            int height = 0;
            if (currentlyLockedOutput)
            {
                MessageBox.Show("The output image is not currently available", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(Voronoi_OutputFileWidth_Textbox.Text, out width))
            {
                MessageBox.Show("The entered width is not a valid number", "Input Width Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (width <= 0)
            {
                MessageBox.Show("The entered width is not a valid number", "Input Width Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(Voronoi_OutputFileHeight_Textbox.Text, out height))
            {
                MessageBox.Show("The entered height is not a valid number", "Input Height Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (height <= 0)
            {
                MessageBox.Show("The entered height is not a valid number", "Input Height Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
			areaSize = new Vector2(width, height);
            string parameter1 = Voronoi_PointGenerator_Parameter1_Textbox.Text;
            string parameter2 = Voronoi_PointGenerator_Parameter2_Textbox.Text;
            string pointGenSeed = Voronoi_PointGenerator_Seed_Textbox.Text;
            Task.Run(() => GeneratePoints(parameter1, parameter2, pointGenSeed));
        }

        private void GeneratePoints(string Parameter1Str, string Parameter2Str, string pointGenSeed)
        {
            
            float parameter1 = 0f;
            float parameter2 = 0f;
            
            if (generatorType == PointGeneratorType.NonSelected)
            {
                MessageBox.Show("There is no method selected for point generation", "Point Generator", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (generatorType == PointGeneratorType.Random)
            {
                //Need to check that parameter1 is fine and sensible
                int param1 = 0;
                if (!int.TryParse(Parameter1Str, out param1))
                {
                    MessageBox.Show("The number of points to generate is not a valid number", "Num. of Points", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (param1 <= 0)
                {
                    MessageBox.Show("The number of points to generate is not a valid number", "Num. of Points", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                parameter1 = param1;
                pointsGeneratedRandomly = true;
                generator = new RandomGenerator();
            }
            else if (generatorType == PointGeneratorType.Uniform)
            {
                //Need to check that both parameters are sensible
                if (!float.TryParse(Parameter1Str, out parameter1))
                {
                    MessageBox.Show("The distance between points is not a valid number", "Radius", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter1 <= 0)
                {
                    MessageBox.Show("The distance between points is not a valid number", "Radius", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!float.TryParse(Parameter2Str, out parameter2))
                {
                    MessageBox.Show("The offset for consecutive rows is not a valid number", "Offset", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter2 < 0)
                {
                    MessageBox.Show("The offset for consecutive rows is not a valid number", "Offset", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                pointsGeneratedRandomly = false;
                generator = new UniformGenerator();
            }
            else if (generatorType == PointGeneratorType.PoissonDisc)
            {
                //Need to check that both parameters are sensible
                if (!float.TryParse(Parameter1Str, out parameter1))
                {
                    MessageBox.Show("The distance between points is not a valid number", "Radius", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter1 <= 0)
                {
                    MessageBox.Show("The distance between points is not a valid number", "Radius", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                int param2 = -5;
                if (!int.TryParse(Parameter2Str, out param2))
                {
                    MessageBox.Show("The number of tries per point is not a valid number", "Num. of Tries", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (param2 <= 1)
                {
                    MessageBox.Show("The number of tries per point is not a valid number", "Num. of Tries", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                parameter2 = param2;
                pointsGeneratedRandomly = false;
                generator = new PoissonDiscGenerator();
            }
			//All the parameters are correct so we now need to generate the point array
			Random getRandom;
			if (pointGenSeed != "")
			{
				getRandom = new Random(pointGenSeed.GetHashCode());
			}
			else
			{
				getRandom = new Random();
			}
            points = generator.GeneratePoints(areaSize, getRandom, parameter1, parameter2);
            if(generatorType == PointGeneratorType.Random)
            {
                radius = Voronoi.calculateRadius(points, true);
            }
            else
            {
                radius = parameter1;
            }

        }
        private void Voronoi_PointGenerator_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Voronoi_PointGenerator_Combobox.SelectedIndex == 0) generatorType = PointGeneratorType.Random;
            else if (Voronoi_PointGenerator_Combobox.SelectedIndex == 1) generatorType = PointGeneratorType.Uniform;
            else if (Voronoi_PointGenerator_Combobox.SelectedIndex == 2) generatorType = PointGeneratorType.PoissonDisc;

            if(generatorType != PointGeneratorType.NonSelected)
            {
                Voronoi_PointGenerator_Parameter1_Label.IsEnabled = true;
                Voronoi_PointGenerator_Parameter2_Label.IsEnabled = true;
                Voronoi_PointGenerator_Parameter1_Textbox.IsEnabled = true;
                Voronoi_PointGenerator_Parameter2_Textbox.IsEnabled = true;
                Voronoi_PointGenerator_Button.IsEnabled = true;
				Voronoi_PointGenerator_Seed_Label.IsEnabled = false;
				Voronoi_PointGenerator_Seed_Textbox.IsEnabled = false;
            }

            if(generatorType == PointGeneratorType.Random)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Num. of Points:";
                Voronoi_PointGenerator_Parameter2_Label.IsEnabled = false;
                Voronoi_PointGenerator_Parameter2_Textbox.IsEnabled = false;
				Voronoi_PointGenerator_Seed_Label.IsEnabled = true;
				Voronoi_PointGenerator_Seed_Textbox.IsEnabled = true;
			}
            else if(generatorType == PointGeneratorType.Uniform)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Radius:";
                Voronoi_PointGenerator_Parameter2_Label.Content = "Offset:";
				Voronoi_PointGenerator_Seed_Label.IsEnabled = false;
				Voronoi_PointGenerator_Seed_Textbox.IsEnabled = false;
			}
            else if(generatorType == PointGeneratorType.PoissonDisc)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Radius:";
                Voronoi_PointGenerator_Parameter2_Label.Content = "Tries Bef. Rejc.:";
                Voronoi_PointGenerator_Parameter2_Textbox.Text = "30";
				Voronoi_PointGenerator_Seed_Label.IsEnabled = true;
				Voronoi_PointGenerator_Seed_Textbox.IsEnabled = true;
			}
        }

        private void Voronoi_ColourChanging_Button_Click(object sender, RoutedEventArgs e)
        {
            Window colourChangingWindow = new ColourManipulation(this);
            colourChangingWindow.Show();          
        }

        private void Voronoi_DisplayOutput_Button_Click(object sender, RoutedEventArgs e)
        {
			currentlyLockedOutput = true;
			//This is where all of the colour manipulation data needs to be read in and put into affect on the file
			outputImage = new Bitmap((int)areaSize.X, (int)areaSize.Y);
			BitmapData outputFileData = outputImage.LockBits(new System.Drawing.Rectangle(0, 0, (int)areaSize.X, (int)areaSize.Y), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int outputByteCount = outputFileData.Stride * outputImage.Height;
			byte[] outputPixels = new byte[outputByteCount];
			IntPtr firstOutputPixelPtr = outputFileData.Scan0;
			int smallestRegionSize = int.MaxValue;
			int largestRegionSize = int.MinValue;
			if(ColourManipulationData.regionInfo1.colouringMethod == ColouringMethod.Size || ColourManipulationData.regionInfo2.colouringMethod == ColouringMethod.Size || (ColourManipulationData.PointsDifferent && (ColourManipulationData.pointsInfo1.colouringMethod == ColouringMethod.Size || ColourManipulationData.pointsInfo2.colouringMethod == ColouringMethod.Size)))
			{
				for (int i = 0; i < regions.Item1.Count; i++)
				{
					smallestRegionSize = Math.Min(smallestRegionSize, regions.Item1[i].Size);
					largestRegionSize = Math.Max(largestRegionSize, regions.Item1[i].Size);
				}
			}
			bool differentSizeRegions = true;
			if (largestRegionSize - smallestRegionSize == 0) differentSizeRegions = false;
			//First do the background pixels
			Random getRandom;
			if (Voronoi_Colour_Seed_Textbox.Text != "")
			{
				getRandom = new Random(Voronoi_Colour_Seed_Textbox.Text.GetHashCode());
			}
			else
			{
				getRandom = new Random();
			}
			//Just doing the background
			#region BackgroundColouring
			if (regions.Item2.Size != 0)
			{
				for (int i = 0; i < regions.Item2.Size; i++)
				{
					Color firstCol = Color.White;
					Color secondCol = Color.White;
					bool useSecondCol = true;
					float amountThrough = 0f;
					if (ColourManipulationData.backgroundInfo2.colouringMethod == ColouringMethod.NoneSelected) useSecondCol = false;
					switch (ColourManipulationData.backgroundInfo1.colouringMethod)
					{
						case ColouringMethod.Constant:
							firstCol = ColourManipulationData.backgroundInfo1.firstColour;
							break;
						case ColouringMethod.XCoordinate:
							int x = (regions.Item2.pixelIndexes[i] % (int)(areaSize.X * bytesPerPixel)) / bytesPerPixel;
							amountThrough = (float)x / areaSize.X;
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.A)), (int) (((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.G)),(int) (((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.B)));
							break;
						case ColouringMethod.YCoordinate:
							int y = regions.Item2.pixelIndexes[i] / (int)(areaSize.X * bytesPerPixel);
							amountThrough = (float)y / areaSize.Y;
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.backgroundInfo1.secondColour.B)));
							break;
					}
					if(useSecondCol == false)
					{
						outputPixels[regions.Item2.pixelIndexes[i]] = firstCol.B;
						outputPixels[regions.Item2.pixelIndexes[i] + 1] = firstCol.G;
						outputPixels[regions.Item2.pixelIndexes[i] + 2] = firstCol.R;
						outputPixels[regions.Item2.pixelIndexes[i] + 3] = firstCol.A;
					}
					else
					{
						switch (ColourManipulationData.backgroundInfo2.colouringMethod)
						{
							case ColouringMethod.Constant:
								secondCol = ColourManipulationData.backgroundInfo2.firstColour;
								break;
							case ColouringMethod.XCoordinate:
								int x = (regions.Item2.pixelIndexes[i] % (int)(areaSize.X * bytesPerPixel)) / bytesPerPixel;
								amountThrough = (float)x / areaSize.X;
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.B)));
								break;
							case ColouringMethod.YCoordinate:
								int y = regions.Item2.pixelIndexes[i] / (int)(areaSize.X * bytesPerPixel);
								amountThrough = (float)y / areaSize.Y;
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.backgroundInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.backgroundInfo2.secondColour.B)));
								break;
						}
						outputPixels[regions.Item2.pixelIndexes[i]] = (byte)((firstCol.B + secondCol.B) / 2);
						outputPixels[regions.Item2.pixelIndexes[i] + 1] = (byte)((firstCol.G + secondCol.G) / 2);
						outputPixels[regions.Item2.pixelIndexes[i] + 2] = (byte)((firstCol.R + secondCol.R) / 2);
						outputPixels[regions.Item2.pixelIndexes[i] + 3] = (byte)((firstCol.A + secondCol.A) / 2);
					}
				}
			}
			#endregion

			//Do Colouring of regions
			#region RegionColouring
			for (int i = 0; i < regions.Item1.Count; i++)
			{
				Color firstCol = Color.White;
				Color secondCol = Color.White;
				bool useSecondCol = true;
				float amountThrough = 0f;
				if (ColourManipulationData.regionInfo2.colouringMethod == ColouringMethod.NoneSelected) useSecondCol = false;
				switch (ColourManipulationData.regionInfo1.colouringMethod)
				{
					case ColouringMethod.Constant:
						firstCol = ColourManipulationData.regionInfo1.firstColour;
						break;
					case ColouringMethod.XCoordinate:
						int x = (int)points[regions.Item1[i].basePoint].X;
						amountThrough = (float)x / areaSize.X;
						firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.B)));
						break;
					case ColouringMethod.YCoordinate:
						int y = (int)points[regions.Item1[i].basePoint].Y;
						amountThrough = (float)y / areaSize.Y;
						firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.B)));
						break;
					case ColouringMethod.OrderPlaced:
						amountThrough = (float)i / (float)regions.Item1.Count;
						firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.B)));
						break;
					case ColouringMethod.RandomOnGrad:
						amountThrough = (float)getRandom.NextDouble();
						firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.B)));
						break;
					case ColouringMethod.TrueRandom:
						firstCol = Color.FromArgb(getRandom.Next(0, 256), getRandom.Next(0, 256), getRandom.Next(0, 256));
						break;
					case ColouringMethod.Size:
						if (differentSizeRegions)
						{
							amountThrough = (float)(regions.Item1[i].Size - smallestRegionSize) / (float)(largestRegionSize - smallestRegionSize);
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo1.secondColour.B)));
						}
						else
						{
							firstCol = Color.FromArgb((ColourManipulationData.regionInfo1.firstColour.A + ColourManipulationData.regionInfo1.secondColour.A) / 2, (ColourManipulationData.regionInfo1.firstColour.R + ColourManipulationData.regionInfo1.secondColour.R) / 2, (ColourManipulationData.regionInfo1.firstColour.G + ColourManipulationData.regionInfo1.secondColour.G) / 2, (ColourManipulationData.regionInfo1.firstColour.B + ColourManipulationData.regionInfo1.secondColour.B) / 2);
						}
						break;
				}
				if (!useSecondCol)
				{
					regions.Item1[i].regionColor = firstCol;
				}
				else
				{
					switch (ColourManipulationData.regionInfo2.colouringMethod)
					{
						case ColouringMethod.Constant:
							secondCol = ColourManipulationData.regionInfo2.firstColour;
							break;
						case ColouringMethod.XCoordinate:
							int x = (int)points[regions.Item1[i].basePoint].X;
							amountThrough = (float)x / areaSize.X;
							secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.B)));
							break;
						case ColouringMethod.YCoordinate:
							int y = (int)points[regions.Item1[i].basePoint].Y;
							amountThrough = (float)y / areaSize.Y;
							secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.B)));
							break;
						case ColouringMethod.OrderPlaced:
							amountThrough = (float)i / (float)regions.Item1.Count;
							secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.B)));
							break;
						case ColouringMethod.RandomOnGrad:
							amountThrough = (float)getRandom.NextDouble();
							secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.B)));
							break;
						case ColouringMethod.TrueRandom:
							secondCol = Color.FromArgb(getRandom.Next(0, 256), getRandom.Next(0, 256), getRandom.Next(0, 256));
							break;
						case ColouringMethod.Size:
							if (differentSizeRegions)
							{
								amountThrough = (float)(regions.Item1[i].Size - smallestRegionSize) / (float)(largestRegionSize - smallestRegionSize);
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.regionInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.regionInfo2.secondColour.B)));
							}
							else
							{
								secondCol = Color.FromArgb((ColourManipulationData.regionInfo2.firstColour.A + ColourManipulationData.regionInfo2.secondColour.A) / 2, (ColourManipulationData.regionInfo2.firstColour.R + ColourManipulationData.regionInfo2.secondColour.R) / 2, (ColourManipulationData.regionInfo2.firstColour.G + ColourManipulationData.regionInfo2.secondColour.G) / 2, (ColourManipulationData.regionInfo2.firstColour.B + ColourManipulationData.regionInfo2.secondColour.B) / 2);
							}
							break;
					}
					regions.Item1[i].regionColor = Color.FromArgb(((firstCol.A + secondCol.A) / 2), ((firstCol.R + secondCol.R) / 2), ((firstCol.G + secondCol.G) / 2), ((firstCol.B + secondCol.B) / 2));
				}
				for (int j = 0; j < regions.Item1[i].pixelIndexes.Count; j++)
				{
					outputPixels[regions.Item1[i].pixelIndexes[j]] = (byte)regions.Item1[i].regionColor.B;
					outputPixels[regions.Item1[i].pixelIndexes[j] + 1] = (byte)regions.Item1[i].regionColor.G;
					outputPixels[regions.Item1[i].pixelIndexes[j] + 2] = (byte)regions.Item1[i].regionColor.R;
					outputPixels[regions.Item1[i].pixelIndexes[j] + 3] = (byte)regions.Item1[i].regionColor.A;
				}
			}
			#endregion

			//Do the colouring of the points if they are meant to be different
			#region PointColouring
			if (ColourManipulationData.PointsDifferent)
			{
				for (int i = 0; i < points.Count; i++)
				{
					Color firstCol = Color.White;
					Color secondCol = Color.White;
					bool useSecondCol = true;
					float amountThrough = 0f;
					if (ColourManipulationData.pointsInfo2.colouringMethod == ColouringMethod.NoneSelected) useSecondCol = false;
					switch (ColourManipulationData.pointsInfo1.colouringMethod)
					{
						case ColouringMethod.Constant:
							firstCol = ColourManipulationData.pointsInfo1.firstColour;
							break;
						case ColouringMethod.XCoordinate:
							int x = (int)points[i].X;
							amountThrough = (float)x / areaSize.X;
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.B)));
							break;
						case ColouringMethod.YCoordinate:
							int y = (int)points[i].Y;
							amountThrough = (float)y / areaSize.Y;
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.B)));
							break;
						case ColouringMethod.OrderPlaced:
							amountThrough = (float)i / (float)regions.Item1.Count;
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.B)));
							break;
						case ColouringMethod.RandomOnGrad:
							amountThrough = (float)getRandom.NextDouble();
							firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.B)));
							break;
						case ColouringMethod.TrueRandom:
							firstCol = Color.FromArgb(getRandom.Next(0, 256), getRandom.Next(0, 256), getRandom.Next(0, 256));
							break;
						case ColouringMethod.Size:
							if (differentSizeRegions)
							{
								amountThrough = (float)(regions.Item1[i].Size - smallestRegionSize) / (float)(largestRegionSize - smallestRegionSize);
								firstCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo1.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo1.secondColour.B)));
							}
							else
							{
								firstCol = Color.FromArgb((ColourManipulationData.pointsInfo1.firstColour.A + ColourManipulationData.pointsInfo1.secondColour.A) / 2, (ColourManipulationData.pointsInfo1.firstColour.R + ColourManipulationData.pointsInfo1.secondColour.R) / 2, (ColourManipulationData.pointsInfo1.firstColour.G + ColourManipulationData.pointsInfo1.secondColour.G) / 2, (ColourManipulationData.pointsInfo1.firstColour.B + ColourManipulationData.pointsInfo1.secondColour.B) / 2);
							}
							break;
					}
					if (!useSecondCol)
					{
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * (int)areaSize.X)] = firstCol.B;
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * (int)areaSize.X) + 1] = firstCol.G;
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * (int)areaSize.X) + 2] = firstCol.R;
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * (int)areaSize.X) + 3] = firstCol.A;
					}
					else
					{
						switch (ColourManipulationData.pointsInfo2.colouringMethod)
						{
							case ColouringMethod.Constant:
								secondCol = ColourManipulationData.pointsInfo2.secondColour;
								break;
							case ColouringMethod.XCoordinate:
								int x = (int)points[i].X;
								amountThrough = (float)x / areaSize.X;
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.B)));
								break;
							case ColouringMethod.YCoordinate:
								int y = (int)points[i].Y;
								amountThrough = (float)y / areaSize.Y;
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.B)));
								break;
							case ColouringMethod.OrderPlaced:
								amountThrough = (float)i / (float)regions.Item1.Count;
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.B)));
								break;
							case ColouringMethod.RandomOnGrad:
								amountThrough = (float)getRandom.NextDouble();
								secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.B)));
								break;
							case ColouringMethod.TrueRandom:
								secondCol = Color.FromArgb(getRandom.Next(0, 256), getRandom.Next(0, 256), getRandom.Next(0, 256));
								break;
							case ColouringMethod.Size:
								if (differentSizeRegions)
								{
									amountThrough = (float)(regions.Item1[i].Size - smallestRegionSize) / (float)(largestRegionSize - smallestRegionSize);
									secondCol = Color.FromArgb((int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.A) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.A)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.R) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.R)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.G) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.G)), (int)(((1f - amountThrough) * (float)ColourManipulationData.pointsInfo2.firstColour.B) + (amountThrough * (float)ColourManipulationData.pointsInfo2.secondColour.B)));
								}
								else
								{
									secondCol = Color.FromArgb((ColourManipulationData.pointsInfo2.firstColour.A + ColourManipulationData.pointsInfo2.secondColour.A) / 2, (ColourManipulationData.pointsInfo2.firstColour.R + ColourManipulationData.pointsInfo2.secondColour.R) / 2, (ColourManipulationData.pointsInfo2.firstColour.G + ColourManipulationData.pointsInfo2.secondColour.G) / 2, (ColourManipulationData.pointsInfo2.firstColour.B + ColourManipulationData.pointsInfo2.secondColour.B) / 2);
								}
								break;
						}

						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * areaSize.X)] = (byte)((firstCol.B + secondCol.B) / 2);
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * areaSize.X) + 1] = (byte)((firstCol.G + secondCol.G) / 2);
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * areaSize.X) + 2] = (byte)((firstCol.R + secondCol.R) / 2);
						outputPixels[(int)((int)points[i].X * bytesPerPixel + (int)points[i].Y * bytesPerPixel * areaSize.X) + 3] = (byte)((firstCol.A + secondCol.A) / 2);
					}
				}
			}

			#endregion			
			Marshal.Copy(outputPixels, 0, firstOutputPixelPtr, outputPixels.Length);
			outputImage.UnlockBits(outputFileData);
            //Testing
            //for (int i = 0; i < points.Count; i++)
            //{
            //    outputImage.SetPixel((int)points[i].X, (int)points[i].Y, Color.Red);
            //}
			currentlyLockedOutput = false;
			DisplayOutputImage();
		}

		private void Voronoi_GenerateRegions_Button_Click(object sender, RoutedEventArgs e)
		{
			//This is where the code to actaully create the regions from the points should execute
			if(points == null)
			{
				MessageBox.Show("There are no generated points", "No Points", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			else if(points.Count == 0)
			{
				MessageBox.Show("There are no generated points", "No Points", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			float borderWidth = 0f;
			if(!float.TryParse(Voronoi_BorderWidth_Textbox.Text,out borderWidth))
			{
				MessageBox.Show("Invalid Border Width", "Invalid Border Width", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if(borderWidth < 0f)
			{
				MessageBox.Show("Invalid Border Width", "Invalid Border Width", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			//Now I need to run the generator
			voronoiPattern = new Voronoi(points, areaSize, radius, bytesPerPixel);
			regions = voronoiPattern.MakeRegions(borderWidth, (float)Voronoi_PValue_Slider.Value, pointsGeneratedRandomly);
		}

        private void Voronoi_SaveOutput_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyLockedOutput)
            {
                MessageBox.Show("The output image is currently locked.  Please try saving again later.", "Output Image Locked", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string OutputFilename = OutputFilenameTextBox.Text;
                try
                {
                    if (OutputFilename == "")
                    {
                        //They have not set a file to save the image to so we will just save it to a default file
                        outputImage.Save(inputImagePath + "example.png", ImageFormat.Png);
                    }
                    else
                    {
                        outputImage.Save(inputImagePath + OutputFilename);
                    }
                }
                catch
                {
                    MessageBox.Show("There was an issue saving the output image", "Output Image Saving", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        #endregion


        #region Blurring
        BlurringType blurringMethod = BlurringType.NotSelected;
        bool isEllipse = false;
        int areaWidth = 10;
        int areaHeight = 10;
        int distanceOffLeft = 300;
        int distanceOffBottom = 300;
        Ellipse areaEllipse = new Ellipse();
        System.Windows.Shapes.Rectangle areaRectangle = new System.Windows.Shapes.Rectangle();
		string inputKernelPath;
		string inputKernelFileName;
        private void Blurring_InputFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyLockedInput)
            {
                MessageBox.Show("Please wait for the image to be finished processed", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
			try
			{
				// Create OpenFileDialog 
				Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
				// Set filter for file extension and default file extension 
				dlg.DefaultExt = ".png";
				dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
				// Display OpenFileDialog by calling ShowDialog method 
				Nullable<bool> result = dlg.ShowDialog();
				// Get the selected file name and display in a TextBox 
				if (result == true)
				{
					// Open document 
					string filename = dlg.FileName;
					string[] pathParts = filename.Split('\\');
					inputImageFileName = pathParts[pathParts.Length - 1];
					inputImagePath = "";
					for (int i = 0; i < pathParts.Length - 1; i++)
					{
						inputImagePath += pathParts[i] + "\\";
					}
					Blurring_InputFileName_Textbox.Text = filename;
					inputImage = new Bitmap(Bitmap.FromFile(inputImagePath + inputImageFileName));
				}

				Blurring_ShowOrginal_Button_Click(this, new RoutedEventArgs());
			}
			catch (System.OutOfMemoryException)
			{
				MessageBox.Show("There was an error with the file you selected.", "Selected File Issue", MessageBoxButton.OK, MessageBoxImage.Error);
				inputImageFileName = "";
				inputImagePath = "";
				Blurring_InputFileName_Textbox.Text = "Example.png";
			}
        }

        private void Blurring_Method_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox sentCombo = sender as ComboBox;
            if (sentCombo.SelectedIndex == 0) blurringMethod = BlurringType.Mean;
            else if (sentCombo.SelectedIndex == 1) blurringMethod = BlurringType.Gaussian;
            else if (sentCombo.SelectedIndex == 2) blurringMethod = BlurringType.Bokeh;
            else if (sentCombo.SelectedIndex == 3) blurringMethod = BlurringType.Shape;

            if(blurringMethod == BlurringType.Shape)
            {
                //Then we need to enable the correct UI elements
                Blurring_InputKernel_Label.IsEnabled = true;
                Blurring_InputKernel_Textbox.IsEnabled = true;
                Blurring_InputKernel_Button.IsEnabled = true;
                Blurring_Radius_Label.IsEnabled = false;
                Blurring_Radius_Textbox.IsEnabled = false;
            }
            else
            {
                Blurring_InputKernel_Label.IsEnabled = false;
                Blurring_InputKernel_Textbox.IsEnabled = false;
                Blurring_InputKernel_Button.IsEnabled = false;
                Blurring_Radius_Label.IsEnabled = true;
                Blurring_Radius_Textbox.IsEnabled = true;
            }
        }

        private void Blurring_ShowNew_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayOutputImage();
        }

        private void Blurring_ShowOrginal_Button_Click(object sender, RoutedEventArgs e)
        {
            DisplayInputImage();
        }

        private void Blurring_StartBlurring_Click(object sender, RoutedEventArgs e)
        {
			if(inputImage == null)
			{
				MessageBox.Show("There is not selected input image", "Input Image", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			bool wholeImage = (bool)Blurring_WholeImage_RadioButton.IsChecked;
			string power = Blurring_Power_Textbox.Text;
			string radius = Blurring_Radius_Textbox.Text;
			bool isEllipse  = (bool)Blurring_IsEllipse_Checkbox.IsChecked;
			bool isInverted = (bool)Blurring_IsInverted_Checkbox.IsChecked;
			Blur.Rectangle areaCovered = new Blur.Rectangle();
            #region CalculateRectangle
            
            int topLeftX = distanceOffLeft;
            int topLeftY = 600 - (distanceOffBottom + areaHeight);
            int bottomRightX = distanceOffLeft + areaWidth;
            int bottomRightY = 600 - distanceOffBottom;

            double topRowY = 0;
			double leftColumnX = 0;

            if (inputImage.Width > inputImage.Height)
            {
                topRowY = (double)(600d - ImageDisplay.ActualHeight) / 2d;
            }
            else if (inputImage.Height > inputImage.Width)
            {
                leftColumnX = (double)(600d - ImageDisplay.ActualWidth) / 2d;
            }
            else
            {
            }
            topLeftX = (int)Math.Floor((topLeftX - leftColumnX) * (inputImage.Width / ImageDisplay.ActualWidth));
            topLeftY = (int)Math.Floor((topLeftY - topRowY) * (inputImage.Height / ImageDisplay.ActualHeight));
            bottomRightX = (int)Math.Floor((bottomRightX - leftColumnX) * (inputImage.Width / ImageDisplay.ActualWidth));
            bottomRightY = (int)Math.Floor((bottomRightY - topRowY) * (inputImage.Height / ImageDisplay.ActualHeight));
            areaCovered.topLeft = new Vector2(topLeftX, topLeftY);
			areaCovered.bottomRight = new Vector2(bottomRightX, bottomRightY);
			#endregion
			string outputFileName = OutputFilenameTextBox.Text;
			Task.Run(() => Blurring(wholeImage, power, radius, isEllipse, isInverted, areaCovered, outputFileName));
		}

		private void Blurring(bool WholeImage, string Power, string Radius, bool IsEllipse, bool IsInverted, Blur.Rectangle AreaCovered, string OutputFilename)
		{
			if (currentlyLockedInput || currentlyLockedOutput)
			{
				MessageBox.Show("Either the input or output image is not currently available", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (blurringMethod == BlurringType.NotSelected)
			{
				MessageBox.Show("There is no method for blurring selected", "Blurring Method", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			int[,] kernel = new int[0,0];
			float radius = 0;
			float power = 0f;
			//Now need to set all the parameters for blurring
			if (WholeImage)
			{
				AreaCovered = new Blur.Rectangle(new Vector2(0, 0), new Vector2(inputImage.Width, inputImage.Height));
				IsInverted = false;
				IsEllipse = false;
			}
			if(blurringMethod == BlurringType.Shape)
			{
				//Need to check and make the kernel
				if (!File.Exists(inputKernelPath + inputKernelFileName))
				{
					//The chosen file no longer exists
					MessageBox.Show("The selected image for the kernel no longer exists", "Kernel Image", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				else
				{
					Bitmap inputKernelImage = new Bitmap(Bitmap.FromFile(inputKernelPath + inputKernelFileName));
					if (inputKernelImage.Width % 2 == 0 || inputKernelImage.Height % 2 == 0)
					{
						MessageBox.Show("The input kernel is not of correct dimensions, no center pixel.", "Input Kernel Size", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					kernel = new int[inputKernelImage.Width, inputKernelImage.Height];
					for (int i = 0; i < inputKernelImage.Width; i++)
					{
						for (int j = 0; j < inputKernelImage.Height; j++)
						{
							kernel[kernel.GetLength(0) - i - 1,kernel.GetLength(1) - j - 1] = inputKernelImage.GetPixel(i, j).R - inputKernelImage.GetPixel(i, j).G;
						}
					}
				}
			}
			else
			{
				//Need to check that the input for the radius
				if(!float.TryParse(Radius, out radius))
				{
					MessageBox.Show("The input radius is not a valid number", "Kernel Radius", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if(radius <= 0)
				{
					MessageBox.Show("The input radius is not a valid number", "Kernel Radius", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if(radius < 0.5f && blurringMethod == BlurringType.Gaussian)
				{
					MessageBox.Show("The input standard deviation is not valid", "Kernel Radius", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
				if(radius < 2 && blurringMethod == BlurringType.Bokeh)
				{
					MessageBox.Show("The input standard deviation is not valid", "Kernel Radius", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}
			}
			//Need to check that the input for the radius
			if (!float.TryParse(Power, out power))
			{
				MessageBox.Show("The input power is not a valid number", "Gamma Power", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			if (power < 1 || power > 3)
			{
				MessageBox.Show("The input power is not a valid number", "Gamma Power", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			IBlur Blurrer = new LinearBlur();
			if (blurringMethod == BlurringType.Mean) Blurrer = new LinearBlur();
			else if (blurringMethod == BlurringType.Gaussian) Blurrer = new GaussianBlur();
			else if (blurringMethod == BlurringType.Bokeh) Blurrer = new BokehBlur();
			else if (blurringMethod == BlurringType.Shape) Blurrer = new ShapeBlur();


			//Now we need to get all the data out of the bitmap to be passed to the resizer
			currentlyLockedInput = true;
			//Generate the required data from the input image
			BitmapData inputFileData = inputImage.LockBits(new System.Drawing.Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, inputImage.PixelFormat);
			int bytesPerPixel = Bitmap.GetPixelFormatSize(inputImage.PixelFormat) / 8;
			int inputByteCount = inputFileData.Stride * inputImage.Height;
			byte[] inputPixels = new byte[inputByteCount];
			IntPtr firstInputPixelPtr = inputFileData.Scan0;
			Marshal.Copy(firstInputPixelPtr, inputPixels, 0, inputPixels.Length);

			currentlyLockedOutput = true;
			//Generate the required data for the output image
			outputImage = new Bitmap(inputImage.Width, inputImage.Height);
			BitmapData outputFileData = outputImage.LockBits(new System.Drawing.Rectangle(0, 0, inputImage.Width, inputImage.Height), ImageLockMode.ReadWrite, inputImage.PixelFormat);
			int outputByteCount = outputFileData.Stride * outputImage.Height;
			byte[] outputPixels = new byte[outputByteCount];
			IntPtr firstOutputPixelPtr = outputFileData.Scan0;

			outputPixels = Blurrer.Blur(inputPixels, inputFileData, radius, power, kernel, AreaCovered, IsEllipse, IsInverted);

			Marshal.Copy(inputPixels, 0, firstInputPixelPtr, inputPixels.Length);
			inputImage.UnlockBits(inputFileData);
			currentlyLockedInput = false;
			Marshal.Copy(outputPixels, 0, firstOutputPixelPtr, outputPixels.Length);
            outputImage.UnlockBits(outputFileData);
			try
			{
				if (OutputFilename == "")
				{
					//They have not set a file to save the image to so we will just save it to a default file
					outputImage.Save(inputImagePath + "example.png", ImageFormat.Png);
				}
				else
				{
					outputImage.Save(inputImagePath + OutputFilename);
				}
			}
			catch
			{
				MessageBox.Show("There was an issue saving the output image", "Output Image Saving", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			currentlyLockedOutput = false;
            // From Stack Overflow https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            this.Dispatcher.Invoke(() => { Blurring_ShowNew_Button_Click(this, new RoutedEventArgs()); });

		}
		#region MovementControls
		private void Blurring_PartOfImage_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            //This means that we only want part of the image to be blurred
            Blurring_IsEllipse_Checkbox_Click(null, null);
        }

        private void Blurring_IsEllipse_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)Blurring_IsEllipse_Checkbox.IsChecked)
            {
                areaEllipse.Visibility = Visibility.Visible;
                areaRectangle.Visibility = Visibility.Hidden;
            }
            else
            {
                areaEllipse.Visibility = Visibility.Hidden;
                areaRectangle.Visibility = Visibility.Visible;
            }
        }

        private void Blurring_MoveLeft_Button_Click(object sender, RoutedEventArgs e)
        {
            //Need to check if pressing shift and control to move 50
            //If just shift then move 10
            //If just control then move 5
            //Else just move 1

            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) distanceOffLeft -= 50;
            else if (isShiftPressed) distanceOffLeft -= 10;
            else if (isControlPressed) distanceOffLeft -= 5;
            else distanceOffLeft--;

            Canvas.SetLeft(areaEllipse, distanceOffLeft);
            Canvas.SetLeft(areaRectangle, distanceOffLeft);
        }

        private void Blurring_MoveRight_Button_Click(object sender, RoutedEventArgs e)
        {
            //Need to check if pressing shift and control to move 50
            //If just shift then move 10
            //If just control then move 5
            //Else just move 1

            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) distanceOffLeft += 50;
            else if (isShiftPressed) distanceOffLeft += 10;
            else if (isControlPressed) distanceOffLeft += 5;
            else distanceOffLeft++;

            Canvas.SetLeft(areaEllipse, distanceOffLeft);
            Canvas.SetLeft(areaRectangle, distanceOffLeft);
        }

        private void Blurring_MoveUp_Button_Click(object sender, RoutedEventArgs e)
        {
            //Need to check if pressing shift and control to move 50
            //If just shift then move 10
            //If just control then move 5
            //Else just move 1

            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) distanceOffBottom += 50;
            else if (isShiftPressed) distanceOffBottom += 10;
            else if (isControlPressed) distanceOffBottom += 5;
            else distanceOffBottom++;

            Canvas.SetBottom(areaEllipse, distanceOffBottom);
            Canvas.SetBottom(areaRectangle, distanceOffBottom);
        }


        private void Blurring_MoveDown_Button_Click(object sender, RoutedEventArgs e)
        {
            //Need to check if pressing shift and control to move 50
            //If just shift then move 10
            //If just control then move 5
            //Else just move 1

            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) distanceOffBottom -= 50;
            else if (isShiftPressed) distanceOffBottom -= 10;
            else if (isControlPressed) distanceOffBottom -= 5;
            else distanceOffBottom--;

            Canvas.SetBottom(areaEllipse, distanceOffBottom);
            Canvas.SetBottom(areaRectangle, distanceOffBottom);
        }

        private void Blurring_WidthDown_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) areaWidth -= 50;
            else if (isShiftPressed) areaWidth -= 10;
            else if (isControlPressed) areaWidth -= 5;
            else areaWidth--;
            if (areaWidth <= 0) areaWidth = 1;
            areaEllipse.Width = areaWidth;
            areaRectangle.Width = areaWidth;
        }

        private void Blurring_WidthUp_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) areaWidth += 50;
            else if (isShiftPressed) areaWidth += 10;
            else if (isControlPressed) areaWidth += 5;
            else areaWidth++;
            areaEllipse.Width = areaWidth;
            areaRectangle.Width = areaWidth;
        }

        private void Blurring_HeightDown_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) areaHeight -= 50;
            else if (isShiftPressed) areaHeight -= 10;
            else if (isControlPressed) areaHeight -= 5;
            else areaHeight--;
            if (areaHeight <= 0) areaHeight = 1;
            areaEllipse.Height = areaHeight;
            areaRectangle.Height = areaHeight;
        }

        private void Blurring_HeightUp_Button_Click(object sender, RoutedEventArgs e)
        {
            bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool isControlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
            if (isShiftPressed && isControlPressed) areaHeight += 50;
            else if (isShiftPressed) areaHeight += 10;
            else if (isControlPressed) areaHeight += 5;
            else areaHeight++;
            areaEllipse.Height = areaHeight;
            areaRectangle.Height = areaHeight;
        }
		#endregion
		private void Blurring_WholeImage_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            areaEllipse.Visibility = Visibility.Hidden;
            areaRectangle.Visibility = Visibility.Hidden;
        }
     
        private void Blurring_InputKernel_Button_Click(object sender, RoutedEventArgs e)
        {
			// Create OpenFileDialog 
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			// Set filter for file extension and default file extension 
			dlg.DefaultExt = ".png";
			dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
			// Display OpenFileDialog by calling ShowDialog method 
			Nullable<bool> result = dlg.ShowDialog();
			// Get the selected file name and display in a TextBox 
			if (result == true)
			{
				// Open document 
				string filename = dlg.FileName;
				string[] pathParts = filename.Split('\\');
				inputKernelFileName = pathParts[pathParts.Length - 1];
				inputKernelPath = "";
				for (int i = 0; i < pathParts.Length - 1; i++)
				{
					inputKernelPath += pathParts[i] + "\\";
				}
				string fileExtension = inputKernelFileName.Substring(inputKernelFileName.LastIndexOf('.'));
				if(fileExtension != ".png" && fileExtension != ".jpg" && fileExtension != ".jpeg")
				{
					//The file is not of valid type so should be rejected
					MessageBox.Show("The file type of input kernel is not valid", "Incorrect File Type", MessageBoxButton.OK, MessageBoxImage.Error);
					inputKernelFileName = "";
					inputKernelPath = "";
					Blurring_InputKernel_Textbox.Text = "Example.png";
				}
				else
				{
					Blurring_InputKernel_Textbox.Text = filename;
				}				
			}
		}


		#endregion

		private void TabItem_LostFocus(object sender, RoutedEventArgs e)
		{
			areaEllipse.Visibility = Visibility.Hidden;
			areaRectangle.Visibility = Visibility.Hidden;
		}

		private void TabItem_GotFocus(object sender, RoutedEventArgs e)
		{
			if (Blurring_PartOfImage_RadioButton.IsChecked == true)
			{
				Blurring_PartOfImage_RadioButton_Checked(null, null);
			}
			else
			{
				areaEllipse.Visibility = Visibility.Hidden;
				areaRectangle.Visibility = Visibility.Hidden;
			}
		}

        private void DisplayOutputImage()
        {
            if (outputImage == null) return;
            if (currentlyLockedOutput)
            {
                MessageBox.Show("The output image is not currently available", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ImageDisplay.Source = BitmapToImageSource(outputImage);
        }
        private void DisplayInputImage()
        {
            if (inputImage == null) return;
            if (currentlyLockedInput)
            {
                MessageBox.Show("The input image is not currently available", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;


            }
            ImageDisplay.Source = BitmapToImageSource(inputImage);
        }

        private void ImageEditingToolHelp_Button_Click(object sender, RoutedEventArgs e)
        {
            Window helpWindow = new HelpWindow("https://tbeakl.github.io/ALevelComputingProjectHelp/");
            helpWindow.Show();
        }
    }
}