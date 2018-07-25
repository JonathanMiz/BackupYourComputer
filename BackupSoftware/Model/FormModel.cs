using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class FormModel : INotifyPropertyChanged
	 {
		  #region Private Members
		  /// <summary>
		  /// The propery name of <see cref="BackupFolder"/>
		  /// </summary>
		  public string BackupFolderPropertyName { get; set; } = "BackupFolder";

		  /// <summary>
		  /// The propertry name of <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  public string FolderPathsToBackupPropertyName { get; set; } = "FolderList";

		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// In order for the view to update the changes that occured in <see cref="m_folderPathsToBackup"/> we need <see cref="ObservableCollection{T}"/>
		  /// </summary>
		  private ObservableCollection<string> m_folderPathsToBackup = new ObservableCollection<string>();

		  /// <summary>
		  /// The folder in which the user want to backup his files
		  /// </summary>
		  private string m_backupFolder;
		  #endregion

		  #region Public Members

		  /// <summary>
		  /// A refrence for <see cref="m_folderPathsToBackup"/> in order for the binding to work
		  /// </summary>
		  public ObservableCollection<string> FolderPathsToBackup
		  {
			   get
			   {
					return m_folderPathsToBackup;
			   }
			   set
			   {
					m_folderPathsToBackup = value;
					OnPropertyChanged(FolderPathsToBackupPropertyName);
			   }
		  }

		  /// <summary>
		  /// Add new folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolderToBackUp(string folder)
		  {
			   m_folderPathsToBackup.Add(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
		  }

		  /// <summary>
		  /// Remove folder to backup to <see cref="FolderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void RemoveFolderToBackUp(string folder)
		  {
			   m_folderPathsToBackup.Remove(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
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
					OnPropertyChanged(BackupFolderPropertyName);
			   }
		  }

		  public event PropertyChangedEventHandler PropertyChanged;

		  /// <summary>
		  /// Notify that a property has been changed
		  /// </summary>
		  /// <param name="propertyName"></param>
		  protected void OnPropertyChanged(string propertyName)
		  {
			   if(PropertyChanged != null)
			   {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			   }
		  }
		  #endregion
	 }
}
