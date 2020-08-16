using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO_Tests;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	[NonParallelizable] //since were sharing a single file across multiple test cases we don't want Parallelizable
	public class IncrumentTest : BaseTest
	{
		public FolderObject TestFolder { get; }
		public FileObject TestFile { get; }




		public IncrumentTest()
		{
			TestFolder = new FolderObject(WorkingDirectory);
			TestFile = new FileObject(TestFolder.FullName + "UnitTestFile");
		}



		[OneTimeSetUp]
		public void ClassInit()
		{
			// Executes once for the test class. (Optional)
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
		}
		[OneTimeTearDown]
		public void ClassCleanup()
		{
			// Runs once after all tests in this class are executed. (Optional)
			// Not guaranteed that it executes instantly after all tests from the class.
		}





		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			TestFolder.Delete(false); // PURGE EVERYTHING
			if (File.Exists(TestFile.FullName))
				Assert.IsFalse(TestFile.Exist, "Unit Test setup failed due to environment not being clean");
		}

		[OneTimeTearDown]
		public void RunAfterAnyTests()
		{

		}



		[SetUp]
		public void Init()
		{

		}

		[TearDown]
		public void Cleanup()
		{

		}



		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test()]

		public void Write_Create_ANewFile_When_Using_IncrementFileExtension_Option([Range(1, 101)] int i)
		{
			// Arrange
			var testFile = new FileObject($"{TestFile.FilePathOnly}incrementTest");
			testFile.CreateOrTruncate(false);

			var newFileName = $"{testFile.FilePathOnly}{testFile.FileNameOnlyNoExtension}.{i}";

			// Act
			testFile.Write($"File #{i}", FileOption.IncrementFileExtensionIfExist, Encoding.ASCII);

			// Assert
			Assert.That(FileExists(newFileName), Is.True);
		}


		[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
		[Test()]
		public void Write_Create_ANewFile_When_Using_IncrementFileNameIfExist_Option([Range(1, 101)] int i)
		{

			var testFile = new FileObject($"{TestFile.FilePathOnly}incrementTest.txt");
			testFile.CreateOrTruncate(false);

			var newFileName = $"{testFile.FilePathOnly}{testFile.FileNameOnlyNoExtension}{i}.txt";

			//TestFile.CreateOrTruncate();
			//         var newFileName = $"{TestFile.FilePathOnly}{TestFile.FileNameOnlyNoExtension}{i}";

			// Act
			testFile.Write($"File #{i}", FileOption.IncrementFileNameIfExist, Encoding.ASCII);

			// Assert
			Assert.That(FileExists(newFileName), Is.True);
		}



		private bool FileExists(string file)
		{
			return File.Exists(file);
		}


	}
}