# DotNetHelper-IO

#### *DotNetHelper-IO is a simple easy to use thread safe library for handling all types of files & folders* 

|| [**View on Github**][Github] || 





## How to use
```csharp
    public enum FileOption
    {
        Append = 1,
        Overwrite = 2,
        DoNothingIfExist = 3,
        IncrementFileNameIfExist = 4,
        IncrementFileExtensionIfExist = 5,
    }
```

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

// Perform thread safe IO operations with progress reporting
public bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null)
public bool CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null)
public void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false)


long? GetFileSize(FileObject.SizeUnits sizeUnits, bool refreshObject = false);  
```


<!-- Links. -->

[1]:  https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[2]: http://themofade.github.io/DotNetHelper-IO

[Cake]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Azure DevOps]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[AppVeyor]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[GitVersion]: https://gitversion.readthedocs.io/en/latest/
[Nuget]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[Chocolately]: https://gist.github.com/davidfowl/ed7564297c61fe9ab814
[WiX]: http://wixtoolset.org/
[DocFx]: https://dotnet.github.io/docfx/
[Github]: https://github.com/TheMofaDe/DotNetHelper-IO


<!-- Documentation Links. -->
[Docs]: https://themofade.github.io/DotNetHelper-IO/index.html
[Docs-API]: https://themofade.github.io/DotNetHelper-IO/api/DotNetHelper-IO.Attribute.html
[Docs-Tutorials]: https://themofade.github.io/DotNetHelper-IO/tutorials/index.html
[Docs-samples]: https://dotnet.github.io/docfx/
[Changelogs]: https://dotnet.github.io/docfx/