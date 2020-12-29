using System.IO;
using System.Linq;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests.IO.FolderObjectTests.Copy
{


	[NonParallelizable]
	public class CopyOperationTextFixture : BaseTest
	{


		public FolderObject TestFolder { get; }
		public FolderObject NewTestSubFolder => new FolderObject(Path.Combine(TestFolder.FullName, RandomAlphaString));

		public CopyOperationTextFixture()
		{
			TestFolder = new FolderObject(WorkingDirectory +
										  Path.DirectorySeparatorChar
										  + nameof(CopyOperationTextFixture));

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
		public void Test_CopyTo_WhenDestinationDoesntExist_IncludeEntireFolderNotJustContentInFolder([Values] FolderOption folderOption)
		{
			var originalFolder = NewTestSubFolder;
			var destinationFolder = NewTestSubFolder;

			originalFolder.AddFile("file1.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file2.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file3.txt", string.Empty, FileOption.Overwrite);

			originalFolder.CopyTo(destinationFolder.FullName, folderOption);


			destinationFolder.RefreshObject();
			originalFolder.RefreshObject();

			var subFolders = destinationFolder.DirectoryInfo.GetDirectories("*", SearchOption.AllDirectories);

			Assert.IsNotEmpty(subFolders);
			Assert.IsNotNull(subFolders.FirstOrDefault(f => f.Name.Equals(originalFolder.Name)));
			Assert.AreEqual(3, destinationFolder.DirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Count());


			Assert.IsEmpty(originalFolder.DirectoryInfo.GetDirectories("*", SearchOption.AllDirectories));
			Assert.AreEqual(3, originalFolder.DirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Count());

		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_CopyContent_WhenDestinationDoesntExist_DoesntIncludeFolderJustItsContent([Values] FolderOption folderOption)
		{
			var originalFolder = NewTestSubFolder;
			var destinationFolder = NewTestSubFolder;

			originalFolder.AddFile("file1.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file2.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file3.txt", string.Empty, FileOption.Overwrite);

			originalFolder.CopyContentsTo(destinationFolder.FullName, folderOption);


			destinationFolder.RefreshObject();
			originalFolder.RefreshObject();

			var destinationFoldersRecursive = destinationFolder.DirectoryInfo.GetDirectories("*", SearchOption.AllDirectories);
			var destinationFilesRecursive = destinationFolder.DirectoryInfo.GetFiles("*", SearchOption.AllDirectories);

			var ogFoldersRecursive = destinationFolder.DirectoryInfo.GetDirectories("*", SearchOption.AllDirectories);
			var ogFilesRecursive = destinationFolder.DirectoryInfo.GetFiles("*", SearchOption.AllDirectories);


			Assert.IsEmpty(destinationFoldersRecursive);
			Assert.IsNull(destinationFoldersRecursive.FirstOrDefault(f => f.Name.Equals(originalFolder.Name)));
			Assert.AreEqual(3, destinationFilesRecursive.Count());

			Assert.IsEmpty(ogFoldersRecursive);
			Assert.IsNull(ogFoldersRecursive.FirstOrDefault(f => f.Name.Equals(originalFolder.Name)));
			Assert.AreEqual(3, ogFilesRecursive.Count());


			// Assert.AreEqual(3, destinationFolder.Files.Count);
		}


	}
}