using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		  private ObservableCollection<FolderInfo> _Folders;

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
