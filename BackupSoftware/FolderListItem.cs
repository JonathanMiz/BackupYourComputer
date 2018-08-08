
using System;



namespace BackupSoftware
{
	 public class ProgressItemModel
	 {
		  public int Count { get; set; } = 0;
	 }

	 /// <summary>
	 /// This class represents each item in the list box 
	 /// </summary>
	 public class FolderListItem : ViewModelBase
	 {
		  /// <summary>
		  /// The source of the folder
		  /// </summary>
		  public string FolderPath { get; set; }

		  /// <summary>
		  /// A check to see if the user whats to delete the prev content in the backup drive
		  /// </summary>
		  public bool DeletePrevContent { get; set; }

		  /// <summary>
		  /// The name of the folder
		  /// </summary>
		  public string Name { get; set; }

		  /// <summary>
		  /// The dest of the folder
		  /// </summary>
		  public string To { get; set; }

		  /// <summary>
		  /// The total number of files and folders
		  /// </summary>
		  public int ContentTotalCount { get; set; }

		  /// <summary>
		  /// The variable to count files and folders that was backed up
		  /// </summary>
		  int m_ContentCount { get; set; }
		  public int ContentCount
		  {
			   get
			   {
					return m_ContentCount;
			   }
			   set
			   {
					if (m_ContentCount == value)
						 return;

					m_ContentCount = value;
					OnPropertyChanged(nameof(ContentCount));
			   }
		  }

		  /// <summary>
		  /// This notify of progress
		  /// </summary>
		  public Progress<ProgressItemModel> Progress { get; set; }

		  public Progress<string> LogProgress { get; set; }

		  private string m_Log { get; set; }
		  public string Log
		  {
			   get
			   {
					return m_Log;
			   }
			   set
			   {
					if (m_Log == value)
						 return;

					m_Log = value;
					OnPropertyChanged(nameof(Log));
			   }
		  }



		  /// <summary>
		  /// Event for updating the value of the count 
		  /// </summary>
		  /// <param name="sender"></param>
		  /// <param name="e"></param>
		  private void Progress_ProgressChanged(object sender, ProgressItemModel e)
		  {
			   ContentCount = e.Count;
		  }

		  private void LogProgress_ProgressChanged(object sender, string e)
		  {
			   Log = e;
		  }

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public FolderListItem()
		  {
			   FolderPath = string.Empty;
			   DeletePrevContent = false;

			   Progress = new Progress<ProgressItemModel>();
			   Progress.ProgressChanged += Progress_ProgressChanged;

			   LogProgress = new Progress<string>();
			   LogProgress.ProgressChanged += LogProgress_ProgressChanged;
		  }

		  public FolderListItem(string folderPath, bool deletePrevContent)
		  {
			   FolderPath = folderPath;
			   DeletePrevContent = deletePrevContent;

			   Progress = new Progress<ProgressItemModel>();
			   Progress.ProgressChanged += Progress_ProgressChanged;

			   LogProgress = new Progress<string>();
			   LogProgress.ProgressChanged += LogProgress_ProgressChanged;
		  }
	 }
}

