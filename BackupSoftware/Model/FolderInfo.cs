using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	 /// <summary>
	 /// The general information about the folder
	 /// </summary>
	 public class FolderInfo
	 {
		  /// <summary>
		  /// The source of the folder
		  /// </summary>
		  public string FolderPath { get; set; }

		  /// <summary>
		  /// The name of the folder
		  /// </summary>
		  public string Name { get; set; }

		  /// <summary>
		  /// How many files and folders this folder contains
		  /// </summary>
		  public int ItemsCount { get; set; }


		  public FolderInfo(string folderPath)
		  {
			   FolderPath = folderPath;
			   Name = Helpers.ExtractFileFolderNameFromFullPath(FolderPath);
			   ItemsCount = CalcItemsCount(FolderPath);
		  }

		  /// <summary>
		  /// Returns the amount of files and folders in the program
		  /// </summary>
		  /// <param name="path"></param>
		  /// <returns></returns>
		  private int CalcItemsCount(string path)
		  {
			   int fileCount = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly).Length;
			   int folderCount = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly).Length;

			   int result = (fileCount + folderCount);

			   return result;
		  }

	 }
}
