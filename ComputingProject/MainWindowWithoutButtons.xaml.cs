using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using ComputingProject.Resizing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ComputingProject.PatternGeneration.Point_Generators;
using System.Numerics;
using ComputingProject.PatternGeneration.Voronoi;
using ComputingProject.Blur;
using Brushes = System.Windows.Media.Brushes;

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
            //areaEllipse.Visibility = Visibility.Visible;
            areaEllipse.Height = areaHeight;
            areaEllipse.Width = areaWidth;
            areaEllipse.Fill = Brushes.Red;

            areaRectangle.IsEnabled = true;
            //areaRectangle.Visibility = Visibility.Visible;
            areaRectangle.Height = areaHeight;
            areaRectangle.Width = areaWidth;
            areaRectangle.Fill = Brushes.Red;

            Canvas.SetLeft(areaEllipse, distanceOffLeft);
            Canvas.SetBottom(areaEllipse, distanceOffBottom);
            Canvas.SetLeft(areaRectangle, distanceOffLeft);
            Canvas.SetBottom(areaRectangle, distanceOffBottom);
            //areaEllipse.Fill = new System.Windows.Media.Brush();

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
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
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
                Enlarging_InputFileName_Textbox.Text = filename;
                inputImage = new Bitmap(Bitmap.FromFile(inputImagePath + inputImageFileName));

                Enlarging_InputFileWidth_Textbox.Text = inputImage.Width.ToString();
                Enlarging_InputFileHeight_Textbox.Text = inputImage.Height.ToString();
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
                }
                else
                {
                    scaleFactor = Math.Abs(scaleFactor);
                    newWidth = (int)scaleFactor * inputImage.Width;
                    newHeight = (int)scaleFactor * inputImage.Height;
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

        }

        #endregion

        private void DisplayOutputImage()
        {
            if (outputImage == null) return;
            if(currentlyLockedOutput)
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


        #region Voronoi
        PointGeneratorType generatorType = PointGeneratorType.NonSelected;
        IPointGenerator generator;
        Vector2 regionSize;
        List<Vector2> points;
        float radius;
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
            regionSize = new Vector2(width, height);
        }

        private void GeneratePoints(string Parameter1Str, string Parameter2Str)
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
                if (!float.TryParse(Parameter1Str, out parameter1))
                {
                    MessageBox.Show("The number of points to generate is not a valid number", "Num. of Points", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter1 <= 0)
                {
                    MessageBox.Show("The number of points to generate is not a valid number", "Num. of Points", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
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

                if (!float.TryParse(Parameter2Str, out parameter2))
                {
                    MessageBox.Show("The number of tries per point is not a valid number", "Num. of Tries", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (parameter2 <= 0)
                {
                    MessageBox.Show("The number of tries per point is not a valid number", "Num. of Tries", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                generator = new PoissonDiscGenerator();
            }
            //All the parameters are correct so we now need to generate the point array
            points = generator.GeneratePoints(regionSize, parameter1, parameter2);
            if(generatorType == PointGeneratorType.Random)
            {
                radius = Voronoi.calculateRadius(points, true);
            }
            else
            {
                radius = parameter1;
            }


            currentlyLockedOutput = true;
            //Generate the required data for the output image
            outputImage = new Bitmap((int)regionSize.X, (int)regionSize.Y);
            BitmapData outputFileData = outputImage.LockBits(new System.Drawing.Rectangle(0, 0, (int)regionSize.X, (int)regionSize.Y), ImageLockMode.ReadWrite, inputImage.PixelFormat);
            int outputByteCount = outputFileData.Stride * outputImage.Height;
            byte[] outputPixels = new byte[outputByteCount];
            IntPtr firstOutputPixelPtr = outputFileData.Scan0;
#warning Still need to make this display them but it will need to change depending on what colour is selected 
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
            }

            if(generatorType == PointGeneratorType.Random)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Num. of Points:";
                Voronoi_PointGenerator_Parameter2_Label.IsEnabled = false;
                Voronoi_PointGenerator_Parameter2_Textbox.IsEnabled = false;
            }
            else if(generatorType == PointGeneratorType.Uniform)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Radius:";
                Voronoi_PointGenerator_Parameter2_Label.Content = "Offset:";
            }
            else if(generatorType == PointGeneratorType.PoissonDisc)
            {
                Voronoi_PointGenerator_Parameter1_Label.Content = "Radius:";
                Voronoi_PointGenerator_Parameter2_Label.Content = "Tries Bef. Rejc.:";
                Voronoi_PointGenerator_Parameter2_Textbox.Text = "30";
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

        private void Blurring_InputFile_Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentlyLockedInput)
            {
                MessageBox.Show("Please wait for the image to be finished processed", "Locked Image Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
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

        }

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
            //If just control then move 50
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
            //If just control then move 50
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
            //If just control then move 50
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
            //If just control then move 50
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

        private void Blurring_WholeImage_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            areaEllipse.Visibility = Visibility.Hidden;
            areaRectangle.Visibility = Visibility.Hidden;
        }
     
        private void Blurring_InputKernel_Button_Click(object sender, RoutedEventArgs e)
        {

        }


        #endregion

    }
}
