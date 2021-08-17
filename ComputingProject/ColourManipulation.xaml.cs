using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using SDColor = System.Drawing.Color;
using SWColor = System.Windows.Media.Color;

namespace ComputingProject
{
    /// <summary>
    /// Interaction logic for ColourManipulation.xaml
    /// </summary>
    public partial class ColourManipulation : Window
    {
		private MainWindow mainWindow;
		private SWColor SDColtoSWCol(SDColor input)
		{
			return SWColor.FromArgb(input.A, input.R, input.G, input.B);
		}
        public ColourManipulation(MainWindow mainWindow)
        {
            InitializeComponent();
			this.mainWindow = mainWindow;
			//Need to fill it in with the information contained within the Data File
			ColourManipulationPoint_PointsDifferent_Checkbox.IsChecked = ColourManipulationData.PointsDifferent;
			ColourManipulationPoint_Colour1PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.pointsInfo1.firstColour);
			ColourManipulationPoint_Colour2PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.pointsInfo1.secondColour);
			ColourManipulationPoint_Colour1PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.pointsInfo2.firstColour);
			ColourManipulationPoint_Colour2PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.pointsInfo2.secondColour);
			ColourManipulationBackground_Colour1PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.backgroundInfo1.firstColour);
			ColourManipulationBackground_Colour2PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.backgroundInfo1.secondColour);
			ColourManipulationBackground_Colour1PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.backgroundInfo2.firstColour);
			ColourManipulationBackground_Colour2PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.backgroundInfo2.secondColour);
			ColourManipulationRegion_Colour1PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.regionInfo1.firstColour);
			ColourManipulationRegion_Colour2PickerMethod1.SelectedColor = SDColtoSWCol(ColourManipulationData.regionInfo1.secondColour);
			ColourManipulationRegion_Colour1PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.regionInfo2.firstColour);
			ColourManipulationRegion_Colour2PickerMethod2.SelectedColor = SDColtoSWCol(ColourManipulationData.regionInfo2.secondColour);
			ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex = (int) ColourManipulationData.pointsInfo1.colouringMethod;
			ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex = (int)ColourManipulationData.pointsInfo2.colouringMethod;
			ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex = (int)ColourManipulationData.regionInfo1.colouringMethod;
			ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex = (int)ColourManipulationData.regionInfo2.colouringMethod;
			if (ColourManipulationData.backgroundInfo1.colouringMethod == ColouringMethod.XCoordinate) ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex = 0;
			else if (ColourManipulationData.backgroundInfo1.colouringMethod == ColouringMethod.YCoordinate) ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex = 1;
			else ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex = 2;

			if (ColourManipulationData.backgroundInfo2.colouringMethod == ColouringMethod.XCoordinate) ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex = 0;
			else if (ColourManipulationData.backgroundInfo2.colouringMethod == ColouringMethod.YCoordinate) ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex = 1;
			else if (ColourManipulationData.backgroundInfo2.colouringMethod == ColouringMethod.Constant) ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex = 2;
			else ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex = 3;
		}
		private void updateWhetherSeedGenShouldBeChangeable()
		{
			if(ColourManipulationData.regionInfo1.colouringMethod == ColouringMethod.RandomOnGrad || ColourManipulationData.regionInfo1.colouringMethod == ColouringMethod.TrueRandom ||
				ColourManipulationData.regionInfo2.colouringMethod == ColouringMethod.RandomOnGrad || ColourManipulationData.regionInfo2.colouringMethod == ColouringMethod.TrueRandom ||
				ColourManipulationData.pointsInfo1.colouringMethod == ColouringMethod.RandomOnGrad || ColourManipulationData.pointsInfo2.colouringMethod == ColouringMethod.TrueRandom ||
				ColourManipulationData.pointsInfo2.colouringMethod == ColouringMethod.RandomOnGrad || ColourManipulationData.pointsInfo2.colouringMethod == ColouringMethod.TrueRandom)
			{
				mainWindow.Voronoi_Colour_Seed_Label.IsEnabled = true;
				mainWindow.Voronoi_Colour_Seed_Textbox.IsEnabled = true;
			}
			else
			{
				mainWindow.Voronoi_Colour_Seed_Label.IsEnabled = false;
				mainWindow.Voronoi_Colour_Seed_Textbox.IsEnabled = false;
			}
		}
		#region Points
		private void ColourManipulationPoint_PointsDifferent_Checkbox_Click(object sender, RoutedEventArgs e)
        {
            ColourManipulationData.PointsDifferent = (bool)ColourManipulationPoint_PointsDifferent_Checkbox.IsChecked;
        }

		private void ColourManipulationPoint_FirstMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 0) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.OrderPlaced;
            else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 1) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 2) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 3) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.RandomOnGrad;
            else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 4) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.TrueRandom;
            else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 5) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.Constant;
			else if (ColourManipulationPoint_FirstMethod_Combobox.SelectedIndex == 6) ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.Size;
			else ColourManipulationData.pointsInfo1.colouringMethod = ColouringMethod.NoneSelected;
			updateWhetherSeedGenShouldBeChangeable();
		}

        private void ColourManipulationPoint_Colour1PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationPoint_Colour1PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationPoint_Colour1PickerMethod1.SelectedColor.Value;
				ColourManipulationData.pointsInfo1.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
            }
        }

        private void ColourManipulationPoint_Colour2PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationPoint_Colour2PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationPoint_Colour2PickerMethod1.SelectedColor.Value;
				ColourManipulationData.pointsInfo1.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationPoint_SecondMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 0) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.OrderPlaced;
            else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 1) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 2) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 3) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.RandomOnGrad;
            else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 4) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.TrueRandom;
            else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 5) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.Constant;
			else if (ColourManipulationPoint_SecondMethod_Combobox.SelectedIndex == 6) ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.Size;
			else ColourManipulationData.pointsInfo2.colouringMethod = ColouringMethod.NoneSelected;
			updateWhetherSeedGenShouldBeChangeable();
		}

        private void ColourManipulationPoint_Colour1PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationPoint_Colour1PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationPoint_Colour1PickerMethod2.SelectedColor.Value;
				ColourManipulationData.pointsInfo2.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationPoint_Colour2PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationPoint_Colour2PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationPoint_Colour2PickerMethod2.SelectedColor.Value;
				ColourManipulationData.pointsInfo2.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }
		#endregion
		#region Background
		private void ColourManipulationBackground_FirstMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex == 0) ColourManipulationData.backgroundInfo1.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex == 1) ColourManipulationData.backgroundInfo1.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationBackground_FirstMethod_Combobox.SelectedIndex == 2) ColourManipulationData.backgroundInfo1.colouringMethod = ColouringMethod.Constant;
            else ColourManipulationData.backgroundInfo1.colouringMethod = ColouringMethod.NoneSelected;
        }

        private void ColourManipulationBackground_Colour1PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationBackground_Colour1PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationBackground_Colour1PickerMethod1.SelectedColor.Value;
				ColourManipulationData.backgroundInfo1.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationBackground_Colour2PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationBackground_Colour2PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationBackground_Colour2PickerMethod1.SelectedColor.Value;
				ColourManipulationData.backgroundInfo1.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationBackground_SecondMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex == 0) ColourManipulationData.backgroundInfo2.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex ==1) ColourManipulationData.backgroundInfo2.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationBackground_SecondMethod_Combobox.SelectedIndex == 2) ColourManipulationData.backgroundInfo2.colouringMethod = ColouringMethod.Constant;
            else ColourManipulationData.backgroundInfo2.colouringMethod = ColouringMethod.NoneSelected;
        }

        private void ColourManipulationBackground_Colour1PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationBackground_Colour1PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationBackground_Colour1PickerMethod2.SelectedColor.Value;
				ColourManipulationData.backgroundInfo2.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationBackground_Colour2PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationBackground_Colour2PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationBackground_Colour2PickerMethod2.SelectedColor.Value;
				ColourManipulationData.backgroundInfo2.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }
		#endregion
		#region Regions
		private void ColourManipulationRegion_FirstMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 0) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.OrderPlaced;
            else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 1) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 2) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 3) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.RandomOnGrad;
            else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 4) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.TrueRandom;
            else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 5) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.Constant;
			else if (ColourManipulationRegion_FirstMethod_Combobox.SelectedIndex == 6) ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.Size;
			else ColourManipulationData.regionInfo1.colouringMethod = ColouringMethod.NoneSelected;
			updateWhetherSeedGenShouldBeChangeable();
		}

        private void ColourManipulationRegion_Colour1PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationRegion_Colour1PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationRegion_Colour1PickerMethod1.SelectedColor.Value;
				ColourManipulationData.regionInfo1.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationRegion_Colour2PickerMethod1_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationRegion_Colour2PickerMethod1.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationRegion_Colour2PickerMethod1.SelectedColor.Value;
				ColourManipulationData.regionInfo1.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationRegion_SecondMethod_Combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 0) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.OrderPlaced;
            else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 1) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.XCoordinate;
            else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 2) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.YCoordinate;
            else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 3) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.RandomOnGrad;
            else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 4) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.TrueRandom;
            else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 5) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.Constant;
			else if (ColourManipulationRegion_SecondMethod_Combobox.SelectedIndex == 6) ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.Size;
			else ColourManipulationData.regionInfo2.colouringMethod = ColouringMethod.NoneSelected;
			updateWhetherSeedGenShouldBeChangeable();
		}

        private void ColourManipulationRegion_Colour1PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationRegion_Colour1PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationRegion_Colour1PickerMethod2.SelectedColor.Value;
				ColourManipulationData.regionInfo2.firstColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }

        private void ColourManipulationRegion_Colour2PickerMethod2_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (ColourManipulationRegion_Colour2PickerMethod2.SelectedColor.HasValue)
            {
				SWColor tempColour = ColourManipulationRegion_Colour2PickerMethod2.SelectedColor.Value;
				ColourManipulationData.regionInfo2.secondColour = SDColor.FromArgb(tempColour.A, tempColour.R, tempColour.G, tempColour.B);
			}
        }
		#endregion
	}

	public static class ColourManipulationData
    {
        private static bool pointsDifferent = false;
        public static bool PointsDifferent
        {
            get
            {
                return pointsDifferent;
            }
            set
            {
                pointsDifferent = value;
            }
        }

        public static ColouringInformation pointsInfo1 = new ColouringInformation();

        public static ColouringInformation pointsInfo2 = new ColouringInformation();

        public static ColouringInformation backgroundInfo1 = new ColouringInformation();

        public static ColouringInformation backgroundInfo2 = new ColouringInformation();

        public static ColouringInformation regionInfo1 = new ColouringInformation();

        public static ColouringInformation regionInfo2 = new ColouringInformation();
    }

    public class ColouringInformation
    {
        public SDColor firstColour = SDColor.FromArgb(255, 0,0,0);
        public SDColor secondColour = SDColor.FromArgb(255, 0, 0, 0);
        public ColouringMethod colouringMethod = ColouringMethod.Constant;
    }


    public enum ColouringMethod
    {
        OrderPlaced,
        XCoordinate,
        YCoordinate,
        RandomOnGrad,
        TrueRandom,
        Constant,
		Size,
        NoneSelected
    }
}