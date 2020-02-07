using DotNetHelper_IO;
using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO_Tests;

namespace Tests
{
    [TestFixture]
    [NonParallelizable] //since were sharing a single file across multiple test cases we don't want Parallelizable
    public class FileObjectTextFixture : BaseTest
    {
        public FolderObject TestFolder { get; }
        public FileObject TestFile { get; }


        public FileObjectTextFixture()
        {
            TestFolder = new FolderObject(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "UnitTestTempDir");
            TestFile = new FileObject(TestFolder.FullFolderPath + "UnitTestFile");
        }


        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            TestFolder.DeleteFolder(e => throw e, true); // PURGE EVERYTHING
            if (File.Exists(TestFile.FullFilePath))
                Assert.IsFalse(TestFile.Exist, "Unit Test setup failed due to environment not being clean");
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            TestFolder.DeleteFolder(e => throw e, true); // PURGE EVERYTHING
        }



        [SetUp]
        public void Init()
        {

        }

        [TearDown]
        public void Cleanup()
        {
            TestFolder.DeleteFolder(e => throw e, true); // PURGE EVERYTHING
        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_IncrementFileExtension()
        {
            TestFile.CreateOrTruncate();
            for (var i = 1; i < 110; i++)
            {
                TestFile.WriteContentToFile($"{i}", Encoding.UTF8, FileOption.IncrementFileExtensionIfExist);
                FileShouldExist($"{TestFile.FilePathOnly}{TestFile.FileNameOnlyNoExtension}.{i}");
            }
        }

        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_IncrementFileName()
        {
            TestFile.CreateOrTruncate();
            for (var i = 1; i < 110; i++)
            {
                TestFile.WriteContentToFile($"{i}", Encoding.UTF8, FileOption.IncrementFileNameIfExist);
                FileShouldExist($"{TestFile.FilePathOnly}{TestFile.FileNameOnlyNoExtension}{i}");
            }
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_WritingToAndReadingFile()
        {
            var content = $"A {Environment.NewLine} B!";
            var newFile = new FileObject($"{TestFolder.FullFolderPath}TestWritingAndReading");
            newFile.WriteContentToFile(content, Encoding.UTF8, FileOption.Overwrite);
            var readValue = newFile.ReadFile();
            Assert.IsTrue(readValue == content, "The content that was written to the file didn't match what was read.");
        }

        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_MoveFile()
        {

            var newFile = $"{TestFolder.FullFolderPath}MOVE";
            TestFile.CreateOrTruncate();
            TestFile.WriteContentToFile($"this file was original name {TestFile.FullFilePath} and should had been moved to the following location {newFile}", Encoding.UTF8);
            TestFile.MoveTo(newFile, FileOption.Overwrite);
            FileShouldExist(newFile);
            FileShouldNotExist(TestFile.FullFilePath);

        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public async Task Test_MoveFile_Async_Overwrite()
        {

            var newFile = $"{TestFolder.FullFolderPath}MOVE";
            TestFile.CreateOrTruncate();
            TestFile.WriteContentToFile($"this file was original name {TestFile.FullFilePath} and should had been moved to the following location {newFile}", Encoding.UTF8);
            var result = await TestFile.MoveToAsync(newFile, FileOption.Overwrite,CancellationToken.None);
            FileShouldExist(newFile);
            FileShouldNotExist(TestFile.FullFilePath);

        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void  Test_CopyFile()
        {

            var newFile = $"{TestFolder.FullFolderPath}COPY";
            TestFile.WriteContentToFile($"This file should have been copied to the following location {newFile}", Encoding.UTF8);
            TestFile.CopyTo(newFile, FileOption.Overwrite);
            FileShouldExist(newFile);
        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public async Task Test_CopyFile_Async_Overwrite()
        {

            var newFile = $"{TestFolder.FullFolderPath}COPY";
            TestFile.WriteContentToFile($"This file should have been copied to the following location {newFile}", Encoding.UTF8); 
            await TestFile.CopyToAsync(newFile, FileOption.Overwrite, CancellationToken.None);
            FileShouldExist(newFile);
            FileShouldExist(newFile);
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_DisposeShouldNotThrowError()
        {
            var newFile = $"{TestFolder.FullFolderPath}DisposeTest";
            using (var file = new FileObject(newFile))
            {

            }
            Assert.Pass("Successfully dispose new instance without error");
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_Change_Extension_With_File_Having_No_Extension()
        {
            TestFile.CreateOrTruncate();
            TestFile.ChangeExtension(".GOKU", FileOption.Overwrite);
            FileShouldExist($"{TestFile.FullFilePath}.GOKU");
            FileShouldNotExist(TestFile.FullFilePath);
            Assert.Pass("Successfully change extension of file");
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_Change_Extension_With_File_Having_A_Extension()
        {
          
            var filePath = TestFile.FilePathOnly + "AnotherTest.Gohan";
            using (var file = new FileObject(filePath))
            {
                file.CreateOrTruncate();
                file.ChangeExtension(".Vegeta", FileOption.Overwrite);
                FileShouldExist($"{file.FilePathOnly}{file.FileNameOnlyNoExtension}.Vegeta");
                FileShouldNotExist(filePath);
                Assert.Pass("Successfully change extension of file");
            }
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


    }
}