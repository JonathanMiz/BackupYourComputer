using Ninject;
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
	 public class Folder
	 {
		  public string Name { get; set; }
		  public string From { get; set; }
		  public string To { get; set; }

	 }

	 /// <summary>
	 /// Interaction logic for BackupDisplayPage.xaml
	 /// </summary>
	 public partial class BackupDisplayPage : Page
	 {
		  public BackupDisplayPage()
		  {
			   InitializeComponent();
			  // DataContext = new BackupDisplayPageViewModel();
		  }
	 }

	 public class BackupDisplayPageViewModel : ViewModelBase
	 {
		  public static BackupDisplayPageViewModel Instance => new BackupDisplayPageViewModel();
		  public List<Folder> Folders { get; set; }

		  public BackupDisplayPageViewModel()
		  {
			   Folders = new List<Folder>
			   {
					IoC.Kernel.Get<SelectedFoldersViewModel>().FolderItems;
			   };
		  }
	 }
}
