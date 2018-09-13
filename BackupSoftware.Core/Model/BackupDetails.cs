using BackupSoftware.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 /// <summary>
	 /// All the details the user gives to the software
	 /// The folders to backup and the destination
	 /// </summary>
	 public class BackupDetails : ObservableObject
	 {
		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// </summary>
		  private ObservableCollection<SourceFolder> _SourceFolders = new ObservableCollection<SourceFolder>();

		  public ObservableCollection<SourceFolder> SourceFolders
		  {
			   get
			   {
					return _SourceFolders;
			   }
			   set
			   {
					if (_SourceFolders == value)
						 return;

					_SourceFolders = value;
					OnPropertyChanged(nameof(SourceFolders));
			   }
		  }

		  /// <summary>
		  /// The dest folder to backup the folders
		  /// </summary>
		  private string _DestFolder { get; set; }
		  public string DestFolder
		  {
			   get
			   {
					return _DestFolder;
			   }
			   set
			   {
					if (_DestFolder == value)
						 return;

					_DestFolder = value;
					OnPropertyChanged(nameof(DestFolder));
			   }
		  }

		  private DateTime _LastBackupTime = new DateTime();

		  public DateTime LastBackupTime
		  {
			   get { return _LastBackupTime; }
			   set
			   {
					if (_LastBackupTime == value)
						 return;

					_LastBackupTime = value;
					OnPropertyChanged(nameof(LastBackupTime));
			   }
		  }


		  /// <summary>
		  /// Add new folder to <see cref="SourceFolders"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolder(string folder)
		  {
			   SourceFolders.Add(new SourceFolder(folder));
		  }

		  /// <summary>
		  /// Remove folder from <see cref="SourceFolders"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  /// TODO: Use dictionary instead?
		  public void RemoveFolder(string folder)
		  {

			   // Find the item by the name of the folder
			   SourceFolder item = SourceFolders.Where(sf => sf.FolderInfo.FullPath == folder).FirstOrDefault();


			   // Check if the item was found
			   if (item != null)
			   {
					// Remove the item from the list
					SourceFolders.Remove(item);
			   }
		  }

		  bool IsEmptyDetails()
		  {
			   return (string.IsNullOrEmpty(DestFolder) || SourceFolders.Count == 0);
		  }

		  bool IsFoldersExist(IDialogService dialogService)
		  {
			   foreach (SourceFolder item in SourceFolders)
			   {
					if (!Directory.Exists(item.FolderInfo.FullPath))
					{
						 dialogService.ShowMessageBox(item.FolderInfo.FullPath + " can not be found!");
						 return false;
					}
			   }
			   return true;
		  }
		  
		  /// <summary>
		  /// Validates if the user gave right input
		  /// </summary>
		  public bool Validate(IDialogService dialogService)
		  {
			   // Checks to see if there is content in the fields
			   if (IsEmptyDetails())
			   {
					dialogService.ShowMessageBox("Fill in the list of folders and hard drive!");
					return false;
			   }

			   // Check to see if the folders exists
			   if(!IsFoldersExist(dialogService))
			   {
					return false;
			   }

			   if (!Directory.Exists(DestFolder))
			   {
					dialogService.ShowMessageBox(DestFolder + " can not be found!");
					return false;
			   }

			   return true;
		  }
	 }
}
