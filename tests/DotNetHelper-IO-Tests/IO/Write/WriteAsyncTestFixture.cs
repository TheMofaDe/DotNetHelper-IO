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
	public class WriteTestFixture : BaseTest
	{

		public FolderObject TestFolder { get; }

		public WriteTestFixture()
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
		public void Test_Write_To_File_Append()
		{
			var testFile = RandomTestFileNoExtension;

			testFile.Write("A", FileOption.Append);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));

			testFile.Write("B".ToByteArray(), FileOption.Append);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("AB"));

			testFile.Write("C".ToStream(), FileOption.Append);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("ABC"));
		}

		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Write_To_File_Overwrite()
		{
			var testFile = RandomTestFileNoExtension;

			testFile.Write("A", FileOption.Overwrite);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));

			testFile.Write("B".ToByteArray(), FileOption.Overwrite);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("B"));

			testFile.Write("C".ToStream(), FileOption.Overwrite);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("C"));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Write_To_File_DoNothingIfFileExist()
		{
			var testFile = RandomTestFileNoExtension;

			testFile.Write("A", FileOption.DoNothingIfExist);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));

			testFile.Write("B".ToByteArray(), FileOption.DoNothingIfExist);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));

			testFile.Write("C".ToStream(), FileOption.DoNothingIfExist);
			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));
		}

		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Write_To_File_IncrementFileExtensionIfExist()
		{
			var testFile = RandomTestFileNoExtension;

			testFile.Write("A", FileOption.IncrementFileExtensionIfExist);
			var newFileWithNameIncruemented = testFile.Write("B".ToByteArray(), FileOption.IncrementFileExtensionIfExist);
			var newFileWithNameIncruemented2 = testFile.Write("C".ToStream(), FileOption.IncrementFileExtensionIfExist);

			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));
			Assert.That(File.ReadAllText(newFileWithNameIncruemented).Equals("B"));
			Assert.That(File.ReadAllText(newFileWithNameIncruemented2).Equals("C"));

			Assert.AreNotSame(newFileWithNameIncruemented2, newFileWithNameIncruemented);
			Assert.AreNotSame(testFile, newFileWithNameIncruemented);
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_Write_To_File_IncrementFileNameIfExist()
		{
			var testFile = RandomTestFileNoExtension;

			testFile.Write("A", FileOption.IncrementFileNameIfExist);
			var newFileWithNameIncruemented = testFile.Write("B".ToByteArray(), FileOption.IncrementFileNameIfExist);
			var newFileWithNameIncruemented2 = testFile.Write("C".ToStream(), FileOption.IncrementFileNameIfExist);

			Assert.That(File.ReadAllText(testFile.FullName).Equals("A"));
			Assert.That(File.ReadAllText(newFileWithNameIncruemented).Equals("B"));
			Assert.That(File.ReadAllText(newFileWithNameIncruemented2).Equals("C"));

			Assert.AreNotSame(newFileWithNameIncruemented2, newFileWithNameIncruemented);
			Assert.AreNotSame(testFile, newFileWithNameIncruemented);
		}














	}
}