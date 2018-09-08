using System.Drawing;
using System.Windows;

namespace BackupSoftware.Services
{
	 public class WindowsTakeScreenshot : Core.Services.ITakeScreenshotService
	 {
		  public void TakeAndSaveScreenshot(string screenshotPath)
		  {
			   double screenLeft = 0;
			   double screenTop = 0;
			   double screenWidth = SystemParameters.PrimaryScreenWidth;
			   double screenHeight = SystemParameters.PrimaryScreenHeight;

			   using (Bitmap bmp = new Bitmap((int)screenWidth, (int)screenHeight))
			   {
					using (Graphics g = Graphics.FromImage(bmp))
					{
						 g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);
						 bmp.Save(screenshotPath);
					}
			   }
		  }
	 }
}
