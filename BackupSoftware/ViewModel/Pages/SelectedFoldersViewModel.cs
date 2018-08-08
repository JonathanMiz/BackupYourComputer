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

		  public ObservableCollection<FolderListItem> FolderItems
		  {
			   get
			   {
					return IoC.Kernel.Get<CacheViewModel>().FolderItems;
			   }
			   set
			   {
					IoC.Kernel.Get<CacheViewModel>().FolderItems = value;
			   }
		  }

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
								   if (IoC.Kernel.Get<CacheViewModel>().FindFolderItemByString(FolderItems, newFolderToBackup) != null)
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
						 IoC.Kernel.Get<CacheViewModel>().AddFolderToBackUp(folder);
					}

					if (foldersToRemoveToListView.Count > 0)
					{
						 MessageBox.Show("You added a parent folder, so all the subfolders are removed!");

						 foreach (var folder in foldersToRemoveToListView)
						 {
							  IoC.Kernel.Get<CacheViewModel>().RemoveFolderToBackUp(folder);
						 }
					}
			   }
		  }

		 void Select()
		  {
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
			   RemoveItemCommand = new RelayParameterizedCommand<string>(IoC.Kernel.Get<CacheViewModel>().RemoveFolderToBackUp);
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

		  #endregion

	 }
}
