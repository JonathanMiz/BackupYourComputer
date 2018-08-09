using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	 public class ProgressItemModel
	 {
		  public int Count { get; set; } = 0;
	 }

	 public class DisplayItemControlViewModel : ViewModelBase
	 {
		  //public static DisplayItemControlViewModel Instance => new DisplayItemControlViewModel();

		  #region Private Members
			   private string m_Log { get; set; }
		  #endregion

		  #region Public Members

		  public FolderInfo FolderInfo { get; set; }

		  public string Destination { get; set; }

		  public Progress<ProgressItemModel> ItemsRemainingProgress { get; set; }

		  public Progress<string> LogProgress { get; set; }

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
			   // Refactor
			   FolderInfo = new FolderInfo(folderPath);
			   FolderInfo.ItemsCount = CalcItemsCount(FolderInfo.FolderPath);

			   ItemsRemainingProgress = new Progress<ProgressItemModel>();
			   ItemsRemainingProgress.ProgressChanged += ItemsRemainingProgress_ProgressChanged;

			   LogProgress = new Progress<string>();
			   LogProgress.ProgressChanged += LogProgress_ProgressChanged;

			   //item.Log += $"Calculating {Environment.NewLine}";
			   //item.ContentTotalCount = await Task.Run<int>(() => { return GetDirCount(item.FolderPath); });
			   //item.Log += $"{item.ContentTotalCount} {Environment.NewLine}";
			   //item.Log += $"End Calculating {Environment.NewLine}";
		  }

		  private void LogProgress_ProgressChanged(object sender, string e)
		  {
			   Log = e;
		  }

		  private void ItemsRemainingProgress_ProgressChanged(object sender, ProgressItemModel e)
		  {
			   ItemsRemainingCounter = e.Count;
		  }

		  /// <summary>
		  /// Returns the amount of files and folders in the program
		  /// </summary>
		  /// <param name="path"></param>
		  /// <returns></returns>
		  private int CalcItemsCount(string path)
		  {
			   int fileCount = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Length;
			   int folderCount = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly).Length;

			   int result = (fileCount + folderCount);

			   return result;
		  }

		  public async Task BackupOneDirAsync(IProgress<ProgressItemModel> progress)
		  {
			   ProgressItemModel report = new ProgressItemModel();

			   string source = FolderInfo.FolderPath;
			   string dest = Destination;
			   string folderName = FolderInfo.Name;

			   int count = 0;

			   // Handling the top directories
			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string src = dir;
					string name = Helpers.ExtractFileFolderNameFromFullPath(src);
					// Refactor slashes
					string dst = $"{Destination}\\{name}";

					Log = $"Start backing up {src} to { dest} ... {Environment.NewLine}";

					Log = await Task<string>.Run(() => { string log = ""; Backup(src, dst, ref log, LogProgress); return log; });
					//Log = "";
					Log = $"End backing up {src} to { dest} ... {Environment.NewLine}";


					count++;

					report.Count = (count * 100 / FolderInfo.ItemsCount);
					progress.Report(report);

			   }

			   // Handling the top files
			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(source, Helpers.ExtractFileFolderNameFromFullPath(file));

					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);

						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  // Repalce
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  Debug.WriteLine("The file " + file + " has been modified, replacing it with new content in " + fullFilePathInDst + "...");
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 Debug.WriteLine("Copying " + file + "...");
					};

					count++;

					report.Count = (count * 100 / FolderInfo.ItemsCount);
					progress.Report(report);
			   }

			   //item.ContentCount = (count * 100 / item.ContentTotalCount);


			   //if (item.DeletePrevContent)
			   //{
			   //	 Debug.WriteLine("Start deleting previous content of " + folderInBackupDrive + "...");
			   //	 DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
			   //	 Debug.WriteLine("End deleting previous content of " + folderInBackupDrive + "...");
			   //}
		  }

		  public async Task StartBackup()
		  {
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
							  log = $"The file {file} has been modified, replacing it with new content in  {fullFilePathInDst}...{Environment.NewLine}";
							  logProgress.Report(log);
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 log = $"Copying {file}...{Environment.NewLine}";
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
