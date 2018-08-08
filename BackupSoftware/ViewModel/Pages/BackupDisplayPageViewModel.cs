using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware
{
	 // TODO: Move to different class
	 public class BackupDisplayPageViewModel : ViewModelBase
	 {
		  // TEMP: Instace to check the design live
		  public static BackupDisplayPageViewModel Instance => new BackupDisplayPageViewModel();

		  public Progress<string> LogProgress { get; set; }

		  /// <summary>
		  /// The command to go back to the backup page
		  /// </summary>
		  public RelayCommand BackupPageCommand { get; set; }

		  /// <summary>
		  /// Backing up one directory asynchronous
		  /// </summary>
		  /// <param name="item"></param>
		  /// <param name="progress"></param>
		  async void BackupOneDirAsync(FolderListItem item, IProgress<ProgressItemModel> progress)
		  {
			   ProgressItemModel report = new ProgressItemModel();

			   string folderFullPathToBackup = item.FolderPath;
			   string folderName = item.Name;
			   string folderInBackupDrive = item.To;

			   int count = 0;

			   foreach (var dir in Directory.GetDirectories(folderFullPathToBackup))
			   {
					string src = dir;
					string name = ExtractFileFolderNameFromFullPath(src);
					string dest = item.To + "\\" + name;

					item.Log += $"Start backing up {src} to { dest} ... {Environment.NewLine}";

					item.Log +=  await Task<string>.Run(() => { string log="";  Backup(src, dest, ref log, item.LogProgress); return log; });

					item.Log += $"End backing up {src} to { dest} ... {Environment.NewLine}";


					count++;

					report.Count = (count * 100 / item.ContentTotalCount);
					progress.Report(report);

			   }

			   foreach (var file in Directory.GetFiles(folderFullPathToBackup))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(folderInBackupDrive, ExtractFileFolderNameFromFullPath(file));

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

					report.Count = (count * 100 / item.ContentTotalCount);
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

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  async void ExecuteBackupAsync(ObservableCollection<FolderListItem> Folders, string BackupFolder)
		  {
			   foreach (FolderListItem item in Folders)
			   {
					await Task.Run(() => { BackupOneDirAsync(item, item.Progress); });

			   }
		  }

		  /// <summary>
		  /// Returns the amount of files and folders in the program
		  /// </summary>
		  /// <param name="path"></param>
		  /// <returns></returns>
		  public int GetDirCount(string path)
		  {
			   int fileCount = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Length;
			   int folderCount = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly).Length;

			   return (fileCount + folderCount);
		  }

		  async void StartBackupAsync()
		  {
			   var FolderItems = IoC.Kernel.Get<CacheViewModel>().FolderItems;
			   var BackupFolder = IoC.Kernel.Get<CacheViewModel>().BackupFolder;

			   // Checks to see if there is content in the fields
			   if (string.IsNullOrEmpty(BackupFolder) || FolderItems.Count == 0)
			   {
					MessageBox.Show("Fill in the list of folder and hard drive!");
					return;
			   }

			   // Check to see if the folders exists
			   foreach (FolderListItem item in FolderItems)
			   {
					if (!Directory.Exists(item.FolderPath))
					{
						 MessageBox.Show(item.FolderPath + " can not be found!");
						 return;
					}

			   }

			   if (!Directory.Exists(BackupFolder))
			   {
					MessageBox.Show(BackupFolder + " can not be found!");
					return;
			   }

			   // Getting all the infomation to Folders

			   foreach (FolderListItem item in FolderItems)
			   {
					string folderFullPathToBackup = item.FolderPath;

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = BackupFolder + "\\" + folderName;

					item.Name = folderName;
					item.To = folderInBackupDrive;

					item.Log = "";
					item.Log += $"Calculating {Environment.NewLine}";
					item.ContentTotalCount = await Task.Run<int>(() => { return GetDirCount(item.FolderPath); });
					item.Log += $"{item.ContentTotalCount} {Environment.NewLine}";
					item.Log += $"End Calculating {Environment.NewLine}";
			   }

			   ExecuteBackupAsync(FolderItems, BackupFolder);
		  }

		  public BackupDisplayPageViewModel()
		  {
			   // Create command
			   BackupPageCommand = new RelayCommand(() => { IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.BackupDetailsForm; });


			   // Start the backup
			   StartBackupAsync();
		  }

		  #region Helpers
		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest, ref string log, IProgress<string> progress)
		  {
			   if (!Directory.Exists(dest))
			   {
					Directory.CreateDirectory(dest);
					log += $"Created new folder {dest}";
			   }

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(file));

					if (File.Exists(fullFilePathInDst))
					{
						 FileInfo fileInDestInfo = new FileInfo(fullFilePathInDst);
						 FileInfo fileInfo = new FileInfo(file);

						 if (fileInfo.Length != fileInDestInfo.Length)
						 {
							  // Repalce
							  File.Delete(fullFilePathInDst);
							  File.Copy(file, fullFilePathInDst);
							  log += $"The file {file} has been modified, replacing it with new content in  {fullFilePathInDst}...{Environment.NewLine}";
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 log += $"Copying {file}...{Environment.NewLine}";
					};
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));
					log += $"Backing up {dir} to {fullDirPathInDst}{Environment.NewLine}";
					progress.Report(log);
					Backup(dir, fullDirPathInDst, ref log, progress);
					log += $"Ended backing up {dir} to {fullDirPathInDst}{Environment.NewLine}";
					progress.Report(log);
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
		  private void DeleteFilesFromBackup(string source, string dest)
		  {

			   foreach (var file in Directory.GetFiles(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(file));


					if (!File.Exists(fullFilePathInDest))
					{
						 Debug.WriteLine("Deleted " + file + "....");
						 // Delete the file
						 File.Delete(file);

					}
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullFilePathInDest = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));

					if (!Directory.Exists(fullFilePathInDest))
					{
						 Debug.WriteLine("Deleted folder " + dir + " and all its subfolders and subfiles!");
						 Directory.Delete(dir, true);
					}
					else
					{
						 DeleteFilesFromBackup(dir, fullFilePathInDest);
					}

			   }

			   if (Directory.GetDirectories(source).Length == 0)
					return;
		  }

		  /// <summary>
		  /// If subfolder is sub folder of folder
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="subFolder">The folder to check if it is subfolder</param>
		  /// <returns></returns>
		  private bool IsSubFolder(string folder, string subFolder)
		  {
			   string normailzedFolder = folder.Replace('\\', '/') + '/';
			   string normlizedSubFolder = subFolder.Replace('\\', '/') + '/';

			   bool result = normlizedSubFolder.Contains(normailzedFolder);
			   return result;
		  }

		  /// <summary>
		  /// Gives us the end of the path whether it's a file or a folder
		  /// </summary>
		  /// <param name="fullPath"></param>
		  /// <returns></returns>
		  private string ExtractFileFolderNameFromFullPath(string fullPath)
		  {
			   var normalizedPath = fullPath.Replace('\\', '/');

			   int lastSlash = normalizedPath.LastIndexOf('/');

			   return normalizedPath.Substring(lastSlash + 1);
		  }

		  #endregion
	 }
}
