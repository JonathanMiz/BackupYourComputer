using System;
using System.Collections.Generic;
using System.Diagnostics;
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
	 /// Interaction logic for BackupDetailsForm.xaml
	 /// This is the page that the user will fill his backup details
	 /// </summary>
	 public partial class BackupDetailsForm : Page
	 {
		  private FormViewModel formViewModel;
		  public BackupDetailsForm()
		  {
			   InitializeComponent();
			   // Create a model form object

			   formViewModel = new FormViewModel();

			   // Set default values
			   //formViewModel.AddFolderToBackUp("C:\\Users\\Jonathan\\Documents\\BackupTest");

			   // Setting some default folders to save me some time
			   // TODO: save all the added folders in database or something similar
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Documents");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Downloads");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Music");
			   //this.FolderList.Items.Add("C:\\Users\\Jonathan\\Pictures");

			   this.DataContext = formViewModel;
		  }
	 }
}
