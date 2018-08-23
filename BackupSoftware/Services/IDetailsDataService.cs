using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware.Services
{
	 public interface IDetailsDataService
	 {
		  Details GetDetails();

		  void Save(Details details);
	 }
}
