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
		  /// In order for the view to update the changes that occured in <see cref="m_folderItems"/> we need <see cref="ObservableCollection{T}"/>
		  /// </summary>
		  private ObservableCollection<FolderListItem> m_folderItems = new ObservableCollection<FolderListItem>();


		  /// <summary>
		  /// The dest folder to backup the folders
		  /// </summary>
		  private string m_backupFolder { get; set; }

		  #endregion

		  #region Public Members
		  /// <summary>
		  /// A refrence for <see cref="m_folderItems"/> in order for the binding to work
		  /// </summary>
		  public ObservableCollection<FolderListItem> FolderItems
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
			   FolderItems.Add(new FolderListItem(folder, false));
			   OnPropertyChanged(nameof(FolderItems));
		  }

		  /// <summary>
		  /// Remove folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void RemoveFolderToBackUp(string folder)
		  {
			   if (MessageBox.Show("Are you sure you want to remove this folder?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			   {
					FolderListItem item = FindFolderItemByString(FolderItems, folder);
					if (item != null)
					{
						 FolderItems.Remove(item);
						 OnPropertyChanged(nameof(FolderItems));
					}
			   }
		  }

		  public FolderListItem FindFolderItemByString(ObservableCollection<FolderListItem> FolderItems, string folder)
		  {
			   for (int i = 0; i < FolderItems.Count; ++i)
			   {
					FolderListItem item = FolderItems[i];
					if (item.FolderPath == folder)
					{
						 return item;
					}
			   }
			   return null;
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
					OnPropertyChanged(nameof(BackupFolder));
			   }
		  }

		  public CacheViewModel()
		  {
			   // TEMP: Default values for tests
			   BackupFolder = "H:\\";

			   AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");

			   // Set default values
			   //formViewModel.AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");

			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");
		  }

		  #endregion
	 }
}
