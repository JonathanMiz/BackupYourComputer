using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackupSoftware
{
	 public class DetailsViewModel : ViewModelBase
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
					return IoC.Get<CacheViewModel>().BackupFolder;
			   }
			   set
			   {
					IoC.Get<CacheViewModel>().BackupFolder = value;
			   }
		  }

		  string ExtractFolderNames()
		  {
			   string list = "";
			   foreach (FolderListItem item in IoC.Get<CacheViewModel>().FolderItems)
			   {
					list += Helpers.ExtractFileFolderNameFromFullPath(item.FolderPath);
					list += ", ";
			   }
			   if(list != "")
					return list.Substring(0, list.Length - 2);
			   return list;
		  }

		  private DisplayViewModel _DisplayViewModel;

		  public DisplayViewModel DisplayViewModel
		  {
			   get { return _DisplayViewModel; }
			   set { if (_DisplayViewModel == value) return;  _DisplayViewModel = value; OnPropertyChanged(nameof(DisplayViewModel)); }
		  }


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

		  public RelayCommand ShowProgressCommand { get; set; }
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
			   IoC.Get<ApplicationViewModel>().CurrentViewModel = new SelectSourceViewModel();
		  }


		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public DetailsViewModel()
		  {
			   ChooseBackupFolderCommand = new RelayCommand(ChooseBackupFolder);
			   ChooseFoldersToBackup = new RelayCommand(ChooseFolder);
			   StartBackupCommand = new RelayCommand(StartBackup);
			   ShowProgressCommand = new RelayCommand(ShowProgress);

			   Folders = ExtractFolderNames();

			   DisplayViewModel = new DisplayViewModel();
		  }

		  private void ShowProgress()
		  {
			   if (IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					IoC.Get<ApplicationViewModel>().CurrentViewModel = DisplayViewModel;
			   }
		  }

		  // Refactor
		  private void StartBackup()
		  {
			   if (!IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					var FolderItems = IoC.Get<CacheViewModel>().FolderItems;
					var BackupFolder = IoC.Get<CacheViewModel>().BackupFolder;

					// Checks to see if there is content in the fields
					if (string.IsNullOrEmpty(BackupFolder) || FolderItems.Count == 0)
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

					if (!Directory.Exists(BackupFolder))
					{
						 MessageBox.Show(BackupFolder + " can not be found!");
						 return;
					}

					DisplayViewModel.RunBackup();
					IoC.Get<ApplicationViewModel>().CurrentViewModel = DisplayViewModel;
			   }
			   else
			   {
					MessageBox.Show("The buckup is already running!");
			   }
		  }
	 }
}
