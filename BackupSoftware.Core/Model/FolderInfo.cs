using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 /// <summary>
	 /// The general information about the folder
	 /// </summary>
	 public class FolderInfo
	 {
		  /// <summary>
		  /// The source of the folder
		  /// </summary>
		  public string FullPath { get; set; }

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
			   FullPath = folderPath;
			   Name = Helpers.ExtractFileFolderNameFromFullPath(FullPath);
			   ItemsCount = CalcItemsCount();
		  }

		  /// <summary>
		  /// Returns the amount of files and folders in the program
		  /// </summary>
		  /// <param name="path"></param>
		  /// <returns></returns>
		  private int CalcItemsCount()
		  {
			   int result = -1;
			   if (!Directory.Exists(FullPath))
					return result;

			   if (FullPath != null)
			   {
					int fileCount = Directory.GetFiles(FullPath, "*.*", SearchOption.TopDirectoryOnly).Length;
					int folderCount = Directory.GetDirectories(FullPath, "*.*", SearchOption.TopDirectoryOnly).Length;

					result = (fileCount + folderCount);
			   }

			   return result;
		  }

	 }
}
