using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core.Services
{
	 public interface ITakeScreenshotService
	 {
		  void TakeAndSaveScreenshot(string screenshotPath);

		  Task CaptureDesktopAsync(string screenshotsFolderPath, bool isCaptureDesktop);

		  Task CaptureFoldersAsync(string screenshotsFolderPath, ObservableCollection<FolderInfo> Folders);
	 }
}
