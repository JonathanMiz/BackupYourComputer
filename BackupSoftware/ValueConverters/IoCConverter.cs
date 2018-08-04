using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace BackupSoftware
{
	 /// <summary>
	 /// Converts a string name to a service pulled form the IoC container
	 /// </summary>
	 public class IoCConverter : MarkupExtension, IValueConverter
	 {
		  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		  {
			   switch((string)parameter)
			   {
					case nameof(ApplicationViewModel):
						 {
							  return IoC.Kernel.Get<ApplicationViewModel>();
						 }
					default:
						 Debugger.Break();
						 return null;
			   }
		  }

		  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		  {
			   throw new NotImplementedException();
		  }

		  public override object ProvideValue(IServiceProvider serviceProvider)
		  {
			   return this;
		  }
	 }
}
