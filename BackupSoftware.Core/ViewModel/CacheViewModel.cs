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

		  private ScreenshotsDetails _ScreenshotsDetails;

		  public ScreenshotsDetails ScreenshotsDetails
		  {
			   get { return _ScreenshotsDetails; }
			   set { if (_ScreenshotsDetails == value) return; _ScreenshotsDetails = value; OnPropertyChanged(nameof(ScreenshotsDetails)); }
		  }

		  private IDetailsDataService _DataService;
		  private IScreenshotsDetailsDataService _ScreenshotsDataService;

		  public void Save()
		  {
			   _DataService.Save(Details);
			   _ScreenshotsDataService.Save(ScreenshotsDetails);
		  }

		  public CacheViewModel(IDetailsDataService dataService, IScreenshotsDetailsDataService screenshotsDetailsDataService)
		  {
			   _DataService = dataService;
			   _ScreenshotsDataService = screenshotsDetailsDataService;

			   Details = _DataService.GetDetails();
			   ScreenshotsDetails = _ScreenshotsDataService.GetDetails();
		  }
	 }
}
