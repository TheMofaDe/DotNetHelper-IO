using System;
using System.IO;
using System.Threading.Tasks;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO_Tests;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests
{
	[NonParallelizable]
	public class CopyAsyncTestFixture : BaseTest
	{

		public FolderObject TestFolder { get; }

		public CopyAsyncTestFixture()
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
		public async Task Test_CopyTo_To_File_Append()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create original file 
			await testFile.WriteAsync(fileContent);


			Assert.IsFalse(File.Exists(outputFile.FullName));
			await testFile.CopyToAsync(outputFile.FullName, FileOption.Append);
			Assert.IsTrue(File.Exists(outputFile.FullName));

			Assert.IsTrue(fileContent.Equals(await File.ReadAllTextAsync(outputFile.FullName)));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_CopyTo_To_File_Overwrite_When_Destination_Doesnt_Exist()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create original file 
			await testFile.WriteAsync(fileContent, FileOption.Overwrite, null);


			Assert.IsFalse(File.Exists(outputFile.FullName));
			await testFile.CopyToAsync(outputFile.FullName, FileOption.Overwrite);
			Assert.IsTrue(File.Exists(outputFile.FullName));

			Assert.IsTrue(fileContent.Equals(await File.ReadAllTextAsync(outputFile.FullName)));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_CopyTo_To_File_Overwrite_When_Destination_Exist()
		{
			var fileContent = $"ABC{Environment.NewLine}";
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			// create both file to make sure overwrite works  
			await testFile.WriteAsync(fileContent);
			await outputFile.WriteAsync("Something Else");

			Assert.IsTrue(File.Exists(outputFile.FullName));
			Assert.IsTrue(File.Exists(outputFile.FullName));

			testFile.CopyTo(outputFile.FullName, FileOption.Overwrite);

			Assert.IsTrue(fileContent.Equals(await File.ReadAllTextAsync(outputFile.FullName)));
		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Copy_To_File_DoNothing_When_FileExist()
		{
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("ABC");

			outputFile.CreateOrTruncate();

			testFile.CopyTo(outputFile.FullName, FileOption.DoNothingIfExist);
			Assert.That((await File.ReadAllTextAsync(outputFile.FullName)).Equals(string.Empty));


		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Copy_To_File_IncrementFileName()
		{
			var testFile = RandomTestFileNoExtension;
			var outputFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("ABC");

			outputFile.CreateOrTruncate();

			var newFileName = await testFile.CopyToAsync(outputFile.FullName, FileOption.IncrementFileNameIfExist);

			Assert.That(!newFileName.Equals(testFile.FullName + "1"));
			Assert.That((await File.ReadAllTextAsync(newFileName)).Equals("ABC"));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Copy_To_File_IncrementFileNameExtension()
		{
			var testFile = RandomTestFileWithExtension;
			var outputFile = RandomTestFileWithExtension;

			await testFile.WriteAsync("ABC");

			outputFile.CreateOrTruncate();

			var newFileName = await testFile.CopyToAsync(outputFile.FullName, FileOption.IncrementFileExtensionIfExist);

			Assert.That(!newFileName.Equals(testFile.FullName + "1"));
			Assert.That((await File.ReadAllTextAsync(newFileName)).Equals("ABC"));
		}



	}
}