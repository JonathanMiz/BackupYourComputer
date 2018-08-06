using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	/// <summary>
	/// The view model for the items of the menu
	/// </summary>
    public class MenuListViewModel : ViewModelBase
    {
		  /// <summary>
		  /// Temporary static member to be able to design the items
		  /// </summary>
		  public static MenuListViewModel Instance => new MenuListViewModel();

		  /// <summary>
		  /// A list of all the items in the menu
		  /// </summary>
		  public List<MenuItemViewModel> Items { get; set; }

		  public RelayCommand OpenPageCommand;

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public MenuListViewModel()
		  {
			   Items = new List<MenuItemViewModel>
			   {
					new MenuItemViewModel
					{
						 IconCode = "\uf574",
						 Title = "Backup",
						 Page = ApplicationPage.BackupDetailsForm,
					},
					new MenuItemViewModel
					{
						 IconCode = "\uf108",
						 Title = "Screenshots",
						 Page = ApplicationPage.ScreenshotsDetailsForm
					},
					new MenuItemViewModel
					{
						 IconCode = "\uf013",
						 Title = "Settings",
						 Page = ApplicationPage.BackupDisplay
					}
			   };
		  }


    }
}
