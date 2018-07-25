using System.Collections.Generic;
using System.ComponentModel;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class FormModel : INotifyPropertyChanged
	 {
		  #region Private Members
		  /// <summary>
		  /// The propery name of <see cref="backupFolder"/>
		  /// </summary>
		  public string BackupFolderPropertyName { get; set; } = "BackupFolder";

		  /// <summary>
		  /// The propertry name of <see cref="folderPathsToBackup"/>
		  /// </summary>
		  public string FolderPathsToBackupPropertyName { get; set; } = "FolderList";
		  #endregion

		  #region Public Members
		  /// <summary>
		  /// The list of the folders the user want to backup
		  /// </summary>
		  public List<string> folderPathsToBackup = new List<string>();

		  /// <summary>
		  /// Add new folder to backup to <see cref="folderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void AddFolderToBackUp(string folder)
		  {
			   folderPathsToBackup.Add(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
		  }

		  /// <summary>
		  /// Remove folder to backup to <see cref="folderPathsToBackup"/>
		  /// </summary>
		  /// <param name="folder"></param>
		  public void RemoveFolderToBackUp(string folder)
		  {
			   folderPathsToBackup.Remove(folder);
			   OnPropertyChanged(FolderPathsToBackupPropertyName);
		  }

		  private string m_backupFolder;

		  /// <summary>
		  /// The folder in which the user want to backup his files
		  /// </summary>
		  public string backupFolder
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
					OnPropertyChanged(FolderPathsToBackupPropertyName);
			   }
		  }

		  public event PropertyChangedEventHandler PropertyChanged;

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
