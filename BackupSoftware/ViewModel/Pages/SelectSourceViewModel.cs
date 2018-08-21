using BackupSoftware.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BackupSoftware
{
	 public class SelectSourceViewModel : ViewModelBase
	 {
		  /// <summary>
		  /// The list of the folders, getting that from <see cref="CacheViewModel.SourceFolders"/>
		  /// </summary>
		  public ObservableCollection<FolderListItem> SourceFolders
		  {
			   get
			   {
					return IoC.Get<CacheViewModel>().SourceFolders;
			   }
			   set
			   {
					IoC.Get<CacheViewModel>().SourceFolders = value;
			   }
		  }

		  /// <summary>
		  /// Dialog service to display messages to the screen
		  /// </summary>
		  private IDialogService _DialogService;

		  #region Helpers
		  /// <summary>
		  /// Validate if the user selected folders that meets the reqiuerments
		  /// </summary>
		  /// <param name="newFolder">folder to check</param>
		  /// <param name="foldersToRemove">folders that need to be removed</param>
		  /// <returns></returns>
		  private bool ValidateFolders(string newFolder, List<string> foldersToRemove)
		  {
			   // Iterate through all of the folders that are already in our data
			   for (int i = 0; i < SourceFolders.Count; ++i)
			   {
					var exisitingFolderToBackup = SourceFolders[i].FolderInfo.FullPath.ToString();
					if (IoC.Get<CacheViewModel>().FindFolderItemByString(SourceFolders, newFolder) != null)
					{
						 _DialogService.ShowMessageBox(newFolder + " already exists in the list!");
						 return false;
					}

					if (Helpers.IsSubFolder(exisitingFolderToBackup, newFolder))
					{
						 _DialogService.ShowMessageBox(newFolder + " is a subfolder of " + exisitingFolderToBackup + ".");
						 return false;
					}

					if (Helpers.IsSubFolder(newFolder, exisitingFolderToBackup))
					{
						 foldersToRemove.Add(exisitingFolderToBackup);
					}
			   }

			   return true;
		  }

		  /// <summary>
		  /// Validate and add the user selected folders to the <see cref="CacheViewModel.SourceFolders"/>
		  /// </summary>
		  /// <param name="newFoldersToBackup"></param>
		  private void ValidateAndAddFolders(IEnumerable<string> newFoldersToBackup)
		  {
			   // All the folders that needs to be removed, if they are subfolders of the folder that was recently been added
			   List<string> foldersToRemove = new List<string>();


			   // If the list is not empty
			   if (SourceFolders.Count > 0)
			   {
					foreach (var newFolder in newFoldersToBackup)
					{
						 // A flag to check if the folder is fine to add   
						 bool CanAdd = ValidateFolders(newFolder, foldersToRemove);

						 if (CanAdd)
						 {
							  // Add to the list
							  IoC.Get<CacheViewModel>().AddFolder(newFolder);
						 }
					}

			   }
			   else
			   {
					foreach (var newFolder in newFoldersToBackup)
					{
						 IoC.Get<CacheViewModel>().AddFolder(newFolder);
					}
			   }

			   if (foldersToRemove.Count > 0)
			   {
					_DialogService.ShowMessageBox("You added a parent folder, so all the subfolders are removed!");

					foreach (var folder in foldersToRemove)
					{
						 IoC.Get<CacheViewModel>().RemoveFolder(folder);
					}
			   }
		  }

		  #endregion

		  #region Commands

		  /// <summary>
		  /// The command to select folder to backup   
		  /// </summary>
		  public ICommand SelectFoldersCommand { get; set; }
		  /// <summary>
		  /// The command to start the backup
		  /// </summary>
		  public ICommand GoToDetailsViewCommand { get; set; }
		  /// <summary>
		  /// The command to remove a list view item
		  /// </summary>
		  public ICommand RemoveItemCommand { get; set; }

		  #endregion

		  #region Command Functions

		  /// <summary>
		  ///  The action when the user choose folder
		  /// </summary>
		  private void SelectFolders()
		  {
			   var newFoldersToBackup = _DialogService.SelectFolders("Select folder / folders");

			   if (newFoldersToBackup != null)
			   {
					ValidateAndAddFolders(newFoldersToBackup);
			   }
		  }

		  /// <summary>
		  /// Redirect back to the <see cref="DetailsViewModel"/>
		  /// </summary>
		  private void GoToDetailsView()
		  {
			   IoC.Get<ApplicationViewModel>().CurrentViewModel = ViewModelLocator.DetailsViewModel;
		  }

		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public SelectSourceViewModel(IDialogService dialogService)
		  {
			   _DialogService = dialogService;

			   SelectFoldersCommand = new RelayCommand(SelectFolders);
			   GoToDetailsViewCommand = new RelayCommand(GoToDetailsView);
			   RemoveItemCommand = new RelayParameterizedCommand<string>(IoC.Get<CacheViewModel>().RemoveFolder);
		  }
	 }
}
