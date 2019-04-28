using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_Contracts.Contracts;
using DotNetHelper_Contracts.Enum.Encryption;
using DotNetHelper_Contracts.Enum.IO;
using DotNetHelper_Contracts.Extension;
//using DotNetHelper_Contracts.Extension;
using DotNetHelper_DeviceInformation;
using DotNetHelper_Encryption;
using DotNetHelper_IO.Interface;

namespace DotNetHelper_IO
{

    /// <inheritdoc />
    /// <summary>
    /// Class FileObject.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public class FileObject : IDisposable, IFileObject
    {
        private object ThreadSafe { get; set; } = new object();

        /// <summary>
        /// Gets the file name only.
        /// </summary>
        /// <value>The file name only.</value>
        public string FileNameOnly { get; private set; }
        /// <summary>
        /// Gets the file name only no extension.
        /// </summary>
        /// <value>The file name only no extension.</value>
        public string FileNameOnlyNoExtension { get; private set; }
        /// <summary>
        /// Gets the file path only.
        /// </summary>
        /// <value>The file path only.</value>
        public string FilePathOnly { get; private set; }
        /// <summary>
        /// Gets the full file path.
        /// </summary>
        /// <value>The full file path.</value>
        public string FullFilePath { get; } // Let not randomly change the file name on developers & lets remove the setter so we remember
        public string IncrementFullFilePath { get; private set; } // This allows support 
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
        /// Gets the extension. Includes the dot (.)
        /// </summary>
        /// <value>The extension.</value>
        public string Extension { get; private set; }
        /// <summary>
        /// Gets the folder name only.
        /// </summary>
        /// <value>The folder name only.</value>
        public string FolderNameOnly { get; private set; }

        /// <summary>
        /// Size is in bytes
        /// </summary>
        /// <value>The size of the file.</value>
        public long? FileSize { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this <see cref="FileObject"/> is exist.
        /// </summary>
        /// <value><c>null</c> if [exist] contains no value, <c>true</c> if [exist]; otherwise, <c>false</c>.</value>
        public bool? Exist { get; private set; }
        /// <summary>
        /// Gets or sets the watch timeout.
        /// </summary>
        /// <value>The watch timeout.</value>
        public int WatchTimeout { get; set; } = int.MaxValue;
        /// <summary>
        /// Gets the watcher.
        /// </summary>
        /// <value>The watcher.</value>
        public FileSystemWatcher Watcher { get; private set; }

        /// <summary>
        /// Gets or sets the notify filters.
        /// </summary>
        /// <value>The notify filters.</value>
        public NotifyFilters NotifyFilters { get; set; } = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;      
        //  public Action<object, FileSystemEventArgs> WatchOnChanged { get; set; }
        //  public Action<object, FileSystemEventArgs> WatchOnCreated { get; set; }
        //  public Action<object, FileSystemEventArgs> WatchOnDeleted { get; set; }
        //  public Action<object, RenamedEventArgs> OnRenamed { get; set; }
        //  public Action<object, ErrorEventArgs> OnError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileObject"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FileObject(string file)
        {
            if (string.IsNullOrEmpty(file)) throw new NullReferenceException($"The file name can't be null or empty.");
            FullFilePath = file;
            RefreshObject();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="FileObject"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FileObject(string file, bool throwOnBadFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(file)) throw new NullReferenceException($"The file name can't be null or empty.");
                FullFilePath = file;
                RefreshObject();
            }
            catch (Exception error)
            {
                if (throwOnBadFileName) throw error;
            }
        }

        /// <summary>
        /// Refreshes the object.
        /// </summary>
        public void RefreshObject()
        {
            var result = IO.IsValidFilePathSyntax(FullFilePath);
            if (result.Item1 != true)
            {
              //  TheMoFaDeDI.Logger.LogError(result.Item2);
                throw result.Item2;
            }

            Exist = File.Exists(FullFilePath);


            try
            {
                if (Exist == true)
                {
                    var info = new FileInfo(FullFilePath);
                    FileNameOnly = info.Name;
                    FolderNameOnly = info.Directory?.Name;
                    FilePathOnly = info.DirectoryName + Path.DirectorySeparatorChar;
                    FileNameOnlyNoExtension = Path.GetFileNameWithoutExtension(FullFilePath);
                    Last​Write​Time​Utc = info.LastWriteTimeUtc;
                    Last​Write​Time = info.LastWriteTime;
                    Last​Access​Time = info.LastAccessTime;
                    Last​Access​Time​Utc = info.LastAccessTimeUtc;
                    Creation​Time​Utc = info.CreationTimeUtc;
                    Creation​Time​ = info.CreationTime;
                    Extension = info.Extension;
                    FileSize = info.Length;
                    if (DeviceInformation.DeviceOS != DeviceInformation.DeviceOs.Android && DeviceInformation.DeviceOS != DeviceInformation.DeviceOs.iOS)
                    {
                        if (Directory.Exists(FullFilePath))
                        {
                            Watcher = new FileSystemWatcher(FilePathOnly, "file");
                            Watcher.Changed += WatcherOnChanged;
                            Watcher.Created += WatcherOnCreated;
                            Watcher.Deleted += WatcherOnDeleted;
                            Watcher.Renamed += WatcherOnRenamed;
                        }
                    }
                }
                else
                {
                    FilePathOnly = Path.GetDirectoryName(FullFilePath) + Path.DirectorySeparatorChar;
                    Extension = Path.GetExtension(FullFilePath);
                    FileNameOnlyNoExtension = Path.GetFileNameWithoutExtension(FullFilePath);
                    FolderNameOnly = Path.GetDirectoryName(FullFilePath);
                    FileNameOnly = Path.GetFileName(FullFilePath);
                    Last​Write​Time​Utc = null;
                    Last​Write​Time = null;
                    Last​Access​Time = null;
                    Last​Access​Time​Utc = null;
                    Creation​Time​Utc = null;
                    Creation​Time​ = null;
                    FileSize = null;
                    if (FullFilePath.EndsWith(Path.DirectorySeparatorChar.ToString())) // we don't want to prevent users from creating file with same folder name but no extension because it not the same
                        if (string.IsNullOrEmpty(Extension) && Directory.Exists(FullFilePath))
                        {
                            throw new Exception($"The Following File Path {FullFilePath} already exist as a folder. Failed To initialize FileObject Because This Path isn't a file");
                        }
                }
            }
            catch (PathTooLongException)
            {
                //TODO FIGURE THIS OUT BRO 
               // do nothing for now
            }
           // catch (Exception)
           // {
           //  //   Console.WriteLine($"The Given File Name {file} Is A Illegal FileName Please Provide A Valid File Name Next Time");
           //     throw;
           // }
        }



    
           



        /// <inheritdoc />
        /// <summary>
        /// return boolean on whether or not the file got move 
        /// </summary>
        /// <param name="copyToFullFilePath">The new file.</param>
        /// <param name="option"></param>
        /// <param name="progress"></param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
        /// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        public bool CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null)
        {
            RefreshObject();
            if (Exist != true) return false; //throw new Exception($"Couldn't Copy {FullFilePath} To {copyToFullFilePath} Because The File Doesn't Exist");
            using (var newFile = new FileObject(copyToFullFilePath))
            using (var stream = ReadFileToStream())
            {
                var copyFile = new FileObject(copyToFullFilePath);
                switch (option)
                {
                    case FileOption.Append:
                    case FileOption.Overwrite:
                    case FileOption.IncrementFileNameIfExist:  // child method picks up name
                    case FileOption.IncrementFileExtensionIfExist:
                        Directory.CreateDirectory(newFile.FilePathOnly);
                        copyFile.WriteStreamToFile(stream, progress, option); 
                        return true;
                    case FileOption.DoNothingIfExist:
                        if (newFile.Exist == true)
                            return false;
                        Directory.CreateDirectory(newFile.FilePathOnly);
                        copyFile.WriteStreamToFile(stream, progress, option);
                        return true;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(option), option, null);
                }
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// return boolean on whether or not the file got move 
        /// </summary>
        /// <param name="copyToFullFilePath"></param>
        /// <param name="option"></param>
        /// <exception cref="T:System.Exception"></exception>
        /// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        public bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null)
        {
            RefreshObject();
            if (Exist != true) return false; //throw new Exception($"Couldn't Copy {FullFilePath} To {moveToFullFilePath} Because The File Doesn't Exist");
            if (moveToFullFilePath == FullFilePath) return true;
            if (option == FileOption.Overwrite)
            {
                File.Move(FullFilePath, moveToFullFilePath); // move the file
                RefreshObject();
                return true;
            }
            using (var newFile = new FileObject(moveToFullFilePath))
            using (var stream = ReadFileToStream())
            {
                var copyFile = new FileObject(moveToFullFilePath);
                switch (option)
                {
                    case FileOption.Append:
                    case FileOption.IncrementFileNameIfExist:  // Child Record Picks Up name
                    case FileOption.IncrementFileExtensionIfExist:
                        Directory.CreateDirectory(newFile.FilePathOnly);
                        copyFile.WriteStreamToFile(stream, progress, option);
                        break;
                    case FileOption.DoNothingIfExist:
                        if (newFile.Exist == true)
                            return false;
                        Directory.CreateDirectory(newFile.FilePathOnly);
                        copyFile.WriteStreamToFile(stream, progress, option);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(option), option, null);
                }
            }
            DeleteFile(e => throw e);
           // FullFilePath = moveToFullFilePath;
            RefreshObject();
            return true;

        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the file attribute.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="list">The list.</param>
        public void SetFileAttribute(AddOrRemoveEnum option, List<FileAttributes> list)
        {
            RefreshObject();
            if (Exist != true) return;
            if (list == null || !list.Any()) return;
          try { 
            if (option == AddOrRemoveEnum.Add)
            {
                foreach (var attr in list)
                {
                    File.SetAttributes(FullFilePath, File.GetAttributes(FullFilePath) | attr);
                }

            }
            else if (option == AddOrRemoveEnum.Remove)
            {
                foreach (var attr in list)
                {
                    File.SetAttributes(FullFilePath, File.GetAttributes(FullFilePath) & ~attr);
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
        /// Changes the extension of the current file. Does nothing if file doesn't exist return boolean on whether or not the file extension actually got change
        /// 
        /// </summary>
        /// <param name="copyToFullFilePath"></param>
        /// <param name="option"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        public bool ChangeExtension(string newExtension, FileOption option, IProgress<double> progress = null)
        {
            if (newExtension == null) throw new NullReferenceException($"Could not change the extension of file {FullFilePath} Because Developer Provided A Null Value");
            return MoveTo(Path.ChangeExtension(FullFilePath, newExtension), option, progress);

            //if (Exist == true)
            //{if (newExtension == null) throw new NullReferenceException($"Could Not Extension Of File {FullFilePath} Because Developer Provided A Null Value");
            //    
            //    // TODO  :: THEMOFADE TEST BEHAVIOR FOR FILES THAT DOESN"T HAVE A EXTENSION & WITH YOU PASS IT NO EXTENSION BECAUSE IT SHOULDN"T ERROR ON THESE OCCASSION
            //    MoveTo(Path.ChangeExtension(FullFilePath, newExtension), option);
            //    FullFilePath = Path.ChangeExtension(FullFilePath, newExtension);
            //    RefreshObject();
            //    
            //}
        }

        /// <summary>
        /// Deletes the file. If you want an 
        /// </summary>
        public void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false)
        {
            RefreshObject();
            if (Exist == true)
            {
                try
                {
                    SetFileAttribute(AddOrRemoveEnum.Remove, new List<FileAttributes>() { FileAttributes.Hidden, FileAttributes.ReadOnly });
                    File.Delete(FullFilePath);
                    if (disposeObject)
                    {
                        Dispose();
                        return;
                    }

                    RefreshObject();
                }
                catch (Exception error)
                {
                    onFailedDeletion?.Invoke(error);
                  //  TheMoFaDeDI.Logger.LogError("Failed To Delete File: " + FullFilePath, error);
                    
                }
            }
        }



        private bool IncrementCreateOrTruncate(bool truncate = true)
        {
            if (truncate)
            {
                if (File.Exists(IncrementFullFilePath))
                {
                    try
                    {
                        File.Delete(IncrementFullFilePath);
                    }
                    catch (Exception)
                    {
                        // ignored
                        // it may not look like it here but trust me this will bubble up if its a problem. This is only used for a special senario
                    }
                }
            }
            else
            {
                if (File.Exists(IncrementFullFilePath)) return true;
            }
            // HAVE TO CHECK IF DIRECTORY EXIST FIRST BEFORE THIS
            if (!Directory.Exists(FilePathOnly))
                Directory.CreateDirectory(FilePathOnly);
                using (var stream = new FileStream(IncrementFullFilePath, FileMode.Create, FileAccess.ReadWrite,FileShare.ReadWrite))
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(string.Empty);
                }
            return true;
        }

        /// <summary>
        /// Creates a empty file if it doesn't exist otherwise truncates it if set to <c>true</c> [overwrite existing files].
        /// </summary>
        /// <param name="truncate">if set to <c>true</c> [truncate].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool CreateOrTruncate(bool truncate = true)
        {
            RefreshObject();

            if (truncate)
            {
                DeleteFile(e =>  throw e);
            }
            else
            {
                if (Exist == true) return true;
            }
            if (!Directory.Exists(FilePathOnly))
                Directory.CreateDirectory(FilePathOnly);
            lock (ThreadSafe)
            {
                using (var stream = new FileStream(FullFilePath, FileMode.Create, FileAccess.ReadWrite,FileShare.ReadWrite))
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(string.Empty);
                }
            }
            RefreshObject();
            return true;

        }

        /// <summary>
        /// Prepares for stream use.  Prevents Exeception From Being Throwned When working with file Streams
        /// </summary>
        /// <param name="option">The option.</param>
        /// <exception cref="ArgumentOutOfRangeException">option - null</exception>
        internal void PrepareForStreamUse(FileOption option)
        {
            switch (option)
            {
                case FileOption.Append:
                    CreateOrTruncate(false);
                    break;
                case FileOption.Overwrite:
                    CreateOrTruncate();
                    break;
                case FileOption.DoNothingIfExist:
                    if (Exist == true)
                        return;
                    CreateOrTruncate();
                    break;
                case FileOption.IncrementFileNameIfExist:
                    TryIncrementFileName();
                    IncrementCreateOrTruncate();
                    break;
                case FileOption.IncrementFileExtensionIfExist:
                    TryIncrementFileExtension();
                    IncrementCreateOrTruncate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }



        private static string ReverseString(string str)
        {
            var charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);

        }

        /// <summary>
        /// Increment file name by 1 only if the current file already exist
        /// </summary>
        internal void TryIncrementFileName()
        {
            IncrementFullFilePath = GetIncrementFileName();
        }

        /// <summary>
        ///  Increment file extension by 1 only if the current file already exist
        /// </summary>
        internal void TryIncrementFileExtension()
        {
            IncrementFullFilePath = GetIncrementExtension();
        }


        public string GetIncrementFileName(string seperator = "")
        {
    
            var fileNameReverse = ReverseString(FileNameOnlyNoExtension);
            var fileNumber = fileNameReverse.TakeWhile(char.IsDigit).ToList();
            if (fileNumber.IsNullOrEmpty())
            {
                var fileVersionNumber = 0;
                 var counter = 1;
                var temp  = $"{FullFilePath}";
                while (File.Exists(temp))
                {
                    var number = fileVersionNumber + counter;
                    temp = $"{FilePathOnly}{FileNameOnlyNoExtension}{seperator}{number}{Extension}";
                    counter++;
                }
                return temp;
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var c in fileNumber)
                {
                    sb.Append(c);
                }
                
                var fileVersionNumber = ReverseString(sb.ToString()).ToInt() + 1;
                var counter = fileVersionNumber;
                var len = (counter - 1).ToString().Length;
                var temp = $"{FilePathOnly}{FileNameOnlyNoExtension.Remove(FileNameOnlyNoExtension.Length - len)}{seperator}{counter}{Extension}";
                while (File.Exists(temp))
                {
                    var number = fileVersionNumber + counter;
                    temp = $"{FilePathOnly}{FileNameOnlyNoExtension.Remove(FileNameOnlyNoExtension.Length - len)}{seperator}{number}{Extension}";
                    counter++;
                }
                return temp;
            }
        }


        public string GetIncrementExtension(string seperator = "")
        {

            var fileNameReverse = ReverseString(Extension);
    
            var fileNumber = fileNameReverse.TakeWhile(char.IsDigit).ToList();
            if (fileNumber.IsNullOrEmpty())
            {
                var fileVersionNumber = 0;
                var counter = 1;
                var temp = $"{FullFilePath}";
                while (File.Exists(temp))
                {

                    var number = fileVersionNumber + counter;
                    var tempExtension = string.IsNullOrEmpty(fileNameReverse) ? "." : Extension;
                    temp = $"{FilePathOnly}{FileNameOnlyNoExtension}{tempExtension}{seperator}{number}";
                    counter++;

                }
                return temp;
            }
            else
            {
                var sb = new StringBuilder();
                foreach (var c in fileNumber)
                {
                    sb.Append(c);
                }
                var fileVersionNumber = ReverseString(sb.ToString()).ToInt() + 1;

                var counter = fileVersionNumber;
                var len = (counter - 1).ToString().Length;
                var tempExtension = string.IsNullOrEmpty(fileNameReverse) ? $".{counter}" : Extension.Remove(Extension.Length - len);
                var temp = $"{FilePathOnly}{FileNameOnlyNoExtension}{tempExtension}{seperator}{counter}";
                while (File.Exists(temp))
                {
                    var number = fileVersionNumber + counter;
                    tempExtension = string.IsNullOrEmpty(fileNameReverse) ? "." : Extension.Remove(Extension.Length - len);
                    temp = $"{FilePathOnly}{FileNameOnlyNoExtension}{tempExtension}{seperator}{number}";
                    counter++;
                }
                return temp;
            }
        }




        /// <summary>
        /// Reads the file to list.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public List<string> ReadFileToList()
        {
            RefreshObject();
            if (Exist != true)
            {
                Console.WriteLine($"Could Read File {FullFilePath} Because It Doesn't Exist"); return null;
            }
            return File.ReadAllLines(FullFilePath).ToList();
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ReadFile()
        {

            RefreshObject();
            if (Exist != true) { Console.WriteLine($"Could Read File {FullFilePath} Because It Doesn't Exist"); return null; }
            using (var sr = new StreamReader(ReadFileToStream()))
            {
                return sr.ReadToEnd();
            }

        }

        /// <summary>
        /// Hey, Developer Don't Forget To Dispose Of This When Your Done .. : )
        /// </summary>
        /// <returns>Stream.</returns>
        public Stream ReadFileToStream()
        {
            RefreshObject();
            if (Exist != true) { Console.WriteLine($"Could Read File {FullFilePath} Because It Doesn't Exist"); return null; }
            var sr = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read) { Position = 0 };
            return sr;

        }


        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="access">The access.</param>
        /// <returns>FileStream.</returns>
        private FileStream GetFileStream(FileMode mode, FileAccess access = FileAccess.ReadWrite)
        {
            RefreshObject();
            if (Exist == true)
            {
                var stream = new FileStream(FullFilePath, mode, access) { };
                return stream;
            }
            else
            {
                if(!Directory.Exists(FilePathOnly))
                Directory.CreateDirectory(FilePathOnly);
            }
            if (mode == FileMode.Truncate)
            {
                CreateOrTruncate(); 
            }
            return new FileStream(FullFilePath, mode, access) { };
        }


        private FileStream IncrementGetFileStream(FileMode mode, FileAccess access = FileAccess.ReadWrite)
        {
           
            if (File.Exists(IncrementFullFilePath))
            {
                var stream = new FileStream(IncrementFullFilePath, mode, access) { };
                return stream;
            }
            else
            {
                if (!Directory.Exists(FilePathOnly))
                    Directory.CreateDirectory(FilePathOnly);
            }
            if (mode == FileMode.Truncate)
            {
                IncrementCreateOrTruncate(); 
            }
            return new FileStream(IncrementFullFilePath, mode, access) { };
        }

        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="access">The access.</param>
        /// <param name="preventErrors">Based on your fileOption -- Prevents Exeception From Being Throwned When working with file Streams</param>
        /// <returns>FileStream.</returns>
        /// <exception cref="ArgumentOutOfRangeException">option - null</exception>
        public FileStream GetFileStream(FileOption option) 
        {

            RefreshObject();
            FileStream stream;
            switch (option)
            {
                case FileOption.Append:
                    //  if (preventErrors)
                    PrepareForStreamUse(option);
                    stream = GetFileStream(FileMode.Append, FileAccess.Write);
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.End);
                    }
                    return stream;
                case FileOption.Overwrite:
                    PrepareForStreamUse(option);
                    stream = GetFileStream(FileMode.Truncate, FileAccess.Write);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                case FileOption.IncrementFileNameIfExist:
                case FileOption.IncrementFileExtensionIfExist:
                    //    if (preventErrors)
                    PrepareForStreamUse(option);
                    stream = IncrementGetFileStream(FileMode.Truncate, FileAccess.Write);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream;
                case FileOption.DoNothingIfExist:   // TODO :: Revisit This THEMOFADE
                    if (Exist == true)
                    {
                        stream = GetFileStream(FileMode.Open);
                        stream.Seek(0, SeekOrigin.Begin);
                        return stream;
                    }
                    else
                    {
                        stream = GetFileStream(FileMode.CreateNew, FileAccess.Write);
                        if (stream.CanSeek)
                        {
                            stream.Seek(0, SeekOrigin.End);
                        }
                        return stream;
                    }
 
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }

        /// <summary>
        /// Gets the stream writer.
        /// </summary>
        /// <param name="append">if set to <c>true</c> [append].</param>
        /// <returns>StreamWriter.</returns>
        public StreamWriter GetStreamWriter(bool append)
        {
            RefreshObject();
            if (Exist == true)
            {
                var stream = new StreamWriter(FullFilePath, append) { };
                return stream;
            }
            else
            {

            }
            return new StreamWriter(FullFilePath, append) { };

        }
        /// <summary>
        /// Same As TextReader 
        /// </summary>
        /// <returns></returns>
        public StreamReader GetStreamReader()
        {
            RefreshObject();
            if (Exist == true)
            {
                var fs = new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                return new StreamReader(fs) { };
            }
            else
            {
                return null;
            }
        }



        // /// <summary>
        // /// Hey, Developer Don't Forget To Dispose Of This When Your Done .. : ) 
        // /// </summary>
        // /// <param name="fullFilePath"></param>
        // /// <returns></returns>
        // public  Stream WriteStreamToFile( FileMode fileMode = FileMode.Truncate)
        // {
        //     RefreshObject();
        //     if (Exist != true) { Console.WriteLine($"Could Read File {FullFilePath} Because It Doesn't Exist"); return null; }
        //     var sr = new FileStream(FullFilePath, fileMode, FileAccess.ReadWrite) { Position = 0 };
        //     return sr;
        //
        // }

        /// <inheritdoc />
        /// <summary>
        /// Writes the content to file. This method is thread safe
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="encoding"></param>
        /// <param name="option">The option.</param>
        public void WriteContentToFile(string content, Encoding encoding, FileOption option = FileOption.Append)
        {
            RefreshObject();
            if (option == FileOption.DoNothingIfExist && Exist == true) return; 
            lock (ThreadSafe)
            {
                using (var stream = GetFileStream(option))
                using (var sw = new StreamWriter(stream,encoding))
                {
                    sw.Write(content);
                }
            }
        }



   

        


        //The block size is the basic unit of data that can be encrypted or decrypted in one operation.
        //Messages longer than the block size are handled as successive blocks; messages shorter than the block size
        //must be padded with extra bits to reach the size of a block. Valid block sizes are determined by the symmetric algorithm used.
        /// <summary>
        /// Encrypts the file. this method is thread safe.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key. defaults to the IAppconfig interface if key is null </param>
        public void EncryptFile(SymmetricProvider algorithm, byte[] key )
        {
            var tempFileName = Path.GetTempFileName();

            using (var cipher = EncryptionSymmetric.Create(algorithm))
            lock (ThreadSafe)
            {
                using (var fileStream = File.OpenRead(FullFilePath))
                using (var tempFile = File.Create(tempFileName))
                {
                    cipher.Key = key;
                    tempFile.Write(cipher.IV, 0, cipher.IV.Length);
                    using (var cryptoStream =
                        new CryptoStream(tempFile, cipher.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        fileStream.CopyTo(cryptoStream);
                    }
                }
            }

            File.Delete(FullFilePath);
            File.Move(tempFileName, FullFilePath);
        }

        /// <summary>
        /// Decrypts the file.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key. defaults to the IAppconfig interface if key is null</param>
        /// <exception cref="EndOfStreamException">
        /// </exception>
        public void DecryptFile(SymmetricProvider algorithm, byte[] key = null)
        {
            var tempFileName = Path.GetTempFileName();

            using (var cipher = EncryptionSymmetric.Create(algorithm))
            using (var fileStream = File.OpenRead(FullFilePath))
            using (var tempFile = File.Create(tempFileName))
            {
              
                cipher.Key = key ?? throw new ArgumentNullException(nameof(key));
                var iv = cipher.IV;
                // var iv = new byte[cipher.BlockSize / 8];
                //  var headerBytes =  new byte[6];
                var remain = key.Length;
                remain = iv.Length;
                while (remain != 0)
                {

                    var read = fileStream.Read(iv, iv.Length - remain, remain);

                    if (read == 0)
                    {
                        throw new EndOfStreamException();
                    }

                    remain -= read;
                }

                cipher.IV = iv;

                using (var cryptoStream = new CryptoStream(tempFile, cipher.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    fileStream.CopyTo(cryptoStream);
                }
            }

            File.Delete(FullFilePath);
            File.Move(tempFileName, FullFilePath);
        }



      
        /// <inheritdoc />
        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public T ImportData<T>(ISerializer serializer) where T : class
        {
            return serializer.DeserializeFromFile<T>(FullFilePath);
        }


        /// <inheritdoc />
        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <returns>List&lt;T&gt;.</returns>
        public IEnumerable<T> ImportDataList<T>(ISerializer serializer, Type type = null) where T : class
        {

            if (type == null)
            {
                return serializer.DeserializeListFromFile<T>(FullFilePath);
            }
            return serializer.DeserializeFromFile(type, FullFilePath) as IEnumerable<T>;
            //   return serializer.DeserializeListFromFile<object>;
        }






        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="options">The options.</param>
        public void ExportData<T>(List<T> data, ISerializer serializer, FileOption options = FileOption.DoNothingIfExist, bool dontCreateFileIfDataEmpty = false) where T : class
        {
            if (dontCreateFileIfDataEmpty && data.IsNullOrEmpty())
            {

            }
            else
            {
                serializer.SerializeToFile(data, FullFilePath, options);
            }
        }







        /// <summary>
        /// write stream to file as an asynchronous operation. this method is not thread safe
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="overwriteIfFileExist">if set to <c>true</c> [overwrite if file exist].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public async Task<bool> WriteStreamToFileAsync(Stream stream, IProgress<double> progress = null, bool overwriteIfFileExist = true)
        {
            try
            {


                if (!overwriteIfFileExist && Exist == true) return true;

                if (stream.Length <= 0 && !stream.CanRead)
                {
                    Console.WriteLine("Couldn't retrieve the data from stream length was zero and also the stream was not readabale");
                    return false;
                }
                stream.Position = 0;
                var start = DateTime.Now;

                using (var file = new FileStream(FullFilePath, FileMode.Create, FileAccess.Write) {Position = 0})
                {
                    var buffer = new byte[4 * 1024];

                    int read;
                    var max = stream.Length;
                    var currentprogress = 0;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        currentprogress = currentprogress + read;
                        var realprogress = decimal.Divide(currentprogress, max) * 100;
                        if (progress != null)
                            if ((Convert.ToInt32(realprogress) / 2) > 0)
                                progress.Report((Convert.ToInt32(realprogress)));
                        await file.WriteAsync(buffer, 0, read, CancellationToken.None);
                    }
                    var elapsedTimeInSeconds = DateTime.Now.Subtract(start).TotalSeconds;
                //    Console.WriteLine($"Async Stream To File Took {elapsedTimeInSeconds} seconds");
                    return true;
                }

            }
            catch (Exception error)
            {
                throw error;
                
            }

        }

        /// <inheritdoc />
        /// <summary>
        /// Writes the stream to file. this method is thread safe
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="overwriteIfFileExist">if set to <c>true</c> [overwrite if file exist].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool WriteStreamToFile(Stream stream, IProgress<double> progress = null, FileOption option = FileOption.Overwrite)
        {
            //    try
            //     {

            RefreshObject();
            if (option == FileOption.DoNothingIfExist && Exist == true) return true;

           if (stream.Length <= 0 && !stream.CanRead)
           {
               Console.WriteLine("Couldn't retrieve the data from stream length was zero and also the stream was not readabale");
               return false;
           }
            stream.Position = 0;
            var start = DateTime.Now;
            lock (ThreadSafe)
            {
                using (var file = GetFileStream(option))
                {
                    var buffer = new byte[4 * 1024];

                    int read;
                    var max = stream.Length;
                    var currentprogress = 0;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        currentprogress = currentprogress + read;
                        if (max != 0)
                        {
                            var realprogress = decimal.Divide(currentprogress, max) * 100;
                            if (progress != null)
                                if ((Convert.ToInt32(realprogress) / 2) > 0)
                                    progress.Report((Convert.ToInt32(realprogress)));
                        }
          
                        file.Write(buffer, 0, read);
                    }
                    var elapsedTimeInSeconds = DateTime.Now.Subtract(start).TotalSeconds;
                    // Console.WriteLine($"Async Stream To File Took {elapsedTimeInSeconds} seconds");
                    return true;
                }
            }

            //    }
            //     catch (Exception error) { TheMoFaDeDI.Logger.LogError("Couldn't Retrieve The Data From Stream ", error); }
            //     return false;
        }



        /// <summary>
        /// Gets the file encoding. if can not determine the file Encoding this return ascii by default
        /// </summary>
        /// <returns>Encoding.</returns>
        public Encoding GetFileEncoding()
        {
            RefreshObject();
            // *** Detect byte order mark if any - otherwise assume default
            var buffer = new byte[5];

            if (Exist != true)
            {
                Console.WriteLine("Couldn't Get File Encoding Because File Doesn't Exist");
                return null;
            }
            using (var z = File.OpenRead(FullFilePath))
            {
                z.Read(buffer, 0, 5);
            }

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                return Encoding.UTF8;
            if (buffer[0] == 0xfe && buffer[1] == 0xff)
                return Encoding.Unicode;
            if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                return Encoding.UTF32;
            if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                return Encoding.UTF7;

            return Encoding.ASCII;
        }


        /// <summary>
        /// Gets the file size display.
        /// </summary>
        /// <returns>System.String.</returns>
        public string GetFileSizeDisplay(bool refreshObject = false)
        {

            if(refreshObject) RefreshObject();
            if (FileSize < 1024)
            {
                if (FileSize == null) return $"File Doesn't Exist";
                return $"{FileSize}B";
            }
            string[] unit = { "KB", "MB", "GB", "TB", "PB" };
            const int filter = 1024;
            long unitsize = 1;
            var flag = true;
            decimal? size = FileSize;
            var index = -1;
            while (flag)
            {
                size = size / filter;
                unitsize = unitsize * filter;
                flag = size > filter;
                index++;
                if (index >= unit.Length - 1) flag = false;
            }
            return $"{size:f2}{unit[index]}";
        }


        /// <inheritdoc />
        /// <summary>
        /// Gets the file size display.
        /// </summary>
        /// <returns>System.String.</returns>
        public long? GetFileSize(SizeUnits sizeUnits, bool refreshObject = false)
        {
            if (refreshObject) RefreshObject();
            if (FileSize == null) return null;
            if (FileSize == 0) return 0;
            const int filter = 1024;
            if (sizeUnits == SizeUnits.Byte) return FileSize;
            return FileSize.Value / (filter * sizeUnits.ToInt() + 1 );
        }

        public enum SizeUnits
        {
            Byte, Kb, Mb, Gb, Tb, Pb, Eb, Zb, Yb
        }

        // public static string ToSize(Int64 value, SizeUnits unit)
        // {
        //     return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        // }


        protected virtual bool IsFileLocked()
        {
            FileStream stream = null;

            try
            {
                var file = new FileInfo(FullFilePath);
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }

            //file is not locked
            return false;
        }


        /// <summary>
        /// Starts the watching.
        /// </summary>
        /// <param name="changeTypes">The change types.</param>
        /// <param name="onNewThread">if set to <c>true</c> [on new thread].</param>
        /// <exception cref="Exception"></exception>
        public void StartWatching(WatcherChangeTypes changeTypes = WatcherChangeTypes.All, bool onNewThread = true)
        {
            RefreshObject();
            if (Watcher == null)
            {
                // TODO :: CUSTOM EXCEPTION THEMOFADE :: 
                throw new Exception($"The Following File {FullFilePath}  Doesn't Exist Therefore FileSystemWatcher Can't Be Started.");
            }
            else
            {
                Watcher.IncludeSubdirectories = false;
                Watcher.NotifyFilter = NotifyFilters;
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
            // TheMoFaDeDI.Logger.LogError($"File Watcher Error Occur.  {FileNameOnly}", errorEventArgs.GetException());
        }

        /// <summary>
        /// Watchers the on renamed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="renamedEventArgs">The <see cref="RenamedEventArgs"/> instance containing the event data.</param>
        private void WatcherOnRenamed(object sender, RenamedEventArgs renamedEventArgs)
        {
            //  Console.WriteLine($"File Renamed. {renamedEventArgs.OldFullPath}--->{renamedEventArgs.FullPath}");
        }

        /// <summary>
        /// Watchers the on deleted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnDeleted(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Exist = false;
            // Console.WriteLine($"File Deleted. {fileSystemEventArgs.FullPath}");
        }

        /// <summary>
        /// Watchers the on created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Exist = true;
            // Console.WriteLine($"File Created. {fileSystemEventArgs.FullPath}");

        }

        /// <summary>
        /// Watchers the on changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private static void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            // Console.WriteLine($"File Changed. {fileSystemEventArgs.FullPath}");
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
            }
            Exist = null;
            FilePathOnly = null;
            Extension = null;
            FileNameOnlyNoExtension = null;
            FolderNameOnly = null;
            FileNameOnly = null;
            Last​Write​Time​Utc = null;
            Last​Write​Time = null;
            Last​Access​Time = null;
            Last​Access​Time​Utc = null;
            Creation​Time​Utc = null;
            Creation​Time​ = null;
            FileSize = null;
        }
    }

   
}
