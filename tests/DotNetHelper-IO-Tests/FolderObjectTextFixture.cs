
using DotNetHelper_IO;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Bogus;
using DotNetHelper_IO_Tests;

namespace Tests
{
    [TestFixture]
    [NonParallelizable] //since were sharing a single file across multiple test cases we don't want Parallelizable
    public class FolderObjectTextFixture : BaseTest
    {



        public FolderObjectTextFixture()
        {
         
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

        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_CreateOrTruncate_Should_Create_New_Folder([Values(true,false)] bool truncate)
        {
            // Arrange
            var randomFolderName = new Randomizer().String(8, 'A', 'Z');
            var path = Path.Combine(WorkingDirectory, randomFolderName);
            var folder = new FolderObject(path);

            // Act
            folder.CreateOrTruncate(truncate);

            // Assert
            Assert.That(Directory.Exists(path),Is.True);
        }

        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_CreateOrTruncateSubFolder_Should_Create_New_SubFolder([Values(true, false)] bool truncate)
        {
            // Arrange
            var randomFolderName = new Randomizer().String(8,'A','Z');
            var path = Path.Combine(WorkingDirectory, randomFolderName);
            var folder = new FolderObject(path);
            var subFolderPath = Path.Combine(path, $"Sub1{Path.DirectorySeparatorChar}Sub2{Path.DirectorySeparatorChar}");
            // Act

            var subFolder = folder.CreateOrTruncateSubFolder(subFolderPath, truncate);
            

            // Assert
            Assert.That(Directory.Exists(subFolderPath), Is.True);
            Assert.That(subFolderPath, Is.EqualTo(subFolder.FullFolderPath));
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_CreateOrTruncateSubFolder_Should_Create_New_SubFolder_With_RelativePath([Values(true, false)] bool truncate)
        {
            // Arrange
            var randomFolderName = new Randomizer().String(8, 'A', 'Z');
            var path = Path.Combine(WorkingDirectory, randomFolderName);
            var folder = new FolderObject(path);
            var subFolderPath = $"./Ham/Chesse/";
            // Act

            var subFolder = folder.CreateOrTruncateSubFolder(subFolderPath, truncate);


            // Assert
            Assert.That(Directory.Exists(subFolder.FullFolderPath), Is.True);
            Assert.That(subFolderPath, Is.EqualTo(subFolder.FullFolderPath));
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_Delete_Should_Delete_Folder()
        {
            // Arrange
            var randomFolderName = new Randomizer().String(8, 'A', 'Z');
            var path = Path.Combine(WorkingDirectory, randomFolderName);
            var folder = new FolderObject(path);

            // Act
            Directory.CreateDirectory(path);
            folder.Delete(true);

            // Assert
            Assert.That(Directory.Exists(path), Is.False);
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_Delete_Should_Delete_Folder_And_Its_Subfolder()
        {
            // Arrange
            var randomFolderName = new Randomizer().String(8, 'A', 'Z');
            var path = Path.Combine(WorkingDirectory, randomFolderName);
            var folder = new FolderObject(path);
            folder.CreateOrTruncateSubFolder($"Sub1{Path.DirectorySeparatorChar}Sub2",true);

            // Act
            Directory.CreateDirectory(path);
            new FolderObject(path).Delete(true);

            // Assert
            Assert.That(Directory.Exists(path), Is.False);
        }






        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_AbsoluteCreateFolderSoRelativeShouldNot()
        //{

        //    var subFolderName = "ABSOLUTE";
        //    var absoluteFolder = new FolderObject(BaseFolder.FullFolderPath + subFolderName);

        //    absoluteFolder.CreateOrTruncate(true);
        //    absoluteFolder.RefreshObject(absoluteFolder.LoadSubFolders, absoluteFolder.LoadFilesInFolder, absoluteFolder.LoadRecursive);
        //    var relativeFolder = new FolderObject(BaseFolderRelative.FullFolderPath + subFolderName);


        //    Assert.IsTrue(relativeFolder.Exist, "Syncing between absolute & relative paths is off");

        //    Assert.IsTrue(relativeFolder.ParentFolder == absoluteFolder.ParentFolder
        //                  && relativeFolder.Files.Count == absoluteFolder.Files.Count
        //                  && relativeFolder.DirectoryInfo.CreationTime == absoluteFolder.DirectoryInfo.Creation​Time​
        //                  && relativeFolder.DirectoryInfo.Last​Access​Time​Utc == absoluteFolder.DirectoryInfo.Last​Access​Time​Utc
        //                  );
        //}



        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_RelativeCreateFolderSoAbsoluteShouldNot()
        //{

        //    var subFolderName = "RELATIVE";
        //    var relativeFolder = new FolderObject(BaseFolder.FullFolderPath + subFolderName);

        //    relativeFolder.CreateOrTruncate(false);
        //    relativeFolder.RefreshObject(relativeFolder.LoadSubFolders, relativeFolder.LoadFilesInFolder, relativeFolder.LoadRecursive);
        //    var absoluteFolder = new FolderObject(BaseFolderRelative.FullFolderPath + subFolderName);


        //    Assert.IsTrue(relativeFolder.Exist, "Syncing between absolute & relative paths is off");

        //    Assert.IsTrue(relativeFolder.ParentFolder == absoluteFolder.ParentFolder
        //                  && relativeFolder.Files.Count == absoluteFolder.Files.Count
        //                  && relativeFolder.DirectoryInfo.Creation​Time​ == absoluteFolder.DirectoryInfo.Creation​Time​
        //                  && relativeFolder.DirectoryInfo.Last​Access​Time​Utc == absoluteFolder.DirectoryInfo.Last​Access​Time​Utc
        //    );
        //}



        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_RelativeAndAbsoluteMatch()
        //{

        //    RelativeTestFolder.CreateOrTruncate(true);
        //    RelativeTestFile.CreateOrTruncate();
        //    Assert.IsTrue(BaseFolderRelative.ParentFolder == BaseFolder.ParentFolder
        //                  && BaseFolderRelative.Files.Count == BaseFolder.Files.Count
        //                  && BaseFolderRelative.DirectoryInfo.Creation​Time​ == BaseFolder.DirectoryInfo.Creation​Time​
        //                  && BaseFolderRelative.DirectoryInfo.Last​Access​Time​Utc == BaseFolder.DirectoryInfo.Last​Access​Time​Utc
        //    );
        //}


        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_DisposeShouldNotThrowError()
        //{
        //    var newFile = $"{BaseFolder.FullFolderPath}DisposeTest";
        //    using (var file = new FileObject(newFile))
        //    {

        //    }
        //    Assert.Pass("Successfully dispose new instance without error");
        //}




        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_RelativeMoveFolder()
        //{

        //    var newFolder = $"{RelativeTestFolder.FullFolderPath}MOVE";
        //    TestFile.CreateOrTruncate();
        //    TestFile.WriteContentToFile($"this file was original name {TestFile.FullFilePath} and should had been moved to the following location {newFolder}");
        //    TestFile.MoveTo(newFolder, FileOption.Overwrite);
        //    FolderShouldExist(newFolder);
        //    FolderShouldNotExist(newFolder.FullFilePath);

        //}

        //[Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        //[Test]
        //public void Test_RelativeCopyFolder()
        //{

        //    var newFile = $"{RelativeTestFolder.FullFolderPath}COPY";
        //    TestFile.WriteContentToFile($"This file should have been copied to the following location {newFile}");
        //    TestFile.CopyTo(newFile, FileOption.Overwrite);
        //    FolderShouldExist(newFile);
        //}


        private void FolderShouldExist(string folder)
        {
            var value = Directory.Exists(folder);
            Assert.IsTrue(value, $"Test failed due to folder not existing {folder}");
        }
        private void FolderShouldNotExist(string folder)
        {
            var value = Directory.Exists(folder);
            Assert.IsFalse(value, $"Test failed due to folder existing {folder}");
        }



    }
}