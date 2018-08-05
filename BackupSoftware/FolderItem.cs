
namespace BackupSoftware
{
	 /// <summary>
	 /// This class represents each item in the list box 
	 /// </summary>
	 public class FolderItem
	 {
		  /// <summary>
		  /// The path of the folder
		  /// </summary>
		  public string FolderPath { get; set; }

		  /// <summary>
		  /// A check to see if the user whats to delete the prev content in the backup drive
		  /// </summary>
		  public bool DeletePrevContent { get; set; }

		  /// <summary>
		  /// The count of the files and folders
		  /// </summary>
		  public int ContentCount { get; set; }

		  /// <summary>
		  /// Default Constructor
		  /// </summary>
		  public FolderItem()
		  {
			   FolderPath = string.Empty;
			   DeletePrevContent = false;
		  }

		  public FolderItem(string folderPath, bool deletePrevContent)
		  {
			   FolderPath = folderPath;
			   DeletePrevContent = deletePrevContent;
		  }
	 }
}
