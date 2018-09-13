using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 public class ObservableObject : INotifyPropertyChanged
	 {
		  public event PropertyChangedEventHandler PropertyChanged;

		  /// <summary>
		  /// Notify that a property has been changed
		  /// </summary>
		  /// <param name="propertyName">the name of the property in the class not in the xaml!!</param>
		  protected virtual void OnPropertyChanged(string propertyName)
		  {
			   if (PropertyChanged != null)
			   {
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			   }
		  }
	 }
}
