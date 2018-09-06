using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware.Core
{
	 public class ScreenshotsDetails : INotifyPropertyChanged
	 {
		  private bool _IsCaptureDesktop;

		  public bool IsCaptureDesktop
		  {
			   get { return _IsCaptureDesktop; }
			   set { if (_IsCaptureDesktop == value) return; _IsCaptureDesktop = value; OnPropertyChanged(nameof(IsCaptureDesktop)); }
		  }

		  private ObservableCollection<FolderInfo> _Folders = new ObservableCollection<FolderInfo>();

		  public ObservableCollection<FolderInfo> Folders
		  {
			   get { return _Folders; }
			   set { if (_Folders == value) return; _Folders = value; OnPropertyChanged(nameof(Folders)); }
		  }

		  private string _DestinationFolder;

		  public string DestinationFolder
		  {
			   get { return _DestinationFolder; }
			   set { if (_DestinationFolder == value) return; _DestinationFolder = value; OnPropertyChanged(nameof(DestinationFolder)); }
		  }

		  [DllImport("user32.dll", SetLastError = true)]
		  internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		  [DllImport("user32.dll")]
		  private extern static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

		  public void OpenFolder(string folderName)
		  {
			   ProcessStartInfo startInfo = new ProcessStartInfo();
			   startInfo.FileName = "explorer.exe";
			   startInfo.Arguments = folderName;
			   startInfo.WindowStyle = ProcessWindowStyle.Maximized;

			   Process proc = Process.Start(startInfo);

			   MoveWindow(proc.MainWindowHandle, 0, 0, 0, 0, true);
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

		  public async Task CaptureDesktopAsync(string screenshotsFolderPath)
		  {
			   if (IsCaptureDesktop)
			   {
					Shell32.Shell shell = new Shell32.Shell();
					shell.ToggleDesktop();

					await Task.Delay(200);

					String filename = screenshotsFolderPath + "\\Desktop_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					TakeAndSaveScreenshot(filename);
			   }
		  }

		  public async Task CaptureFoldersAsync(string screenshotsFolderPath)
		  {

			   foreach (var folder in Folders)
			   {
					String locationToSaveScreenshot = screenshotsFolderPath + "\\" + folder.Name + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".png";
					OpenFolder(folder.FullPath);

					// Giving the folder time to be loaded properly
					await Task.Delay(650);

					await Task.Run(() => { TakeAndSaveScreenshot(locationToSaveScreenshot); });
					CloseFolder(folder.FullPath);
			   }
		  }

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

		  

		  public event PropertyChangedEventHandler PropertyChanged;

		  private void OnPropertyChanged(string propertyName)
		  {
			   if(PropertyChanged != null)
			   {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			   }
		  }
	 }
}
