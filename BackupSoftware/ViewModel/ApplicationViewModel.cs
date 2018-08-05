using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		  private ApplicationPage m_CurrentPage { get; set; } = ApplicationPage.BackupDetailsForm;
		  public ApplicationPage CurrentPage
		  {
			   get
			   {
					return m_CurrentPage;
			   }
			   set
			   {
					if (m_CurrentPage == value)
						 return;

					m_CurrentPage = value;
					OnPropertyChanged(nameof(CurrentPage));
			   }
		  }
	}
}
