using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BackupSoftware
{
	 class BooleanToVisibleConverter : MarkupExtension, IValueConverter
	 {
		  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		  {
			   if((bool)value)
			   {
					return Visibility.Visible;
			   }
			   else
			   {
					return Visibility.Hidden;
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

	 /// <summary>
	 /// If the value is false the object will be visible
	 /// </summary>
	 class InverseBooleanToVisibleConverter : MarkupExtension, IValueConverter
	 {
		  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		  {
			   if (!(bool)value)
			   {
					return Visibility.Visible;
			   }
			   else
			   {
					return Visibility.Hidden;
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
