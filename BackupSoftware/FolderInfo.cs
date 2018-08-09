﻿using System;
using System.Collections.Generic;
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

		  public FolderInfo()
		  {

		  }

		  public FolderInfo(string folderPath)
		  {
			   FolderPath = folderPath;
			   Name = Helpers.ExtractFileFolderNameFromFullPath(FolderPath);
		  }

	 }
}
