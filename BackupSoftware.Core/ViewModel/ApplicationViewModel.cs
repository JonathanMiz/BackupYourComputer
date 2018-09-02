using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 /// <summary>
	 /// The view model of the application state
	 /// </summary>
    public class ApplicationViewModel : ViewModelBase
    {
		  private ViewModelBase _CurrentViewModel { get; set; } = ViewModelLocator.DetailsViewModel;
		  public ViewModelBase CurrentViewModel
		  {
			   get
			   {
					return _CurrentViewModel;
			   }
			   set
			   {
					if (_CurrentViewModel == value)
						 return;

					_CurrentViewModel = value;
					OnPropertyChanged(nameof(CurrentViewModel));
			   }
		  }

		  private double _ContentPresentorOpacity { get; set; } = 1.0f;

		  public double ContentPresentorOpacity
		  {
			   get { return _ContentPresentorOpacity; }
			   set { if (_ContentPresentorOpacity == value) return; _ContentPresentorOpacity = value; OnPropertyChanged(nameof(ContentPresentorOpacity)); }
		  }


		  public void GoToView(ViewModelBase ViewModel)
		  {
			   SlowOpacity(ViewModel);
		  }

		  private async void SlowOpacity(ViewModelBase ViewModel)
		  {
			   await Task.Factory.StartNew(() =>
			   {
					for(double i = 1.0; i > 0.0; i -= 0.1)
					{
						 ContentPresentorOpacity = i;
						 Thread.Sleep(10);
					}
					CurrentViewModel = ViewModel;
					for (double i = 0.0; i < 1.1; i += 0.1)
					{
						 ContentPresentorOpacity = i;
						 Thread.Sleep(10);
					}
			   });
		  }
	 }
}
