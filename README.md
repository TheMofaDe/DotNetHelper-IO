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
    + Writing with async support
    + Reading with async support
    + Get file/folder file sizes by any size unit, example , bytes,kb,mb,gb etc..
## Tutorial
</br>

## Write Operation 

##### Overloads for writing string,stream, or bytes exist
##### Async methods exist as well


```csharp

var file = new FileObject($@"C:\Temp\MyTestFile.txt"); 

file.Write("Will create MyTestFile.txt with this content", FileOption.Append); 
file.Write("will ovewrite the file MyTestFile.txt with this text", FileOption.Overwrite);
file.Write("nothing will happen since file already exist", FileOption.DoNothingIfExist); 
var newFileName = file.Write("will create file MyTestFile.txt1 and this method returns file name", FileOption.IncrementFileExtensionIfExist); 
var newFileName2 = file.Write("will create file MyTestFile1.txt and this method returns file name ", FileOption.IncrementFileNameIfExist); 
```



## Copying Operation

##### Async methods exist as well

```csharp
var file = new FileObject($@"C:\Temp\MyTestFile.txt");
// Copies & Append file content to specified path
file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.Append); 

// Copies file content and paste it to specified path and will overwrite if other file already exist
file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.Overwrite);

// Copies file content to specified path only if it doesn't exist otherwise do nothing
file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.DoNothingIfExist);

// Copy file content to specified path. If path already exist then create a new file with the file extension increment.
var newFileName = file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.IncrementFileExtensionIfExist);

// Copy file content to specified path. If path already exist then create a new file with the file name increment.
var newFileName2 = file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.IncrementFileNameIfExist); 
```


## Much more not shown here 

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


long? GetFileSize(FileObject.SizeUnits sizeUnits);  
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


