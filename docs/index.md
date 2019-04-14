# DotNetHelper-IO

This is a simple easy to use thread safe library for handling all types of files & folders

## How to use
```csharp
var sampleFile = "C:\Temp\dotnet-hosting-2.2.1-win.exe";
var file = new FileObject(sampleFile);
var folder = new FolderObject(sampleFile);
var zipFile = new ZipFileObject(sampleFile);

// Quick access to all your file information
DateTime? CreationTime = file.CreationTime; // 1/23/2019 9:03:26 PM
DateTime? CreationTimeUtc = file.CreationTimeUtc; // 1/24/2019 3:03:26 AM
Boolean? Exist = file.Exist; // True
String Extension = file.Extension; // .exe
String FileNameOnly = file.FileNameOnly; // dotnet-hosting-2.2.1-win.exe
String FileNameOnlyNoExtension = file.FileNameOnlyNoExtension; // dotnet-hosting-2.2.1-win
String FilePathOnly = file.FilePathOnly; // C:\Temp\
Int64? FileSize = file.FileSize; // 100720328
String FolderNameOnly = file.FolderNameOnly; // Temp
String FullFilePath = file.FullFilePath; // C:\Temp\dotnet-hosting-2.2.1-win.exe
DateTime? LastAccessTime = file.LastAccessTime; // 1/23/2019 9:03:26 PM
DateTime? LastAccessTimeUtc = file.LastAccessTimeUtc; // 1/24/2019 3:03:26 AM
DateTime? LastWriteTime = file.LastWriteTime; // 1/23/2019 9:03:47 PM
DateTime? LastWriteTimeUtc = file.LastWriteTimeUtc; // 1/24/2019 3:03:47 AM
NotifyFilters NotifyFilters = file.NotifyFilters; // FileName, LastWrite, LastAccess, CreationTime
FileSystemWatcher Watcher = file.Watcher; // NULL
Int32 WatchTimeout = file.WatchTimeout; // 2147483647

// Quick access to IO operations (Not all available methods are showned in this snippet)
void EncryptFile(SymmetricProvider algorithm, byte[] key);
void DecryptFile(SymmetricProvider algorithm, byte[] key = null);
public bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null)
public bool CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null)
long? GetFileSize(FileObject.SizeUnits sizeUnits, bool refreshObject = false);  
public void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false)
```

## Targeted .NET Frameworks
    NET452
    NETSTANDARD2.0

