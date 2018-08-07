using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackupSoftware
{
	 public class Folder
	 {
		  public string Name { get; set; }
		  public string From { get; set; }
		  public string To { get; set; }

	 }

	 /// <summary>
	 /// Interaction logic for BackupDisplayPage.xaml
	 /// </summary>
	 public partial class BackupDisplayPage : Page
	 {
		  public BackupDisplayPage()
		  {
			   InitializeComponent();
			  // DataContext = new BackupDisplayPageViewModel();
		  }
	 }

	 public class BackupDisplayPageViewModel : ViewModelBase
	 {
		  public static BackupDisplayPageViewModel Instance => new BackupDisplayPageViewModel();
		  public List<Folder> Folders { get; set; }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  void StartBackup()
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



			   foreach (FolderListItem item in FolderItems)
			   {
					string folderFullPathToBackup = item.FolderPath;

					// Extract the name of the folder
					string folderName = ExtractFileFolderNameFromFullPath(folderFullPathToBackup);

					string folderInBackupDrive = BackupFolder + "\\" + folderName;

					Debug.WriteLine("Start backing up " + folderFullPathToBackup + " to " + folderInBackupDrive + "...");
					Backup(folderFullPathToBackup, folderInBackupDrive);
					Debug.WriteLine("End backing up " + folderFullPathToBackup + " to " + folderInBackupDrive + "...");


					if (item.DeletePrevContent)
					{
						 Debug.WriteLine("Start deleting previous content of " + folderInBackupDrive + "...");
						 DeleteFilesFromBackup(folderInBackupDrive, folderFullPathToBackup);
						 Debug.WriteLine("End deleting previous content of " + folderInBackupDrive + "...");
					}

			   }
		  }

		  public BackupDisplayPageViewModel()
		  {
			   Folders = new List<Folder>
			   {
					//IoC.Kernel.Get<SelectedFoldersViewModel>().FolderItems;
			   };
		  }

		  #region Helpers
		  /// <summary>
		  /// Backups all the files and folders that in source to dest
		  /// </summary>
		  /// <param name="source"></param>
		  /// <param name="dest"></param>
		  private void Backup(string source, string dest)
		  {
			   if (!Directory.Exists(dest))
			   {
					Directory.CreateDirectory(dest);
					Debug.WriteLine("Created new folder " + dest);
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
							  Debug.WriteLine("The file " + file + " has been modified, replacing it with new content in " + fullFilePathInDst + "...");
						 }
					}
					else
					{
						 File.Copy(file, fullFilePathInDst);
						 Debug.WriteLine("Copying " + file + "...");
					};
			   }

			   foreach (var dir in Directory.GetDirectories(source))
			   {
					string fullDirPathInDst = System.IO.Path.Combine(dest, ExtractFileFolderNameFromFullPath(dir));
					Debug.WriteLine("Backing up " + dir + " to " + fullDirPathInDst);
					Backup(dir, fullDirPathInDst);
					Debug.WriteLine("Ended backing up " + dir + " to " + fullDirPathInDst);
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
