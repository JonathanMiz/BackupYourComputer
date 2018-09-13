using System;
using BackupSoftware.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackupSoftware.Services;
using System.Collections.ObjectModel;

namespace BackupSoftware.UnitTests
{
	 [TestClass]
	 public class BackupDetailsTest
	 {
		  [TestMethod]
		  public void Validate_DestFolderIsEmpty_ReturnsFalse()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails {
					SourceFolders = new ObservableCollection<SourceFolder>
					{
						 new SourceFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
					}
			   };

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void Validate_SourceFoldersIsEmpty_ReturnsFalse()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails
			   {
					DestFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			   };

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void Validate_SourceFoldersAndDestFolderAreEmpty_ReturnsFalse()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails();

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void Validate_SourceFoldersDontExist_ReturnsFalse()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails {
					SourceFolders = new ObservableCollection<SourceFolder>
					{
						 new SourceFolder("C:/test")
					},
					DestFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			   };

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void Validate_DestFolderDoesntExist_ReturnsFalse()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails
			   {
					SourceFolders = new ObservableCollection<SourceFolder>
					{
						   new SourceFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
					},
					DestFolder = "C:/test"
			   };

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void Validate_SourceFoldersAndDestFolderExist_ReturnsTrue()
		  {
			   // Arrange
			   var backupDetails = new BackupDetails
			   {
					SourceFolders = new ObservableCollection<SourceFolder>
					{
						   new SourceFolder(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
					},
					DestFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			   };

			   // Act 
			   var result = backupDetails.Validate(new WindowsDialogService());


			   // Assert
			   Assert.IsTrue(result);
		  }
	 }
}
