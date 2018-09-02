﻿using BackupSoftware.Core.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;

namespace BackupSoftware.Services
{
	 class WindowsDialogService : IDialogService
	 {
		  public string SelectFolder(string title)
		  {
			   var dlg = new CommonOpenFileDialog
			   {
					Title = title,
					IsFolderPicker = true,
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			   };

			   if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			   {
					return dlg.FileName;
			   }

			   return null;
		  }

		  public IEnumerable<string> SelectFolders(string title)
		  {
			   var dlg = new CommonOpenFileDialog
			   {
					Title = title,
					IsFolderPicker = true,
					Multiselect = true,
					InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			   };

			   if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
			   {
					return dlg.FileNames;
			   }

			   return null;
		  }

		  public void ShowMessageBox(string message)
		  {
			   MessageBox.Show(message);
		  }

		  public bool ShowYesNoMessageBox(string message, string title)
		  {
			   return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
		  }
	 }
}
