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
		  public static DisplayItemControlViewModel Instance => new DisplayItemControlViewModel(new SourceFolder("c:/jonathan/test"));

		  #region Private Members
			   private string _Log { get; set; }
		  #endregion

		  #region Public Members

		  /// <summary>
		  /// The info of the folder: FolderPath, Name, ItemsCount
		  /// </summary>
		  private SourceFolder _SourceFolder;

		  public SourceFolder SourceFolder
		  {
			   get { return _SourceFolder; }
			   set { _SourceFolder = value; }
		  }

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
					return _Log;
			   }
			   set
			   {
					if (_Log == value)
						 return;
					_Log = value;
					OnPropertyChanged(nameof(Log));
			   }
		  }

		  /// <summary>
		  /// The variable to count files and folders that was backed up
		  /// </summary>
		  int _ItemsCompletedCounter { get; set; } = 0;
		  public int ItemsCompletedCounter
		  {
			   get
			   {
					return _ItemsCompletedCounter;
			   }
			   set
			   {
					if (_ItemsCompletedCounter == value)
						 return;

					_ItemsCompletedCounter = value;
					OnPropertyChanged(nameof(ItemsCompletedCounter));
			   }
		  }

		  /// <summary>
		  /// The variable to show how many items remains to backup
		  /// </summary>
		  int _ItemsRemainingCounter { get; set; } = 0;
		  public int ItemsRemainingCounter
		  {
			   get
			   {
					return _ItemsRemainingCounter;
			   }
			   set
			   {
					if (_ItemsRemainingCounter == value)
						 return;

					_ItemsRemainingCounter = value;
					OnPropertyChanged(nameof(ItemsRemainingCounter));
			   }
		  }

		  #endregion

		  public DisplayItemControlViewModel(SourceFolder sourceFolder)
		  {
			   SourceFolder = sourceFolder;

			   ItemsRemainingProgress = new Progress<int>();
			   ItemsRemainingProgress.ProgressChanged += ItemsRemainingProgress_ProgressChanged;

			   LogProgress = new Progress<string>();
			   LogProgress.ProgressChanged += LogProgress_ProgressChanged;

			   ItemsRemainingCounter = SourceFolder.FolderInfo.ItemsCount;
		  }

		  private void LogProgress_ProgressChanged(object sender, string e)
		  {
			   Log = e;
		  }

		  private void ItemsRemainingProgress_ProgressChanged(object sender, int e)
		  {
			   ItemsCompletedCounter = (e * 100 / SourceFolder.FolderInfo.ItemsCount);
			   ItemsRemainingCounter = SourceFolder.FolderInfo.ItemsCount - e;
		  }

		  /// <summary>
		  /// Backing up one folder
		  /// </summary>
		  /// <param name="progress"></param>
		  /// <returns></returns>
		  private async Task BackupOneDirAsync(IProgress<int> progress)
		  {
			   string source = SourceFolder.FolderInfo.FullPath;
			   string dest = Destination;
			   string folderName = SourceFolder.FolderInfo.Name;

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
					await Task.Run(() => { Backup(src, dst, LogProgress);});
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
		  /// Starts the backup
		  /// </summary>
		  /// <returns></returns>
		  public async Task StartBackup()
		  {

			   // First check if the user wants to delete pervious content
			   if (SourceFolder.DeletePrevContent)
			   {
					Log = $"Deleting previous content from {Destination}...{Environment.NewLine}";
					await Task.Run(() => { DeleteFilesFromBackup(Destination, SourceFolder.FolderInfo.FullPath, LogProgress); });
			   }

			   await BackupOneDirAsync(ItemsRemainingProgress);
		  }

		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest, IProgress<string> logProgress)
		  {
			   string log = "";

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
					Backup(dir, fullDirPathInDst, logProgress);
					log = $"Ended backing up {dir} to {fullDirPathInDst}{Environment.NewLine}";
					logProgress.Report(log);

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// Deletes the files that doesn't exist in dest
		  /// </summary>
		  /// <param name="source">The backup folder</param>
		  /// <param name="dest">The folder to backup</param>
		  /// TODO: Change the dest to be source in the parameters
		  private void DeleteFilesFromBackup(string source, string dest, IProgress<string> logProgress)
		  {
			   string log = "";

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(file));


					if (!File.Exists(fullFilePathInDest))
					{

						 log = $"Deleted {file}...";
						 logProgress.Report(log);
						 // Delete the file
						 File.Delete(file);

					}
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(dir));

					if (!Directory.Exists(fullFilePathInDest))
					{
						 log = $"Deleted folder {dir} and all its subfolders and subfiles!";
						 logProgress.Report(log);
						 Directory.Delete(dir, true);
					}
					else
					{
						 DeleteFilesFromBackup(dir, fullFilePathInDest, logProgress);
					}

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }
	 }
}
