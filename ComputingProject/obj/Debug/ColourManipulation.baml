   M S B A M L   `   `   `  ����K  GComputingProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ComputingProjectComputingProject  5   	   )    #ComputingProject.ColourManipulation   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TXceed.Wpf.Toolkit, Version=3.6.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5   	   *localclr-namespace:ComputingProject  5   	   -   5          �Name$  ColourManipulationWindow��5   	    � Title$ ColourManipulation��5	   	   $	��450��6$   $	��860��61   .��5
      � $��0,0,-8,0q�6   .+�5   
   �� -   5         ��Name$ ColourManipulationTitle��5      $��40��65   $��
10,10,10,0q�6A   $
��Wrap=�6U   $��Colour Manipulation��6i   $	��Top=�6�   $��30(�6�   $
��Bold"�6�    �� TextAlignment$ Center=�6�   �� 5   
   -   5         ��Name$3 -ColourManipulationPoint_PointsDifferent_Label��5      $��Colour Points Diff.:��6G   $
��Left=�6f   $��25��6�   $��	10,85,0,0q�6�   $	��Top=�6�   $	��110��6�   �� 5   
   -   5         ��Name$6 0ColourManipulationPoint_PointsDifferent_Checkbox��5      $�� ��6M   $
��Left=�6X   $��17��6s   $��
125,93,0,0q�6   $	��Top=�6�   $��17��6�   6�   �� 5   
   -   5        $/ )ColourManipulationPoint_FirstMethod_Label��5      $��First Method:��6C   $
��Left=�6[   $��
10,115,0,0q�6v   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+ �� ElementName$6 0ColourManipulationPoint_PointsDifferent_Checkbox���� 5   
   -   5         ��Name$2 ,ColourManipulationPoint_FirstMethod_Combobox��5      $
��Left=�6I   $��125,115,0,0q�6d   $	��Top=�6y   $	��150��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��.@�5      �� $��Order Placed��6   �� 5      $��By X Coordinate��6   �� 5      $��By Y Coordinate��6   �� 5      $��Random on Gradient��6   �� 5      $��Completely Random��6   �� 5      $��Constant Colour��6   �� 5      $��By Size��6   5      �� 5   
   -   5        $6 0ColourManipulationPoint_FirstColourMethod1_Label��5      $��First Colour:��6J   $
��Left=�6b   $��
10,145,0,0q�6}   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��#  Xceed.Wpf.Toolkit.ColorPicker  5   
   -   5          Name$2 ,ColourManipulationPoint_Colour1PickerMethod1��5      $��125,0,0,249q�6Q   	   DisplayColorAndName	 . !
   AvailableColorsSortingMode$
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�    J� RenderTransformOrigin$ 0.5,0.5&�6b  {�6  �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��z�5      V� 5      .~�5      �� �� 5      �� 5      T� 5       T� Y$ 10Q�6)   5       5"      �� 5#   
   -	   5        $7 1ColourManipulationPoint_SecondColourMethod1_Label��5#      $��Second Colour:��6K   $
��Left=�6d   $��
10,175,0,0q�6   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��  5$   
   -
   5        $2 ,ColourManipulationPoint_Colour2PickerMethod1��5$      $��125,0,0,219q�6Q   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6b  {�6  �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��z�5%      V� 5&      .~�5'      �� �� 5(      �� 5)      T� 5*      $ 10Q�6)   5+      5-      �� 5.   
   -   5         ��Name$  ColourManipulation_Divider��5.      
���   �$��1q�6I   $
��Left=�6]   $	��354��6x   $��
280,55,0,0q�6�   $	��Top=�6�   $��1��6�   �� 5/   
   -   5        $6 0ColourManipulation_PointColouringTitle_Textblock��5/      $
��Left=�6N   $��22��6i   $��	10,55,0,0q�6u   $
��Wrap=�6�   $��Colour Points��6�   $	��Top=�6�   $	��265��6�   $ Center=�6�   $
��Bold"�6�   �� 50   
   -   5        $0 *ColourManipulationPoint_SecondMethod_Label��50      $��Second Method:��6D   $
��Left=�6]   $��
10,206,0,0q�6x   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox���� 51   
   -   5        $3 -ColourManipulationPoint_SecondMethod_Combobox��51      $
��Left=�6J   $��125,206,0,0q�6e   $	��Top=�6z   $	��150��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��.@�52      �� $��Order Placed��6   �� 53      $��By X Coordinate��6   �� 54      $��By Y Coordinate��6   �� 55      $��Random on Gradient��6   �� 56      $��Completely Random��6   �� 57      $��Constant Colour��6   �� 58      $��By Size��6   �� 59      $
��None��6   5:      �� 5;   
   -   5        $6 0ColourManipulationPoint_FirstColourMethod2_Label��5;      $��First Colour:��6J   $
��Left=�6b   $��
10,236,0,0q�6}   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��  5<   
   -   5        $2 ,ColourManipulationPoint_Colour1PickerMethod2��5<      $��125,0,0,158q�6Q   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6b  {�6  �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��z�5=      V� 5>      .~�5?      �� �� 5@      �� 5A      T� 5B      $ 10Q�6)   5C      5E      �� 5F   
   -   5        $7 1ColourManipulationPoint_SecondColourMethod2_Label��5F      $��Second Colour:��6K   $
��Left=�6d   $��
10,266,0,0q�6   $	��Top=�6�   $	��110��6�   $��25��6�   {�6�   �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��  5G   
   -   5        $2 ,ColourManipulationPoint_Colour2PickerMethod2��5G      $��125,0,0,128q�6Q   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6b  {�6  �� *	IsChecked+$6 0ColourManipulationPoint_PointsDifferent_Checkbox��z�5H      V� 5I      .~�5J      �� �� 5K      �� 5L      T� 5M      $ 10Q�6)   5N      5P      �� 5Q   
   -   5        $4 .ColourManipulationBackground_FirstMethod_Label��5Q      $��First Method:��6H   $
��Left=�6`   $��
290,85,0,0q�6{   $	��Top=�6�   $	��110��6�   $��25��6�   �� 5R   
   -   5        $7 1ColourManipulationBackground_FirstMethod_Combobox��5R      $
��Left=�6N   $��
405,85,0,0q�6i   $	��Top=�6}   $	��150��6�   $��25��6�   .@�5S      �� $��By X Coordinate��6   �� 5T      $��By Y Coordinate��6   �� 5U      $��Constant Colour��6   5V      �� 5W   
   -   5        $; 5ColourManipulationBackground_FirstColourMethod1_Label��5W      $��First Colour:��6O   $
��Left=�6g   $��290,115,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5X   
   -   5        $7 1ColourManipulationBackground_Colour1PickerMethod1��5X      $��405,0,0,279q�6V   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6	  z�5Y      V� 5Z      .~�5[      �� �� 5\      �� 5]      T� 5^      $ 10Q�6)   5_      5a      �� 5b   
   -   5        $< 6ColourManipulationBackground_SecondColourMethod1_Label��5b      $��Second Colour:��6P   $
��Left=�6i   $��290,145,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5c   
   -   5        $7 1ColourManipulationBackground_Colour2PickerMethod1��5c      $��405,0,0,249q�6V   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6	  z�5d      V� 5e      .~�5f      �� �� 5g      �� 5h      T� 5i      $ 10Q�6)   5j      5l      �� 5m   
   -   5        $; 5ColourManipulation_BackgroundColouringTitle_Textblock��5m      $
��Left=�6S   $��22��6n   $��
290,55,0,0q�6z   $
��Wrap=�6�   $��Background Colour��6�   $	��Top=�6�   $	��265��6�   $ Center=�6�   $
��Bold"�6�   �� 5n   
   -   5        $5 /ColourManipulationBackground_SecondMethod_Label��5n      $��Second Method:��6I   $
��Left=�6b   $��290,176,0,0q�6}   $	��Top=�6�   $	��110��6�   $��25��6�   �� 5o   
   -   5        $8 2ColourManipulationBackground_SecondMethod_Combobox��5o      $
��Left=�6O   $��405,176,0,0q�6j   $	��Top=�6   $	��150��6�   $��25��6�   .@�5p      �� $��By X Coordinate��6   �� 5q      $��By Y Coordinate��6   �� 5r      $��Constant Colour��6   �� 5s      $
��None��6   5t      �� 5u   
   -   5        $; 5ColourManipulationBackground_FirstColourMethod2_Label��5u      $��First Colour:��6O   $
��Left=�6g   $��290,206,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5v   
   -   5        $7 1ColourManipulationBackground_Colour1PickerMethod2��5v      $��405,0,0,188q�6V   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6	  z�5w      V� 5x      .~�5y      �� �� 5z      �� 5{      T� 5|      $ 10Q�6)   5}      5      �� 5�   
   -   5        $< 6ColourManipulationBackground_SecondColourMethod2_Label��5�      $��Second Colour:��6P   $
��Left=�6i   $��290,236,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5�   
   -   5        $7 1ColourManipulationBackground_Colour2PickerMethod2��5�      $��405,0,0,158q�6V   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6	  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $ 10Q�6)   5�      5�      �� 5�   
   -    5        $! ColourManipulation_Divider2��5�      
���   �$��1q�6J   $
��Left=�6^   $	��354��6y   $��
563,55,0,0q�6�   $	��Top=�6�   $��1��6�   �� 5�   
   -!   5        $0 *ColourManipulationRegion_FirstMethod_Label��5�      $��First Method:��6D   $
��Left=�6\   $��
573,85,0,0q�6w   $	��Top=�6�   $	��110��6�   $��25��6�   �� 5�   
   -"   5        $3 -ColourManipulationRegion_FirstMethod_Combobox��5�      $
��Left=�6J   $��
688,85,0,0q�6e   $	��Top=�6y   $	��150��6�   $��25��6�   .@�5�      �� $��Order Placed��6   �� 5�      $��By X Coordinate��6   �� 5�      $��By Y Coordinate��6   �� 5�      $��Random on Gradient��6   �� 5�      $��Completely Random��6   �� 5�      $��Constant Colour��6   �� 5�      $��By Size��6   5�      �� 5�   
   -#   5        $7 1ColourManipulationRegion_FirstColourMethod1_Label��5�      $��First Colour:��6K   $
��Left=�6c   $��573,115,0,0q�6~   $	��Top=�6�   $	��110��6�   $��25��6�     5�   
   -$   5        $3 -ColourManipulationRegion_Colour1PickerMethod1��5�      $��688,0,0,279q�6R   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $ 10Q�6)   5�      5�      �� 5�   
   -%   5        $8 2ColourManipulationRegion_SecondColourMethod1_Label��5�      $��Second Colour:��6L   $
��Left=�6e   $��573,145,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5�   
   -&   5        $3 -ColourManipulationRegion_Colour2PickerMethod1��5�      $��688,0,0,249q�6R   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $ 10Q�6)   5�      5�      �� 5�   
   -'   5        $7 1ColourManipulation_RegionColouringTitle_Textblock��5�      $
��Left=�6O   $��22��6j   $��
573,55,0,0q�6v   $
��Wrap=�6�   $��Regions Colour��6�   $	��Top=�6�   $	��265��6�   $ Center=�6�   $
��Bold"�6�   �� 5�   
   -(   5        $1 +ColourManipulationRegion_SecondMethod_Label��5�      $��Second Method:��6E   $
��Left=�6^   $��573,176,0,0q�6y   $	��Top=�6�   $	��110��6�   $��25��6�   �� 5�   
   -)   5        $4 .ColourManipulationRegion_SecondMethod_Combobox��5�      $
��Left=�6K   $��688,176,0,0q�6f   $	��Top=�6{   $	��150��6�   $��25��6�   .@�5�      �� $��Order Placed��6   �� 5�      $��By X Coordinate��6   �� 5�      $��By Y Coordinate��6   �� 5�      $��Random on Gradient��6   �� 5�      $��Completely Random��6   �� 5�      $��Constant Colour��6   �� 5�      $��By Size��6   �� 5�      $
��None��6   5�      �� 5�   
   -*   5        $7 1ColourManipulationRegion_FirstColourMethod2_Label��5�      $��First Colour:��6K   $
��Left=�6c   $��573,206,0,0q�6~   $	��Top=�6�   $	��110��6�   $��25��6�     5�   
   -+   5        $3 -ColourManipulationRegion_Colour1PickerMethod2��5�      $��688,0,0,188q�6R   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $ 10Q�6)   5�      5�      �� 5�   
   -,   5        $8 2ColourManipulationRegion_SecondColourMethod2_Label��5�      $��Second Colour:��6L   $
��Left=�6e   $��573,236,0,0q�6�   $	��Top=�6�   $	��110��6�   $��25��6�     5�   
   --   5        $3 -ColourManipulationRegion_Colour2PickerMethod2��5�      $��688,0,0,158q�6R   	 . $
 HueSaturationBrightness=�6�   $
��Left=�6�   $	��150��6�   $��25��6�   $��Bottom=�6�   $ 0.5,0.5&�6  z�5�      V� 5�      .~�5�      �� �� 5�      �� 5�      T� 5�      $ 10Q�6)   5�      5�      5�      5�      