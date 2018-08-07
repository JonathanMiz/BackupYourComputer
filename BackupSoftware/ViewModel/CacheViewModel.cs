using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			   // TEMP: Default value for tests
			   BackupFolder = "C:\\";
		  }

		  #endregion
	 }
}
