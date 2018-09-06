using Newtonsoft.Json;
using System.IO;

namespace BackupSoftware.Core.Services
{
	 class JsonScreenshotsDetailsDataService : IScreenshotsDetailsDataService
	 {
		  private readonly string DATA_PATH = "screenshots_details.json";

		  public ScreenshotsDetails GetDetails()
		  {
			   if (!File.Exists(DATA_PATH))
			   {
					File.Create(DATA_PATH).Close();
			   }

			   string serializedDetails = File.ReadAllText(DATA_PATH);
			   ScreenshotsDetails screenshotDetails = JsonConvert.DeserializeObject<ScreenshotsDetails>(serializedDetails);


			   if (screenshotDetails == null)
					return new ScreenshotsDetails();

			   return screenshotDetails;

		  }

		  public void Save(ScreenshotsDetails details)
		  {
			   string serializedDetails = JsonConvert.SerializeObject(details);
			   File.WriteAllText(DATA_PATH, serializedDetails);
		  }
	 }
}
