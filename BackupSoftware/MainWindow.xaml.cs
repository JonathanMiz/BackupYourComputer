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
		  private FormViewModel formViewModel;
		  public MainWindow()
		  {
			   InitializeComponent();

			   // Create a model form object

			   formViewModel = new FormViewModel();

			   // Set default values
			   formViewModel.AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");
			   formViewModel.BackupFolder = "H:\\JonathanCompterBackup";

			   this.DataContext = formViewModel;
			   
			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");
		  }
	 }
}
