using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Core.Services
{
	 public interface IScreenshotsDetailsDataService
	 {
		  ScreenshotsDetails GetDetails();

		  void Save(ScreenshotsDetails details);
	 }
}
