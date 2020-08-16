using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotNetHelper.IO.Tests.Extensions;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO_Tests;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests
{
	[NonParallelizable]
	public class CopyTestFixture : BaseTest
	{

		public FolderObject TestFolder { get; }

		public CopyTestFixture()
		{
			TestFolder = new FolderObject(WorkingDirectory);
		}



		[OneTimeSetUp]
		public void ClassInit()
		{
			// Executes once for the test class. (Optional)
			new FolderObject(BaseFolder).Delete(true); // PURGE EVERYTHING
		}

		[OneTimeTearDown]
		public void ClassCleanup()
		{
			// Runs once after all tests in this class are executed. (Optional)
			// Not guaranteed that it executes instantly after all tests from the class.
			new FolderObject(BaseFolder).Delete(true); // PURGE EVERYTHING
		}

		[SetUp]
		public void TestInit()
		{
			// Runs before each test. (Optional)
		}

		[TearDown]
		public void TestCleanup()
		{
			// Runs after each test. (Optional)
			TestFolder.Delete(true); // PURGE EVERYTHING
		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_CopyTo_To_File_Append()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create original file 
			testFile.Write(fileContent);


			Assert.IsFalse(File.Exists(outputFile.FullName));
			testFile.CopyTo(outputFile.FullName, FileOption.Append);
			Assert.IsTrue(File.Exists(outputFile.FullName));

			Assert.IsTrue(fileContent.Equals(File.ReadAllText(outputFile.FullName)));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_CopyTo_To_File_Overwrite_When_Destination_Doesnt_Exist()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create original file 
			testFile.Write(fileContent);


			Assert.IsFalse(File.Exists(outputFile.FullName));
			testFile.CopyTo(outputFile.FullName, FileOption.Overwrite);
			Assert.IsTrue(File.Exists(outputFile.FullName));

			Assert.IsTrue(fileContent.Equals(File.ReadAllText(outputFile.FullName)));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_CopyTo_To_File_Overwrite_When_Destination_Exist()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create both file to make sure overwrite works  
			testFile.Write(fileContent);
			outputFile.Write("Something Else");

			Assert.IsTrue(File.Exists(outputFile.FullName));
			Assert.IsTrue(File.Exists(outputFile.FullName));

			testFile.CopyTo(outputFile.FullName, FileOption.Overwrite);

			Assert.IsTrue(fileContent.Equals(File.ReadAllText(outputFile.FullName)));
		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Copy_To_File_DoNothing_When_FileExist()
		{
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			testFile.Write("ABC");

			outputFile.CreateOrTruncate();

			testFile.CopyTo(outputFile.FullName, FileOption.DoNothingIfExist);
			Assert.That(File.ReadAllText(outputFile.FullName).Equals(string.Empty));


		}

















		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Copy_To_File_IncrementFileName()
		{
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			testFile.Write("ABC");

			outputFile.CreateOrTruncate();

			var newFileName = testFile.CopyTo(outputFile.FullName, FileOption.IncrementFileNameIfExist);

			Assert.That(!newFileName.Equals(testFile.FullName + "1"));
			Assert.That(File.ReadAllText(newFileName).Equals("ABC"));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Copy_To_File_IncrementFileNameExtension()
		{
			var testFile = RandomTestFileWithExtension;
			var outputFile = RandomTestFileWithExtension;

			testFile.Write("ABC");

			outputFile.CreateOrTruncate();

			var newFileName = testFile.CopyTo(outputFile.FullName, FileOption.IncrementFileExtensionIfExist);

			Assert.That(!newFileName.Equals(testFile.FullName + "1"));
			Assert.That(File.ReadAllText(newFileName).Equals("ABC"));
		}










	}
}