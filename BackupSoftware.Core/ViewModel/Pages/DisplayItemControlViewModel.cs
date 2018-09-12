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

		  private BackupStatus _BackupStatus;

		  public BackupStatus BackupStatus
		  {
			   get { return _BackupStatus; }
			   set { _BackupStatus = value; }
		  }


		  private IDialogService _DialogService;
		  private IBackupService _BackupService;

		  #endregion

		  public DisplayItemControlViewModel(IDialogService dialogService, IBackupService backupService, SourceFolder sourceFolder, string destPath)
		  {
			   _DialogService = dialogService;
			   _BackupService = backupService;
			   SourceFolder = sourceFolder;
			   DestinationPath = destPath;

			   BackupStatus = new BackupStatus(_DialogService);
			   BackupStatus.ItemsRemainingCounter = SourceFolder.FolderInfo.ItemsCount;
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
					BackupStatus.State = $"Deleting previous content from {DestinationPath}...{Environment.NewLine}";
					await Task.Run(() => { _BackupService.DeleteFilesFromBackup(DestinationPath, SourceFolder.FolderInfo.FullPath, BackupStatus); });
			   }

			   await _BackupService.BackupOneDirAsync(DestinationPath, SourceFolder, BackupStatus);

			   BackupStatus.IsBackupDone = true;
		  }


	 }
}