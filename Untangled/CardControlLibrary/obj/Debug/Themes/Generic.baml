   M S B A M L   `   `   `  ����M  ICardControlLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:CardControlLibraryCardControlLibrary  5           CardControlLibrary.Generic\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089$   CardControlLibrary.AwesomeCard  �� 
TargetType �� Property �� Value �� 
TargetType � 	Resources   Key �� Triggers P� Property P� Value �� 
TargetName	 ��Name
 J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   
BlurRadius   	Direction   Color �Name � RowDefinitions � ColumnDefinitions ��Name �� Path ��Name �� Mode ��Name A�Name+   %CardControlLibrary.AwesomeProgressBar  Name
 � Tag �� RelativeSource �� AncestorType   Value ��Name  PauseButton  
PlayButton   5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      :x,http://schemas.microsoft.com/winfx/2006/xaml     5      'cc clr-namespace:CardControlLibrary  5      %$  '        7  7  �� 5      "   6   .�5   
   ��  � �� �#f6f6f669   �� 5   
    � ��$	 460��6%   63   �� 5	   
    � ��$
 Left=�60   6?   �� 5
   
    � ��$ 90��6&   63   �� 5   
    � ��$ 0,10,0,0q�6#   66   �� 5   
    � �� 5      �� 5      "  6"    5      ���� 5      
PlayButton  " ��6=   .R�5      � .+�5   "   �� $��Normal��6(   $6��0/CardControlLibrary;component/Icons/play_btn.png��66   �� 5   "   $��Hover��6(   $<��6/CardControlLibrary;component/Icons/play_btn_hover.png��65   $y�Hidden=�6u   5       5      P� 5   "    �@J�IsMouseOver . . �5   &   �� $ Normal��6-    � y�$ Hidden=�6W   �� 5   &   $ Hover��6-    � y�$ Visible=�6V   5   #   5      �� 5      PauseButton  " ��6>   .R�5      � .+�5    "   �� $��Normal��6(   $7��1/CardControlLibrary;component/Icons/pause_btn.png��66   �� 5!   "   $��Hover��6(   $9��3/CardControlLibrary;component/Icons/pause_hover.png��65   $y�Hidden=�6r   5"       5#      P� 5$   "    �@J�IsMouseOver . . �5%   &   �� $ Normal��6-    � y�$ Hidden=�6W   �� 5&   &   $ Hover��6-    � y�$ Visible=�6V   5'   #   5)      5*      .R�5-      �� $	 	TheBorder��6   #��z��60   #��z��5.      #��z��5/      
 50        51      $ 8Q�6/   $	 -90Q�6>   $ #E1E2E3��6N   ._�54      � $ CardGrid��6    55      �� 56   "   $��* �60   6=    58      �� 59   "   $��90 �63   6@   �� 5:   "   $
��270* �63   6B   �� 5;   "   $	��100 �63   6A   .+�5>      �� $ FileDetails��6)   $��0��6>   $��1��6K   $��Center=�6[   .+�5?   "   �  5@   &   �� 5A   *   $��80 �6;   6H   �� 5B   *   $��* �6;   6G    5D   &   �� 5E   *   $��* �68   6E   �� 5F   *   $��* �68   6E   �� 5G   *   $��* �68   6E   .+�5I   &   �� $��0��6,   $��0��69   $��Center=�6I   $��Consolas)�6�   ���#7e7e7e$��
ExtraLight"�6�   $��13(�6�   ��6d   �� $ FileTypeLabel�5J   '   �� 5K   &   $ 	NameField��60   $��0��6D   $��1��6Q   $��Center=�6a   $��Consolas)�6|   ���#7e7e7e$��CharacterEllipsis=�6�   $��
ExtraLight"�5L   0   $��13(�6H   ��6V   �� $
 Name�$ TwoWay=�6�   �� 5M   &   $��1��6,   $��0��69   $��Center=�6I   $��Consolas)�6d   ���#7e7e7e$��
ExtraLight"�6�   $��13(�6�   .��6�   Path:5O   '   �� 5P   &   $ 	PathField��60   $��1��6C   $��1��6P   $��Center=�6`   $��Consolas)�6{   ���#7e7e7e$��CharacterEllipsis=�6�   $��
ExtraLight"�5Q   0   $��13(�6H   ��6V   �� $
 Path�$ TwoWay=�6�   �� 5R   &   $��2��6,   $��0��69   $��Center=�6I   $��Consolas)�6d   ���#7e7e7e$��
ExtraLight"�6�   $��13(�6�   .��6�   	Activity:5T   '   �� 5U   &   $ ActivityField��60   $��2��6G   $��1��6T   $��Center=�6d   $��Consolas)�6   ���#7e7e7e$��CharacterEllipsis=�6�   $��
ExtraLight"�5V   0   $��13(�6H   ��6V   �� $ Activity�$ TwoWay=�6�   5W   #   5X      �� 5Y      $��0��6(   $��90��68   ���#E6E3E36T   �� 5Z      $ 	ImageIcon��6$   $��0��67   $��40��6G   $��40��6R   ��6^   �� $ 	IconImage�6�   � 5[      $ FullProgress��6#   $��2��69   .+�5\   "   A� $ TimerEllipse��6*   $��40��6@   $��40��6K   $��Center=�5]   *   ��6G   �� $ EllipseFill�6k   �� 5^   "   $ ProgressValue��6,   $��Center=�6C   $��Center=�6^   $��Consolas)�6{   ���#FFFFFF$��
ExtraLight"�6�   $��11(�6�   ��6�   �� $ ProgressIndicator�6�     5_   "   $ 
AwesomeBar��68   $��55��6�   $��55��6�   $��2��6�   $��Center=�6  ��6L   �� $ StrokeBrush� 6p   ��  �� "   6�   �� $ Progress�$ TwoWay=�69  �� 5`   "   $ PauseButton��6)   $��20��6>   $��20��6I   8��  $y�	Collapsed=�6}   $��Center=�6�   6�   �� 5a   "   $ 
PlayButton��6)   $��20��6=   $��20��6H   8�� $y�	Collapsed=�6{   $��Center=�6�   6�   5b      5c      5d      5f      5h      5i      5j      