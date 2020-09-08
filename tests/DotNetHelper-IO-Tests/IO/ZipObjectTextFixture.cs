using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DotNetHelper.IO.Tests.Extensions;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Helpers;
using DotNetHelper_IO_Tests;
using NUnit.Framework;
using SharpCompress.Common;


namespace Tests
{
	[TestFixture]
	//[SingleThreadedAttribute]
	[NonParallelizable]
	public class ZipObjectTextFixture : BaseTest
	{
		public FolderObject TestFolder { get; }
		// public FileObject TestFile { get; }

		private readonly char[] Chars =
			Enumerable
				.Range(char.MinValue, char.MaxValue)
				.Select(x => (char)x)
				.Where(c => !char.IsControl(c))
				.ToArray();


		public ZipObjectTextFixture()
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
		[Test()]
		public void Test_Zip_Folder_Contains_All_Files_And_Match_Content([Values(ArchiveType.Zip, ArchiveType.GZip)] ArchiveType archiveType)
		{
			// Arrange
			var testFiles = new List<FileObject>()
			{
				RandomTestFileNoExtension, RandomTestFileNoExtension, RandomTestFileNoExtension
			};

			testFiles.ForEach(f => f.Write("A"));

			var folderWithTestFile = new FolderObject(testFiles.First().FilePathOnly);
			var zipFileName = folderWithTestFile.FullName + $"{folderWithTestFile.Name}.zip";
			var zipFile = folderWithTestFile.ZipFolderToFileSystem(zipFileName, ArchiveType.Zip, null, "*", SearchOption.AllDirectories);

			using (var archive = zipFile.GetReadableArchive())
			{
				foreach (var entry in archive.Entries)
				{
					Assert.Contains(entry.Key, testFiles.Select(f => f.Name).ToList());

					var stream = entry.OpenEntryStream();
					var content = stream.ReadToString();
					Assert.That(content.Equals(testFiles.First(f => f.FileNameOnly == entry.Key).ReadAllText()));

				}
			}

			Assert.That(zipFile.GetEntriesCount(), Is.EqualTo(testFiles.Count));

		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test()]
		public void Test_GetFileCount_Return_3_When_Adding_3_Files([Values(ArchiveType.Zip, ArchiveType.GZip)] ArchiveType archiveType)
		{
			// Arrange
			var testFiles = new List<FileObject>()
			{
				RandomTestFileNoExtension, RandomTestFileNoExtension, RandomTestFileNoExtension
			};
			if (archiveType == ArchiveType.GZip)
			{
				testFiles.RemoveAt(2);
				testFiles.RemoveAt(1);
			}
			var zipFile = new ZipFileObject(Path.Combine(TestFolder.FullName, "Test" + CompressExtensionHelper.ZipExtensionLookup[archiveType]), archiveType);

			// Act
			foreach (var file in testFiles)
			{
				file.Write(file.Name); // write file name to file
				zipFile.Add(file);
			}

			// Assert
			Assert.That(zipFile.GetEntriesCount(), Is.EqualTo(testFiles.Count));
			//	Assert.That(zipFile.GetArchiveEntries().Count(), Is.EqualTo(3));
		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test()]
		public void Test_GetArchiveEntries_Return_Correct_Content([Values(ArchiveType.Zip, ArchiveType.GZip, ArchiveType.Tar)] ArchiveType archiveType)
		{
			if (archiveType == ArchiveType.Rar || archiveType == ArchiveType.SevenZip)
				return;
			// Arrange
			var testFiles = new List<FileObject>()
			{
				RandomTestFileNoExtension, RandomTestFileNoExtension, RandomTestFileNoExtension
			};
			if (archiveType == ArchiveType.GZip)
			{
				testFiles.RemoveAt(2);
				testFiles.RemoveAt(1);
			}
			var zipFile = new ZipFileObject(Path.Combine(TestFolder.FullName, $"Test{CompressExtensionHelper.ZipExtensionLookup[archiveType]}"), archiveType);

			// Act
			foreach (var file in testFiles)
			{
				file.Write(file.Name); // write file name to file
				zipFile.Add(file);
			}


			using (var filesInZip = zipFile.GetReadableArchive())
			{
				foreach (var file in filesInZip.Entries)
				{
					using (var archiveFileStream = file.OpenEntryStream())
					{
						var content = archiveFileStream.ReadToString();
						Assert.That(content, Is.EqualTo(file.Key));
					}
				}
			}

			// Assert


		}



		private void FileShouldExist(string file)
		{
			var value = File.Exists(file);
			Assert.IsTrue(value, $"Test failed due to file not existing {file}");
		}
		private void FileShouldNotExist(string file)
		{
			var value = File.Exists(file);
			Assert.IsFalse(value, $"Test failed due to file existing {file}");
		}

		private bool FileExists(string file)
		{
			return File.Exists(file);
		}




	}
}