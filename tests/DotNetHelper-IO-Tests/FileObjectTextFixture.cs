using DotNetHelper_IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly char[] Chars =
            Enumerable
                .Range(char.MinValue, char.MaxValue)
                .Select(x => (char)x)
                .Where(c => !char.IsControl(c))
                .ToArray();


        public FileObjectTextFixture()
        {
            TestFolder = new FolderObject(WorkingDirectory);
            TestFile = new FileObject(TestFolder.FullFolderPath + "UnitTestFile");
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
        public void Test_GetFileSize_Return_Correct_Size()
        {
            // Arrange
            var fileSize = 100000000;
            var expectedSizeInByte = $"{fileSize}B";
            var expectedSizeInKiloByte = $"100000";
            var expectedSizeInMB = $"95.37MB";

            var content = new string('A', fileSize);
            var stream = GenerateStreamFromString(content);

            // Act
            TestFile.Write(stream);
            var fileSizeInBytes = TestFile.GetFileSize(SizeUnits.Byte);
            var fileSizeInKiloBytes = TestFile.GetFileSize(SizeUnits.Kb);
            var fileSizeInMegaBytes = TestFile.GetFileSize(SizeUnits.Mb);

            var fileSizeAsString = TestFile.GetFileSize();

            // Assert
            Assert.That(fileSizeInBytes, Is.EqualTo(fileSize));
            Assert.That(fileSizeInKiloBytes, Is.EqualTo(97656));
            Assert.That(fileSizeInMegaBytes, Is.EqualTo(95));
            Assert.That(fileSizeAsString, Is.EqualTo(expectedSizeInMB));
        }






        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test()]
        //[Category("PerformanceTest")]
        //public void Test_Proof_Test2_B()
        //{
        //    var stream = GenerateStreamFromString(new string('A', 100000000));
        //    var progress = new Progress<long>();
        //    progress.ProgressChanged += delegate (object sender, long l)
        //    {
        //        Console.WriteLine($"Progress {l}%");
        //    };
        //    var file = TestFile.Write(stream, progress, FileOption.Overwrite);
        //}





        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test()]
        public void Test_Write_And_Read_Hello_To_And_From_File([Values]FileOption fileOption)
        {

            // Arrange
            var content = $"Hello";
            var encoding = Encoding.ASCII;
            // Act
            // Assert
            if (fileOption == FileOption.ReadOnly)
            {
                // Writing to file is not allow when requesting read-only option
                Assert.That(() => { TestFile.Write(content, fileOption, encoding); }, Throws.Exception);
            }
            else
            {
                Assert.That(() =>
                {
                    var file = new FileObject(TestFile.Write(content, fileOption, encoding));
                    Assert.That(file.ReadToString(), Is.EqualTo(content));

                }, Throws.Nothing);
            }
        }





        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_MoveFile()
        {
            // Arrange

            var moveToFile = $"{TestFolder.FullFolderPath}MOVE";
            TestFile.CreateOrTruncate();
            TestFile.Write($"this file was original name {TestFile.FullFilePath} and should had been moved to the following location {moveToFile}", FileOption.Overwrite, Encoding.UTF8);
            TestFile.MoveTo(moveToFile, FileOption.Overwrite);
            FileShouldExist(moveToFile);
            FileShouldNotExist(TestFile.FullFilePath);
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_Move_EmptyFile()
        {

            var newFile = $"{TestFolder.FullFolderPath}MOVE";
            TestFile.CreateOrTruncate();
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
            TestFile.Write($"this file was original name {TestFile.FullFilePath} and should had been moved to the following location {newFile}", FileOption.Overwrite, Encoding.UTF8);
            var result = await TestFile.MoveToAsync(newFile, FileOption.Overwrite, CancellationToken.None);
            FileShouldExist(newFile);
            FileShouldNotExist(TestFile.FullFilePath);

        }




        //public void Test_GetFileSize([Values(5,50,150,400,700)] int repeatCounter)
        //{
        //    // Arrange
        //    var content = new StringBuilder($"Hello",repeatCounter).ToString();
        //    var encoding = Encoding.ASCII;
        //    // Act
        //    var expectedFileSize =  encoding.GetPreamble(encoding.get);
        //    // Assert
        //    if (fileOption == FileOption.ReadOnly)
        //    {
        //        // Writing to file is not allow when requesting read-only option
        //        Assert.That(() => { TestFile.Write(content, fileOption, encoding); }, Throws.Exception);
        //    }
        //    else
        //    {
        //        Assert.That(() =>
        //        {
        //            var file = new FileObject(TestFile.Write(content, fileOption, encoding));
        //            Assert.That(file.ReadToString(), Is.EqualTo(content));

        //        }, Throws.Nothing);
        //    }
        //}



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_CopyFile()
        {

            var newFile = $"{TestFolder.FullFolderPath}COPY";
            TestFile.Write($"This file should have been copied to the following location {newFile}", FileOption.Overwrite, Encoding.UTF8);
            TestFile.CopyTo(newFile, FileOption.Overwrite);
            FileShouldExist(newFile);
        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public async Task Test_CopyFile_Async_Overwrite()
        {

            var newFile = $"{TestFolder.FullFolderPath}COPY";
            TestFile.Write($"This file should have been copied to the following location {newFile}", FileOption.Overwrite, Encoding.UTF8);
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

        private bool FileExists(string file)
        {
            return File.Exists(file);
        }




        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}