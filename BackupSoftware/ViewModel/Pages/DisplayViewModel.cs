using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
    public class DisplayViewModel : ViewModelBase
    {
		  // TEMP: Instance to check the design live
		  public static DisplayViewModel Instance => new DisplayViewModel();
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
		  public RelayCommand BackToDetailsCommand { get; set; }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  async void StartBackupAsync(IProgress<int> progress)
		  {
			   IoC.Get<CacheViewModel>().IsBackupRunning = true;
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

							  if (count == Items.Count)
							  {
								   IoC.Get<CacheViewModel>().IsBackupRunning = false;
								   Debug.WriteLine("Done!");
							  }
						 }
					});
			   });


		  }

		  private void GetItemsInformation()
		  {
			   // Getting all the infomation to Items
			   foreach (var item in IoC.Get<CacheViewModel>().FolderItems)
			   {
					DisplayItemControlViewModel displayItemControlViewModel = new DisplayItemControlViewModel(item.FolderPath);
					displayItemControlViewModel.Destination = $"{IoC.Get<CacheViewModel>().BackupFolder}{displayItemControlViewModel.FolderInfo.Name}";

					Items.Add(displayItemControlViewModel);
			   }

		  }

		  public void RunBackup()
		  {
			   if (!IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					Items = new List<DisplayItemControlViewModel>();

					// Getting all the infomation to Items
					GetItemsInformation();

					// Create command
					BackToDetailsCommand = new RelayCommand(() => { IoC.Get<ApplicationViewModel>().CurrentViewModel = ViewModelLocator.DetailsViewModel; });

					CountProgress = new Progress<int>();
					CountProgress.ProgressChanged += CountProgress_ProgressChanged;

					// Start the backup
					StartBackupAsync(CountProgress);
			   }
		  }

		  public DisplayViewModel()
		  {

		  }

		  private void CountProgress_ProgressChanged(object sender, int e)
		  {
			   DoneFoldersCount = (e * 100) / Items.Count;
		  }
	 }
}
