using BackupSoftware.Core.Services;

namespace BackupSoftware.Core
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

			   Details = _DataService.GetDetails();
		  }
	 }
}
