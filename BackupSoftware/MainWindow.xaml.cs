using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace BackupSoftware
{
	 /// <summary>
	 /// Interaction logic for MainWindow.xaml
	 /// </summary>
	 public partial class MainWindow : Window
	 {
		  public MainWindow()
		  {
			   InitializeComponent();
			   this.DataContext = new WindowViewModel(this);

		  }
	 }
}
