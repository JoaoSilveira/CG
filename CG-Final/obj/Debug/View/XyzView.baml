   M S B A M L   `   `   `  ����C  ?CG-Final, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:CG_Final.ViewCG_Final.View  5          CG_Final.View.XyzView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      -localclr-namespace:CG_Final.View     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� -   5         ��Name$ label��5      $��X��6   $
��Left=�6+   $��5,5,0,0q�6F   $	��Top=�6W   �� 5   
   -   5        $ label1��5      $��Y��6    $
��Left=�6,   $��5,5,0,0q�6G   $��1��6X   $	��Top=�6e   �� 5   
   -   5        $ label2��5      $��Z��6    $
��Left=�6,   $��5,5,0,0q�6G   $��2��6X   $	��Top=�6e   �� 5   
   -   5         ��Name$ textBox��5      $��1��6#   $��Stretch=�63   $��23��6Q   $��5,5,5,0q�6]    �� TextWrapping$
 Wrap=�6n   $	��Top=�6�   ��6�   �� *X+   DecimalConverter �� 	Converter# [   �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=�6�   �� 5   
   -   5        $ textBox1��5      $��1��6$   $��Stretch=�64   $��23��6R   $��5,5,5,0q�6^   $��1��6o   $
 Wrap=�6|   $	��Top=�6  ��6�   �� *Y+# [  $ PropertyChanged=�$ TwoWay=�6  �� 5   
   -   5        $ textBox2��5      $��1��6$   $��Stretch=�64   $��23��6R   $��5,5,5,0q�6^   $��2��6o   $
 Wrap=�6|   $	��Top=�6  ��6�   �� *Z+# [  $ PropertyChanged=�$ TwoWay=�6  �� 5   
   -   5         ��Name$ button��5      $��Apply��6!   $��1��61   $��Right=�6A   $��5q�6]   $��3��6h   $��Bottom=�6u   $��75��6�   6�   5      5      