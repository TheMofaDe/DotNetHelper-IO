//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Bogus;
//using DotNetHelper_IO;
//using DotNetHelper_IO.Enum;
//using DotNetHelper_IO_Tests;
//using NUnit.Framework;


//namespace Tests
//{
//	[TestFixture]
//	//[SingleThreadedAttribute]
//	[NonParallelizable]
//	public class FileObjectAsyncTestFixture : BaseTest
//	{
//		public FolderObject TestFolder { get; }
//		// public FileObject TestFile { get; }

//		private readonly char[] Chars =
//			Enumerable
//				.Range(char.MinValue, char.MaxValue)
//				.Select(x => (char)x)
//				.Where(c => !char.IsControl(c))
//				.ToArray();


//		public FileObjectAsyncTestFixture()
//		{
//			TestFolder = new FolderObject(WorkingDirectory);
//		}



//		[OneTimeSetUp]
//		public void ClassInit()
//		{
//			// Executes once for the test class. (Optional)
//			new FolderObject(BaseFolder).Delete(true); // PURGE EVERYTHING
//		}

//		[OneTimeTearDown]
//		public void ClassCleanup()
//		{
//			// Runs once after all tests in this class are executed. (Optional)
//			// Not guaranteed that it executes instantly after all tests from the class.
//			new FolderObject(BaseFolder).Delete(true); // PURGE EVERYTHING
//		}

//		[SetUp]
//		public void TestInit()
//		{
//			// Runs before each test. (Optional)
//		}

//		[TearDown]
//		public void TestCleanup()
//		{
//			// Runs after each test. (Optional)
//			TestFolder.Delete(true); // PURGE EVERYTHING
//		}





//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test()]
//		public void Test_Write_And_Read_Hello_To_And_From_File([Values] FileOption fileOption)
//		{

//			// Arrange
//			var content = $"Hello";
//			var encoding = Encoding.ASCII;
//			var testFile = RandomTestFileNoExtension;
//			// Act
//			// Assert
//			if (fileOption == FileOption.ReadOnly)
//			{
//				// Writing to file is not allow when requesting read-only option
//				Assert.That(async () => { await testFile.WriteAsync(content, fileOption, encoding); }, Throws.Exception);
//			}
//			else
//			{
//				Assert.That(async () =>
//				{
//					var file = new FileObject(await testFile.WriteAsync(content, fileOption, encoding));
//					Assert.That(file.ReadAllText(), Is.EqualTo(content));

//				}, Throws.Nothing);
//			}
//		}




//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_Move_EmptyFile()
//		{
//			var testFile = RandomTestFileNoExtension;

//			var newFile = $"{TestFolder.FullName}MOVE";
//			testFile.CreateOrTruncate();
//			await testFile.MoveToAsync(newFile, FileOption.Overwrite, CancellationToken.None);
//			FileShouldExist(newFile);
//			FileShouldNotExist(testFile.FullName);

//		}


//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_MoveFile_Async_Overwrite()
//		{
//			var testFile = RandomTestFileNoExtension;

//			var newFile = RandomTestFileNoExtension.FullName;
//			testFile.CreateOrTruncate();
//			await testFile.WriteAsync($"this file was original name {testFile.FullName} and should had been moved to the following location {newFile}", FileOption.Overwrite, Encoding.Default);
//			var result = await testFile.MoveToAsync(newFile, FileOption.Overwrite, CancellationToken.None);
//			FileShouldExist(newFile);
//			FileShouldNotExist(testFile.FullName);

//		}




//		//public void Test_GetFileSize([Values(5, 50, 150, 400, 700)] int repeatCounter)
//		//{
//		//	// Arrange
//		//	var content = new StringBuilder($"Hello", repeatCounter).ToString();
//		//	var encoding = Encoding.ASCII;
//		//	// Act
//		//	var expectedFileSize = encoding.GetPreamble();
//		//	// Assert
//		//	if (fileOption == FileOption.ReadOnly)
//		//	{
//		//		// Writing to file is not allow when requesting read-only option
//		//		Assert.That(() => { TestFile.Write(content, fileOption, encoding); }, Throws.Exception);
//		//	}
//		//	else
//		//	{
//		//		Assert.That(() =>
//		//		{
//		//			var file = new FileObject(TestFile.Write(content, fileOption, encoding));
//		//			Assert.That(file.ReadToString(), Is.EqualTo(content));

//		//		}, Throws.Nothing);
//		//	}
//		//}



//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_CopyFile()
//		{
//			var testFile = RandomTestFileNoExtension;

//			var newFile = $"{TestFolder.FullName}COPY";
//			await testFile.WriteAsync($"This file should have been copied to the following location {newFile}", FileOption.Overwrite, Encoding.UTF8);
//			testFile.CopyTo(newFile, FileOption.Overwrite);
//			FileShouldExist(newFile);
//		}



//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_CopyFile_Async_Overwrite()
//		{
//			var testFile = RandomTestFileNoExtension;

//			var newFile = $"{TestFolder.FullName}COPY";
//			await testFile.WriteAsync($"This file should have been copied to the following location {newFile}", FileOption.Overwrite, Encoding.UTF8);
//			await testFile.CopyToAsync(newFile, FileOption.Overwrite, CancellationToken.None);
//			FileShouldExist(newFile);
//			FileShouldExist(newFile);
//		}



//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_Change_Extension_With_File_Having_No_Extension()
//		{
//			var testFile = RandomTestFileNoExtension;

//			testFile.CreateOrTruncate();
//			await testFile.ChangeExtensionAsync(".GOKU", FileOption.Overwrite, CancellationToken.None);
//			FileShouldExist($"{testFile.FullName}.GOKU");
//			FileShouldNotExist(testFile.FullName);
//			Assert.Pass("Successfully change extension of file");
//		}


//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_Change_Extension_With_File_Having_No_Extension_Async()
//		{
//			var testFile = RandomTestFileNoExtension;

//			testFile.CreateOrTruncate();
//			await testFile.ChangeExtensionAsync(".GOKU", FileOption.Overwrite, CancellationToken.None);
//			FileShouldExist($"{testFile.FullName}.GOKU");
//			FileShouldNotExist(testFile.FullName);
//			Assert.Pass("Successfully change extension of file");
//		}



//		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
//		[Test]
//		public async Task Test_Change_Extension_With_File_Having_A_Extension()
//		{

//			var testFile = RandomTestFileNoExtension;


//			var filePath = testFile.FilePathOnly + "AnotherTest.Gohan";
//			using (var file = new FileObject(filePath))
//			{
//				file.CreateOrTruncate();
//				await file.ChangeExtensionAsync(".Vegeta", FileOption.Overwrite);
//				FileShouldExist($"{file.FilePathOnly}{file.FileNameOnlyNoExtension}.Vegeta");
//				FileShouldNotExist(filePath);
//				Assert.Pass("Successfully change extension of file");
//			}
//		}



//		private void FileShouldExist(string file)
//		{
//			var value = File.Exists(file);
//			Assert.IsTrue(value, $"Test failed due to file not existing {file}");
//		}
//		private void FileShouldNotExist(string file)
//		{
//			var value = File.Exists(file);
//			Assert.IsFalse(value, $"Test failed due to file existing {file}");
//		}

//		private bool FileExists(string file)
//		{
//			return File.Exists(file);
//		}




//		public static Stream GenerateStreamFromString(string s)
//		{
//			var stream = new MemoryStream();
//			var writer = new StreamWriter(stream);
//			writer.Write(s);
//			writer.Flush();
//			stream.Position = 0;
//			return stream;
//		}
//	}
//}