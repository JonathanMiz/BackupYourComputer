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
		  private FormModel formModel;
		  public MainWindow()
		  {
			   InitializeComponent();

			   // Create a model form object

			   formModel = new FormModel();

			   // Set values
			   formModel.AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");
			   formModel.backupFolder = "H:\\JonathanCompterBackup";

			   this.DataContext = formModel;

			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");

			   // Set default hard drive for backup
			   //this.BackupFolder.Text = formModel.backupFolder;

			  // this.FolderList.SetBinding(ListBox.ItemsSourceProperty, new Binding("folderPathsToBackup")
			  // {
					//Source = formModel,
					//Mode = BindingMode.TwoWay
			  // });

			  // this.BackupFolder.SetBinding(TextBox.TextProperty, new Binding("backupFolder")
			  // {
					//Source = formModel,
					//Mode = BindingMode.TwoWay
			  // });

			   //formModel.PropertyChanged += FormModel_PropertyChanged;

			   // Button events
			   this.browseFolderToBackupButton.Click += Button_Click;
			   this.browseBackupFolderButton.Click += Button_Click_1;
			   this.startBackupButton.Click += Button_Click_2;


		  }

		  //private void FormModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		  //{
			 //  if (e.PropertyName == formModel.FolderPathsToBackupPropertyName)
			 //  {
				//	this.FolderList.Items.Clear();
				//	foreach (var item in formModel.folderPathsToBackup)
				//	{
				//		 this.FolderList.Items.Add(item);
				//	}
			 //  }

			 //  if (e.PropertyName == formModel.BackupFolderPropertyName)
			 //  {
				//	this.BackupFolder.Text = formModel.backupFolder;
			 //  }
		  //}

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

					// Temp list to add all the folders, because we cannot add to list while iterating on it
					List<string> foldersToAddToListView = new List<string>();

					// Iterate through all of the folders that the user added
					foreach (var folderName in folders)
					{
						 // // Iterate through all of the folders that are already in our data
						 foreach (var folder in formModel.folderPathsToBackup)
						 {
							  if (formModel.folderPathsToBackup.Contains(folderName))
							  {
								   MessageBox.Show("The folder you are trying to add is already exists!");
								   break;
							  }

							  // Check to see if the new folder is not a sub folder of existing item
							  if (!IsSubFolder(folder.ToString(), folderName))
							  {
								   // Add to the list
								   foldersToAddToListView.Add(folderName);
							  }
							  else
							  {
								   // Break the loop, since if it exists we don't need to keep iterating
								   MessageBox.Show("The folder you are trying to add is a subfolder of an existing folder!");
								   break;
							  }

						 }

					}

					// TODO: Fix duplications of folders!!!!!
					foreach (var folder in foldersToAddToListView)
					{
						 formModel.AddFolderToBackUp(folder);
					}
			   }

		  }

		  private void Button_Click_1(object sender, RoutedEventArgs e)
		  {
			   var dlg = new CommonOpenFileDialog();
			   dlg.ResetUserSelections();
			   dlg.Title = "Choose folder to backup in hard drive";
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
					formModel.backupFolder = folder;
			   }
		  }

		  // TODO: Refactor
		  private void Button_Click_2(object sender, RoutedEventArgs e)
		  {
			   if (string.IsNullOrEmpty(this.formModel.backupFolder) || this.formModel.folderPathsToBackup.Count == 0)
			   {
					MessageBox.Show("Fill in the list of folder and hard drive!");
					return;
			   }

			   foreach (var folderPath in this.formModel.folderPathsToBackup)
			   {
					string folderFullPathToBackup = folderPath;

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = this.formModel.backupFolder + "\\" + folderName;

		  			Debug.WriteLine("Backup...");
					Backup(folderFullPathToBackup, folderInBackupDrive);
					Debug.WriteLine("End backup...");


					Debug.WriteLine("DeleteFiles...");
					DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
					Debug.WriteLine("End deletefiles...");
					
			   }
		  }

		  #region Helper Functions
		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest)
		  {
			   if (!Directory.Exists(dest))
			   {
					Directory.CreateDirectory(dest);
			   }

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(file));

					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);


						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  // Repalce
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  Debug.WriteLine("Replaced " + file + "...");
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 Debug.WriteLine("Copied " + file + "...");
					};
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));
					Backup(dir, fullDirPathInDst);
			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// Deletes the files that doesn't exist in dest
		  /// </summary>
		  /// <param name="source">The backup folder</param>
		  /// <param name="dest">The folder to backup</param>
		  /// TODO: Change the dest to be source in the parameters
		  private void DeleteFilesFromBackup(string source, string dest)
		  {

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(file));


					if (!File.Exists(fullFilePathInDest))
					{
						 Debug.WriteLine("Deleted " + file + "....");
						 // Delete the file
						 File.Delete(file);

					}
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));

					if (!Directory.Exists(fullFilePathInDest))
					{
						 Debug.WriteLine("Deleted folder " + fullFilePathInDest + " and all its subfolders and subfiles!");
						 Directory.Delete(dir, true);
					}
					else
					{
						 DeleteFilesFromBackup(dir, fullFilePathInDest);
					}

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// If the path of folder is in checkFolder than the folder is subfolder
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="checkFolder">The folder to check if it is subfolder</param>
		  /// <returns></returns>
		  /// Note(Jonathan): What if there is a folder names documents and a folder name documents-new
		  /// Bad implementation
		  private bool IsSubFolder(string folder, string checkFolder)
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

		  #endregion
	 }
}
