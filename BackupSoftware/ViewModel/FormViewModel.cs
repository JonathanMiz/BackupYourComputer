using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.IO;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class FormViewModel : ViewModelBase
	 {
		  #region Private Members
		  /// <summary>
		  /// The propery name of <see cref="BackupFolder"/>
		  /// </summary>
		  public string BackupFolderPropertyName { get; set; } = "BackupFolder";

		  /// <summary>
		  /// The propertry name of <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  public string FolderPathsToBackupPropertyName { get; set; } = "FolderList";

		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// In order for the view to update the changes that occured in <see cref="m_folderPathsToBackup"/> we need <see cref="ObservableCollection{T}"/>
		  /// </summary>
		  private ObservableCollection<string> m_folderPathsToBackup = new ObservableCollection<string>();

		  /// <summary>
		  /// The folder in which the user want to backup his files
		  /// </summary>
		  private string m_backupFolder;
		  #endregion

		  #region Public Members

		  /// <summary>
		  /// A refrence for <see cref="m_folderPathsToBackup"/> in order for the binding to work
		  /// </summary>
		  public ObservableCollection<string> FolderPathsToBackup
		  {
			   get
			   {
					return m_folderPathsToBackup;
			   }
			   set
			   {
					m_folderPathsToBackup = value;
					OnPropertyChanged(FolderPathsToBackupPropertyName);
			   }
		  }

		  /// <summary>
		  /// Add new folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolderToBackUp(string folder)
		  {
			   m_folderPathsToBackup.Add(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
		  }

		  /// <summary>
		  /// Remove folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void RemoveFolderToBackUp(string folder)
		  {
			   m_folderPathsToBackup.Remove(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
		  }

		  /// <summary>
		  /// A refrence for <see cref="m_backupFolder"/> in order for the binding to work
		  /// </summary>
		  public string BackupFolder
		  {
			   get
			   {
					return m_backupFolder;
			   }
			   set
			   {
					if (m_backupFolder == value)
						 return;

					m_backupFolder = value;
					OnPropertyChanged(BackupFolderPropertyName);
			   }
		  }


		  #endregion

		  #region Commands

		  public RelayCommand ChooseFolderCommand { get; set; }
		  public RelayCommand ChooseBackupFolderCommand { get; set; }
		  public RelayCommand StartBackupCommand { get; set; }

		   
		  #endregion

		  public FormViewModel()
		  {
			   ChooseFolderCommand = new RelayCommand(ChooseFolder);
			   ChooseBackupFolderCommand = new RelayCommand(ChooseBackupFolder);
			   StartBackupCommand = new RelayCommand(StartBackup);
		  }

		  #region Command Functions
		  void ChooseFolder()
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

					bool Added = false;

					// Iterate through all of the folders that the user added
					foreach (var folderName in folders)
					{
						 // // Iterate through all of the folders that are already in our data
						 for (int i = 0; i < FolderPathsToBackup.Count && !Added; ++i)
						 {
							  var folder = FolderPathsToBackup[i];
							  if (FolderPathsToBackup.Contains(folderName))
							  {
								   MessageBox.Show("The folder you are trying to add is already exists!");
								   break;
							  }

							  // Check to see if the new folder is not a sub folder of existing item
							  if (!IsSubFolder(folder.ToString(), folderName))
							  {
								   // Add to the list
								   foldersToAddToListView.Add(folderName);
								   Added = true;
							  }
							  else
							  {
								   // Break the loop, since if it exists we don't need to keep iterating
								   MessageBox.Show("The folder you are trying to add is a subfolder of an existing folder!");
								   break;
							  }

						 }

					}

					foreach (var folder in foldersToAddToListView)
					{
						 AddFolderToBackUp(folder);
					}
			   }
		  }

		  void ChooseBackupFolder()
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
					BackupFolder = folder;
			   }
		  }
		  
		  void StartBackup()
		  {
			   if (string.IsNullOrEmpty(this.BackupFolder) || this.FolderPathsToBackup.Count == 0)
			   {
					MessageBox.Show("Fill in the list of folder and hard drive!");
					return;
			   }

			   foreach (var folderPath in this.FolderPathsToBackup)
			   {
					string folderFullPathToBackup = folderPath;

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = this.BackupFolder + "\\" + folderName;

					Debug.WriteLine("Backup...");
					Backup(folderFullPathToBackup, folderInBackupDrive);
					Debug.WriteLine("End backup...");


					Debug.WriteLine("DeleteFiles...");
					DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
					Debug.WriteLine("End deletefiles...");

			   }
		  }

		  #endregion

		  #region Helpers
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
