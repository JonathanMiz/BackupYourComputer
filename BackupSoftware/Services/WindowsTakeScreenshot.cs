using BackupSoftware.Core;
using BackupSoftware.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware.Services
{
	 public class WindowsTakeScreenshot : ITakeScreenshotService
	 {
		  public void TakeAndSaveScreenshot(string screenshotPath)
		  {
			   double screenLeft = 0;
			   double screenTop = 0;
			   double screenWidth = SystemParameters.PrimaryScreenWidth;
			   double screenHeight = SystemParameters.PrimaryScreenHeight;

			   using (Bitmap bmp = new Bitmap((int)screenWidth, (int)screenHeight))
			   {
					using (Graphics g = Graphics.FromImage(bmp))
					{
						 g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
						 bmp.Save(screenshotPath);
					}
			   }
		  }

		  public void OpenFolder(string folderName)
		  {
			   ProcessStartInfo startInfo = new ProcessStartInfo();
			   startInfo.FileName = "explorer.exe";
			   startInfo.Arguments = folderName;
			   startInfo.WindowStyle = ProcessWindowStyle.Maximized;

			   Process proc = Process.Start(startInfo);

		  }

		  public void CloseFolder(string folderName)
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

		  public async Task CaptureDesktopAsync(string screenshotsFolderPath, bool isCaptureDesktop)
		  {
			   if (isCaptureDesktop)
			   {
					Shell32.Shell shell = new Shell32.Shell();
					shell.ToggleDesktop();

					await Task.Delay(200);

					String filename = screenshotsFolderPath + "\\Desktop_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					TakeAndSaveScreenshot(filename);
			   }
		  }

		  public async Task CaptureFoldersAsync(string screenshotsFolderPath, ObservableCollection<FolderInfo> Folders)
		  {
			   foreach (var folder in Folders)
			   {
					String locationToSaveScreenshot = screenshotsFolderPath + "\\" + folder.Name + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					OpenFolder(folder.FullPath);

					// Giving the folder time to be loaded properly
					await Task.Delay(750);

					await Task.Run(() => { TakeAndSaveScreenshot(locationToSaveScreenshot); });
					CloseFolder(folder.FullPath);
			   }
		  }
	 }
}
