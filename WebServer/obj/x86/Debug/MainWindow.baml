   M S B A M L   `   `   `  ����  	WebServer    WebServer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   �   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6  -   5          � Title$  
Web Server��5   	    � WindowState$ Normal=�6V    � Icon$- '/WebServer;component/res/ico/server.ico��6k    � WindowStyle$ ThreeDBorderWindow=�6�    � ShowInTaskbar . $	��400��6e  $	��410��6r   � 
ResizeMode$ NoResize=�6~  .��5      � .+�5   
   �� -   5        $��groupBoxSettings��5   F   $��Settings��6   $	��173��6%   $��
12,0,12,46q�62   $��Bottom=�6^   .��5      � .+�5      �� -   5        $��	labelPort��5   b   $��Port:��6   $��28��6(   $
��Left=�64   $��	10,10,0,0q�6O   $	��Top=�6s   �� 5	      -   5        $��labelMaxusers��5	   g   $��
Max users:��6   $��28��6-   $
��Left=�69   $��	10,48,0,0q�6T   $	��Top=�6|   �� 5
      -   5        $��labelRootDirectory��5
   l   $��Root directory:��6   $��28��62   $
��Left=�6>   $��	10,86,0,0q�6Y   $	��Top=�6�   �� 5      -   5        $��textBoxPort��5   U   $��23��6   $
��Left=�6&   $��
104,12,0,0q�6A   $	��Top=�6h   $��63��6�   �� 5      -   5        $��textBoxMaxUsers��5   U   $��23��6   $
��Left=�6&   $��
104,53,0,0q�6A   $	��Top=�6l   $��63��6�   �� 5      -   5        $��textBoxRootDirectory��5   <   $��23��6   $��104,88,109,0q�6&   $	��Top=�6X   �� 5      -	   5        $��buttonBrowse��5   A   $��	Browse...��6   $��
0,88,28,39q�6-   $��Right=�6p   $��75��6�   �� 5      -
   5        $��buttonLBConfig��5   h   $��	LB Config��6   $��23��6-   $
��Left=�69   $��
10,121,0,0q�6T   $	��Top=�6~   $��75��6�   6�   5      5      �� 5   
   -   5        $��buttonApply��5   4   $��Apply��6   $��	0,0,12,12q�6!   $��Right=�6a   $��75��6}   $��23��6�   $��Bottom=�6�   �� 5   
   -   5        $��labelConnections��5   I   $�� ��6   $
��Left=�6   $��12,0,0,7q�67   $��28��6y   $��Bottom=�6�    � DataContext 6a   �� 6�   �� 5   
   -   5        $��	groupBox1��5   7   $
��Mode��6   $��12,12,12,228q�6!   .��5      � .+�5      �� -   5        $��stackPanel1��5   I   $
��Left=�6   $��6,6,0,6q�68   $	��117��6\   .+�5      	� -   5        $��radioButtonNormal��5   J   $��5q�6"   $��Normal��6-   $��16��6>   6J   	� 5      -   5        $��radioButtonRedirect��5   L   $��5q�6"   $��Redirect��6-   $��16��6@   6L   	� 5      -   5        $��radioButtonLoadBalancer��5   Q   $��5q�6"   $��Load Balancer��6-   $��16��6E   6p   5      �� 5      -   5        $��checkBoxLoadBalancer��5   r   $��Is load balancer.��6   $��16��67   $
��Left=�6C   $��
129,63,0,0q�6^   $	��Top=�6�   5      5      5      5      