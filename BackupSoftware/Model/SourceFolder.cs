
using System;



namespace BackupSoftware
{
	 /// <summary>
	 /// This class represents each item in the list box 
	 /// </summary>
	 public class SourceFolder
	 {
		  /// <summary>
		  /// The source of the folder
		  /// </summary>
		  public FolderInfo FolderInfo { get; set; }

		  /// <summary>
		  /// A check to see if the user whats to delete the prev content in the backup drive
		  /// </summary>
		  public bool DeletePrevContent { get; set; } = false;

		  public SourceFolder(string folderPath, bool deletePrevContent = false)
		  {
			   FolderInfo = new FolderInfo(folderPath);
			   DeletePrevContent = deletePrevContent;
		  }
	 }
}

