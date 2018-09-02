using System.ComponentModel;

namespace BackupSoftware.Core
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
		  /// <param name="propertyName">the name of the property in the class not in the xaml!!</param>
		  protected void OnPropertyChanged(string propertyName)
		  {
			   if (PropertyChanged != null)
			   {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			   }
		  }
	 }
}
