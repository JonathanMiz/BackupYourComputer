using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware
{
	 public class SelectSourceViewModel : ViewModelBase
	 {
		  #region Private Members

		  public ObservableCollection<FolderListItem> FolderItems
		  {
			   get
			   {
					return IoC.Get<CacheViewModel>().FolderItems;
			   }
			   set
			   {
					IoC.Get<CacheViewModel>().FolderItems = value;
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
			   dlg.Title = "Choose folder";
			   dlg.IsFolderPicker = true;
			   dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


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
								   if (IoC.Get<CacheViewModel>().FindFolderItemByString(FolderItems, newFolderToBackup) != null)
								   {
										MessageBox.Show(newFolderToBackup + " already exists in the list!");
										Added = false;
										break;
								   }

								   if (Helpers.IsSubFolder(exisitingFolderToBackup, newFolderToBackup))
								   {
										MessageBox.Show(newFolderToBackup + " is a subfolder of " + exisitingFolderToBackup + ".");
										Added = false;
										break;
								   }

								   if (Helpers.IsSubFolder(newFolderToBackup, exisitingFolderToBackup))
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
						 IoC.Get<CacheViewModel>().AddFolderToBackUp(folder);
					}

					if (foldersToRemoveToListView.Count > 0)
					{
						 MessageBox.Show("You added a parent folder, so all the subfolders are removed!");

						 foreach (var folder in foldersToRemoveToListView)
						 {
							  IoC.Get<CacheViewModel>().RemoveFolderToBackUp(folder);
						 }
					}
			   }
		  }

		  void Select()
		  {
			   IoC.Get<ApplicationViewModel>().CurrentViewModel = ViewModelLocator.DetailsViewModel;
		  }

		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public SelectSourceViewModel()
		  {
			   ChooseFolderCommand = new RelayCommand(ChooseFolder);
			   SelectButtonCommand = new RelayCommand(Select);
			   RemoveItemCommand = new RelayParameterizedCommand<string>(IoC.Get<CacheViewModel>().RemoveFolderToBackUp);
		  }
	 }
}
