using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotNetHelper.IO.Tests.Extensions;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests.IO.FileObject.Write
{
	[NonParallelizable]
	public class WriteAsyncTestFixture : BaseTest
	{

		public FolderObject TestFolder { get; }

		public WriteAsyncTestFixture()
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
		public async Task Test_Write_To_File_Append()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.Append);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

			await testFile.WriteAsync("B".ToByteArray(), FileOption.Append);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("AB"));

			await testFile.WriteAsync("C".ToStream(), FileOption.Append);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("ABC"));
		}

		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Write_To_File_Overwrite()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.Overwrite);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

			await testFile.WriteAsync("B".ToByteArray(), FileOption.Overwrite);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("B"));

			await testFile.WriteAsync("C".ToStream(), FileOption.Overwrite);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("C"));
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Write_To_File_DoNothingIfFileExist()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.DoNothingIfExist);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

			await testFile.WriteAsync("B".ToByteArray(), FileOption.DoNothingIfExist);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

			await testFile.WriteAsync("C".ToStream(), FileOption.DoNothingIfExist);
			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));
		}

		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Write_To_File_IncrementFileExtensionIfExist()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.IncrementFileExtensionIfExist);
			var newFileWithNameIncruemented = await testFile.WriteAsync("B".ToByteArray(), FileOption.IncrementFileExtensionIfExist);
			var newFileWithNameIncruemented2 = await testFile.WriteAsync("C".ToStream(), FileOption.IncrementFileExtensionIfExist);

			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));
			Assert.That((await File.ReadAllTextAsync(newFileWithNameIncruemented)).Equals("B"));
			Assert.That((await File.ReadAllTextAsync(newFileWithNameIncruemented2)).Equals("C"));

			Assert.AreNotSame(newFileWithNameIncruemented2, newFileWithNameIncruemented);
			Assert.AreNotSame(testFile, newFileWithNameIncruemented);
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Write_To_File_IncrementFileNameIfExist()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.IncrementFileNameIfExist);
			var newFileWithNameIncruemented = await testFile.WriteAsync("B".ToByteArray(), FileOption.IncrementFileNameIfExist);
			var newFileWithNameIncruemented2 = await testFile.WriteAsync("C".ToStream(), FileOption.IncrementFileNameIfExist);

			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));
			Assert.That((await File.ReadAllTextAsync(newFileWithNameIncruemented)).Equals("B"));
			Assert.That((await File.ReadAllTextAsync(newFileWithNameIncruemented2)).Equals("C"));

			Assert.AreNotSame(newFileWithNameIncruemented2, newFileWithNameIncruemented);
			Assert.AreNotSame(testFile, newFileWithNameIncruemented);
		}




		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_Write_To_Existing_File_With_DoNotIfExist_Set()
		{
			var testFile = RandomTestFileNoExtension;

			await testFile.WriteAsync("A", FileOption.Overwrite);

			await testFile.WriteAsync("A", FileOption.DoNothingIfExist);

			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public async Task Test_GetFileStream_With_DoNotIfExist_When_File_Exist_Isnt_Writable()
		{
			var testFile = RandomTestFileNoExtension;
			await testFile.WriteAsync("A", FileOption.Overwrite);
			using (var fileStream = testFile.GetFileStream(FileOption.DoNothingIfExist, 4096, false).fileStream)
			{
				var bytes = Encoding.UTF8.GetBytes("B");
				Assert.ThrowsAsync<NotSupportedException>(() => fileStream.WriteAsync(bytes, 0, bytes.Length));
			}

			Assert.That((await File.ReadAllTextAsync(testFile.FullName)).Equals("A"));

		}














	}
}