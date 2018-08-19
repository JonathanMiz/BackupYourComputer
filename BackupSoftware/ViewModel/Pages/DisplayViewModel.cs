using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
		  private int _DoneFoldersCount { get; set; } = 0;
		  public int DoneFoldersCount
		  {
			   get
			   {
					return _DoneFoldersCount;
			   }
			   set
			   {
					if (_DoneFoldersCount == value)
						 return;

					_DoneFoldersCount = value;
					OnPropertyChanged(nameof(DoneFoldersCount));
			   }
		  }

		  /// <summary>
		  /// Notifies when the done folder count needs to be updated
		  /// </summary>
		  public Progress<int> CountProgress { get; set; }


		  #region Commands
		  /// <summary>
		  /// The command to go back to the backup page
		  /// </summary>
		  public ICommand BackToDetailsCommand { get; set; }

		  #endregion

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

		  /// <summary>
		  /// Setting all the information(DestFolder, FolderPath) to <see cref="Items"/>
		  /// </summary>
		  private void SetItemsInformation()
		  {
			   // Getting all the infomation to Items
			   foreach (var item in IoC.Get<CacheViewModel>().SourceFolders)
			   {
					DisplayItemControlViewModel displayItemControlViewModel = new DisplayItemControlViewModel(item.FolderPath);
					displayItemControlViewModel.Destination = $"{IoC.Get<CacheViewModel>().DestFolder}{displayItemControlViewModel.FolderInfo.Name}";

					Items.Add(displayItemControlViewModel);
			   }

		  }

		  public void RunBackup()
		  {
			   if (!IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					// Setting all the infomation to Items
					SetItemsInformation();

					// Start the backup
					StartBackupAsync(CountProgress);
			   }
		  }

		  public DisplayViewModel()
		  {
			   Items = new List<DisplayItemControlViewModel>();

			   // Create command
			   BackToDetailsCommand = new RelayCommand(() => { IoC.Get<ApplicationViewModel>().CurrentViewModel = ViewModelLocator.DetailsViewModel; });

			   CountProgress = new Progress<int>();

			   // Set the event for handling the progression
			   CountProgress.ProgressChanged += CountProgress_ProgressChanged;
		  }

		  private void CountProgress_ProgressChanged(object sender, int e)
		  {
			   DoneFoldersCount = (e * 100) / Items.Count;
		  }
	 }
}
