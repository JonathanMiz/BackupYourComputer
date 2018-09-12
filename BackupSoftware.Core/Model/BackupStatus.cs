using BackupSoftware.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 public class BackupStatus : ViewModelBase // Change the name of viewmodelbase to something more appropriate
	 {
		  /// <summary>
		  /// Text to display the user with the current status
		  /// </summary>
		  private string _State { get; set; } = "";
		  public string State
		  {
			   get
			   {
					return _State;
			   }
			   set
			   {
					if (_State == value)
						 return;
					_State = value;
					OnPropertyChanged(nameof(State));
			   }
		  }

		  /// <summary>
		  /// The variable to count files and folders that was backed up
		  /// </summary>
		  int _ItemsCompletedCounter { get; set; } = 0;
		  public int ItemsCompletedCounter
		  {
			   get
			   {
					return _ItemsCompletedCounter;
			   }
			   set
			   {
					if (_ItemsCompletedCounter == value)
						 return;

					_ItemsCompletedCounter = value;
					OnPropertyChanged(nameof(ItemsCompletedCounter));
			   }
		  }

		  /// <summary>
		  /// The variable to show how many items remains to backup
		  /// </summary>
		  int _ItemsRemainingCounter { get; set; } = 0;
		  public int ItemsRemainingCounter
		  {
			   get
			   {
					return _ItemsRemainingCounter;
			   }
			   set
			   {
					if (_ItemsRemainingCounter == value)
						 return;

					_ItemsRemainingCounter = value;
					OnPropertyChanged(nameof(ItemsRemainingCounter));
			   }
		  }

		  /// <summary>
		  /// True if the backup for the folder is done
		  /// </summary>
		  private bool _IsBackupDone;

		  public bool IsBackupDone
		  {
			   get { return _IsBackupDone; }
			   set { if (_IsBackupDone == value) return; _IsBackupDone = value; OnPropertyChanged(nameof(IsBackupDone)); }
		  }

		  /// <summary>
		  /// Progress object to update <see cref="ItemsRemainingCounter"/> and <see cref="ItemsCompletedCounter"/> while running a task
		  /// </summary>
		  public Progress<ProgressionItemsReport> ItemsProgress { get; set; }

		  /// <summary>
		  /// Progress object to update <see cref="State"/> while running a task
		  /// </summary>
		  public Progress<string> StateProgress { get; set; }

		  private IDialogService _DialogService;

		  public IDialogService DialogService
		  {
			   get { return _DialogService; }
			   set { _DialogService = value; }
		  }


		  public BackupStatus(IDialogService dialogService)
		  {
			   DialogService = dialogService;

			   StateProgress = new Progress<string>();
			   StateProgress.ProgressChanged += StateProgress_ProgressChanged;

			   ItemsProgress = new Progress<ProgressionItemsReport>();
			   ItemsProgress.ProgressChanged += ItemsProgress_ProgressChanged;
		  }

		  private void StateProgress_ProgressChanged(object sender, string e)
		  {
			   State = e;
		  }

		  private void ItemsProgress_ProgressChanged(object sender, ProgressionItemsReport e)
		  {
			   ItemsCompletedCounter = e.ItemsCompletedCounter;
			   ItemsRemainingCounter = e.ItemsRemainingCounter;
		  }
	 }
}
