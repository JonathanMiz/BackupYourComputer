using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackupSoftware
{
	 // TODO: Move to different class
	 public class BackupDisplayPageViewModel : ViewModelBase
	 {
		  
		  // TEMP: Instace to check the design live
		  public static BackupDisplayPageViewModel Instance => new BackupDisplayPageViewModel();
		  //

		  public List<DisplayItemControlViewModel> Items { get; set; }


		  /// <summary>
		  /// The command to go back to the backup page
		  /// </summary>
		  public RelayCommand BackupPageCommand { get; set; }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  async void StartBackupAsync()
		  {
			   foreach (DisplayItemControlViewModel item in Items)
			   {
					await item.StartBackup();
			   }
		  }

		  private void GetItemsInformation()
		  {
			   // Getting all the infomation to Items
			   foreach (var item in IoC.Kernel.Get<CacheViewModel>().FolderItems)
			   {
					DisplayItemControlViewModel displayItemControlViewModel = new DisplayItemControlViewModel(item.FolderPath);
					displayItemControlViewModel.Destination = $"{IoC.Kernel.Get<CacheViewModel>().BackupFolder}{displayItemControlViewModel.FolderInfo.Name}";

					Items.Add(displayItemControlViewModel);
			   }

		  }

		  public BackupDisplayPageViewModel()
		  {
			   Items = new List<DisplayItemControlViewModel>();

			   // Getting all the infomation to Items
			   GetItemsInformation();

			   // Create command
			   BackupPageCommand = new RelayCommand(() => { IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = ApplicationPage.BackupDetailsForm; });


			   // Start the backup
			   StartBackupAsync();
		  }
	 }
}
