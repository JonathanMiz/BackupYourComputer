using BackupSoftware.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BackupSoftware.Core
{
	 public class ScreenshotsViewModel : ViewModelBase
	 {
		  public ScreenshotsDetails ScreenshotsDetails
		  {
			   get { return ViewModelLocator.CacheViewModel.ScreenshotsDetails; }
			   set { ViewModelLocator.CacheViewModel.ScreenshotsDetails = value; }
		  }


		  private IDialogService _DialogService;

		  public ICommand StartCaptureScreenshotsCommand { get; set; }
		  public ICommand BrowseDestFolderCommand { get; set; }
		  public ICommand BrowseFoldersToScreenshotCommand { get; set; }
		  public ICommand RemoveItemCommand { get; set; }

		  public ScreenshotsViewModel(IDialogService dialogService)
		  {
			   _DialogService = dialogService;

			   StartCaptureScreenshotsCommand = new RelayCommand(StartCaptureScreenshots);
			   BrowseDestFolderCommand = new RelayCommand(BrowseDestFolder);
			   BrowseFoldersToScreenshotCommand = new RelayCommand(BrowseFoldersToScreenshot);

			   RemoveItemCommand = new RelayParameterizedCommand<string>(RemoveItem);
		  }

		  private void RemoveItem(string folderName)
		  {
			   if (_DialogService.ShowYesNoMessageBox("Are you sure you want to delete the folder from the list?", "Are you sure?"))
			   {
					var item = ScreenshotsDetails.Folders.Where(f => f.FullPath == folderName).FirstOrDefault();

					if (item != null)
					{
						 ScreenshotsDetails.Folders.Remove(item);
					}
					// TODO: Handle the case when item is null
			   }


		  }

		  private bool IsFolderInList(string newFolder)
		  {
			   var Folders = ScreenshotsDetails.Folders;

			   return (Folders.Where(f => f.FullPath == newFolder).Count() > 0);
		  }

		  private void BrowseFoldersToScreenshot()
		  {
			   var newFoldersToBackup = _DialogService.SelectFolders("Select folder / folders", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			   if (newFoldersToBackup != null)
			   {
					foreach (var folder in newFoldersToBackup)
					{
						 if (!IsFolderInList(folder))
						 {
							  ScreenshotsDetails.Folders.Add(new FolderInfo(folder));

						 }
						 else
						 {
							  _DialogService.ShowMessageBox($"Folder {folder} already exists in the list!");
						 }
					}
					ViewModelLocator.CacheViewModel.Save();
			   }
		  }

		  private void BrowseDestFolder()
		  {
			   var result = _DialogService.SelectFolder("Choose destination folder", Environment.GetFolderPath(Environment.SpecialFolder.MyComputer));
			   if (result != null)
			   {
					ScreenshotsDetails.DestinationFolder = result;
					ViewModelLocator.CacheViewModel.Save();
			   }
		  }

		  private bool ValidateScreenshotsDetails()
		  {
			   if(!ScreenshotsDetails.IsCaptureDesktop && ScreenshotsDetails.Folders.Count == 0)
			   {
					_DialogService.ShowMessageBox("You didn't choose folders to capture!");
			   }

			   if (string.IsNullOrEmpty(ScreenshotsDetails.DestinationFolder))
			   {
					_DialogService.ShowMessageBox("Fill in the destination folder!");
					return false;
			   }

			   foreach (FolderInfo item in ScreenshotsDetails.Folders)
			   {
					if (!Directory.Exists(item.FullPath))
					{
						 _DialogService.ShowMessageBox(item.FullPath + " can not be found!");
						 return false;
					}

			   }

			   if (!Directory.Exists(ScreenshotsDetails.DestinationFolder))
			   {
					_DialogService.ShowMessageBox(ScreenshotsDetails.DestinationFolder + " can not be found!");
					return false;
			   }

			   return true;
		  }

		  public async void StartCaptureScreenshots()
		  {
			   if (ValidateScreenshotsDetails())
			   {
					_DialogService.ShowMessageBox("Make sure your folders are opening in the primary screen!");
					if (_DialogService.ShowYesNoMessageBox("Are you ready to start?", "Are you ready?"))
					{
						 string mainSaveFolder = ScreenshotsDetails.DestinationFolder + "\\Computer's screenshots\\";
						 string saveScreenshootesLocationFolder = mainSaveFolder + "\\" + DateTime.Now.ToString("dd.MM.yyyy");

						 Helpers.CreateDirectoryIfNotExists(mainSaveFolder);

						 Helpers.CreateDirectoryIfNotExists(saveScreenshootesLocationFolder);

						 await ScreenshotsDetails.CaptureDesktopAsync(saveScreenshootesLocationFolder);

						 await ScreenshotsDetails.CaptureFoldersAsync(saveScreenshootesLocationFolder);
					}
			   }
		  }


	 }
}
