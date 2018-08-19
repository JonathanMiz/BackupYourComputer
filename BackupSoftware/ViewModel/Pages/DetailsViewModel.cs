using BackupSoftware.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Input;

namespace BackupSoftware
{
	 public class DetailsViewModel : ViewModelBase
	 {

		  #region Private Members

		  /// <summary>
		  /// The text to display which folders the user chose
		  /// </summary>
		  private string _SourceFoldersText { get; set; }


		  /// <summary>
		  /// Dialog service to display messages to the screen
		  /// </summary>
		  private IDialogService _DialogService;

		  /// <summary>
		  /// The view model of the display view page
		  /// </summary>
		  private DisplayViewModel _DisplayViewModel;




		  #endregion

		  #region Public Members

		  public string SourceFoldersText
		  {
			   get
			   {
					return _SourceFoldersText;
			   }
			   set
			   {
					if (_SourceFoldersText == value)
						 return;

					_SourceFoldersText = value;
					OnPropertyChanged(nameof(SourceFoldersText));
			   }
		  }


		  /// <summary>
		  /// Reference to <see cref="CacheViewModel.SourceFolders"/>
		  /// </summary>
		  public ObservableCollection<FolderListItem> SourceFolders
		  {
			   get
			   {
					return IoC.Get<CacheViewModel>().SourceFolders;
			   }
			   set
			   {
					IoC.Get<CacheViewModel>().SourceFolders = value;
			   }
		  }

		  /// <summary>
		  /// The destination folder
		  /// </summary>
		  public string DestFolder
		  {
			   get
			   {
					return IoC.Get<CacheViewModel>().DestFolder;
			   }
			   set
			   {
					IoC.Get<CacheViewModel>().DestFolder = value;
			   }
		  }

		  public DisplayViewModel DisplayViewModel
		  {
			   get { return _DisplayViewModel; }
			   set { if (_DisplayViewModel == value) return; _DisplayViewModel = value; OnPropertyChanged(nameof(DisplayViewModel)); }
		  }


		  #endregion

		  #region Helpers
		  /// <summary>
		  /// Extracting the names from the folders that the user chose
		  /// </summary>
		  /// <returns></returns>
		  private string ExtractFolderNames()
		  {
			   string list = "";
			   foreach (FolderListItem item in IoC.Get<CacheViewModel>().SourceFolders)
			   {
					list += Helpers.ExtractFileFolderNameFromFullPath(item.FolderPath);
					list += ", ";
			   }
			   if(list != "")
					return list.Substring(0, list.Length - 2);
			   return list;
		  }
		  #endregion

		  #region Commands

		  /// <summary>
		  /// The command to choose destination folder
		  /// </summary>
		  public ICommand SelectDestFolderCommand { get; set; }
		  /// <summary>
		  /// The command to start the backup
		  /// </summary>
		  public ICommand StartBackupCommand { get; set; }
		  /// <summary>
		  /// The command to open the page of selecting folders to backup
		  /// </summary>
		  public ICommand GoToSelectSourceCommand { get; set; }

		  /// <summary>
		  /// The command to redirect to the display view
		  /// </summary>
		  public ICommand GoToDisplayCommand { get; set; }
		  #endregion

		  #region Command Functions

		  /// <summary>
		  /// The action when the user choose backup folder
		  /// </summary>
		  void SelectDestFolder()
		  {
			   string folder = _DialogService.SelectFolder("Choose folder to backup in hard drive");

			   if (folder != null)
					DestFolder = folder;

		  }

		  /// <summary>
		  /// Redirecting the user to the select source view page
		  /// </summary>
		  void GoToSelectSource()
		  {
			   // Injecting the SelectSourceViewModel
			   IoC.Get<ApplicationViewModel>().CurrentViewModel = IoC.Kernel.Get<SelectSourceViewModel>();
		  }

		  /// <summary>
		  /// Go to the display view
		  /// </summary>
		  private void GoToDisplay()
		  {
			   if (IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					IoC.Get<ApplicationViewModel>().CurrentViewModel = DisplayViewModel;
			   }
		  }
		  
		  /// <summary>
		  /// Validates if the user gave right input
		  /// </summary>
		  private void ValidateUserInput()
		  {
			   // Checks to see if there is content in the fields
			   if (string.IsNullOrEmpty(DestFolder) || SourceFolders.Count == 0)
			   {
					_DialogService.ShowMessageBox("Fill in the list of folder and hard drive!");
					return;
			   }

			   // Check to see if the folders exists
			   foreach (FolderListItem item in SourceFolders)
			   {
					if (!Directory.Exists(item.FolderPath))
					{
						 _DialogService.ShowMessageBox(item.FolderPath + " can not be found!");
						 return;
					}

			   }

			   if (!Directory.Exists(DestFolder))
			   {
					_DialogService.ShowMessageBox(DestFolder + " can not be found!");
					return;
			   }
		  }

		  private void StartBackup()
		  {
			   if (!IoC.Get<CacheViewModel>().IsBackupRunning)
			   {
					// Validate user input
					ValidateUserInput();

					// Run the back up
					DisplayViewModel.RunBackup();

					// Redirect the user to the display view page
					GoToDisplay();
			   }
			   else
			   {
					_DialogService.ShowMessageBox("The buckup is already running!");
			   }
		  }
		  #endregion

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public DetailsViewModel(IDialogService dialogService, DisplayViewModel displayViewModel)
		  {
			   _DialogService = dialogService;
			   DisplayViewModel = displayViewModel;


			   SelectDestFolderCommand = new RelayCommand(SelectDestFolder);
			   GoToSelectSourceCommand = new RelayCommand(GoToSelectSource);
			   StartBackupCommand = new RelayCommand(StartBackup);
			   GoToDisplayCommand = new RelayCommand(GoToDisplay);

			   SourceFoldersText = ExtractFolderNames();

			  
		  }
	 }
}
