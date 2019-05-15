# DotNetHelper-IO


| Package  | Tests | Coverage |
| :-----:  | :---: | :------: |
| ![Build Status][nuget-downloads]  | ![Build Status][tests]  | [![Coverage Status](https://coveralls.io/repos/github/TheMofaDe/DotNetHelper-IO/badge.svg)](https://coveralls.io/github/TheMofaDe/DotNetHelper-IO) |

### *Azure DevOps*
| Windows | Linux | MacOS |
| :-----: | :-----: | :---: | 
| ![Build Status][azure-windows]  | ![Build Status][azure-linux]  | ![Build Status][azure-macOS] 

### *AppVeyor*
| Windows |
| :-----: | 
| ![Build Status][appveyor-windows]


#####  DotNetHelper-IO is a simple easy to use thread safe library for handling all types of files & folders


## Why use this
Working with files,folders & stream you will run into many edge-cases that throws IO exception
like attempting to write to a file but is parent folder doesn't exist. This library is aware of those edge-cases
and will make working with IO less stressful. With quick access to all the file options below
we do the hard work
 
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

// Perform thread safe IO operations with progress reporting
public bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null)
public bool CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null)
public void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false)


// Simplify Encryption
void EncryptFile(SymmetricProvider algorithm, byte[] key);
void DecryptFile(SymmetricProvider algorithm, byte[] key = null);


long? GetFileSize(FileObject.SizeUnits sizeUnits, bool refreshObject = false);  
```


## Exception Callback 
```csharp
You may notice some methods will have an Action<Exception> parameter this callback can be null 
but it will not throw the exception under any circumstances. I understand its normal for libraries to bubble up exceptions
but you are literally implementing how to handle the exception so does not make sense for me to throw it
```

## Documentation
For more information, please refer to the [Officials Docs][2]

Created Using [DotNet-Starter-Template](http://themofade.github.io/DotNet-Starter-Template) 


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



<!-- BADGES. -->

[nuget-downloads]: https://img.shields.io/nuget/dt/DotNetHelper-IO.svg?style=flat-square
[tests]: https://img.shields.io/appveyor/tests/themofade/DotNetHelper-IO.svg?style=flat-square
[coverage-status]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master&jobName=Windows

[azure-windows]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master&jobName=Windows
[azure-linux]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master&jobName=Linux
[azure-macOS]: https://dev.azure.com/Josephmcnealjr0013/DotNetHelper-IO/_apis/build/status/TheMofaDe.DotNetHelper-IO?branchName=master&jobName=macOS

[appveyor-windows]: https://ci.appveyor.com/project/TheMofaDe/DotNetHelper-IO/branch/master
