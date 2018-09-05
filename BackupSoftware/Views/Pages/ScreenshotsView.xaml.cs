using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackupSoftware
{
	 /// <summary>
	 /// Interaction logic for ScreenshotsView.xaml
	 /// </summary>
	 public partial class ScreenshotsView : UserControl
	 {
		  public ScreenshotsView()
		  {
			   InitializeComponent();
		  }

		  private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		  {
			   e.CanExecute = true;
		  }

		  private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
		  {

		  }
	 }
}
