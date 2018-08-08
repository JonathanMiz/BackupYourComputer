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

		  public string BackupFolder
		  {
			   get
			   {
					return IoC.Kernel.Get<CacheViewModel>().BackupFolder;
			   }
			   set
			   {
					IoC.Kernel.Get<CacheViewModel>().BackupFolder = value;
			   }
		  }

		  string Test()
		  {
			   string list = "";
			   foreach (FolderListItem item in IoC.Kernel.Get<CacheViewModel>().FolderItems)
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
		  }


		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public FormViewModel()
		  {
			   ChooseBackupFolderCommand = new RelayCommand(ChooseBackupFolder);
			   ChooseFoldersToBackup = new RelayCommand(ChooseFolder);
			   StartBackupCommand = new RelayCommand(() => { IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.BackupDisplay; });
		  }

		  #region Helpers
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
