using System.ComponentModel;

namespace BackupSoftware
{
	 /// <summary>
	 /// The class that every view model needs to inherit from
	 /// </summary>
	 public class ViewModelBase : INotifyPropertyChanged
	 {
		  public event PropertyChangedEventHandler PropertyChanged;

		  /// <summary>
		  /// Notify that a property has been changed
		  /// </summary>
		  /// <param name="propertyName"></param>
		  protected void OnPropertyChanged(string propertyName)
		  {
			   if (PropertyChanged != null)
			   {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			   }
		  }
	 }
}
