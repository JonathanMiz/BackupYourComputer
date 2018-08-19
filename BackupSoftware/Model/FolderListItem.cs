
using System;



namespace BackupSoftware
{
	 /// <summary>
	 /// This class represents each item in the list box 
	 /// </summary>
	 public class FolderListItem
	 {
		  /// <summary>
		  /// The source of the folder
		  /// </summary>
		  public string FolderPath { get; set; }

		  /// <summary>
		  /// A check to see if the user whats to delete the prev content in the backup drive
		  /// </summary>
		  public bool DeletePrevContent { get; set; } = false;

		  public FolderListItem(string folderPath, bool deletePrevContent = false)
		  {
			   FolderPath = folderPath;
			   DeletePrevContent = deletePrevContent;
		  }
	 }
}

