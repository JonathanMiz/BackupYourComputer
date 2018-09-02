using BackupSoftware.Core;
using BackupSoftware.Core.Services;
using BackupSoftware.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace BackupSoftware
{
	 /// <summary>
	 /// Interaction logic for App.xaml
	 /// </summary>
	 public partial class App : Application
	 {
		  /// <summary>
		  /// Costum startup to load the IoC immediately before anything else
		  /// </summary>
		  /// <param name="e"></param>
		  protected override void OnStartup(StartupEventArgs e)
		  {
			   // Let the base do what it needs
			   base.OnStartup(e);

			   // Bind Windows specific dialog service
			   IoC.Kernel.Bind<IDialogService>().To<WindowsDialogService>();

			   // Setup IoC
			   IoC.Setup();


			   Current.MainWindow = new MainWindow();
			   Current.MainWindow.Show();
		  }

		  protected override void OnExit(ExitEventArgs e)
		  {
			   base.OnExit(e);

			   IoC.Get<CacheViewModel>().Save();
		  }
	 }
}
