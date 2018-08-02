using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
    public class MenuListViewModel : ViewModelBase
    {
		  public static MenuListViewModel Instance => new MenuListViewModel();

		  public List<MenuItemViewModel> Items { get; set; }

		  public MenuListViewModel()
		  {
			   Items = new List<MenuItemViewModel>
			   {
					new MenuItemViewModel
					{
						 IconCode = "\uf574",
						 Title = "Backup",
					},
					new MenuItemViewModel
					{
						 IconCode = "\uf108",
						 Title = "Screenshots"
					},
					new MenuItemViewModel
					{
						 IconCode = "\uf013",
						 Title = "Settings"
					}
			   };
		  }


    }
}
