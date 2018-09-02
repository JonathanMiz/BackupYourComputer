using Newtonsoft.Json;
using System.IO;

namespace BackupSoftware.Core.Services
{
	 class JsonDetailsDataService : IDetailsDataService
	 {
		  private readonly string DATA_PATH = "details.json";

		  public Details GetDetails()
		  {
			   if (!File.Exists(DATA_PATH))
			   {
					File.Create(DATA_PATH).Close();
			   }

			   string serializedDetails = File.ReadAllText(DATA_PATH);
			   Details details = JsonConvert.DeserializeObject<Details>(serializedDetails);


			   if (details == null)
					return new Details();

			   return details;
			   
		  }

		  public void Save(Details details)
		  {
			   string serializedDetails = JsonConvert.SerializeObject(details);
			   File.WriteAllText(DATA_PATH, serializedDetails);
		  }
	 }
}
