
using Ninject;

namespace BackupSoftware.Core
{
    public class ViewModelLocator
    {
		  public static ViewModelLocator Instance  { get; private set; } = new ViewModelLocator();

		  public static ApplicationViewModel ApplicationViewModel => IoC.Kernel.Get<ApplicationViewModel>();
		  public static CacheViewModel CacheViewModel => IoC.Kernel.Get<CacheViewModel>();

		  // Pages
		  public static DetailsViewModel DetailsViewModel { get; private set; } = IoC.Kernel.Get<DetailsViewModel>(); // Injecting the DetailsViewModel
		  public static ScreenshotsViewModel ScreenshotsViewModel{ get; private set; } = IoC.Kernel.Get<ScreenshotsViewModel>(); // Injecting the ScreenshotsViewModel


	 }
}
