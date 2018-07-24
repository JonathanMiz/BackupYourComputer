
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

			   //this.DataContext = ;

			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents\\BackupTest");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");

			   // Set default hard drive for backup
			   this.BackupDrive.Text = "H:\\JonathanCompterBackup";
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
						 foreach (var folder in this.FolderList.Items)
						 {
							  // Check to see if the new folder is not a sub folder of existing item
							  if (!IsExistsOrSubFolder(folder.ToString(), folderName))
							  {
								   foldersToAddToListView.Add(folderName);
							  }
							  else
							  {
								   break;
							  }

						 }

					}

					foreach (var folder in foldersToAddToListView)
					{
						 this.FolderList.Items.Add(folder);
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

					string folderInBackupDrive = BackupDrive.Text + "\\" + folderName;

					Debug.WriteLine("Backup...");
					CopyAll(folderFullPathToBackup, folderInBackupDrive);
					Debug.WriteLine("End backup...");

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

					Debug.WriteLine(fullFilePathInDst);
					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);


						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  Debug.WriteLine(file);
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
					};
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
		  /// Note(Jonathan): What if there is a folder names documents and a folder name documents-new
		  /// Bad implementation
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

		  #endregion
	 }
}
