using System.IO;
using System.Linq;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests.IO.FolderObjectTests.Move
{


	[NonParallelizable]
	public class MoveOperationTextFixture : BaseTest
	{


		public FolderObject TestFolder { get; }
		public FolderObject NewTestSubFolder => new FolderObject(Path.Combine(TestFolder.FullName, RandomAlphaString));

		public MoveOperationTextFixture()
		{
			TestFolder = new FolderObject(WorkingDirectory +
										  Path.DirectorySeparatorChar
										  + nameof(MoveOperationTextFixture));

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
		public void Test_MoveTo_WhenDestinationDoesntExist_IncludeEntireFolderNotJustContentInFolder([Values] FolderOption folderOption)
		{
			var originalFolder = NewTestSubFolder;
			var destinationFolder = NewTestSubFolder;

			originalFolder.AddFile("file1.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file2.txt", string.Empty, FileOption.Overwrite);
			originalFolder.AddFile("file3.txt", string.Empty, FileOption.Overwrite);

			originalFolder.MoveTo(destinationFolder.FullName, folderOption);


			destinationFolder.RefreshObject();
			originalFolder.RefreshObject();

			var ogSubfolders = originalFolder.GetDirectories("*", SearchOption.AllDirectories);
			var destinationSubFolders = destinationFolder.GetDirectories("*", SearchOption.AllDirectories);

			var ogFiles = originalFolder.GetFiles("*", SearchOption.AllDirectories);
			var destinationFiles = destinationFolder.GetFiles("*", SearchOption.AllDirectories);


			Assert.IsEmpty(ogFiles);
			Assert.IsEmpty(ogSubfolders);

			Assert.AreEqual(3, destinationFiles.Count());
			Assert.AreEqual(1, destinationSubFolders.Count());

			Assert.IsFalse(originalFolder.Exist);
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test]
		public void Test_MoveContent_WhenDestinationDoesntExist_DoesntIncludeFolderJustItsContent([Values] FolderOption folderOption)
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