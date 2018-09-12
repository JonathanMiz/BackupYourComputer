using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core
{
	 public class ReportFile
	 {
		  public static string LogPath = "log.txt";

		  private static string log = "";

		  public static void ClearLog()
		  {
			   log = "";
		  }

		  public static void WriteToLog(string text)
		  {
			   log += $"[{ DateTime.Now.ToString("dd.MM.yy HH:mm:ss")}] {text}{Environment.NewLine}";
		  }

		  public static void SaveToLog()
		  {
			   File.WriteAllText(LogPath, "");
			   File.AppendAllText(LogPath, log);
		  }
	 }
}
