using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 /// <summary>
	 /// All the details the user gives to the software
	 /// The folders to backup and the destination
	 /// </summary>
	 public class Details : INotifyPropertyChanged
	 {
		  public event PropertyChangedEventHandler PropertyChanged;

		  private void OnPropertyChanged(string propertyName)
		  {
			   if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		  }

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
	 }
}
