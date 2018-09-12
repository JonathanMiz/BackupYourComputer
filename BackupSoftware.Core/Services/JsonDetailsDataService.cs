using Newtonsoft.Json;
using System.IO;

namespace BackupSoftware.Core.Services
{
	 class JsonDetailsDataService : IDetailsDataService
	 {
		  private readonly string DATA_PATH = "details.json";

		  public BackupDetails GetDetails()
		  {
			   if (!File.Exists(DATA_PATH))
			   {
					File.Create(DATA_PATH).Close();
			   }

			   string serializedDetails = File.ReadAllText(DATA_PATH);
			   BackupDetails details = JsonConvert.DeserializeObject<BackupDetails>(serializedDetails);


			   if (details == null)
					return new BackupDetails();

			   return details;
			   
		  }

		  public void Save(BackupDetails details)
		  {
			   string serializedDetails = JsonConvert.SerializeObject(details);
			   File.WriteAllText(DATA_PATH, serializedDetails);
		  }
	 }
}
