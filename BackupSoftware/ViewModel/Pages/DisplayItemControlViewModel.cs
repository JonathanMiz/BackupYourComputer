using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	 public class DisplayItemControlViewModel : ViewModelBase
	 {
		  public static DisplayItemControlViewModel Instance => new DisplayItemControlViewModel("c:/test");

		  #region Private Members
			   private string m_Log { get; set; }
		  #endregion

		  #region Public Members

		  /// <summary>
		  /// The info of the folder: FolderPath, Name, ItemsCount
		  /// </summary>
		  public FolderInfo FolderInfo { get; set; }

		  /// <summary>
		  /// The backup destination
		  /// </summary>
		  public string Destination { get; set; }

		  /// <summary>
		  /// Progress object to update <see cref="ItemsRemainingCounter"/> and <see cref="ItemsCompletedCounter"/> while running a task
		  /// </summary>
		  public Progress<int> ItemsRemainingProgress { get; set; }

		  /// <summary>
		  /// Progress object to update <see cref="Log"/> while running a task
		  /// </summary>
		  public Progress<string> LogProgress { get; set; }

		  /// <summary>
		  /// True if the backup for the folder is done
		  /// </summary>
		  private bool _BackupDone;

		  public bool BackupDone
		  {
			   get { return _BackupDone; }
			   set { if (_BackupDone == value) return; _BackupDone = value; OnPropertyChanged(nameof(BackupDone));  }
		  }


		  /// <summary>
		  /// Text to display the user with the current status
		  /// </summary>
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
		  /// The variable to count files and folders that was backed up
		  /// </summary>
		  int m_ItemsCompletedCounter { get; set; } = 0;
		  public int ItemsCompletedCounter
		  {
			   get
			   {
					return m_ItemsCompletedCounter;
			   }
			   set
			   {
					if (m_ItemsCompletedCounter == value)
						 return;

					m_ItemsCompletedCounter = value;
					OnPropertyChanged(nameof(ItemsCompletedCounter));
			   }
		  }

		  /// <summary>
		  /// The variable to show how many items remains to backup
		  /// </summary>
		  int m_ItemsRemainingCounter { get; set; } = 0;
		  public int ItemsRemainingCounter
		  {
			   get
			   {
					return m_ItemsRemainingCounter;
			   }
			   set
			   {
					if (m_ItemsRemainingCounter == value)
						 return;

					m_ItemsRemainingCounter = value;
					OnPropertyChanged(nameof(ItemsRemainingCounter));
			   }
		  }

		  #endregion

		  public DisplayItemControlViewModel(string folderPath)
		  {
			   FolderInfo = new FolderInfo(folderPath);

			   ItemsRemainingProgress = new Progress<int>();
			   ItemsRemainingProgress.ProgressChanged += ItemsRemainingProgress_ProgressChanged;

			   LogProgress = new Progress<string>();
			   LogProgress.ProgressChanged += LogProgress_ProgressChanged;
		  }

		  private void LogProgress_ProgressChanged(object sender, string e)
		  {
			   Log = e;
		  }

		  private void ItemsRemainingProgress_ProgressChanged(object sender, int e)
		  {
			   ItemsCompletedCounter = (e * 100 / FolderInfo.ItemsCount);
			   ItemsRemainingCounter = FolderInfo.ItemsCount - e;
		  }

		  /// <summary>
		  /// Backing up one folder
		  /// </summary>
		  /// <param name="progress"></param>
		  /// <returns></returns>
		  private async Task BackupOneDirAsync(IProgress<int> progress)
		  {
			   string source = FolderInfo.FolderPath;
			   string dest = Destination;
			   string folderName = FolderInfo.Name;

			   if (!Directory.Exists(dest))
			   {
					Directory.CreateDirectory(dest);
					Log = $"Created new folder {dest}";
			   }

			   int count = 0;

			   // Handling the top directories
			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string src = dir;
					string name = Helpers.ExtractFileFolderNameFromFullPath(src);

					// Refactor slashes
					string dst = $"{Destination}\\{name}";

					Log = $"Start backing up {src} to { dst} {Environment.NewLine}";
					Log = await Task<string>.Run(() => { string log = ""; Backup(src, dst, ref log, LogProgress); return log; });
					Log = $"End backing up {src} to { dst} {Environment.NewLine}";


					count++;

					progress.Report(count);

			   }

			   // Handling the top files
			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(file));

					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);

						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  // Repalce
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  Log = $"The file {file} has been modified, replacing it with new content in {fullFilePathInDst}{Environment.NewLine}";
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 Log = $"Copying {file}{Environment.NewLine}";
					};

					count++;

					progress.Report(count);
			   }

			   Log = $"Backing up from {source} to {dest} have been completed successfuly!{Environment.NewLine}";

			   BackupDone = true;
		  }

		  /// <summary>
		  /// Public function to call <see cref="BackupOneDirAsync(IProgress{ProgressItemModel})"/> from outside of the class while using await
		  /// </summary>
		  /// <returns></returns>
		  public async Task StartBackup()
		  {
			   //if (item.DeletePrevContent)
			   //{
			   //	 Debug.WriteLine("Start deleting previous content of " + folderInBackupDrive + "...");
			   //	 DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
			   //	 Debug.WriteLine("End deleting previous content of " + folderInBackupDrive + "...");
			   //}

			   await BackupOneDirAsync(ItemsRemainingProgress);
		  }

		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest, ref string log, IProgress<string> logProgress)
		  {
			   if (!Directory.Exists(dest))
			   {
					Directory.CreateDirectory(dest);
					log = $"Created new folder {dest}";
					logProgress.Report(log);
			   }

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(file));

					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);

						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  // Repalce
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  log = $"The file {file} has been modified, replacing it with new content in {fullFilePathInDst}{Environment.NewLine}";
							  logProgress.Report(log);
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 log = $"Copying {file}{Environment.NewLine}";
						 logProgress.Report(log);
					};
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(dir));
					log = $"Backing up {dir} to {fullDirPathInDst}{Environment.NewLine}";
					logProgress.Report(log);
					Backup(dir, fullDirPathInDst, ref log, logProgress);
					log = $"Ended backing up {dir} to {fullDirPathInDst}{Environment.NewLine}";
					logProgress.Report(log);

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }
	 }
}
