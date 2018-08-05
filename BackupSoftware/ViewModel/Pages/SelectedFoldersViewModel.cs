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
using System.Threading.Tasks;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class SelectedFoldersViewModel : ViewModelBase
	 {
		  #region Private Members

		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// In order for the view to update the changes that occured in <see cref="m_folderItems"/> we need <see cref="ObservableCollection{T}"/>
		  /// </summary>
		  private ObservableCollection<FolderItem> m_folderItems = new ObservableCollection<FolderItem>();

		  private string m_Log { get; set; }

		  #endregion

		  #region Public Members

		  public string Log
		  {
			   get
			   {
					return m_Log;
			   }
			   set
			   {
					if (m_Log == value)
						 return;

					m_Log = value;
					OnPropertyChanged(nameof(Log));
			   }
		  }


		  /// <summary>
		  /// A refrence for <see cref="m_folderItems"/> in order for the binding to work
		  /// </summary>
		  public ObservableCollection<FolderItem> FolderItems
		  {
			   get
			   {
					return m_folderItems;
			   }
			   set
			   {
					m_folderItems = value;
					OnPropertyChanged(nameof(FolderItems));
			   }
		  }

		  /// <summary>
		  /// Add new folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolderToBackUp(string folder)
		  {
			   m_folderItems.Add(new FolderItem(folder, false));
			   OnPropertyChanged(nameof(FolderItems));
		  }

		  /// <summary>
		  /// Remove folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void RemoveFolderToBackUp(string folder)
		  {
			   FolderItem item = FindFolderItemByString(m_folderItems, folder);
			   if (item != null)
			   {
					m_folderItems.Remove(item);
					OnPropertyChanged(nameof(FolderItems));
			   }
		  }

		  #endregion

		  #region Commands

		  /// <summary>
		  /// The command to choose folder to backup
		  /// </summary>
		  public RelayCommand ChooseFolderCommand { get; set; }
		  /// <summary>
		  /// The command to start the backup
		  /// </summary>
		  public RelayCommand SelectButtonCommand { get; set; }
		  /// <summary>
		  /// The command to remove a list view item
		  /// </summary>
		  public RelayParameterizedCommand<string> RemoveItemCommand { get; set; }

		  #endregion



		  #region Command Functions

		  /// <summary>
		  ///  The action when the user choose folder add to the <see cref="FolderPathsToBackup"/>
		  /// </summary>
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
					var newFoldersToBackup = dlg.FileNames;

					// Temp list to add all the folders, because we cannot add to list while iterating on it
					List<string> foldersToAddToListView = new List<string>();

					// All the folders that needs to be removed, if they are subfolders of the new added folder
					List<string> foldersToRemoveToListView = new List<string>();


					// If the list is not empty
					if (FolderItems.Count > 0)
					{
						 foreach (var newFolderToBackup in newFoldersToBackup)
						 {
							  bool Added = true;
							  // // Iterate through all of the folders that are already in our data
							  for (int i = 0; i < FolderItems.Count; ++i)
							  {
								   var exisitingFolderToBackup = FolderItems[i].FolderPath.ToString();
								   if (FindFolderItemByString(FolderItems, newFolderToBackup) != null)
								   {
										MessageBox.Show(newFolderToBackup + " already exists in the list!");
										Added = false;
										break;
								   }

								   if (IsSubFolder(exisitingFolderToBackup, newFolderToBackup))
								   {
										MessageBox.Show(newFolderToBackup + " is a subfolder of " + exisitingFolderToBackup + ".");
										Added = false;
										break;
								   }

								   if (IsSubFolder(newFolderToBackup, exisitingFolderToBackup))
								   {
										foldersToRemoveToListView.Add(exisitingFolderToBackup);
								   }
							  }

							  if (Added)
							  {
								   // Add to the list
								   foldersToAddToListView.Add(newFolderToBackup);
							  }
						 }

					}
					else
					{
						 foreach (var newFolderToBackup in newFoldersToBackup)
						 {
							  foldersToAddToListView.Add(newFolderToBackup);
						 }
					}


					foreach (var folder in foldersToAddToListView)
					{
						 AddFolderToBackUp(folder);
					}

					if (foldersToRemoveToListView.Count > 0)
					{
						 MessageBox.Show("You added a parent folder, so all the subfolders are removed!");

						 foreach (var folder in foldersToRemoveToListView)
						 {
							  RemoveFolderToBackUp(folder);
						 }
					}
			   }
		  }

		 void Select()
		  {
			   Log = "Calculating folders' size...";

			   for(int i = 0; i < FolderItems.Count; ++i)
			   {
					FolderItem item = FolderItems[i];
					int fileCount = Directory.GetFiles(item.FolderPath, "*.*", SearchOption.TopDirectoryOnly).Length;
					int folderCount = Directory.GetDirectories(item.FolderPath, "*.*", SearchOption.TopDirectoryOnly).Length;


					item.ContentCount = fileCount + folderCount;
					
			   }

			   Log = "Transfering you to backup page...";
			   IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.BackupDetailsForm;
		  }

		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public SelectedFoldersViewModel()
		  {
			   ChooseFolderCommand = new RelayCommand(ChooseFolder);
			   SelectButtonCommand = new RelayCommand(Select);
			   RemoveItemCommand = new RelayParameterizedCommand<string>(RemoveFolderToBackUp);

			   AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");
		  }

		  #region Helpers

		  /// <summary>
		  /// If subfolder is sub folder of folder
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="subFolder">The folder to check if it is subfolder</param>
		  /// <returns></returns>
		  /// Note(Jonathan): What if there is a folder names documents and a folder name documents-new
		  /// Bad implementation
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


		  public FolderItem FindFolderItemByString(ObservableCollection<FolderItem> FolderItems, string folder)
		  {
			   for (int i = 0; i < FolderItems.Count; ++i)
			   {
					FolderItem item = FolderItems[i];
					if (item.FolderPath == folder)
					{
						 return item;
					}
			   }
			   return null;
		  }

		  #endregion

	 }
}
