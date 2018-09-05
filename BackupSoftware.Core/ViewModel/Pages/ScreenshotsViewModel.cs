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
		  private ScreenshotsDetails _ScreenshotsDetails;

		  public ScreenshotsDetails ScreenshotsDetails
		  {
			   get { return _ScreenshotsDetails; }
			   set { _ScreenshotsDetails = value; }
		  }


		  private IDialogService _DialogService;

		  public ICommand StartCaptureCommand { get; set; }
		  public ICommand BrowseCommand { get; set; }

		  public ScreenshotsViewModel(IDialogService dialogService)
		  {
			   _DialogService = dialogService;

			   StartCaptureCommand = new RelayCommand(PerformAllCaptures);
			   BrowseCommand = new RelayCommand(Browse);

			   // Temporary: Only for testing the logic
			   _ScreenshotsDetails = new ScreenshotsDetails()
			   {
					IsCaptureDesktop = true,
					DestinationFolder = "H://"
			   };
		  }

		  private void Browse()
		  {
			   var result = _DialogService.SelectFolder("Choose destination folder");
			   if (result != null)
					_ScreenshotsDetails.DestinationFolder = result;
		  }

		  private void OpenFolder(string folderName)
		  {
			   ProcessStartInfo startInfo = new ProcessStartInfo();
			   startInfo.FileName = "explorer.exe";
			   startInfo.Arguments = folderName;
			   startInfo.WindowStyle = ProcessWindowStyle.Maximized;

			   Process proc = Process.Start(startInfo);
		  }

		  private void CloseFolder(string folderName)
		  {
			   foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
			   {
					if (Path.GetFileNameWithoutExtension(window.FullName).ToLowerInvariant() == "explorer")
					{
						 if (Uri.IsWellFormedUriString(window.LocationURL, UriKind.Absolute))
						 {
							  string location = new Uri(window.LocationURL).LocalPath;

							  if (string.Equals(location, folderName, StringComparison.OrdinalIgnoreCase))
								   window.Quit();
						 }
					}
			   }
		  }

		  private void CreateDirectory(string directoryPath)
		  {
			   if (!Directory.Exists(directoryPath))
			   {
					Directory.CreateDirectory(directoryPath);
			   }

		  }

		  private async Task CaptureDesktopAsync(string saveLocationPath)
		  {
			   if (ScreenshotsDetails.IsCaptureDesktop)
			   {
					Shell32.Shell shell = new Shell32.Shell();
					//shell.MinimizeAll();
					shell.ToggleDesktop();

					await Task.Delay(200);

					String filename = saveLocationPath + "\\Desktop_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					TakeScreenshot(filename);
			   }
		  }

		  private async Task CaptureFoldersAsync(string saveLocationPath)
		  {

			   foreach (var folder in ViewModelLocator.CacheViewModel.Details.SourceFolders)//ScreenshotsDetails.Folders)
			   {
					String locationToSaveFilename = saveLocationPath + "\\" + folder.FolderInfo.Name + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					OpenFolder(folder.FolderInfo.FullPath);

					// Make the folder to stay open
					await Task.Delay(500);

					await Task.Run(() => { TakeScreenshot(locationToSaveFilename); });
					CloseFolder(folder.FolderInfo.FullPath);
			   }
		  }

		  public async void PerformAllCaptures()
		  {
			   string mainSaveFolder = ScreenshotsDetails.DestinationFolder + "\\Computer's screenshots\\";
			   string saveScreenshootesLocationFolder = mainSaveFolder + "\\" + DateTime.Now.ToString("dd.MM.yyyy");

			   CreateDirectory(mainSaveFolder);

			   CreateDirectory(saveScreenshootesLocationFolder);

			   await CaptureDesktopAsync(saveScreenshootesLocationFolder);

			   await CaptureFoldersAsync(saveScreenshootesLocationFolder);
		  }

		  private void TakeScreenshot(string saveLocationPath)
		  {
			   double screenLeft = 0;
			   double screenTop = 0;
			   double screenWidth = SystemParameters.PrimaryScreenWidth;
			   double screenHeight = SystemParameters.PrimaryScreenHeight;

			   using (Bitmap bmp = new Bitmap((int)screenWidth,(int)screenHeight))
			   {
					using (Graphics g = Graphics.FromImage(bmp))
					{
						 g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
						 bmp.Save(saveLocationPath);
					}
			   }
		  }
	 }
}
