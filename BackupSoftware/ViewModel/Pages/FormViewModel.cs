using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System;
using System.Xaml;
using System.Configuration;
using Ninject;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class FormViewModel : ViewModelBase
	 {
		  #region Private Members

		  /// <summary>
		  /// The dest folder to backup the folders
		  /// </summary>
		  private string m_backupFolder { get; set; }

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
					OnPropertyChanged(nameof(BackupFolder));
			   }
		  }

		  private string m_Folders { get; set; }

		  /// <summary>
		  /// A refrence for <see cref="m_backupFolder"/> in order for the binding to work
		  /// </summary>
		  public string Folders
		  {
			   get
			   {
					return m_Folders;
			   }
			   set
			   {
					if (m_Folders == value)
						 return;

					m_Folders = value;
					OnPropertyChanged(nameof(Folders));
			   }
		  }

		  string Test()
		  {
			   string list = "";
			   foreach (FolderListItem item in IoC.Kernel.Get<SelectedFoldersViewModel>().FolderItems)
			   {
					list += ExtractFileFolderNameFromFullPath(item.FolderPath);
					list += ", ";
			   }
			   return list.Substring(0, list.Length - 2);
		  }

		  //public void SaveSettings()
		  //{
			 //  XamlServices.Save(@"settings.xaml", m_folderItems);
			 //  Debugger.Break();
		  //}

		  //public void LoadSettings()
		  //{
			 //  if (File.Exists("settings.xaml"))
			 //  {
				//	m_folderItems = (ObservableCollection<FolderItem>)XamlServices.Load("settings.xaml");
				//	Debugger.Break();
			 //  }
		  //}

		  #endregion

		  #region Commands

		  /// <summary>
		  /// The command to choose backup folder
		  /// </summary>
		  public RelayCommand ChooseBackupFolderCommand { get; set; }
		  /// <summary>
		  /// The command to start the backup
		  /// </summary>
		  public RelayCommand StartBackupCommand { get; set; }
		  /// <summary>
		  /// The command to open the page of selecting folders to backup
		  /// </summary>
		  public RelayCommand ChooseFoldersToBackup { get; set; }
		  #endregion



		  #region Command Functions

		  /// <summary>
		  /// The action when the user choose backup folder
		  /// </summary>
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

		  void ChooseFolder()
		  {
			   IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.SelectFolders;
			   Folders = Test();
			   Debug.WriteLine(Folders);
		  }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  void StartBackup()
		  {
			   var FolderItems = IoC.Kernel.Get<SelectedFoldersViewModel>().FolderItems;
			   // Checks to see if there is content in the fields
			   if (string.IsNullOrEmpty(this.BackupFolder) || FolderItems.Count == 0)
			   {
					MessageBox.Show("Fill in the list of folder and hard drive!");
					return;
			   }

			   // Check to see if the folders exists
			   foreach (FolderListItem item in FolderItems)
			   {
					if (!Directory.Exists(item.FolderPath))
					{
						 MessageBox.Show(item.FolderPath + " can not be found!");
						 return;
					}

			   }

			   if (!Directory.Exists(this.BackupFolder))
			   {
					MessageBox.Show(BackupFolder + " can not be found!");
					return;
			   }



			   foreach (FolderListItem item in FolderItems)
			   {
					string folderFullPathToBackup = item.FolderPath;

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = this.BackupFolder + "\\" + folderName;

					Debug.WriteLine("Start backing up " + folderFullPathToBackup + " to " + folderInBackupDrive + "...");
					Backup(folderFullPathToBackup, folderInBackupDrive);
					Debug.WriteLine("End backing up " + folderFullPathToBackup + " to " + folderInBackupDrive + "...");


					if (item.DeletePrevContent)
					{
						 Debug.WriteLine("Start deleting previous content of " + folderInBackupDrive + "...");
						 DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
						 Debug.WriteLine("End deleting previous content of " + folderInBackupDrive + "...");
					}

			   }
		  }

		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public FormViewModel()
		  {
			   ChooseBackupFolderCommand = new RelayCommand(ChooseBackupFolder);
			   ChooseFoldersToBackup = new RelayCommand(ChooseFolder);
			   StartBackupCommand = new RelayCommand(StartBackup);
		  }

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
					Debug.WriteLine("Created new folder " + dest);
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
							  Debug.WriteLine("The file " + file + " has been modified, replacing it with new content in " + fullFilePathInDst + "...");
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 Debug.WriteLine("Copying " + file + "...");
					};
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));
					Debug.WriteLine("Backing up " + dir + " to " + fullDirPathInDst);
					Backup(dir, fullDirPathInDst);
					Debug.WriteLine("Ended backing up " + dir + " to " + fullDirPathInDst);
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
						 Debug.WriteLine("Deleted folder " + dir + " and all its subfolders and subfiles!");
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
		  /// If subfolder is sub folder of folder
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="subFolder">The folder to check if it is subfolder</param>
		  /// <returns></returns>
		  private bool IsSubFolder(string folder, string subFolder)
		  {
			   string normailzedFolder = folder.Replace('\\', '/') + '/';
			   string normlizedSubFolder = subFolder.Replace('\\', '/') + '/';

			   bool result = normlizedSubFolder.Contains(normailzedFolder);
			   return result;
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
