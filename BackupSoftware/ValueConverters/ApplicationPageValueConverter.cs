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
	 public class ApplicationPageValueConverter : MarkupExtension, IValueConverter
	 {
		  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		  {
			   switch((ApplicationPage)value)
			   {
					case ApplicationPage.BackupDetailsForm:
						 {
							  return new BackupDetailsForm();
						 }
					case ApplicationPage.ScreenshotsDetailsForm:
						 {
							  return new ScreenshotsDetails();
						 }
					case ApplicationPage.SelectFolders:
						 {
							  return new SelectFoldersPage();
						 }
					case ApplicationPage.BackupDisplay:
						 {
							  return new BackupDisplayPage();
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
