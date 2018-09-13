using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core.Services
{
	 public interface IBackupService
	 {
		  void DeleteFilesFromBackup(string source, string dest, BackupStatus backupStatus);

		  void Backup(string source, string dest, BackupStatus backupStatus);

		  void BackupFile(string destFolder, string fileToCopy, BackupStatus backupStatus);

		  Task BackupOneDirAsync(string destinationPath, SourceFolder sourceFolder, BackupStatus backupStatus);
	 }
}
