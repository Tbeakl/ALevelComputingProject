   M S B A M L   `   `   `  ����K  GComputingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ComputingProjectComputingProject  5   	   !    ComputingProject.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=3.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   *localclr-namespace:ComputingProject  5   	     � Title$  ImageEditingTeachingTool��5	   	   $	��800��6*   $
��1000��67    � 
ResizeMode$ NoResize=�6D   .��5
      � -   5         �Name$ ImageBorder��5
      .+�5   
   �� -   5         ��Name$ Title��5      $��50��6#   $��246,10,246,0q�6/   $
��Wrap=�6E   $!��Image Editing Teaching Tool��6Y   $	��Top=�6|   $��36(�6�   $
��Bold"�6�    �� TextAlignment$ Center=�6�   �� 5   
   -   5         ��Name$ PictureCanvas��5      $��
30,119,0,0q�6(   $
��Left=�6<   $	��600��6W   $	��600��6c   $	��Top=�6p   .+�5      �� -   5         ��Name$ ImageDisplay��5       J� SnapsToDevicePixels .  �� BitmapScalingMode$ NearestNeighbor=�6E   	 K� ZIndex$		 -10��6w   $��Uniform=�6�   $��Stretch=�6�   $	��600��6�   $	��600��6�   $��Stretch=�6�   5      �� 5   
   $��
0,119,10,0q�6   $��Right=�6)   $	��327��6E   $	��550��6Q   $	��Top=�6^   .@�5      �� -   5        $��Blurring��5      .��5      � ���	#FFE5E5E5$	��522��6.   $��Bottom=�6;   .+�5      �� -   5        
 ��Name$
 Blurring_Label_Method��5      $��Method:��6;   $
��Left=�6M   $��30��6h   $��	10,10,0,0q�6t   $	��Top=�6�   $	��100��6�   �� 5      -   5         ��Name$ Blurring_Method_ComboBox��5      $
��Left=�6A   $��30��6\   $��
125,10,0,0q�6h   $	��Top=�6|   $	��175��6�    �� 
IsReadOnly . .@�5      �� $��	Mean Blur��6&   �� 5      $��Gaussian Blur��6&   �� 5      $��
Bokeh Blur��6&   �� 5      $��Custom Kernel��6&   5      �� 5      -   5        $
 Blurring_InputFile_Label��5      $��Input File:��6>   $
��Left=�6T   $��30��6o   $��	10,45,0,0q�6{   $	��Top=�6�   $	��100��6�   �� 5      -	   5         ��Name$$ Blurring_InputFileName_Textbox��5      $
��Left=�6F   $��30��6a   $��
125,45,0,0q�6m    �� TextWrapping$
 Wrap=�6�   $��Example.png��6�   $	��Top=�6�   $	��126��6�    J� RenderTransformOrigin$ 0.5,0.5&�6�    �� 
IsReadOnly . 6�   �� 5      -
   5         ��Name$ Blurring_InputFile_Button��5      $��Select��6@   $
��Left=�6Q   $��30��6l   $��
256,45,0,0q�6x   $	��Top=�6�   $��44��6�   6�   �� 5      -   5        $
 Blurring_Power_Label��5      $��Power:��6:   $
��Left=�6K   $��30��6f   $��	10,80,0,0q�6r   $	��Top=�6�   $	��100��6�   �� 5      -   5        $ Blurring_Power_Textbox��5      $
��Left=�6>   $��30��6Y   $��
125,80,0,0q�6e   $
 Wrap=�6y   $��1��6�   $	��Top=�6�   $	��175��6�   �� 5      -   5        $
 Blurring_Radius_Label��5      $��Radius/Std. Dev:��6;   $
��Left=�6V   $��30��6q   $��
10,115,0,0q�6}   $	��Top=�6�   $	��100��6�   {�.  6�   �� 5       -   5        $ Blurring_Radius_Textbox��5       $
��Left=�6?   $��30��6Z   $��125,115,0,0q�6f   $
 Wrap=�6{   $��5��6�   $	��Top=�6�   $	��175��6�   {�.  6�   �� 5!      -   5        $ 
 Blurring_InputKernel_Label��5!      $��Input Kernel:��6@   $
��Left=�6X   $��30��6s   $��
10,150,0,0q�6   $	��Top=�6�   $	��100��6�   {�.  6�   �� 5"      -   5        $" Blurring_InputKernel_Textbox��5"      $
��Left=�6D   $��30��6_   $��125,150,0,0q�6k   $
 Wrap=�6�   $��Example.png��6�   $	��Top=�6�   $	��126��6�   $ 0.5,0.5&�6�    . {�.  6�   �� 5#      -   5        $! Blurring_InputKernel_Button��5#      $��Select��6B   $
��Left=�6S   $��30��6n   $��256,150,0,0q�6z   $	��Top=�6�   $��44��6�   {�.  6�   �� 5$      -   5        $#
 Blurring_WholeImageTick_Label��5$      $��Whole Image:��6C   $
��Left=�6Z   $��30��6u   $��
10,185,0,0q�6�   $	��Top=�6�   $	��100��6�   	� 5%      -   5         	�Name$% Blurring_WholeImage_RadioButton��5%   "   $�� ��6K   $
��Left=�6V   $��21��6q   $��125,193,0,0q�6}   $	��Top=�6�   $��20��6�   $ 
0.95,0.571&�6�    d� 	IsChecked$
 TrueY�6�    � BindingGroup 5&      &   System.Windows.Data.BindingGroup  5'      5)      �� 5*      -   5        $$
 Blurring_PartOfImageTick_Label��5*      $��Area in Image:��6D   $
��Left=�6]   $��30��6x   $��
10,220,0,0q�6�   $	��Top=�6�   $	��100��6�   	� 5+      -   5        $&  Blurring_PartOfImage_RadioButton��5+   "   $�� ��6L   $
��Left=�6W   $��21��6r   $��125,229,0,0q�6~   $	��Top=�6�   $��20��6�   $ 
0.95,0.571&�6�    5,        5-      5/      �� 50      -   5        $
 Blurring_IsEllipse_Label��50      $��Ellipse:��6>   $
��Left=�6Q   $��30��6l   $��
10,255,0,0q�6x   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+ �� ElementName$&  Blurring_PartOfImage_RadioButton���� 51      -   5         ��Name$! Blurring_IsEllipse_Checkbox��51      $�� ��6D   $
��Left=�6O   $��17��6j   $��125,261,0,0q�6v   $	��Top=�6�   $��17��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 52      -   5        $#
 Blurring_MoveHorizontal_Label��52      $��Horizontal:��6C   $
��Left=�6Y   $��30��6t   $��
10,290,0,0q�6�   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton���� 53      -   5        $ Blurring_MoveLeft_Button��53      $	��⮘��6?   $
��Left=�6K   $��30��6f   $��125,290,0,0q�6r   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 54      -   5        $ Blurring_MoveRight_Button��54      $	��⮚��6@   $
��Left=�6L   $��30��6g   $��225,290,0,0q�6s   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 55      -   5        $!
 Blurring_MoveVertical_Label��55      $��	Vertical:��6A   $
��Left=�6U   $��30��6p   $��
10,325,0,0q�6|   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton���� 56      -   5        $ Blurring_MoveUp_Button��56      $	��⮙��6=   $
��Left=�6I   $��30��6d   $��125,325,0,0q�6p   $	��Top=�6�   $��75��6�   $ 0.5,0.5&�6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6  �� 57      -   5        $ Blurring_MoveDown_Button��57      $	��⮛��6?   $
��Left=�6K   $��30��6f   $��225,325,0,0q�6r   $	��Top=�6�   $��75��6�   $ 0.5,0.5&�6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6  �� 58      -   5        $$
 Blurring_ScaleHorizontal_Label��58      $��Width:��6D   $
��Left=�6U   $��30��6p   $��
10,360,0,0q�6|   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton���� 59      -   5        $ Blurring_WidthDown_Button��59      $	��⮘��6@   $
��Left=�6L   $��30��6g   $��125,360,0,0q�6s   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 5:      -    5        $ Blurring_WidthUp_Button��5:      $	��⮚��6>   $
��Left=�6J   $��30��6e   $��225,360,0,0q�6q   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 5;      -!   5        $"
 Blurring_ScaleVertical_Label��5;      $��Height:��6B   $
��Left=�6T   $��30��6o   $��
10,395,0,0q�6{   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton���� 5<      -"   5        $  Blurring_HeightDown_Button��5<      $	��⮛��6A   $
��Left=�6M   $��30��6h   $��125,395,0,0q�6t   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 5=      -#   5        $ Blurring_HeightUp_Button��5=      $	��⮙��6?   $
��Left=�6K   $��30��6f   $��225,395,0,0q�6r   $	��Top=�6�   $��75��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��6�   �� 5>      -$   5        $ Blurring_StartBlurring��5>      $
��Blur��6=   $
��Left=�6L   $��30��6g   $��
10,430,0,0q�6s   $	��Top=�6�   $	��290��6�   6�   �� 5?      -%   5        $! Blurring_ShowOrginal_Button��5?      $��Show Original��6B   $
��Left=�6Z   $��30��6u   $��
10,465,0,0q�6�   $	��Top=�6�   $	��135��6�   6�   �� 5@      -&   5        $ Blurring_ShowNew_Button��5@      $��Show Blurred��6>   $
��Left=�6U   $��30��6p   $��165,465,0,0q�6|   $	��Top=�6�   $	��135��6�   6�   �� 5A      -'   5        $
 Blurring_IsInverted_Label��5A      $��	Inverted:��6?   $
��Left=�6S   $��30��6n   $��168,255,0,0q�6z   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton���� 5B      -(   5        $" Blurring_IsInverted_Checkbox��5B      $�� ��6E   $
��Left=�6P   $��17��6k   $��283,261,0,0q�6w   $	��Top=�6�   $��17��6�   {�6�   �� *	IsChecked+$&  Blurring_PartOfImage_RadioButton��5C      5D      �� 5E      $��	Enlarging��6   .��5F      � ���	#FFE5E5E5.+�5G      �� -)   5        $
 Englarging_Label_Method��5G      $��Method:��6=   $
��Left=�6O   $��30��6j   $��	10,10,0,0q�6v   $	��Top=�6�   $	��100��6�   �� 5H      -*   5        $  Englarging_Method_ComboBox��5H      $
��Left=�6C   $��30��6^   $��
125,10,0,0q�6j   $	��Top=�6~   $	��175��6�    . .@�5I      �� $"��Nearest Neighbour Round Down��6&   �� 5J      $)��#Nearest Neighbour Round Up and Down��6&   �� 5K      $��BiLinear��6&   �� 5L      $��BiCubic��6&   5M      �� 5N      -+   5        $
 Enlarging_InputFile_Label��5N      $��Input File:��6?   $
��Left=�6U   $��30��6p   $��	10,45,0,0q�6|   $	��Top=�6�   $	��100��6�   �� 5O      -,   5        $% Enlarging_InputFileName_Textbox��5O      $
��Left=�6G   $��30��6b   $��
125,45,0,0q�6n   $
 Wrap=�6�   $��Example.png��6�   $	��Top=�6�   $	��126��6�   $ 0.5,0.5&�6�    . .��6�   5P      �� 5Q      --   5        $$
 Enlarging_InputFileWidth_Label��5Q      $��Input Width:��6D   $
��Left=�6[   $��30��6v   $��	10,80,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5R      -.   5        $! Enlargring_InputFile_Button��5R      $��Select��6B   $
��Left=�6S   $��30��6n   $��
256,45,0,0q�6z   $	��Top=�6�   $��44��6�   6�   �� 5S      -/   5        $&  Enlarging_InputFileWidth_Textbox��5S      $
��Left=�6H   $��30��6c   $��
125,80,0,0q�6o   $
 Wrap=�6�   $��
File Width��6�   $	��Top=�6�   $	��175��6�    . 
����������	#FF4040406�   �� 5T      -0   5        $%
 Enlarging_InputFileHeight_Label��5T      $��Input Height:��6E   $
��Left=�6]   $��30��6x   $��
10,115,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5U      -1   5        $' !Enlarging_InputFileHeight_Textbox��5U      $
��Left=�6I   $��30��6d   $��125,115,0,0q�6p   $
 Wrap=�6�   $��File Height��6�   $	��Top=�6�   $	��175��6�    . 
����������	#FF4040406�   �� 5V      -2   5        $%
 Enlarging_ScaleFactorTick_Label��5V      $��By Scale Factor:��6E   $
��Left=�6`   $��30��6{   $��
10,150,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5W      -3   5        $!
 Enlarging_ScaleFactor_Label��5W      $��Scale Factor:��6A   $
��Left=�6Y   $��30��6t   $��
10,185,0,0q�6�   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$' !Enlarging_ScaleFactor_RadioButton���� 5X      -4   5        $# Enlarging_ScaleFactor_Textbox��5X      $
��Left=�6E   $��30��6`   $��125,185,0,0q�6l   $
 Wrap=�6�   $��2��6�   $	��Top=�6�   $	��175��6�   {�6�   �� *	IsChecked+$' !Enlarging_ScaleFactor_RadioButton���� 5Y      -5   5        $!
 Enlarging_WidthHeight_Label��5Y      $��By Width/Height:��6A   $
��Left=�6\   $��30��6w   $��
10,220,0,0q�6�   $	��Top=�6�   $	��100��6�   	� 5Z      -6   5        $' !Enlarging_ScaleFactor_RadioButton��5Z   "   $�� ��6M   $
��Left=�6X   $��21��6s   $��125,159,0,0q�6   $	��Top=�6�   $��20��6�   $ 
0.95,0.571&�6�   $
 TrueY�6�    5[        5\      5^      	� 5_      -7   5        $' !Enlarging_WidthHeight_RadioButton��5_   "   $�� ��6M   $
��Left=�6X   $��22��6s   $��125,228,0,0q�6   $	��Top=�6�   $��19��6�   $ 0.316,0.364&�6�    5`        5a      5c      �� 5d      -8   5        $%
 Enlarging_OutputFileWidth_Label��5d      $��Output Width:��6E   $
��Left=�6]   $��30��6x   $��
10,255,0,0q�6�   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$' !Enlarging_WidthHeight_RadioButton���� 5e      -9   5        $' !Enlarging_OutputFileWidth_Textbox��5e      $
��Left=�6I   $��30��6d   $��125,255,0,0q�6p   $
 Wrap=�6�   $��
File Width��6�   $	��Top=�6�   $	��175��6�   {�6�   �� *	IsChecked+$' !Enlarging_WidthHeight_RadioButton���� 5f      -:   5        $&
  Enlarging_OutputFileHeight_Label��5f      $��Output Height:��6F   $
��Left=�6_   $��30��6z   $��
10,290,0,0q�6�   $	��Top=�6�   $	��100��6�   {�6�   �� *	IsChecked+$' !Enlarging_WidthHeight_RadioButton���� 5g      -;   5        $( "Enlarging_OutputFileHeight_Textbox��5g      $
��Left=�6J   $��30��6e   $��125,290,0,0q�6q   $
 Wrap=�6�   $��File Height��6�   $	��Top=�6�   $	��175��6�   {�6�   �� *	IsChecked+$' !Enlarging_WidthHeight_RadioButton���� 5h      -<   5        $ Enlarging_StartEnglarging��5h      $��Enlarge��6@   $
��Left=�6R   $��30��6m   $��
10,325,0,0q�6y   $	��Top=�6�   $	��290��6�   6�   �� 5i      -=   5        $" Enlarging_ShowOrginal_Button��5i      $��Show Original��6C   $
��Left=�6[   $��30��6v   $��
10,360,0,0q�6�   $	��Top=�6�   $	��135��6�   6�   �� 5j      ->   5        $ Enlarging_ShowNew_Button��5j      $��Show Enlarged��6?   $
��Left=�6W   $��30��6r   $��165,360,0,0q�6~   $	��Top=�6�   $	��135��6�   6�   5k      5l      �� 5m      $��Voronoi Generation��6   .��5n      � ���	#FFE5E5E5.+�5o      �� -?   5        $#
 Voronoi_OutputFileWidth_Label��5o      $��Output Width:��6C   $
��Left=�6[   $��30��6v   $��	10,10,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5p      -@   5        $% Voronoi_OutputFileWidth_Textbox��5p      $
��Left=�6G   $��30��6b   $��
125,10,0,0q�6n   $
 Wrap=�6�   $��
File Width��6�   $	��Top=�6�   $	��175��6�   �� 5q      -A   5        $$
 Voronoi_OutputFileHeight_Label��5q      $��Output Height:��6D   $
��Left=�6]   $��30��6x   $��	10,45,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5r      -B   5        $&  Voronoi_OutputFileHeight_Textbox��5r      $
��Left=�6H   $��30��6c   $��
125,45,0,0q�6o   $
 Wrap=�6�   $��File Height��6�   $	��Top=�6�   $	��175��6�   �� 5s      -C   5        $"
 Voronoi_PointGenerator_Label��5s      $��Point Generator:��6B   $
��Left=�6]   $��30��6x   $��	10,80,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5t      -D   5        $% Voronoi_PointGenerator_Combobox��5t      $
��Left=�6H   $��30��6c   $��
125,80,0,0q�6o   $	��Top=�6�   $	��175��6�   .@�5u      �� $��Random��6&   �� 5v      $��Uniform Generator��6&   �� 5w      $��Poisson Disc��6&   5x      �� 5y      -E   5        $-
 'Voronoi_PointGenerator_Parameter1_Label��5y      $��Parameter 1:��6M   $
��Left=�6d   $��30��6   $��
10,115,0,0q�6�   $	��Top=�6�   $	��100��6�   {�.  6�   �� 5z      -F   5        $/ )Voronoi_PointGenerator_Parameter1_Textbox��5z      $
��Left=�6Q   $��30��6l   $��125,115,0,0q�6x   $
 Wrap=�6�   $��Parameter 1��6�   $	��Top=�6�   $	��175��6�   {�.  6�   �� 5{      -G   5        $-
 'Voronoi_PointGenerator_Parameter2_Label��5{      $��Parameter 2:��6M   $
��Left=�6d   $��30��6   $��
10,150,0,0q�6�   $	��Top=�6�   $	��100��6�   {�.  6�   �� 5|      -H   5        $/ )Voronoi_PointGenerator_Parameter2_Textbox��5|      $
��Left=�6Q   $��30��6l   $��125,150,0,0q�6x   $
 Wrap=�6�   $��Parameter 2��6�   $	��Top=�6�   $	��175��6�   {�.  6�   �� 5}      -I   5        $# Voronoi_PointGenerator_Button��5}      $��Generate Points��6D   $
��Left=�6^   $��30��6y   $��
10,185,0,0q�6�   $	��Top=�6�   $	��290��6�   {�.  6�   �� 5~      -J   5        $$
 Voronoi_RegionGenerator_pValue��5~      $��Dist. Power:��6D   $
��Left=�6[   $��34��6v   $��
10,220,0,0q�6�   $	��Top=�6�   $	��100��6�   �� 5      -K   5         ��Name$ Voronoi_PValue_Slider��5      $
��Left=�6<   $��30��6W   $��125,220,0,0q�6c   $	��Top=�6x   $	��175��6�    � Maximum$ 2Q�6�    � Minimum$ 1Q�6�    � SmallChange$
 0.01Q�6�   $ 0.5,0.5&�6�    � Value$ 1Q�6�    � LargeChange$
 0.01Q�6�   z�5�      V� 5�      .~�5�   "   �� �� 5�   "   �� 5�   "   T� 5�   "    T� Y$ 5Q�65   5�      5�      �� 5�      $��1��6   $
��Left=�6(   $��25��6C   $��122,236,0,0q�6O   $	��Top=�6d   $��17��6|   �� 5�      $��2��6   $
��Left=�6(   $��25��6C   $��286,236,0,0q�6O   $	��Top=�6d   $��17��6|   �� 5�      -L   5        $
 Voronoi_BorderWidth_Label��5�      $��Border Width:��6?   $
��Left=�6W   $��30��6r   $��
10,259,0,0q�6~   $	��Top=�6�   $	��100��6�   �� 5�      -M   5        $! Voronoi_BorderWidth_Textbox��5�      $
��Left=�6C   $��30��6^   $��125,259,0,0q�6j   $
 Wrap=�6   $��0��6�   $	��Top=�6�   $	��175��6�   �� 5�      -N   5        $# Voronoi_ColourChanging_Button��5�      $��Colour Controls��6D   $
��Left=�6^   $��30��6y   $��
10,294,0,0q�6�   $	��Top=�6�   $	��290��6�   6�   �� 5�      -O   5        $$ Voronoi_GenerateRegions_Button��5�      $��Generate Regions��6E   $
��Left=�6`   $��30��6{   $��
10,399,0,0q�6�   $	��Top=�6�   $	��290��6�   6�   �� 5�      -P   5        $" Voronoi_DisplayOutput_Button��5�      $��Update Display��6C   $
��Left=�6\   $��30��6w   $��
10,447,0,0q�6�   $	��Top=�6�   $	��290��6�   6�   �� 5�      -Q   5        $ Voronoi_SaveOutput_Button��5�      $��
Save Image��6@   $
��Left=�6U   $��30��6p   $��
10,482,0,0q�6|   $	��Top=�6�   $	��290��6�   6�   �� 5�      -R   5        $'
 !Voronoi_PointGenerator_Seed_Label��5�      $��Point Gen. Seed:��6G   $
��Left=�6b   $��30��6}   $��
10,329,0,0q�6�   $	��Top=�6�   $	��110��6�   {�.  6�   �� 5�      -S   5        $) #Voronoi_PointGenerator_Seed_Textbox��5�      $
��Left=�6K   $��30��6f   $��125,329,0,0q�6r   $
 Wrap=�6�   $	��Top=�6�   $	��175��6�   {�.  6�   �� 5�      -T   5        $
 Voronoi_Colour_Seed_Label��5�      $��Colour Gen. Seed:��6?   $
��Left=�6[   $��30��6v   $��
10,364,0,0q�6�   $	��Top=�6�   $	��110��6�   {�.  6�   �� 5�      -U   5        $! Voronoi_Colour_Seed_Textbox��5�      $
��Left=�6C   $��30��6^   $��125,364,0,0q�6j   $
 Wrap=�6   $	��Top=�6�   $	��175��6�   {�.  6�   5�      5�      5�      �� 5�   
   -V   5        $
 Output_Label��5�      $��Output Filename:��6&   $��0,674,237,0q�6A   $��Right=�6V   $	��100��6r   $��45��6~   $	��Top=�6�   �� 5�   
   -W   5        $ OutputFilenameTextBox��5�      $��760,674,60,0q�61   $
 Wrap=�6G   $��Example.png��6[   $��45��6n   $	��Top=�6z   �� 5�   
   -X   5        $! ImageEditingToolHelp_Button��5�      $��?��66   $��937,674,10,50q�6B   6Y   5�      5�      