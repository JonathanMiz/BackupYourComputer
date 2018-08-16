using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BackupSoftware
{
	 /// <summary>
	 /// The view model of the application state
	 /// </summary>
    public class ApplicationViewModel : ViewModelBase
    {
		  private ViewModelBase m_CurrentViewModel { get; set; } = ViewModelLocator.DetailsViewModel;
		  public ViewModelBase CurrentViewModel
		  {
			   get
			   {
					return m_CurrentViewModel;
			   }
			   set
			   {
					if (m_CurrentViewModel == value)
						 return;

					m_CurrentViewModel = value;
					OnPropertyChanged(nameof(CurrentViewModel));
			   }
		  }

	 }
}
