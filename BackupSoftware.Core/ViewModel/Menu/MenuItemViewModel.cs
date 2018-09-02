
namespace BackupSoftware.Core
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class MenuItemViewModel : ViewModelBase
	 {
		  /// <summary>
		  /// The code of the icon from FontAwesome
		  /// </summary>
		  public string IconCode { get; set; }

		  /// <summary>
		  /// The name of the item
		  /// </summary>
		  public string Title { get; set; }

		  /// <summary>
		  /// The page of this menu item
		  /// </summary>
		  public ViewModelBase ViewModel { get; set; }

		  /// <summary>
		  /// True if the item is selected
		  /// </summary>
		  private bool _IsSelected;
		  public bool IsSelected
		  {
			   get { return _IsSelected; }
			   set
			   {
					if (_IsSelected == value)
						 return;

					_IsSelected = value;
					OnPropertyChanged(nameof(IsSelected));
			   }
		  }

		  /// <summary>
		  /// Default constructor
		  /// </summary>
		  public MenuItemViewModel()
		  {

		  }

		  /// <summary>
		  /// The commnad for opening a page
		  /// </summary>
		  public void GoToView()
		  {
			   // Change the current page to the page of the menu item
			   ViewModelLocator.ApplicationViewModel.GoToView(ViewModel);
			   
		  }
	 }
}
