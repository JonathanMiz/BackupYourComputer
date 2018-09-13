using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core.Services
{
	 public class BackupService : IBackupService
	 {
		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  public void Backup(string source, string dest, BackupStatus backupStatus)
		  {
			   try
			   {
					IProgress<string> stateProgress = backupStatus.StateProgress as IProgress<string>;

					if (Helpers.CreateDirectoryIfNotExists(dest))
					{
						 stateProgress.Report($"Created new folder {dest}");
					}

					foreach (var file in Directory.GetFiles(source))
					{
						 BackupFile(dest, file, backupStatus);
					}

					foreach (var dir in Directory.GetDirectories(source))
					{
						 string fullDirPathInDst = System.IO.Path.Combine(dest, Helpers.ExtractFileFolderNameFromFullPath(dir));

						 stateProgress.Report($"Backing up {dir} to {fullDirPathInDst}{Environment.NewLine}");

						 Backup(dir, fullDirPathInDst, backupStatus);

						 stateProgress.Report($"Ended backing up {dir} to {fullDirPathInDst}{Environment.NewLine}");
					}

					if (Directory.GetDirectories(source).Length == 0)
						 return;
			   }
			   catch (Exception e)
			   {
					Directory.Delete(dest);
					backupStatus.DialogService.ShowMessageBox(e.Message);
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
		  public void DeleteFilesFromBackup(string source, string dest, BackupStatus backupStatus)
		  {
			   IProgress<string> stateProgress = backupStatus.StateProgress as IProgress<string>;

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
						 DeleteFilesFromBackup(dir, fullFilePathInDest, backupStatus);
					}

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// Backing up one folder
		  /// </summary>
		  /// <param name="itemsProgress"></param>
		  /// <returns></returns>
		  public async Task BackupOneDirAsync(string destinationPath, SourceFolder sourceFolder, BackupStatus backupStatus)
		  {
			   string source = sourceFolder.FolderInfo.FullPath;
			   string dest = destinationPath;
			   string folderName = sourceFolder.FolderInfo.Name;

			   IProgress<string> stateProgress = backupStatus.StateProgress as IProgress<string>;
			   IProgress<ProgressionItemsReport> itemsProgress = backupStatus.ItemsProgress as IProgress<ProgressionItemsReport>;

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
					string dst = $"{destinationPath}\\{name}";

					stateProgress.Report($"Start backing up {src} to { dst} {Environment.NewLine}");

					await Task.Run(() => { Backup(src, dst, backupStatus); });

					stateProgress.Report($"End backing up {src} to { dst} {Environment.NewLine}");


					count++;
					itemsProgress.Report(new ProgressionItemsReport { ItemsCompletedCounter = count * 100 / sourceFolder.FolderInfo.ItemsCount, ItemsRemainingCounter = sourceFolder.FolderInfo.ItemsCount - count });

			   }

			   // Handling the top files
			   foreach (var file in Directory.GetFiles(source))
			   {
					BackupFile(dest, file, backupStatus);

					count++;
					itemsProgress.Report(new ProgressionItemsReport { ItemsCompletedCounter = count * 100 / sourceFolder.FolderInfo.ItemsCount, ItemsRemainingCounter = sourceFolder.FolderInfo.ItemsCount - count });
			   }

			   string successMessage = $"Backing up from {source} to {dest} have been completed successfuly!";
			   stateProgress.Report(successMessage);
			   ReportFile.WriteToLog(successMessage);
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

		  public void BackupFile(string destFolder, string fileToCopy, BackupStatus backupStatus)
		  {
			   IProgress<string> stateProgress = backupStatus.StateProgress as IProgress<string>;

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
						 backupStatus.DialogService.ShowMessageBox(excp.Message);
						 ReportFile.WriteToLog(excp.Message);
					}
			   };
		  }
	 }
}
