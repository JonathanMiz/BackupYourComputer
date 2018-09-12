using BackupSoftware.Core.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 public class DisplayItemControlViewModel : ViewModelBase
	 {
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
		  /// The backup destination path
		  /// </summary>
		  public string DestinationPath { get; set; }

		  /// <summary>
		  /// Progress object to update <see cref="ItemsRemainingCounter"/> and <see cref="ItemsCompletedCounter"/> while running a task
		  /// </summary>
		  public Progress<int> ItemsProgress { get; set; }

		  /// <summary>
		  /// Progress object to update <see cref="State"/> while running a task
		  /// </summary>
		  public Progress<string> StateProgress { get; set; }


		  /// <summary>
		  /// True if the backup for the folder is done
		  /// </summary>
		  private bool _IsBackupDone;

		  public bool IsBackupDone
		  {
			   get { return _IsBackupDone; }
			   set { if (_IsBackupDone == value) return; _IsBackupDone = value; OnPropertyChanged(nameof(IsBackupDone)); }
		  }


		  /// <summary>
		  /// Text to display the user with the current status
		  /// </summary>
		  private string _State { get; set; } = "";
		  public string State
		  {
			   get
			   {
					return _State;
			   }
			   set
			   {
					if (_State == value)
						 return;
					_State = value;
					OnPropertyChanged(nameof(State));
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

		  private IDialogService _DialogService;

		  #endregion

		  public DisplayItemControlViewModel(IDialogService dialogService, SourceFolder sourceFolder, string destPath)
		  {
			   _DialogService = dialogService;
			   SourceFolder = sourceFolder;
			   DestinationPath = destPath;


			   ItemsProgress = new Progress<int>();
			   ItemsProgress.ProgressChanged += ItemsProgress_ProgressChanged;

			   StateProgress = new Progress<string>();
			   StateProgress.ProgressChanged += StateProgress_ProgressChanged;

			   ItemsRemainingCounter = SourceFolder.FolderInfo.ItemsCount;
		  }

		  private void StateProgress_ProgressChanged(object sender, string e)
		  {
			   State = e;
		  }

		  private void ItemsProgress_ProgressChanged(object sender, int e)
		  {
			   ItemsCompletedCounter = (e * 100 / SourceFolder.FolderInfo.ItemsCount);
			   ItemsRemainingCounter = SourceFolder.FolderInfo.ItemsCount - e;
		  }

		  /// <summary>
		  /// Backing up one folder
		  /// </summary>
		  /// <param name="itemsProgress"></param>
		  /// <returns></returns>
		  private async Task BackupOneDirAsync()
		  {
			   string source = SourceFolder.FolderInfo.FullPath;
			   string dest = DestinationPath;
			   string folderName = SourceFolder.FolderInfo.Name;

			   IProgress<string> stateProgress = StateProgress as IProgress<string>;
			   IProgress<int> itemsProgress = ItemsProgress as IProgress<int>;

			   if (Helpers.CreateDirectoryIfNotExists(dest))
			   {
					stateProgress.Report($"Created new folder {dest}");
			   }

			   int count = 0;

			   // Handling the top directories
			   foreach (var src in Directory.GetDirectories(source))
			   {
					string name = Helpers.ExtractFileFolderNameFromFullPath(src);

					// Refactor slashes
					string dst = $"{DestinationPath}\\{name}";

					stateProgress.Report($"Start backing up {src} to { dst} {Environment.NewLine}");

					await Task.Run(() => { Backup(src, dst); });

					stateProgress.Report($"End backing up {src} to { dst} {Environment.NewLine}");


					count++;
					itemsProgress.Report(count);

			   }

			   // Handling the top files
			   foreach (var file in Directory.GetFiles(source))
			   {
					BackupFile(dest, file);

					count++;
					itemsProgress.Report(count);
			   }

			   string successMessage = $"Backing up from {source} to {dest} have been completed successfuly!";
			   stateProgress.Report(successMessage);
			   ReportFile.WriteToLog(successMessage);

			   IsBackupDone = true;
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
					State = $"Deleting previous content from {DestinationPath}...{Environment.NewLine}";
					await Task.Run(() => { DeleteFilesFromBackup(DestinationPath, SourceFolder.FolderInfo.FullPath, StateProgress); });
			   }

			   await BackupOneDirAsync();
		  }

		  private void ReplaceFile(string sourceFile, string destFile)
		  {
			   File.Delete(destFile);
			   File.Copy(sourceFile, destFile);
		  }

		  private bool IsFileSizeInDestModified(string DestFilePath, string SourceFilePath)
		  {
			   FileInfo sourceFileInfo = new FileInfo(SourceFilePath);
			   FileInfo destFileInfo = new FileInfo(DestFilePath);

			   if (sourceFileInfo.Length != destFileInfo.Length)
			   {
					return true;
			   }

			   return false;
		  }

		  private bool IsFileLastWriteTimeInDestModified(string DestFilePath, string SourceFilePath)
		  {
			   FileInfo sourceFileInfo = new FileInfo(SourceFilePath);
			   FileInfo destFileInfo = new FileInfo(DestFilePath);

			   if (sourceFileInfo.LastWriteTime != destFileInfo.LastWriteTime)
			   {
					return true;
			   }

			   return false;
		  }

		  private bool CheckIfFileModified(string destFolder, string fileToCopy)
		  {
			   return (IsFileSizeInDestModified(destFolder, fileToCopy) || IsFileLastWriteTimeInDestModified(destFolder, fileToCopy));
		  }

		  private void BackupFile(string destFolder, string fileToCopy)
		  {
			   IProgress<string> stateProgress = StateProgress as IProgress<string>;

			   string DestFilePath = System.IO.Path.Combine(destFolder, Helpers.ExtractFileFolderNameFromFullPath(fileToCopy));

			   if (File.Exists(DestFilePath))
			   {
					if (CheckIfFileModified(DestFilePath, fileToCopy))
					{
						 ReplaceFile(fileToCopy, DestFilePath);

						 stateProgress.Report($"The file {fileToCopy} has been modified, replacing it with new content in {DestFilePath}{Environment.NewLine}");
					}
					else
					{
						 stateProgress.Report($"The file {fileToCopy} already exists in {DestFilePath}{Environment.NewLine}");
					}
			   }
			   else
			   {
					try
					{
						 File.Copy(fileToCopy, DestFilePath);
						 stateProgress.Report($"Copying {fileToCopy}{Environment.NewLine}");
					}
					catch (IOException excp)
					{
						 _DialogService.ShowMessageBox(excp.Message);
						 ReportFile.WriteToLog(excp.Message);
					}
			   };
		  }


		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest)
		  {
			   try
			   {
					IProgress<string> stateProgress = StateProgress as IProgress<string>;

					if (Helpers.CreateDirectoryIfNotExists(dest))
					{
						 stateProgress.Report($"Created new folder {dest}");
					}

					foreach (var file in Directory.GetFiles(source))
					{
						 BackupFile(dest, file);
					}

					foreach (var dir in Directory.GetDirectories(source))
					{
						 string fullDirPathInDst = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(dir));

						 stateProgress.Report($"Backing up {dir} to {fullDirPathInDst}{Environment.NewLine}");

						 Backup(dir, fullDirPathInDst);

						 stateProgress.Report($"Ended backing up {dir} to {fullDirPathInDst}{Environment.NewLine}");
					}

					if (Directory.GetDirectories(source).Length == 0)
						 return;
			   }
			   catch (Exception e)
			   {
					Directory.Delete(dest);
					_DialogService.ShowMessageBox(e.Message);
					ReportFile.WriteToLog(e.Message);
					return;
			   }
		  }

		  /// <summary>
		  /// Deletes the files that doesn't exist in dest
		  /// </summary>
		  /// <param name="source">The backup folder</param>
		  /// <param name="dest">The folder to backup</param>
		  /// TODO: Change the dest to be source in the parameters
		  private void DeleteFilesFromBackup(string source, string dest, IProgress<string> stateProgress)
		  {

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(file));


					if (!File.Exists(fullFilePathInDest))
					{
						 stateProgress.Report($"Deleted {file}...");

						 File.Delete(file);
					}
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(dir));

					if (!Directory.Exists(fullFilePathInDest))
					{
						 stateProgress.Report($"Deleted folder {dir} and all its subfolders and subfiles!");

						 Directory.Delete(dir, true);
					}
					else
					{
						 DeleteFilesFromBackup(dir, fullFilePathInDest, stateProgress);
					}

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }
	 }
}