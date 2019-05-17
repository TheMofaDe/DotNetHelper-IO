
using DotNetHelper_IO;
using NUnit.Framework;
using System;
using System.IO;
using DotNetHelper_IO_Tests;

namespace Tests
{
    [TestFixture]
    [NonParallelizable] //since were sharing a single file across multiple test cases we don't want Parallelizable
    public class FolderObjectTextFixture : BaseTest
    {
        public FolderObject AbsoluteTestFolder { get; }
        public  FileObject AbsoluteTestFile { get;  }
        public FolderObject RelativeTestFolder { get; }
        public FileObject RelativeTestFile { get; }


        public FolderObjectTextFixture()
        {
            AbsoluteTestFolder = new FolderObject(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "UnitTestTempDir",true,true,true);
            RelativeTestFolder = new FolderObject("./UnitTestTempDir", true, true, true);

            AbsoluteTestFile = new FileObject(AbsoluteTestFolder.FullFolderPath + "UnitTestFile");
            RelativeTestFile = new FileObject(RelativeTestFolder.FullFolderPath + "UnitTestFile");
        }


        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            AbsoluteTestFolder.DeleteFolder(e => throw e, true); // PURGE EVERYTHING
            if(File.Exists(AbsoluteTestFile.FullFilePath))
                Assert.IsFalse(AbsoluteTestFile.Exist, "Unit Test setup failed due to environment not being clean");
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            AbsoluteTestFolder.DeleteFolder(e => throw e, true); // PURGE EVERYTHING
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
        [Test]
        public void Test_AbsoluteCreateFolderSoRelativeShouldNot()
        {

            var subFolderName = "ABSOLUTE";
            var absoluteFolder =  new FolderObject(AbsoluteTestFolder.FullFolderPath + subFolderName);
   
                absoluteFolder.Create(delegate(Exception exception)
            {
                Assert.Fail($"{exception.Message}");
            });
                absoluteFolder.RefreshObject(absoluteFolder.LoadSubFolders,absoluteFolder.LoadFilesInFolder,absoluteFolder.LoadRecursive);
            var relativeFolder = new FolderObject(RelativeTestFolder.FullFolderPath + subFolderName);
         

            Assert.IsTrue(relativeFolder.Exist,"Syncing between absolute & relative paths is off");

            Assert.IsTrue(relativeFolder.ParentFolder == absoluteFolder.ParentFolder
                          && relativeFolder.Files.Count == absoluteFolder.Files.Count
                          && relativeFolder.Creation​Time​ == absoluteFolder.Creation​Time​
                          && relativeFolder.Last​Access​Time​Utc == absoluteFolder.Last​Access​Time​Utc
                          );
        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_RelativeCreateFolderSoAbsoluteShouldNot()
        {

            var subFolderName = "RELATIVE";
            var relativeFolder = new FolderObject(AbsoluteTestFolder.FullFolderPath + subFolderName);

            relativeFolder.Create(delegate (Exception exception)
            {
                Assert.Fail($"{exception.Message}");
            });
            relativeFolder.RefreshObject(relativeFolder.LoadSubFolders, relativeFolder.LoadFilesInFolder, relativeFolder.LoadRecursive);
            var absoluteFolder = new FolderObject(RelativeTestFolder.FullFolderPath + subFolderName);


            Assert.IsTrue(relativeFolder.Exist, "Syncing between absolute & relative paths is off");

            Assert.IsTrue(relativeFolder.ParentFolder == absoluteFolder.ParentFolder
                          && relativeFolder.Files.Count == absoluteFolder.Files.Count
                          && relativeFolder.Creation​Time​ == absoluteFolder.Creation​Time​
                          && relativeFolder.Last​Access​Time​Utc == absoluteFolder.Last​Access​Time​Utc
            );
        }



        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_RelativeAndAbsoluteMatch()
        {



            Assert.IsTrue(RelativeTestFolder.ParentFolder == AbsoluteTestFolder.ParentFolder
                          && RelativeTestFolder.Files.Count == AbsoluteTestFolder.Files.Count
                          && RelativeTestFolder.Creation​Time​ == AbsoluteTestFolder.Creation​Time​
                          && RelativeTestFolder.Last​Access​Time​Utc == AbsoluteTestFolder.Last​Access​Time​Utc
            );
        }


        [Author("Joseph McNeal Jr", "josephmcnealjr@gmail.com")]
        [Test]
        public void Test_DisposeShouldNotThrowError()
        {
            var newFile = $"{AbsoluteTestFolder.FullFolderPath}DisposeTest";
            using (var file = new FileObject(newFile))
            {

            }
            Assert.Pass("Successfully dispose new instance without error");
        }




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