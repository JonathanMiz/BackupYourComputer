using BackupSoftware.Core.Services;
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
	 public class ScreenshotsDetails : ObservableObject
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
			   set
			   {
					if (_DestinationFolder == value)
						 return;
					_DestinationFolder = value;
					OnPropertyChanged(nameof(DestinationFolder));
			   }
		  }
	 }
}
