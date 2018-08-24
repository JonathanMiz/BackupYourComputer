using BackupSoftware.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xaml;

namespace BackupSoftware
{
	 /// <summary>
	 /// This class will hold all the data that needs to be passed between pages
	 /// </summary>
	 public class CacheViewModel : ViewModelBase
	 {

		  private Details _Details;

		  public Details Details
		  {
			   get { return _Details; }
			   set { if (_Details == value) return; _Details = value; OnPropertyChanged(nameof(Details)); }
		  }

		  private IDetailsDataService _DataService;

		  public void Save()
		  {
			   _DataService.Save(Details);
		  }

		  public CacheViewModel(IDetailsDataService dataService)
		  {
			   _DataService = dataService;

			   Details = new Details(); //_DataService.GetDetails();

			   // TEMP: Default values for tests
			   Details.DestFolder = "H:\\";

			   Details.AddFolder("C:\\Users\\Jonathan\\Documents\\BackupTest");
			   Details.AddFolder("C:\\Users\\Jonathan\\Documents\\Army");
			   //Details.AddFolder("C:\\Users\\Jonathan\\Documents\\Blender");
			   //Details.AddFolder("C:\\Users\\Jonathan\\Documents\\Books");
			   //Details.AddFolder("C:\\Users\\Jonathan\\Documents\\boost_1_65_1");
		  }
	 }
}
