using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DotNetHelper_Contracts.Enum.Encryption;
using DotNetHelper_IO.Enum;

namespace DotNetHelper_IO.Interface
{
    public interface IFileObject 
    {
        /// <summary>
        /// Gets the file name only.
        /// </summary>
        /// <value>The file name only.</value>
        string FileNameOnly { get; }

        /// <summary>
        /// Gets the file name only no extension.
        /// </summary>
        /// <value>The file name only no extension.</value>
        string FileNameOnlyNoExtension { get; }

        /// <summary>
        /// Gets the file path only.
        /// </summary>
        /// <value>The file path only.</value>
        string FilePathOnly { get; }

        /// <summary>
        /// Gets the full file path.
        /// </summary>
        /// <value>The full file path.</value>
        string FullFilePath { get; }

        /// <summary>
        /// Gets the last​ write​ time​ UTC.
        /// </summary>
        /// <value>The last​ write​ time​ UTC.</value>
        DateTime? Last​Write​Time​Utc { get; }

        /// <summary>
        /// Gets the last​ write​ time.
        /// </summary>
        /// <value>The last​ write​ time.</value>
        DateTime? Last​Write​Time { get; }

        /// <summary>
        /// Gets the last​ access​ time.
        /// </summary>
        /// <value>The last​ access​ time.</value>
        DateTime? Last​Access​Time { get; }

        /// <summary>
        /// Gets the last​ access​ time​ UTC.
        /// </summary>
        /// <value>The last​ access​ time​ UTC.</value>
        DateTime? Last​Access​Time​Utc { get; }

        /// <summary>
        /// Gets the creation​ time​ UTC.
        /// </summary>
        /// <value>The creation​ time​ UTC.</value>
        DateTime? Creation​Time​Utc { get; }

        /// <summary>
        /// Gets the creation​ time​.
        /// </summary>
        /// <value>The creation​ time​.</value>
        DateTime? Creation​Time​ { get; }

        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <value>The extension.</value>
        string Extension { get; }

        /// <summary>
        /// Gets the folder name only.
        /// </summary>
        /// <value>The folder name only.</value>
        string FolderNameOnly { get; }

        /// <summary>
        /// Size is in bytes
        /// </summary>
        /// <value>The size of the file.</value>
        long? FileSize { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IFileObject"/> is exist.
        /// </summary>
        /// <value><c>null</c> if [exist] contains no value, <c>true</c> if [exist]; otherwise, <c>false</c>.</value>
        bool? Exist { get; }

        /// <summary>
        /// Gets or sets the watch timeout.
        /// </summary>
        /// <value>The watch timeout.</value>
        int WatchTimeout { get; set; }

        /// <summary>
        /// Gets the watcher.
        /// </summary>
        /// <value>The watcher.</value>
        FileSystemWatcher Watcher { get; }

        /// <summary>
        /// Gets or sets the notify filters.
        /// </summary>
        /// <value>The notify filters.</value>
        NotifyFilters NotifyFilters { get; set; }

        /// <summary>
        /// Refreshes the object.
        /// </summary>
        void RefreshObject();

        /// <summary>
        /// return boolean on whether or not the file got move and refreshes object with the new file path if it was successfully moved
        /// </summary>
        /// <param name="copyToFullFilePath">The new file.</param>
        /// <param name="option"></param>
        /// <param name="progress"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        bool CopyTo(string copyToFullFilePath, FileOption option, IProgress<double> progress = null);

        /// <summary>
        /// return boolean on whether or not the file got move and refreshes object with the new file path if it was successfully moved
        /// </summary>
        /// <param name="copyToFullFilePath"></param>
        /// <param name="option"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        bool MoveTo(string moveToFullFilePath, FileOption option, IProgress<double> progress = null);

        /// <summary>
        /// Sets the file attribute.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="list">The list.</param>
        void SetFileAttribute(AddOrRemoveEnum option, List<FileAttributes> list);

        /// <summary>
        /// Changes the extension of the current file. Does nothing if file doesn't exist return boolean on whether or not the file extension actually got change
        /// and refreshes the object with the new file path if it was successful
        /// </summary>
        /// <param name="copyToFullFilePath"></param>
        /// <param name="option"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
        bool ChangeExtension(string newExtension, FileOption option, IProgress<double> progress = null);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        void DeleteFile(Action<Exception> onFailedDeletion, bool disposeObject = false);

        /// <summary>
        /// Creates a empty file if it doesn't exist otherwise truncates it if set to <c>true</c> [overwrite existing files].
        /// </summary>
        /// <param name="truncate">if set to <c>true</c> [truncate].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool CreateOrTruncate(bool truncate = true);

        /// <summary>
        /// Reads the file to list.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        List<string> ReadFileToList();

        /// <summary>
        /// Reads the file.
        /// </summary>
        /// <returns>System.String.</returns>
        string ReadFile();

        /// <summary>
        /// Hey, Developer Don't Forget To Dispose Of This When Your Done .. : )
        /// </summary>
        /// <returns>Stream.</returns>
        Stream ReadFileToStream();

        /// <summary>
        /// Gets the file stream.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="access">The access.</param>
        /// <param name="preventErrors">Based on your fileOption -- Prevents Exeception From Being Throwned When working with file Streams</param>
        /// <returns>FileStream.</returns>
        /// <exception cref="ArgumentOutOfRangeException">option - null</exception>
        FileStream GetFileStream(FileOption option) // LEAVE DEFAULT FALSE PLEASE
            ;

        /// <summary>
        /// Gets the stream writer.
        /// </summary>
        /// <param name="append">if set to <c>true</c> [append].</param>
        /// <returns>StreamWriter.</returns>
        StreamWriter GetStreamWriter(bool append);

        /// <summary>
        /// Same As TextReader 
        /// </summary>
        /// <returns></returns>
        StreamReader GetStreamReader();

        /// <summary>
        /// Writes the content to file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="option">The option.</param>
        void WriteContentToFile(string content, Encoding encoding,FileOption option = FileOption.Append);

        /// <summary>
        /// Encrypts the file.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key. defaults to the IAppconfig interface if key is null </param>
        void EncryptFile(SymmetricProvider algorithm, byte[] key);

        /// <summary>
        /// Decrypts the file.
        /// </summary>
        /// <param name="algorithm">The algorithm.</param>
        /// <param name="key">The key. defaults to the IAppconfig interface if key is null</param>
        /// <exception cref="EndOfStreamException">
        /// </exception>
        void DecryptFile(SymmetricProvider algorithm, byte[] key);

        /// <summary>
        /// write stream to file as an asynchronous operation.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="overwriteIfFileExist">if set to <c>true</c> [overwrite if file exist].</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> WriteStreamToFileAsync(Stream stream, IProgress<double> progress = null, bool overwriteIfFileExist = true);

        /// <summary>
        /// Writes the stream to file.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="progress">The progress.</param>
        /// <param name="overwriteIfFileExist">if set to <c>true</c> [overwrite if file exist].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        bool WriteStreamToFile(Stream stream, IProgress<double> progress = null, FileOption option = FileOption.Overwrite);

        // MOVED TO EXTENSION METHOD
        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="serializer">The serializer.</param>
        /// <returns>List&lt;T&gt;.</returns>
        T ImportData<T>(ISerializer serializer) where T : class;



        /// <summary>
        /// Imports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer">The serializer.</param>
        /// <returns>List&lt;T&gt;.</returns>
        IEnumerable<T> ImportDataList<T>(ISerializer serializer, Type type = null) where T : class;



        /// <summary>
        /// Exports the data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="options">The options.</param>
        void ExportData<T>(List<T> data, ISerializer serializer, FileOption options = FileOption.DoNothingIfExist, bool dontCreateFileIfDataEmpty = false)
            where T : class;




        /// <summary>
        /// Gets the file encoding.
        /// </summary>
        /// <returns>Encoding.</returns>
        Encoding GetFileEncoding();

        /// <summary>
        /// Gets the file size display.
        /// </summary>
        /// <returns>System.String.</returns>
        string GetFileSizeDisplay(bool refreshObject = false);


        /// <summary>
        /// Gets the file size in the specified sizeUnit.
        /// </summary>
        /// <returns>System.String.</returns>
        long? GetFileSize(FileObject.SizeUnits sizeUnits, bool refreshObject = false);

        /// <summary>
        /// Starts the watching.
        /// </summary>
        /// <param name="changeTypes">The change types.</param>
        /// <param name="onNewThread">if set to <c>true</c> [on new thread].</param>
        /// <exception cref="Exception"></exception>
        void StartWatching(WatcherChangeTypes changeTypes = WatcherChangeTypes.All, bool onNewThread = true);

        /// <summary>
        /// Stops the watching.
        /// </summary>
        void StopWatching();

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void Dispose();
    }
}
