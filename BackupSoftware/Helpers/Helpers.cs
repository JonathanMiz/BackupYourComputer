using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
{
	/// <summary>
	/// General class for helpers functions
	/// </summary>
    public class Helpers
    {
		  /// <summary>
		  /// If subfolder is sub folder of folder
		  /// </summary>
		  /// <param name="folder">The folder </param>
		  /// <param name="subFolder">The folder to check if it is subfolder</param>
		  /// <returns></returns>
		  public static bool IsSubFolder(string folder, string subFolder)
		  {
			   string normailzedFolder = folder.Replace('\\', '/') + '/';
			   string normlizedSubFolder = subFolder.Replace('\\', '/') + '/';

			   bool result = normlizedSubFolder.Contains(normailzedFolder);
			   return result;
		  }

		  /// <summary>
		  /// Gives us the end of the path whether it's a file or a folder
		  /// </summary>
		  /// <param name="fullPath"></param>
		  /// <returns></returns>
		  public static  string ExtractFileFolderNameFromFullPath(string fullPath)
		  {
			   if (fullPath != null)
			   {
					var normalizedPath = fullPath.Replace('\\', '/');

					int lastSlash = normalizedPath.LastIndexOf('/');

					return normalizedPath.Substring(lastSlash + 1);
			   }

			   return "";
		  }
	 }
}
