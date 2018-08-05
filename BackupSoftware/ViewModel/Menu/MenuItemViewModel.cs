using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System;
using System.Xaml;
using System.Configuration;
using Ninject;

namespace BackupSoftware
{
	 /// <summary>
	 /// The model class that represents the UI form
	 /// </summary>
	 public class MenuItemViewModel : ViewModelBase
	 {
		  /// <summary>
		  /// The code of the icon from FontAwesome
		  /// </summary>
		  public string IconCode { get; set; }

		  /// <summary>
		  /// The name of the item
		  /// </summary>
		  public string Title { get; set; }

		  /// <summary>
		  /// The page of this menu item
		  /// </summary>
		  public ApplicationPage Page { get; set; }

		  public RelayCommand OpenPageCommand { get; set; }

		  public MenuItemViewModel()
		  {
			   OpenPageCommand = new RelayCommand(OpenPage);
		  }

		  void OpenPage()
		  {
			   IoC.Kernel.Get<ApplicationViewModel>().CurrentPage = Page;
		  }
	 }
}
