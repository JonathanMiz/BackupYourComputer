using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	 /// <summary>
	 /// The view model of the application state
	 /// </summary>
    public class ApplicationViewModel : ViewModelBase
    {
		  public ApplicationPage CurrentPage { get; set; } = ApplicationPage.SelectFolders;
	}
}
