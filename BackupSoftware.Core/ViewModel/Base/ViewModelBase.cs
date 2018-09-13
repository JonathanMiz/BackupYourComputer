using System.ComponentModel;

namespace BackupSoftware.Core
{
	 /// <summary>
	 /// The class that every view model needs to inherit from
	 /// </summary>
	 public class ViewModelBase : ObservableObject
	 {
		  protected override void OnPropertyChanged(string propertyName)
		  {
			   base.OnPropertyChanged(propertyName);
		  }
	 }
}
