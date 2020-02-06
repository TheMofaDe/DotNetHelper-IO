 # DotNetHelper-IO


#### *DotNetHelper-IO is a simple easy to use thread safe library for handling all types of files & folders* 

|| [**Documentation**][Docs] • [**API**][Docs-API] • [**Tutorials**][Docs-Tutorials] ||  [**Change Log**][Changelogs] • || [**View on Github**][Github]|| 

| AppVeyor | AzureDevOps |
| :-----: | :-----: |
| [![Build status](https://ci.appveyor.com/api/projects/status/4sx1v8sem7283fuc?svg=true)](https://ci.appveyor.com/project/TheMofaDe/dotnethelper-io)  | [![Build Status](https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master)](https://dev.azure.com/Josephmcnealjr0013/DotNetHelper.ObjectToSql/_build/latest?definitionId=5&branchName=master)  

| Package  | Tests | Code Coverage |
| :-----:  | :---: | :------: |
| ![Build Status][nuget-downloads]  | ![Build Status][tests]  | [![codecov](https://codecov.io/gh/TheMofaDe/DotNetHelper-IO/branch/master/graph/badge.svg)](https://codecov.io/gh/TheMofaDe/DotNetHelper-IO) |


## Features
    + Auto Increment File Name
    + Auto Increment File Extension

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


## Documentation
For more information, please refer to the [Officials Docs][Docs]

<!-- Links. -->
## Solution Template
[![badge](https://img.shields.io/badge/Built%20With-DotNet--Starter--Template-orange.svg)](https://github.com/TheMofaDe/DotNet-Starter-Template)


<!-- Links. -->

[1]:  https://gist.github.com/davidfowl/ed7564297c61fe9ab814

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
[Docs-API]: https://themofade.github.io/DotNetHelper-IO/api/DotNetHelper-IO.html
[Docs-Tutorials]: https://themofade.github.io/DotNetHelper-IO/tutorials/index.html
[Docs-samples]: https://dotnet.github.io/docfx/
[Changelogs]: https://dotnet.github.io/docfx/


<!-- BADGES. -->

[nuget-downloads]: https://img.shields.io/nuget/dt/DotNetHelper-IO.svg?style=flat-square
[tests]: https://img.shields.io/appveyor/tests/TheMofaDe/DotNetHelper-IO.svg?style=flat-square
[coverage-status]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master&jobName=Windows


