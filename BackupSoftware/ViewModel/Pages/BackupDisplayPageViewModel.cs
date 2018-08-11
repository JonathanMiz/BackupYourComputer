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
		  
		  // TEMP: Instance to check the design live
		  public static BackupDisplayPageViewModel Instance => new BackupDisplayPageViewModel();
		  //

		  /// <summary>
		  /// A list of all the DisplayItemControlViewModel
		  /// </summary>
		  public List<DisplayItemControlViewModel> Items { get; set; }

		  /// <summary>
		  /// How many folders done with the backup
		  /// </summary>
		  private int m_DoneFoldersCount { get; set; } = 0;
		  public int DoneFoldersCount
		  {
			   get
			   {
					return m_DoneFoldersCount;
			   }
			   set
			   {
					if (m_DoneFoldersCount == value)
						 return;

					m_DoneFoldersCount = value;
					OnPropertyChanged(nameof(DoneFoldersCount));
			   }
		  }

		  public Progress<int> CountProgress { get; set; }

		  /// <summary>
		  /// The command to go back to the backup page
		  /// </summary>
		  public RelayCommand BackupPageCommand { get; set; }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  async void StartBackupAsync(IProgress<int> progress)
		  {
			   
			   await Task.Run(() =>
			   {
					int count = 0;
					Parallel.ForEach<DisplayItemControlViewModel>(Items, async (item) =>
					{
						 await item.StartBackup();
						 if (item.BackupDone)
						 {
							  DoneFoldersCount++;
							  count++;
							  progress.Report(count);

							  if(count == Items.Count)
								   Debug.WriteLine("Done!");
						 }
					});
			   });

			   
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

			   CountProgress = new Progress<int>();
			   CountProgress.ProgressChanged += CountProgress_ProgressChanged;

			   // Start the backup
			   StartBackupAsync(CountProgress);

			   
		  }

		  private void CountProgress_ProgressChanged(object sender, int e)
		  {
			   DoneFoldersCount = (e * 100) / Items.Count;
		  }
	 }
}
