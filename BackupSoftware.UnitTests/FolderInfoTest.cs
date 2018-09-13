using System;
using BackupSoftware.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BackupSoftware.Services;
using System.Collections.ObjectModel;

namespace BackupSoftware.UnitTests
{
	 [TestClass]
	 public class FolderInfoTest
	 {
		  [TestMethod]
		  public void CalcItemsCount_PathIsEmpty_ReturnsFalse()
		  {
			   // Arrange
			   var folderInfo = new FolderInfo(string.Empty);

			   // Act 
			   var result = folderInfo.ItemsCount != -1;

			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void CalcItemsCount_PathNotExists_ReturnsFalse()
		  {
			   // Arrange
			   var folderInfo = new FolderInfo("c:/test");

			   // Act 
			   var result = folderInfo.ItemsCount != -1;

			   // Assert
			   Assert.IsFalse(result);
		  }

		  [TestMethod]
		  public void CalcItemsCount_PathExists_ReturnsTrue()
		  {
			   // Arrange
			   var folderInfo = new FolderInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			   // Act 
			   var result = folderInfo.ItemsCount != -1;

			   // Assert
			   Assert.IsTrue(result);
		  }
	 }
}
