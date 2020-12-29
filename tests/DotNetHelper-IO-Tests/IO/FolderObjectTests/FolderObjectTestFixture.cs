using System.IO;
using System.Linq;
using System.Text;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using NUnit.Framework;

namespace DotNetHelper.IO.Tests.IO.FolderObjectTests
{
	[TestFixture]
	//[SingleThreadedAttribute]
	[NonParallelizable]
	public class FolderObjectTestFixture : BaseTest
	{
		public FolderObject TestFolder { get; }
		public FolderObject NewTestSubFolder => new FolderObject(Path.Combine(TestFolder.FullName, RandomAlphaString));
		// public FileObject TestFile { get; }

		private readonly char[] Chars =
			Enumerable
				.Range(char.MinValue, char.MaxValue)
				.Select(x => (char)x)
				.Where(c => !char.IsControl(c))
				.ToArray();


		public FolderObjectTestFixture()
		{
			TestFolder = new FolderObject(WorkingDirectory +
										  Path.DirectorySeparatorChar
										  + nameof(FolderObjectTestFixture));

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
		public void Test_AddFile_CreatesFile()
		{
			// Arrange
			var fileContent = Encoding.ASCII.GetBytes("Something");
			var originalFolder = NewTestSubFolder;

			// add file to root folder 
			originalFolder.AddFile("file1.txt", fileContent, FileOption.Overwrite);

			// Create subfolder and a 3 files to it 
			var subFolder1 = originalFolder.AddFolder("3Files");
			subFolder1.AddFile("file1.txt", fileContent, FileOption.Overwrite);
			subFolder1.AddFile("file2.txt", fileContent, FileOption.Overwrite);
			subFolder1.AddFile("file3.txt", fileContent, FileOption.Overwrite);
			subFolder1.AddFolder("ASubFolderInSubFolder");

			Assert.AreEqual(4, originalFolder.GetFiles("*", SearchOption.AllDirectories).Count());
			Assert.AreEqual(1, originalFolder.GetFiles("*", SearchOption.TopDirectoryOnly).Count());
			Assert.AreEqual(1, originalFolder.GetDirectories("*", SearchOption.TopDirectoryOnly).Count());
			Assert.AreEqual(2, originalFolder.GetDirectories("*", SearchOption.AllDirectories).Count());

		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test()]
		public void Test_Create_DoesCreateFolder([Values] FolderOption folderOption)
		{
			// Arrange
			
			var originalFolder = NewTestSubFolder;

			originalFolder.Create(folderOption);

			Assert.True(originalFolder.Exist);
		
		}




	}
}