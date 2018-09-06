using BackupSoftware.Core.Services;
using Ninject;

namespace BackupSoftware.Core
{
	/// <summary>
	/// The IoC container for our application
	/// </summary>
    public static class IoC
    {
		  /// <summary>
		  /// The kernel for IoC
		  /// Gat and bind all the information in the hole of ninject
		  /// </summary>
		  public static IKernel Kernel { get; private set; } = new StandardKernel();

		  /// <summary>
		  /// Sets up the IoC, binds all information required and is ready for use
		  /// </summary>
		  public static void Setup()
		  {
			   
			   // Binding IDialogService to WindowDialogService, so ninject could instantiate WindowDialogService whenever
			   // IDialogService is a dependency of some class constructor
			   Kernel.Bind<IDetailsDataService>().To<JsonDetailsDataService>();
			   Kernel.Bind<IScreenshotsDetailsDataService>().To<JsonScreenshotsDetailsDataService>();

			   // Bind to a single instance of Application view model
			   Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());

			   // Injecting CacheViewModel class
			   var cacheViewModel = Kernel.Get<CacheViewModel>();

			   // Bind CacheViewModel so it would act like a singleton read more about this!!!
			   Kernel.Bind<CacheViewModel>().ToConstant(cacheViewModel);

		  }

		  public static T Get<T>()
		  {
			   return Kernel.Get<T>();
		  }
    }
}
