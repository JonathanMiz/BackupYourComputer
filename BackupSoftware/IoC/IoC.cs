using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSoftware
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
			   // Bind all required singleton view models

			   // Bind to a single instance of Application view model
			   Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
			   Kernel.Bind<SelectedFoldersViewModel>().ToConstant(new SelectedFoldersViewModel());

		  }
    }
}
