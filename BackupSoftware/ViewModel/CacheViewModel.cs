using BackupSoftware.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware
{
	/// <summary>
	/// This class will hold all the data that needs to be passed between pages
	/// </summary>
    public class CacheViewModel : ViewModelBase
    {
		  #region Private Members
		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// </summary>
		  private ObservableCollection<FolderListItem> _SourceFolders = new ObservableCollection<FolderListItem>();

		  /// <summary>
		  /// Dialog service to display messages to the screen
		  /// </summary>
		  private IDialogService _DialogService;

		  /// <summary>
		  /// The dest folder to backup the folders
		  /// </summary>
		  private string _DestFolder { get; set; }

		  #endregion

		  #region Public Members

		  #region Variables
		  
		  public ObservableCollection<FolderListItem> SourceFolders
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

		  /// <summary>
		  /// True if the backup is running
		  /// </summary>
		  public bool IsBackupRunning { get; set; } = false;

		  #endregion

		  #region Functions

		  /// <summary>
		  /// Add new folder to <see cref="SourceFolders"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolder(string folder)
		  {
			   SourceFolders.Add(new FolderListItem(folder));
		  }

		  /// <summary>
		  /// Remove folder from <see cref="SourceFolders"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  /// TODO: Use dictionary instead?
		  public void RemoveFolder(string folder)
		  {
			   if(_DialogService.ShowYesNoMessageBox("Are you sure you want to remove this folder?", "Question"))
			   {
					// Find the item by the name of the folder
					FolderListItem item = FindFolderItemByString(SourceFolders, folder);

					// Check if the item was found
					if (item != null)
					{
						 // Remove the item from the list
						 SourceFolders.Remove(item);
					}
			   }
		  }

		  /// <summary>
		  /// Finds FolderListItem by folder name
		  /// </summary>
		  /// <param name="FolderItems"></param>
		  /// <param name="folder"></param>
		  /// <returns></returns>
		  public FolderListItem FindFolderItemByString(ObservableCollection<FolderListItem> FolderItems, string folder)
		  {
			   for (int i = 0; i < FolderItems.Count; ++i)
			   {
					FolderListItem item = FolderItems[i];
					if (item.FolderInfo.FullPath == folder)
					{
						 return item;
					}
			   }
			   return null;
		  }

		  #endregion

		  #endregion

		  public CacheViewModel(IDialogService dialogService)
		  {
			   _DialogService = dialogService;

			   // TEMP: Default values for tests
			   DestFolder = "H:\\";

			   AddFolder("C:\\Users\\Jonathan\\Documents\\BackupTest");
			   //AddFolder("C:\\Users\\Jonathan\\Documents\\Army");
			   //AddFolder("C:\\Users\\Jonathan\\Documents\\Blender");
			   //AddFolder("C:\\Users\\Jonathan\\Documents\\Books");
			   //AddFolder("C:\\Users\\Jonathan\\Documents\\boost_1_65_1");
		  }
	 }
}
