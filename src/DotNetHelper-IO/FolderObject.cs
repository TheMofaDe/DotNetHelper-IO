using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace DotNetHelper_IO
{
    /// <inheritdoc />
    /// <summary>
    /// Class FolderObject.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public class FolderObject : IDisposable
    {
        /// <summary>
        /// Gets the folder name only.
        /// </summary>
        /// <value>The folder name only.</value>
        public string FolderNameOnly { get; private set; }
        /// <summary>
        /// Gets the full folder path.
        /// </summary>
        /// <value>The full folder path.</value>
        public string FullFolderPath { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FolderObject"/> is exist.
        /// </summary>
        /// <value><c>null</c> if [exist] contains no value, <c>true</c> if [exist]; otherwise, <c>false</c>.</value>
        public bool? Exist { get; private set; }
        /// <summary>
        /// Gets the watcher.
        /// </summary>
        /// <value>The watcher.</value>
        public FileSystemWatcher Watcher { get; private set; }
        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public List<FileObject> Files { get; } = new List<FileObject>() { };
        /// <summary>
        /// Gets the subfolders.
        /// </summary>
        /// <value>The subfolders.</value>
        public List<FolderObject> Subfolders { get; } = new List<FolderObject>() { };
        /// <summary>
        /// Gets or sets the watch timeout.
        /// </summary>
        /// <value>The watch timeout.</value>
        public int WatchTimeout { get; set; } = int.MaxValue;
        /// <summary>
        /// Gets or sets the notify filters.
        /// </summary>
        /// <value>The notify filters.</value>
        public NotifyFilters NotifyFilters { get; set; } = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
        /// <summary>
        /// Gets the parent folder.
        /// </summary>
        /// <value>The parent folder.</value>
        public string ParentFolder { get; private set; }
        /// <summary>
        /// Gets the last​ write​ time​ UTC.
        /// </summary>
        /// <value>The last​ write​ time​ UTC.</value>
        public DateTime? Last​Write​Time​Utc { get; private set; }
        /// <summary>
        /// Gets the last​ write​ time.
        /// </summary>
        /// <value>The last​ write​ time.</value>
        public DateTime? Last​Write​Time { get; private set; }
        /// <summary>
        /// Gets the last​ access​ time.
        /// </summary>
        /// <value>The last​ access​ time.</value>
        public DateTime? Last​Access​Time { get; private set; }
        /// <summary>
        /// Gets the last​ access​ time​ UTC.
        /// </summary>
        /// <value>The last​ access​ time​ UTC.</value>
        public DateTime? Last​Access​Time​Utc { get; private set; }
        /// <summary>
        /// Gets the creation​ time​ UTC.
        /// </summary>
        /// <value>The creation​ time​ UTC.</value>
        public DateTime? Creation​Time​Utc { get; private set; }
        /// <summary>
        /// Gets the creation​ time​.
        /// </summary>
        /// <value>The creation​ time​.</value>
        public DateTime? Creation​Time​ { get; private set; }
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public string ParentNameOnly { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [load sub folders].
        /// </summary>
        /// <value><c>true</c> if [load sub folders]; otherwise, <c>false</c>.</value>
        public bool LoadSubFolders { get; }
        /// <summary>
        /// Gets a value indicating whether [load files in folder].
        /// </summary>
        /// <value><c>true</c> if [load files in folder]; otherwise, <c>false</c>.</value>
        public bool LoadFilesInFolder { get; }
        /// <summary>
        /// Gets a value indicating whether [load files in folder recursively].
        /// </summary>
        /// <value><c>true</c> if [load files in folder recursively]; otherwise, <c>false</c>.</value>
        public bool LoadRecursive { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderObject"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="loadSubfolders">if set to <c>true</c> [load subfolders].</param>
        /// <param name="loadFilesInFolder">if set to <c>true</c> [load files in folder].</param>
        /// <param name="loadRecursive"></param>
        public FolderObject(string path, bool loadSubfolders = false, bool loadFilesInFolder = false, bool loadRecursive = false)
        {
            // NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime
            FullFolderPath = FormatPath(path);
            LoadSubFolders = loadSubfolders;
            LoadFilesInFolder = loadFilesInFolder;
            LoadRecursive = loadRecursive;
            RefreshObject(LoadSubFolders, LoadFilesInFolder, LoadRecursive);
        }



        /// <summary>
        /// Formats the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.String.</returns>
        internal string FormatPath(string path)
        {
            var result = IO.IsValidFolderSyntax(path);
            if (result.Item1 != true)
            {
                throw result.Item2;
            }

            var separator = Path.IsPathRooted(path) ? Path.DirectorySeparatorChar : Path.AltDirectorySeparatorChar;
            if (path.EndsWith(separator.ToString()))
            {

            }
            else
            {
                path += separator;
            }
            return path;
        }


        /// <summary>
        /// Refreshes the object.
        /// </summary>
        /// <param name="loadSubfolders">if set to <c>true</c> [load subfolders].</param>
        /// <param name="loadFilesInFolder">if set to <c>true</c> [load files in folder].</param>
        public void RefreshObject(bool loadSubfolders = false, bool loadFilesInFolder = false, bool loadRecursive = false)
        {
            var path = FormatPath(FullFolderPath);
            Files.Clear();
            Subfolders.Clear();
            var result = IO.IsValidFolderSyntax(path);
            if (result.Item1)
            {

                Exist = Directory.Exists(path);
                if (Exist == true)
                {
                    var info = new DirectoryInfo(path);
                    FolderNameOnly = info.Name;
                    ParentFolder = info.Parent?.FullName;
                    Last​Write​Time​Utc = info.LastWriteTimeUtc;
                    Last​Write​Time = info.LastWriteTime;
                    Last​Access​Time = info.LastAccessTime;
                    Last​Access​Time​Utc = info.LastAccessTimeUtc;
                    Creation​Time​Utc = info.CreationTimeUtc;
                    Creation​Time​ = info.CreationTime;
                    ParentNameOnly = info.Parent?.Name;
                    if (loadFilesInFolder)
                    {
                        Files.AddRange(GetAllFiles("*", loadRecursive));

                    }

                    if (loadSubfolders)
                    {
                        Subfolders.AddRange(GetAllFolders("*", loadRecursive));
                    }

                    try
                    {
                        // if (Exist == true)
                        // {
                        Watcher = new FileSystemWatcher(path, "*");
                        Watcher.Changed += WatcherOnChanged;
                        Watcher.Created += WatcherOnCreated;
                        Watcher.Deleted += WatcherOnDeleted;
                        Watcher.Renamed += WatcherOnRenamed;
                        // }
                    }
                    catch (Exception) // TODO :: File watcher is not supported on every os platform so I need to find the exact exception that gets thrown and ignore 
                    {

                    }

                }
                else
                {

                    ParentFolder = null;
                    Last​Write​Time​Utc = null;
                    Last​Write​Time = null;
                    Last​Access​Time = null;
                    Last​Access​Time​Utc = null;
                    Creation​Time​Utc = null;
                    Creation​Time​ = null;
                    ParentNameOnly = null;
                }
            }
            else
            {
                throw result.Item2;
            }
        }


        /// <summary>Creates all directories and subdirectories in the specified path unless they already exist.</summary>
        /// <param name="onException">callback to execute if an exception is catch. </param>
        /// <returns>An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified path already exists.</returns>
        /// <exception cref="T:System.IO.IOException">The directory specified is a file.-or-The network name is not known.</exception>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="T:System.ArgumentException"> </exception>
        /// <exception cref="T:System.ArgumentNullException"> </exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="T:System.NotSupportedException"></exception>
        public bool Create(Action<Exception> onException)
        {
            RefreshObject(LoadSubFolders, LoadFilesInFolder);
            if (Exist != true)
            {
                try
                {
                    Directory.CreateDirectory(FullFolderPath);
                }
                catch (Exception error)
                {
                    onException?.Invoke(error);
                    return false;
                }
            }
            return true;
        }



        private List<string> GetDirectoriesRecursive(string sDir)
        {
            var list = new List<string>() { };
            try
            {
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    list.Add(d);
                    list.AddRange(GetDirectoriesRecursive(d));

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
            return list;
        }


        /// <summary>
        /// return all files in current folder object path and with filtering if pattern parameter is set
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// https://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
        public IEnumerable<FileObject> GetAllFiles(string pattern = "*", bool recursive = false)
        {

            var queue = new Queue<string>() { };
            var path = FullFolderPath;
            queue.Enqueue(path);
            if (recursive)
            {

                var allSubFolders = GetDirectoriesRecursive(path);
                allSubFolders.ForEach(delegate (string s)
                {
                    if (s?.Length <= 248)
                        queue.Enqueue(s);
                });
            }
            while (queue.Count > 0)
            {
                path = queue.Dequeue();

                var files = new List<string>() { };
                try
                {
                    files = Directory.GetFiles(path).ToList();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                foreach (var t in files)
                {
                    if (!string.IsNullOrEmpty(pattern) && !WildCardHelper.IsMatch(t, pattern, '*', '*')) continue;
                    yield return new FileObject(t);
                }

            }
        }


        /// <summary>
        /// return all files in current folder object path and with filtering if pattern parameter is set
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public IEnumerable<FolderObject> GetAllFolders(string pattern = "*", bool recursive = false)
        {

            if (recursive)
                return Directory.GetDirectories(FullFolderPath, pattern ?? "*", SearchOption.AllDirectories).Select(s => new FolderObject(s));
            return Directory.GetDirectories(FullFolderPath, pattern ?? "*", SearchOption.TopDirectoryOnly).Select(s => new FolderObject(s));
        }


        /// <summary>
        /// Directories the copy.
        /// </summary>
        /// <param name="sourceDirName">Name of the source dir.</param>
        /// <param name="destDirName">Name of the dest dir.</param>
        /// <param name="copySubDirs">if set to <c>true</c> [copy sub dirs].</param>
        /// <exception cref="DirectoryNotFoundException">Source directory does not exist or could not be found: " + sourceDirName</exception>
        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {

            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }



        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="overwriteIfExist">if set to <c>true</c> [overwrite if exist].</param>
        /// <exception cref="Exception">
        /// </exception>
        public void CopyTo(string location, bool overwriteIfExist = true)
        {
            RefreshObject();
            if (Exist != true) throw new Exception($"Couldn't Copy Folder {FullFolderPath} To {location} Because The Folder Doesn't Exist");

            if (File.Exists(location)) throw new Exception($"Couldn't Copy Folder {FullFolderPath} To {location} Because Their Is A File That Exist With That Name");

            if (overwriteIfExist)
            {
                var temp = new FolderObject(location);
                temp.DeleteFolder(e => throw e);
            }
            DirectoryCopy(FullFolderPath, location, false);
        }


        /// <summary>
        /// Moves to.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="overwriteIfExist">if set to <c>true</c> [overwrite if exist].</param>
        /// <exception cref="Exception">
        /// </exception>
        public void MoveTo(string location, bool overwriteIfExist = true)
        {
            RefreshObject();
            if (Exist != true) throw new Exception($"Couldn't Move Folder {FullFolderPath} To {location} Because The Folder Doesn't Exist");

            if (File.Exists(location)) throw new Exception($"Couldn't Move Folder {FullFolderPath} To {location} Because Their Is A File That Exist With That Name");

            if (overwriteIfExist)
            {
                var temp = new FolderObject(location);
                temp.DeleteFolder(e => throw e);
                temp.Dispose();
            }
            DirectoryCopy(FullFolderPath, location, false);
            FullFolderPath = FormatPath(location);
            RefreshObject();
        }

        /// <summary>
        /// Zips the folder to file system.
        /// </summary>
        /// <param name="zipfile">The zipfile.</param>
        /// <param name="archiveType">Type of the archive.</param>
        /// <param name="overWrite">if set to <c>true</c> [over write].</param>
        /// <exception cref="NotImplementedException">
        /// This Feature Hasn't Be Implemented Yet For Rar Files
        /// or
        /// This Feature Hasn't Be Implemented Yet For Rar Files
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">archiveType - null</exception>
        public void ZipFolderToFileSystem(FileObject zipfile, ArchiveType archiveType, bool overWrite = false)
        {
            if (zipfile.Exist == true && !overWrite)
            {
                return;
            }

            switch (archiveType)
            {
                case ArchiveType.Rar:
                    throw new NotImplementedException("This Feature Hasn't Be Implemented Yet For Rar Files");
                case ArchiveType.Zip:
                    using (var archive = ZipArchive.Create())
                    {
                        archive.AddAllFromDirectory(FullFolderPath);
                        archive.SaveTo(zipfile.FullFilePath, CompressionType.Deflate);
                    }
                    break;
                case ArchiveType.Tar:
                    using (var archive = TarArchive.Create())
                    {
                        archive.AddAllFromDirectory(FullFolderPath);
                        archive.SaveTo(zipfile.FullFilePath, CompressionType.Deflate);
                    }
                    break;
                case ArchiveType.SevenZip:
                    throw new NotImplementedException("This Feature Hasn't Be Implemented Yet For Rar Files");
                case ArchiveType.GZip:
                    using (var archive = GZipArchive.Create())
                    {
                        archive.AddAllFromDirectory(FullFolderPath);
                        archive.SaveTo(zipfile.FullFilePath, CompressionType.Deflate);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(archiveType), archiveType, null);
            }



        }
        /// <summary>
        /// Zips the folder to memory.
        /// </summary>
        /// <returns>MemoryStream.</returns>
        public MemoryStream ZipFolderToMemory()
        {

            var memoryStream = new MemoryStream();
            using (var archive = ZipArchive.Create())
            {
                archive.AddAllFromDirectory(FullFolderPath);
                // TODO :: Make This Work For .Net Standard
                archive.SaveTo(memoryStream, new WriterOptions(CompressionType.Deflate)
                {
                    LeaveStreamOpen = true
                });
            }
            //reset memoryStream to be usable now
            memoryStream.Position = 0;
            return memoryStream;
        }


        /// <summary>
        /// Sets the file attribute.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="list">The list.</param>
        public void SetFileAttribute(AddOrRemoveEnum option, List<FileAttributes> list)
        {
            RefreshObject();
            if (Exist != true) return;
            try
            {
                if (option == AddOrRemoveEnum.Add)
                {
                    foreach (var attr in list)
                    {
                        var di = new DirectoryInfo(FullFolderPath);
                        di.Attributes &= ~attr;

                    }

                }
                else if (option == AddOrRemoveEnum.Remove)
                {
                    foreach (var attr in list)
                    {
                        var di = new DirectoryInfo(FullFolderPath);
                        di.Attributes &= attr;
                    }
                }
            }
            catch (Exception)
            {

                // ignored because this require the user to have full control permission set and we don't problems over that 
                // if developer is doing something that require full conrol permission let the application throw the error
            }
        }

        /// <summary>
        /// Deletes the folder.
        /// </summary>
        public void DeleteFolder(Action<Exception> onException, bool dispose = true)
        {

            RefreshObject();
            if (Exist == true)
            {


                try
                {
                    SetFileAttribute(AddOrRemoveEnum.Remove, new List<FileAttributes>() { FileAttributes.Hidden, FileAttributes.ReadOnly });

                    GetAllFiles("*", true).ToList().ForEach(f => f.DeleteFile(e => throw e, true));
                    GetAllFolders("*", false).ToList().ForEach(delegate (FolderObject subfolder)
                    {
                        Directory.Delete(subfolder.FullFolderPath, true);
                    });
                    Directory.Delete(FullFolderPath, true);
                }
                catch (Exception error)
                {
                    onException?.Invoke(error);
                }
            }
        }







        /// <summary>
        /// A synchronous method that returns a structure that contains specific information on the change that occurred, given the type of change you want to monitor and the time
        /// (in milliseconds) to wait before timing out.
        /// </summary>
        /// <param name="changeTypes">The change types.</param>
        /// <param name="onNewThread">if set to <c>true</c> [on new thread].</param>
        /// <exception cref="Exception"></exception>
        public void StartWatching(WatcherChangeTypes changeTypes = WatcherChangeTypes.All, bool onNewThread = true)
        {
            if (Watcher == null)
            {
                // TODO :: CUSTOM EXCEPTION THEMOFADE :: 
                throw new Exception($"The Following Folder {FullFolderPath} Is Not A Valid Folder Or Doesn't Exist Therefore FileSystemWatcher Can't Be Started");
            }
            else
            {
                Watcher.Error += WatcherOnError;
                Watcher.EnableRaisingEvents = true;
                // Watcher.BeginInit(); Seems to cause problems
                if (!onNewThread)
                {
                    Watcher.WaitForChanged(changeTypes, WatchTimeout);
                }
                else
                {
                    Task.Run(delegate
                    {
                        Watcher.WaitForChanged(changeTypes, WatchTimeout);
                    }, CancellationToken.None);
                }
            }
        }
        /// <summary>
        /// Stops the watching.
        /// </summary>
        public void StopWatching()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.IncludeSubdirectories = false;
                Watcher.NotifyFilter = NotifyFilters;
            }
            //   Watcher.EndInit(); Seems to cause problems

        }

        /// <summary>
        /// Watchers the on error.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="errorEventArgs">The <see cref="ErrorEventArgs"/> instance containing the event data.</param>
        private void WatcherOnError(object sender, ErrorEventArgs errorEventArgs)
        {
            // TheMoFaDeDI.Logger.LogError($"File Watcher Error Occur.  {FolderNameOnly}", errorEventArgs.GetException());
        }

        /// <summary>
        /// Watchers the on renamed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="renamedEventArgs">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void WatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            //     Console.WriteLine($"Directory Renamed. {renamedEventArgs.OldFullPath}--->{renamedEventArgs.FullPath}");
        }

        /// <summary>
        /// Watchers the on deleted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Exist = false;
            //  Console.WriteLine($"Directory Deleted. {fileSystemEventArgs.FullPath}");
        }

        /// <summary>
        /// Watchers the on created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Exist = true;
            //  Console.WriteLine($"Directory Created. {fileSystemEventArgs.FullPath}");

        }

        /// <summary>
        /// Watchers the on changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            //  Console.WriteLine($"Directory Changed. {fileSystemEventArgs.FullPath}");
        }


        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.EndInit();
                Watcher.Dispose();
                Watcher.Changed -= WatcherOnChanged;
                Watcher.Created -= WatcherOnCreated;
                Watcher.Deleted -= WatcherOnDeleted;
                Watcher.Error -= WatcherOnError;
            }
            Exist = null;
            FolderNameOnly = null;
            Last​Write​Time​Utc = null;
            Last​Write​Time = null;
            Last​Access​Time = null;
            Last​Access​Time​Utc = null;
            Creation​Time​Utc = null;
            Creation​Time​ = null;
        }
    }





























    ///// <summary>
    ///// Class SubFolderObject. This class cannot be inherited.
    ///// </summary>
    ///// <seealso cref="IDisposable" />
    //public sealed class SubFolderObject : IDisposable
    //{
    //    /// <summary>
    //    /// Gets the folder name only.
    //    /// </summary>
    //    /// <value>The folder name only.</value>
    //    public string FolderNameOnly { get; private set; }
    //    /// <summary>
    //    /// Gets the full folder path.
    //    /// </summary>
    //    /// <value>The full folder path.</value>
    //    public string FullFolderPath { get; private set; }
    //    /// <summary>
    //    /// Gets a value indicating whether this <see cref="SubFolderObject"/> is exist.
    //    /// </summary>
    //    /// <value><c>null</c> if [exist] contains no value, <c>true</c> if [exist]; otherwise, <c>false</c>.</value>
    //    public bool? Exist { get; private set; }
    //    /// <summary>
    //    /// Gets the dir information.
    //    /// </summary>
    //    /// <value>The dir information.</value>
    //    public DirectoryInfo DirInfo { get; private set; } = null;
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="SubFolderObject"/> class.
    //    /// </summary>
    //    /// <param name="path">The path.</param>
    //    public SubFolderObject(string path)
    //    {

    //        //   if (IO.ValidFolder(path))
    //        //    {
    //        FullFolderPath = FormatPath(path);
    //        //          Exist = IO.DirectoryExist(path);
    //        //        if (Exist == true)
    //        //         {
    //        DirInfo = new DirectoryInfo(path);
    //        FolderNameOnly = DirInfo.Name;
    //        //         }
    //        //     }
    //        ///     else
    //        //     {

    //        //     }

    //    }


    //    /// <summary>
    //    /// Formats the path.
    //    /// </summary>
    //    /// <param name="path">The path.</param>
    //    /// <returns>System.String.</returns>
    //    public string FormatPath(string path)
    //    {
    //        var result = IO.IsValidFolderSyntax(path);
    //        if (result.Item1 != true)
    //        {
    //            throw result.Item2;
    //        }
    //        if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
    //        {

    //        }
    //        else
    //        {
    //            path = path + Path.DirectorySeparatorChar;
    //        }
    //        return path;
    //    }

    //    /// <summary>
    //    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    //    /// </summary>
    //    public void Dispose()
    //    {

    //        FolderNameOnly = null;
    //        FullFolderPath = null;
    //        DirInfo = null;
    //        Exist = null;

    //    }
    //}


}
