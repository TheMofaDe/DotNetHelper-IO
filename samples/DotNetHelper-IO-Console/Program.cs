using System;
using System.Dynamic;
using System.IO;
using System.Text;

using DotNetHelper_IO;
using DotNetHelper_IO.Enum;

namespace DotNetHelper_IO_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var sampleFile = @"C:\Temp\dotnet-hosting-2.2.1-win.exe";
            var file = new FileObject(sampleFile);
            var folder = new FolderObject(sampleFile);
            var zipFile = new ZipFileObject(sampleFile);

            // Quick access to all your file information
            DateTime? CreationTime = file.FileInfo.CreationTime; // 1/23/2019 9:03:26 PM
            DateTime? CreationTimeUtc = file.FileInfo.CreationTimeUtc; // 1/24/2019 3:03:26 AM
            bool? Exist = file.Exist; // True
            var Extension = file.Extension; // .exe
            var FileNameOnly = file.FileNameOnly; // dotnet-hosting-2.2.1-win.exe
            var FileNameOnlyNoExtension = file.FileNameOnlyNoExtension; // dotnet-hosting-2.2.1-win
            var FilePathOnly = file.FilePathOnly; // C:\Temp\
            var FileSize = file.FileSize; // 100720328
            var FolderNameOnly = file.FolderNameOnly; // Temp
            var FullFilePath = file.FullFilePath; // C:\Temp\dotnet-hosting-2.2.1-win.exe
            DateTime? LastAccessTime = file.FileInfo.LastAccessTime; // 1/23/2019 9:03:26 PM
            DateTime? LastAccessTimeUtc = file.FileInfo.LastAccessTimeUtc; // 1/24/2019 3:03:26 AM
            DateTime? LastWriteTime = file.FileInfo.LastWriteTime; // 1/23/2019 9:03:47 PM
            DateTime? LastWriteTimeUtc = file.FileInfo.LastWriteTimeUtc; // 1/24/2019 3:03:47 AM
            var NotifyFilters = file.NotifyFilters; // FileName, LastWrite, LastAccess, CreationTime
            var Watcher = file.Watcher; // NULL
            var WatchTimeout = file.WatchTimeout; // 2147483647


            // CREATE OR TRUNCATE A FILE
            var testFile = new FileObject(Path.Combine(Environment.CurrentDirectory,"File.txt"));

            // Creates a empty file if it doesn't exist otherwise truncates it paramter set to <c>true</c> [overwrite existing files].
            testFile.CreateOrTruncate(true);


            



        }


    }
}
