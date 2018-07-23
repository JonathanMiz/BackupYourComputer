
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System;

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

			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents\\Test");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");

			   // Set default hard drive for backup
			   this.BackupDrive.Text = "H:\\";
		  }

		  public object DataTime { get; private set; }

		  private void Button_Click(object sender, RoutedEventArgs e)
		  {
			   var dlg = new CommonOpenFileDialog();
			   dlg.ResetUserSelections();
			   dlg.Title = "Choose folder";
			   dlg.IsFolderPicker = true;
			   dlg.InitialDirectory = null;

			   dlg.AddToMostRecentlyUsedList = false;
			   dlg.AllowNonFileSystemItems = false;
			   dlg.DefaultDirectory = null;
			   dlg.EnsureFileExists = true;
			   dlg.EnsurePathExists = true;
			   dlg.EnsureReadOnly = false;
			   dlg.EnsureValidNames = true;
			   dlg.Multiselect = true;
			   dlg.ShowPlacesList = true;

			   if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			   {
					// Take all the folders that was chosen
					var folders = dlg.FileNames;

					List<string> foldersToAddToListView = new List<string>();

					// Iterate through all of the folders
					foreach (var folderName in folders)
					{
						 foreach(var folder in this.FolderList.Items)
						 {
							  // Check to see if the new folder is not a sub folder of existing item
							  if(!IsExistsOrSubFolder(folder.ToString(), folderName))
							  {
								   foldersToAddToListView.Add(folderName);
							  }
							  else
							  {
								   break;
							  }

						 }
						 
					}

					foreach(var folder in foldersToAddToListView)
					{
						 this.FolderList.Items.Add(folder);
					}
			   }
			   
		  }

		  private void Button_Click_1(object sender, RoutedEventArgs e)
		  {
			   var dlg = new CommonOpenFileDialog();
			   dlg.ResetUserSelections();
			   dlg.Title = "Choose hard drive";
			   dlg.IsFolderPicker = true;
			   dlg.InitialDirectory = null;

			   dlg.AddToMostRecentlyUsedList = false;
			   dlg.AllowNonFileSystemItems = false;
			   dlg.DefaultDirectory = null;
			   dlg.EnsureFileExists = true;
			   dlg.EnsurePathExists = true;
			   dlg.EnsureReadOnly = false;
			   dlg.EnsureValidNames = true;
			   dlg.Multiselect = false;
			   dlg.ShowPlacesList = true;

			   if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			   {
					// Take all the folders that was chosen
					var folder = dlg.FileName;
					this.BackupDrive.Text = folder;
			   }
		  }

		  private void Button_Click_2(object sender, RoutedEventArgs e)
		  {
			   if (string.IsNullOrEmpty(this.BackupDrive.Text) || FolderList.Items.Count == 0)
			   {
					MessageBox.Show("Fill in the list of folder and hard drive!");
					return;
			   }

			   foreach (var listBoxItem in FolderList.Items)
			   {
					string folderFullPathToBackup = listBoxItem.ToString();

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = BackupDrive.Text + folderName;

					// Check if the folder already exists in the backup drive
					if (Directory.Exists(folderInBackupDrive))
					{
						 // Check if the folder has been modified
						 if (DateTime.Compare(Directory.GetLastWriteTime(folderFullPathToBackup), Directory.GetLastWriteTime(folderInBackupDrive)) > 0)
						 {
							  Debug.WriteLine("Need to be backed up!");
						 }
	
					}
					else
					{
						 CopyAll(folderFullPathToBackup, folderInBackupDrive);
						 Debug.WriteLine("end");
					}
					
					// Get all the path of all the files and folders
					List<string> Paths = GetFileFolderPaths(listBoxItem.ToString());

					foreach (var path in Paths)
					{
						
						 //Debug.WriteLine(path);
						 //Directory.Exists(path)
					}
					
			   }
			   
			   //Directory.GetDirectories();
		  }

		  #region Helper Functions

		  private void CopyAll(string source, string dst)
		  {
			   if (!Directory.Exists(dst))
			   {
					Directory.CreateDirectory(dst);
			   }

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(dst, ExtractFileFolderNameFromFullPath(file));
					File.Copy(file, fullFilePathInDst);
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dst, ExtractFileFolderNameFromFullPath(dir));
					CopyAll(dir, fullDirPathInDst);
			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// If the path of folder is in checkFolder than the folder is subfolder or the same
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="checkFolder">The folder to check if it is subfolder or the same</param>
		  /// <returns></returns>
		  private bool IsExistsOrSubFolder(string folder, string checkFolder)
		  {
			   return checkFolder.Contains(folder);
		  }

		  /// <summary>
		  /// Gives us the end of the path whether it's a file or a folder
		  /// </summary>
		  /// <param name="fullPath"></param>
		  /// <returns></returns>
		  private string ExtractFileFolderNameFromFullPath(string fullPath)
		  {
			   var normalizedPath = fullPath.Replace('\\', '/');

			   int lastSlash = normalizedPath.LastIndexOf('/');
			   
			   return normalizedPath.Substring(lastSlash + 1);
		  }

		  /// <summary>
		  /// Returns only the containg folders and files
		  /// </summary>
		  /// <param name="path"></param>
		  /// <returns></returns>
		  private List<string> GetFileFolderPaths(string path)
		  {
			   List<string> paths = new List<string>();

			   if (Directory.GetDirectories(path).Length == 0)
					return null;

			   foreach (var dir in Directory.GetDirectories(path))
			   {
					string folderPath = dir;
					// First add the current dir name
					paths.Add(folderPath);
					// Then add his sub folders
					//var list = GetFileFolderPaths(folderPath);
					//if (list != null)
					//{
					//paths.AddRange(list);
					//Debug.WriteLine(folderPath);
					//}

					//Debug.WriteLine(folderPath);
			   }

			   foreach (var file in Directory.GetFiles(path))
			   {
					string folderPath = file;
					// First add the current dir name
					paths.Add(folderPath);
			   }

			   return paths;
		  }

		  #endregion
	 }
}
