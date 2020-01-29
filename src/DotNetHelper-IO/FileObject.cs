using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Extension;


namespace DotNetHelper_IO
{

    /// <inheritdoc />
    /// <summary>
    /// Class FileObject.
    /// </summary>
    /// <seealso cref="T:System.IDisposable" />
    public class FileObject : IDisposable
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FileObject"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        public FileObject(string file, bool throwOnBadFileName = true)
        {
            FullFilePath = file;
            Init(throwOnBadFileName);
        }



        public void Init(bool throwOnBadFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(FullFilePath)) throw new NullReferenceException($"The file name can't be null or empty.");
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
                    try
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
                    catch (Exception) // TODO :: File watcher is not supported on every os platform so I need to find the exact exception that gets thrown and ignore 
                    {

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
                // FILE MAY STILL EXIST BUT WINDOWS DUMB 255 CHARACTER LIMIT
            }
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
        public void CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null)
        {
         

            switch (option)
            {
                case FileOption.Append:
                    Directory.CreateDirectory(Path.GetDirectoryName(copyToFullFilePath));
                    File.Copy(FullFilePath, copyToFullFilePath, false);
                    return;
                case FileOption.Overwrite:
                    Directory.CreateDirectory(Path.GetDirectoryName(copyToFullFilePath));
                    File.Copy(FullFilePath,copyToFullFilePath,true);
                    return;
                case FileOption.IncrementFileNameIfExist:
                case FileOption.IncrementFileExtensionIfExist:
                    using (var newFile = new FileObject(copyToFullFilePath))
                    {
                        newFile.WriteStreamToFile(GetFileStream(option));
                    }
                    return;
                case FileOption.DoNothingIfExist:
                    if (!File.Exists(copyToFullFilePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(copyToFullFilePath));
                        File.Copy(FullFilePath, copyToFullFilePath, true);
                    }
                    return;
                case FileOption.ReadOnly:
                    throw new Exception("The fileoption read-only isn't valid for the method CopyTo");
                default:
                    throw new ArgumentOutOfRangeException(nameof(option), option, null);
            }
        }


        /// <inheritdoc />
        /// <summary>
        /// return boolean on whether or not the file got move 
        /// </summary>
        /// <param name="moveToFullFilePath"></param>
        /// <param name="option"></param>
        /// <exception cref="T:System.Exception"></exception>
        /// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        public bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null)
        {
            if (Exist != true) return false; //throw new Exception($"Couldn't Copy {FullFilePath} To {moveToFullFilePath} Because The File Doesn't Exist");
            if (moveToFullFilePath == FullFilePath) return true;
            if (option == FileOption.Overwrite)
            {
                File.Move(FullFilePath, moveToFullFilePath); // move the file
                return true;
            }
            using (var newFile = new FileObject(moveToFullFilePath))
            {
                using var stream = GetFileStream(FileOption.ReadOnly);
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
            if (Exist != true) return;
            if (list == null || !list.Any()) return;
            try
            {
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
        /// <param name="newExtension"></param>
        /// <param name="option"></param>
        /// <param name="progress"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        public bool ChangeExtension(string newExtension, FileOption option, IProgress<double> progress = null)
        {
            if (newExtension == null) throw new NullReferenceException($"Could not change the extension of file {FullFilePath} Because Developer Provided A Null Value");
            return MoveTo(Path.ChangeExtension(FullFilePath, newExtension), option, progress);
        }

        /// <summary>
        /// Deletes the file. If you want an 
        /// </summary>
        public void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false)
        {
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
                }
                catch (Exception error)
                {
                    onFailedDeletion?.Invoke(error);
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
            using (var stream = new FileStream(IncrementFullFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using var sw = new StreamWriter(stream);
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
            if (truncate)
            {
                DeleteFile(e => throw e);
            }
            else
            {
                if (Exist == true) return true;
            }
            if (!Directory.Exists(FilePathOnly))
                Directory.CreateDirectory(FilePathOnly);
            lock (ThreadSafe)
            {
                using var stream = new FileStream(FullFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                using var sw = new StreamWriter(stream);
                sw.Write(string.Empty);
            }
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
                var temp = $"{FullFilePath}";
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
        public List<string> ReadFileToList(bool throwOnFileNotFound = true)
        {
            return File.ReadAllLines(FullFilePath).AsList();
        }

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>System.String.</returns>
        public string ReadFile(bool throwOnFileNotFound = true)
        {
            if (Exist != true)
            {
                if (throwOnFileNotFound)
                {
                    throw new FileNotFoundException($"Couldn't read file {FullFilePath} because it doesn't exist");
                }
                return null;
            }
            using var sr = new StreamReader(GetFileStream(FileOption.ReadOnly));
            return sr.ReadToEnd();
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
                if (!Directory.Exists(FilePathOnly))
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
        /// <returns>FileStream.</returns>
        /// <exception cref="ArgumentOutOfRangeException">option - null</exception>
        public FileStream GetFileStream(FileOption option)
        {

            RefreshObject();
            FileStream stream;
            switch (option)
            {
                case FileOption.ReadOnly:
                    return new FileStream(FullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read) { };
                case FileOption.Append:
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
                case FileOption.DoNothingIfExist: 
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
    


        #region  WRITING


        /// <summary>
        /// Writes the content to file. This method is thread safe
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="encoding"></param>
        /// <param name="option">The option.</param>
        public void WriteContentToFile(string content, Encoding encoding, FileOption option = FileOption.Append)
        {
            if (option == FileOption.DoNothingIfExist && Exist == true) return;
            lock (ThreadSafe)
            {
                using var stream = GetFileStream(option);
                using var sw = new StreamWriter(stream, encoding);
                sw.Write(content);
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
            if (!overwriteIfFileExist && Exist == true) return true;

                if (stream.Length <= 0 && !stream.CanRead)
                {
                    Console.WriteLine("Couldn't retrieve the data from stream length was zero and also the stream was not readabale");
                    return false;
                }
                stream.Position = 0;
                var start = DateTime.Now;

                using var file = new FileStream(FullFilePath, FileMode.Create, FileAccess.Write) { Position = 0 };
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
                return true;
            }

        /// <summary>
        /// Writes the stream to file. this method is thread safe
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="option"></param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool WriteStreamToFile(Stream stream, IProgress<double> progress = null, FileOption option = FileOption.Overwrite)
        {
            if (option == FileOption.DoNothingIfExist && Exist == true) return true;

            if (stream.Length <= 0) // COPY NOTHING
            {
                return true;
            }
            stream.Position = 0;
            var start = DateTime.Now;
            lock (ThreadSafe)
            {
                using var file = GetFileStream(option);
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
                return true;
            }
        }

        #endregion   

        /// <summary>
        /// Gets the file encoding. if can not determine the file Encoding this return ascii by default
        /// </summary>
        /// <returns>Encoding.</returns>
        public Encoding GetFileEncoding()
        {
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

            if (refreshObject) RefreshObject();
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
            return FileSize.Value / (filter * sizeUnits.ToInt() + 1);
        }




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
            if (Watcher == null)
            {
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
        }

        /// <summary>
        /// Watchers the on created.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private void WatcherOnCreated(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            Exist = true;
        }

        /// <summary>
        /// Watchers the on changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="fileSystemEventArgs">The <see cref="FileSystemEventArgs"/> instance containing the event data.</param>
        private static void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {

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
