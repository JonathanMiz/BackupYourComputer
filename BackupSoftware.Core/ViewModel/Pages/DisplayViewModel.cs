using BackupSoftware.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupSoftware.Core
{
    public class DisplayViewModel : ViewModelBase
    {
		  // TEMP: Instance to check the design live
		  //public static DisplayViewModel Instance => new DisplayViewModel();
		  //

		  /// <summary>
		  /// A list of all the DisplayItemControlViewModel
		  /// </summary>
		  public List<DisplayItemControlViewModel> DisplayItems { get; set; }

		  /// <summary>
		  /// How many items done with the backup
		  /// </summary>
		  private int _CompletedItemsCount { get; set; } = 0;
		  public int CompletedItemsCount
		  {
			   get
			   {
					return _CompletedItemsCount;
			   }
			   set
			   {
					if (_CompletedItemsCount == value)
						 return;

					_CompletedItemsCount = value;
					OnPropertyChanged(nameof(CompletedItemsCount));
			   }
		  }

		  /// <summary>
		  /// Notifies when the done folder count needs to be updated
		  /// </summary>
		  public Progress<int> CompletedItemsProgress { get; set; }

		  /// <summary>
		  /// True if the backup is running
		  /// </summary>
		  public bool IsBackupRunning { get; set; } = false;

		  /// <summary>
		  /// True if the overall backup is done
		  /// </summary>
		  private bool _IsBackupDone;

		  public bool IsBackupDone
		  {
			   get { return _IsBackupDone; }
			   set { if (_IsBackupDone == value) return; _IsBackupDone = value; OnPropertyChanged(nameof(IsBackupDone)); }
		  }

		  private IDialogService _DialogService;

		  #region Commands
		  /// <summary>
		  /// The command to go back to the backup page
		  /// </summary>
		  public ICommand BackToDetailsCommand { get; set; }

		  #endregion

		  private void GoToDetails()
		  {
			   ViewModelLocator.ApplicationViewModel.GoToView(ViewModelLocator.DetailsViewModel);
		  }

		  /// <summary>
		  /// Start backing up all the <see cref="FolderPathsToBackup"/> to <see cref="BackupFolder"/>
		  /// </summary>
		  async void StartBackupAsync(IProgress<int> progress)
		  {
			   IsBackupRunning = true;
			   await Task.Run(() =>
			   {
					int count = 0;
					Parallel.ForEach<DisplayItemControlViewModel>(DisplayItems, async (item) =>
					{
						 await item.StartBackup();
						 if (item.IsBackupDone)
						 {
							  count++;
							  progress.Report(count);

							  if (count == DisplayItems.Count)
							  {
								   IsBackupRunning = false;
								   IsBackupDone = true;

								   ViewModelLocator.CacheViewModel.Details.LastBackupTime = DateTime.Now;

								   // Redircet to details page
								   GoToDetails();
							  }
						 }
					});
			   });


		  }

		  /// <summary>
		  /// Setting all the information(DestFolder, FolderPath) to <see cref="DisplayItems"/>
		  /// </summary>
		  private void SetItemsInformation()
		  {
			   // Getting all the infomation to Items
			   foreach (var item in ViewModelLocator.CacheViewModel.Details.SourceFolders)
			   {
					DisplayItemControlViewModel displayItemControlViewModel = new DisplayItemControlViewModel(_DialogService, item)
					{
						 DestinationPath = $"{ViewModelLocator.CacheViewModel.Details.DestFolder}{item.FolderInfo.Name}"
					};

					DisplayItems.Add(displayItemControlViewModel);
			   }

		  }

		  public void RunBackup()
		  {
			   if (!IsBackupRunning)
			   {
					// In the begining the backup is not yet started
					IsBackupDone = false;

					// Setting all the infomation to Items
					SetItemsInformation();

					// Start the backup
					StartBackupAsync(CompletedItemsProgress);
			   }
		  }

		  public DisplayViewModel(IDialogService dialogService)
		  {
			   _DialogService = dialogService;

			   DisplayItems = new List<DisplayItemControlViewModel>();

			   // Create command
			   BackToDetailsCommand = new RelayCommand(GoToDetails);

			   CompletedItemsProgress = new Progress<int>();

			   // Set the event for handling the progression
			   CompletedItemsProgress.ProgressChanged += CountProgress_ProgressChanged;
		  }

		  private void CountProgress_ProgressChanged(object sender, int e)
		  {
			   if (DisplayItems.Count != 0)
			   {
					CompletedItemsCount = (e * 100) / DisplayItems.Count;

					// If the backup is done clear the existing items
					if(e == DisplayItems.Count)
					{
						 DisplayItems.Clear();
					}
			   }
		  }

		  /// <summary>
		  /// Reset the state to the initial values
		  /// </summary>
		  public void ResetState()
		  {
			   CompletedItemsCount = 0;
			   IsBackupDone = false;
			   IsBackupRunning = false;
		  }
	 }
}
