   M S B A M L   `   `   `  ����C  ?CG-Final, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:CG_FinalCG_Final  5   	   5clr-namespace:CG_Final.CommandsCG_Final.Commands  5   	   -clr-namespace:CG_Final.ViewCG_Final.View  5	   	       CG_Final.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:CG_Final     5   	   /cmdclr-namespace:CG_Final.Commands     5   	   +vieclr-namespace:CG_Final.View     5	   	   $	��720��5
   Y   $
��1280��6f     � 
ResizeMode$  NoResize=�6s    � WindowStartupLocation$ CenterScreen=�6�    � Title 6	   �� *CurrentScene.Title+   TitleConverter �� 	Converter# [   � 	Resources 5       �� DataType'   !CG_Final.Commands.CreateNewObject   CG_Final.View.NewObject � DataContext �� ElementName(  "CG_Final.Commands.TranslateCommand   CG_Final.View.XyzView$  CG_Final.Commands.ScaleCommand'  !CG_Final.Commands.RotationCommand   CG_Final.Scene    CG_Final.View.SceneControl��%3  (��       5      *, +)(�� a     5      *, +)(�� �     5      *, +)(�� #    5      *, +)(�� �    5      *, +)�� 5   
   "  6   .R�5         6   �� *	Content+$ CommandContent��5      �� 5   
   "  6   .R�5         6   �� *	Content+$ CommandContent��5      �� 5   
   "  6   .R�5         6   �� *	Content+$ CommandContent��5      �� 5   
   "  6   .R�5         6   �� *	Content+$ CommandContent��5      �� 5   
   "  6   .R�5        5      5       J� InputBindings 5      �� 5   
   
	 �� Key$	 N��6   
 �� Command
 6   �� *NewObjectCommand+�� 5   
   $	 T��6   
 6   �� *TranslateObjectCommand+�� 5   
   $	 S��6   
 6   �� *ScaleObjectCommand+�� 5    
   $	 R��6   
 6   �� *RotateObjectCommand+�� 5!   
   $	 X��6   
 6   �� *DeleteObjectCommand+�� 5"   
    �� 	Modifiers$ Controlf�6   $	 O��6)   
 61   �� *LoadSceneCommand+�� 5#   
   $ Controlf�6   $	 S��6)   
 61   �� *SaveSceneCommand+ 5%      	   CG_Final.WindowVM	  5&   
   -   5        WindowDataContext��5&      .��5(      �  � RowDefinitions 5)   
   �� 5*      $
��Auto �6   �� 5+      $��* �6    � ColumnDefinitions 5-   
   �� 5.      $��2* �6   �� 5/      $��7* �6   .+�51   
   o� -   5         o�Name$
 menu��51      $��2��6   .@�52      m� $��_File��6   m� 53      $��_Help��6   54      � 55   
   $��1��6   $��1��6   ���	#FFC0DAE2 56      �� 57      $��* �6    �� 58      $��* �6     5:      �� 5;      $��* �6#   �� 5<      $��* �6#   .+�5>      �� $��0,0,1,1q�6?   ��6   �� *CurrentScene.Cam1Source+6?   �� 5?      $��0��6   $��1��6!   $��1,0,0,1q�6\   ��61   �� *CurrentScene.Cam2Source+6\   �� 5@      $��0,1,1,0q�6?   $��1��6P   $��0��5A      ��5@      �� *CurrentScene.Cam3Source+5A      �� 5B      $��1��6   $��1,1,0,0q�6O   $��1��6`   ��6$   �� *CurrentScene.Cam4Source+6`   5C      � 5D   
   $��1��6    5E      �� 5F      $��* �6    $	��220��6+   �� 5G      $��5 �6    �� 5H      $��* �6    .+�5J      �� -   5         ��Name$ gridSplitter��5J      $��Stretch=�61   $��1��6O    �� ResizeDirection$
 Rows=�6\   � 5K       5L      �� 5M      $
��Auto �6$   $��50��62   �� 5N      $��* �6$   $	��100��6/   .+�5P      �� -   5         ��Name$ listBox��5P      $	��100��6+   $��5q�68   ��6C   �� *CurrentScene.Objects+ �� SelectedItem 6p   �� *SelectedObject+ �� Mode$ TwoWay=��� 5Q      -   5         ��Name$ 
tabControl��5Q      $��5q�61   $��1��6<    �� TabStripPlacement$ Bottom=�6I   .@�5R      �� $��Actions��6   .��5S      �� $
��Auto=�6'   $
��Auto=�6J   .��5T      � ���	#FFE5E5E5 5U   "   �� 5V   &   $��* �64   $��25��6?   �� 5W   &   $��* �64   $��25��6?   �� 5X   &   $��* �64   $��25��6?   �� 5Y   &   $��* �64   $��25��6?    5[   "   �� 5\   &   $��* �67   �� 5]   &   $��* �67    5_   "    �� 
TargetType �� Property �� Value��%�   '��       �� 5`   &   " ��6,   .�5a   *   ��  � ��$ Center=�6P   �� 5b   *    � ��$ Center=�6N   �� 5c   *    � ��$ 75��6B   �� 5d   *    � ��$ 20��6C   5e   '   5f   #   .+�5g   "   �� -   5         ��Name$ button��5g   )   $	��New��69   ��6G   �� *NewObjectCommand+�� 5h   "   -   5        $ button1��5h   )   $��Delete��6:   $��1��6K   ��6[   �� *DeleteObjectCommand+�� 5i   "   -   5        $ button2��5i   )   $��	Translate��6:   $��1��6N   ��6[   �� *TranslateObjectCommand+�� 5j   "   -	   5        $ button3��5j   )   $��Scale��6:   $��1��6J   $��1��6Z   ��6g   �� *ScaleObjectCommand+�� 5k   "   -
   5        $ button4��5k   )   $��Rotate��6:   $��2��6K   ��6X   �� *RotateObjectCommand+�� 5l   "   -   5        $ button5��5l   )   $��Twist��6:   $��1��6J   $��2��6Z   �� 5m   "   -   5        $ button6��5m   )   $
��Bend��6:   $��3��6I   �� 5n   "   -   5        $ button7��5n   )   $��Bevel��6:   $��1��6J   $��3��6Z   5o      5p      5q      �� 5r      $��World��6   .��5s      �� $
��Auto=�6'   $
��Auto=�6J   .��5t      � ���	#FFE5E5E56#   5u      5v      �� 5w      $��Object��6   .��5x      �� $
��Auto=�6'   $
��Auto=�6J   .��5y      � ���	#FFE5E5E5.+�5z   "   �� -   5         ��Name$ MinhocaUhaha��5z   1   ��6G   �� *SelectedObject+��5{   &   �� 5|   *   .R�5}   .   !
   CG_Final.View.ObjectControl
   6@   �� *	Content+$ MinhocaUhaha��5~   +   5�   #   5�      5�      5�      �� 5�      $��	Modifiers��6   .��5�      �� $
��Auto=�6'   $
��Auto=�6J   .��5�      � ���	#FFE5E5E56#   5�      5�      �� 5�      $��Scene��6   .��5�      �� $
��Auto=�6'   $
��Auto=�6J   .��5�      �� ��6-   �� *CurrentScene+$ TwoWay=�5�      5�      5�      5�      �� 5�      -   5        $ CommandContent��5�   *   $��2��6   $��5q�6B   ��6M   �� *LastCommand+5�      5�      5�      