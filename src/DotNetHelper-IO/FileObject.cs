using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Extension;
using DotNetHelper_IO.Helpers;


namespace DotNetHelper_IO
{

	/// <inheritdoc />
	/// <summary>
	/// Class FileObject.
	/// </summary>
	/// <seealso cref="T:System.PathObject" />
	/// <seealso cref="T:System.IDisposable" />
	public class FileObject : PathObject, IDisposable
	{

		/// <summary>
		/// https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.default?view=netcore-3.1
		/// </summary>
		public Encoding DefaultEncoding { get; }

		public FileInfo FileInfo { get; private set; }

		/// <summary>
		/// Gets the file name only.
		/// </summary>
		/// <value>The file name only.</value>
		public string FileNameOnly => FileInfo?.Name;
		/// <summary>
		/// Gets the file name only no extension.
		/// </summary>
		/// <value>The file name only no extension.</value>
		public string FileNameOnlyNoExtension => Path.GetFileNameWithoutExtension(FileInfo?.Name);

		/// <summary>
		/// Gets the file path only.
		/// </summary>
		/// <value>The file path only.</value>
		public string FilePathOnly => FileInfo?.DirectoryName + Path.DirectorySeparatorChar;
		/// <summary>
		/// Gets the full file path.
		/// </summary>
		/// <value>The full file path.</value>
		//  public string FullName { get; } // Let not randomly change the file name on developers & lets remove the setter so we remember


		public string Extension => FileInfo?.Extension;
		/// <summary>
		/// Gets the folder name only.
		/// </summary>
		/// <value>The folder name only.</value>
		public string FolderNameOnly => FileInfo?.Directory?.Name;

		/// <summary>
		/// Size is in bytes
		/// </summary>
		/// <value>The size of the file.</value>
		// public long? FileSize => FileInfo?.Length;


		public override string Name => FileInfo?.Name;



		public override DateTime? LastWriteTime => FileInfo?.LastWriteTime;
		public override DateTime? LastWriteTimeUtc => FileInfo?.LastWriteTimeUtc;
		public override DateTime? CreationTimeUtc => FileInfo?.CreationTimeUtc;
		public override DateTime? CreationTime => FileInfo?.CreationTime;
		public override DateTime? LastAccessTimeUtc => FileInfo?.LastAccessTimeUtc;
		public override DateTime? LastAccessTime => FileInfo?.LastAccessTime;

		//    public override string FullName => FileInfo?.FullName;



		/// <summary>
		/// Gets the file size display.
		/// </summary>
		/// <returns>System.String.</returns>
		public override string GetSize()
		{
			RefreshObject();
			if (FileInfo?.Length < 1024)
			{
				return $"{FileInfo?.Length}B";
			}
			string[] unit = { "KB", "MB", "GB", "TB", "PB" };
			const int filter = 1024;
			long unitsize = 1;
			var flag = true;
			decimal? size = FileInfo?.Length;
			var index = -1;
			while (flag)
			{
				size /= filter;
				unitsize *= filter;
				flag = size > filter;
				index++;
				if (index >= unit.Length - 1)
					flag = false;
			}
			return $"{size:f2}{unit[index]}";
		}





		/// <summary>
		/// Gets the file size display.
		/// </summary>
		/// <returns>System.String.</returns>
		public override long? GetSize(SizeUnits sizeUnits)
		{
			// RefreshObject();
			if (FileInfo?.Length == null)
				return null;
			return ByteSizeHelper.GetSize(FileInfo.Length, sizeUnits);
		}


		public override FolderObject GetParentFolder()
		{
			return new FolderObject(FileInfo?.Directory?.Parent?.FullName);
		}

		internal override bool Exists()
		{
			var exist = File.Exists(FullName);
			if (exist && FileInfo == null)
				RefreshObject(); // FORCE SYNC
			return exist;
		}

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
		/// <param name="defaultEncoding"></param>
		public FileObject(string file, Encoding defaultEncoding = null) : base(PathType.File, file)
		{
			DefaultEncoding = defaultEncoding ?? Encoding.Default;
			FileInfo = new FileInfo(file);
			Init(true);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="FileObject"/> class.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <param name="defaultEncoding"></param>
		public FileObject(FileInfo fileInfo, Encoding defaultEncoding = null) : base(PathType.File, fileInfo.FullName)
		{
			FileInfo = fileInfo;
			DefaultEncoding = defaultEncoding ?? Encoding.Default;

		}



		public void Init(bool throwOnBadFileName)
		{
	


			try
			{
				if (string.IsNullOrEmpty(FullName))
					throw new NullReferenceException($"The file name can't be null or empty.");
				RefreshObject();
			}
			catch (Exception error)
			{
				if (throwOnBadFileName)
					throw error;
			}
		}

		/// <summary>
		/// Refreshes the object.
		/// </summary>
		public void RefreshObject()
		{
			try
			{
				FileInfo = new FileInfo(FullName);
			}
			catch (PathTooLongException) // WINDOWS PROBLEM
			{
				//if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				//{

				//}

			}

		}









		/// <summary>
		/// return the FullName of where the file was copied to.
		/// </summary>
		/// <param name="copyToFullName">The file path to copy to .</param>
		/// <param name="option"></param>
		/// <exception cref="T:System.ArgumentOutOfRangeException"></exception>
		/// <exception cref="T:System.ArgumentNullException"></exception>
		/// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
		public string CopyTo(string copyToFullName, FileOption option)
		{
			copyToFullName.IsNullThrow();

			switch (option)
			{
				case FileOption.Append:
					Directory.CreateDirectory(Path.GetDirectoryName(copyToFullName));
					File.Copy(FullName, copyToFullName, false);
					return copyToFullName;
				case FileOption.Overwrite:
					Directory.CreateDirectory(Path.GetDirectoryName(copyToFullName));
					File.Copy(FullName, copyToFullName, true);
					return copyToFullName;
				case FileOption.IncrementFileNameIfExist:
					var incrementedFileName = new FileObject(copyToFullName).GetIncrementFileName();
					Directory.CreateDirectory(Path.GetDirectoryName(incrementedFileName));
					File.Copy(FullName, incrementedFileName, true);
					return incrementedFileName;
				case FileOption.IncrementFileExtensionIfExist:
					var incrementedFileExtension = new FileObject(copyToFullName).GetIncrementFileName();
					Directory.CreateDirectory(Path.GetDirectoryName(incrementedFileExtension));
					File.Copy(FullName, incrementedFileExtension, true);
					return incrementedFileExtension;
				case FileOption.DoNothingIfExist:
					if (!File.Exists(copyToFullName))
					{
						Directory.CreateDirectory(Path.GetDirectoryName(copyToFullName));
						File.Copy(FullName, copyToFullName, true);
					}
					return copyToFullName;
				case FileOption.ReadOnly:
					throw new Exception("The fileoption read-only isn't valid for a write operation of CopyTo");
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
		}



		/// <summary>
		/// Copy the current file to the destination. Returns the destination file name
		/// </summary>
		/// <param name="copyToFullName"></param>
		/// <param name="option"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="bufferSize"></param>
		/// <returns>The copy to file</returns>
		public async Task<string> CopyToAsync(string copyToFullName, FileOption option, CancellationToken cancellationToken = default, int bufferSize = 4096)
		{
			using var sourceStream = GetFileStream(FileOption.ReadOnly, bufferSize, true).fileStream;
			var fileStreamAndFileName = new FileObject(copyToFullName).GetFileStream(option, bufferSize, true);
			using (var destinationStream = fileStreamAndFileName.fileStream)
			{
				await sourceStream.CopyToAsync(destinationStream, bufferSize, cancellationToken)
					.ConfigureAwait(continueOnCapturedContext: false);

			}
			return fileStreamAndFileName.fullName;
		}


		/// <summary>
		/// Copy the current file to the destination with progress
		/// </summary>
		/// <param name="copyToFullName"></param>
		/// <param name="option"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="progress"></param>
		/// <param name="bufferSize"></param>
		/// <returns></returns>
		public async Task<string> CopyToAsync(string copyToFullName, FileOption option, CancellationToken cancellationToken, IProgress<long> progress, int bufferSize = 4096)
		{
#if NETSTANDARD2_1
			await using var sourceStream = GetFileStream(FileOption.ReadOnly, bufferSize, true).fileStream;
            var (destinationStream, fullName) = new FileObject(copyToFullName).GetFileStream(option, bufferSize, true);
			await using (destinationStream)
			{
				await sourceStream.CopyToAsync(destinationStream, progress, cancellationToken, bufferSize)
					.ConfigureAwait(continueOnCapturedContext: false);

			}
#else
			using var sourceStream = GetFileStream(FileOption.ReadOnly, bufferSize, true).fileStream;
			var (destinationStream, fullName) = new FileObject(copyToFullName).GetFileStream(option, bufferSize, true);
			using (destinationStream)
			{
				await sourceStream.CopyToAsync(destinationStream, progress, cancellationToken, bufferSize)
					.ConfigureAwait(continueOnCapturedContext: false);
			}
#endif
			return fullName;
		}



		/// <summary>
		/// Copies the file and deletes the original 
		/// </summary>
		/// <param name="moveToFullName"></param>
		/// <param name="option"></param>
		/// <exception cref="T:System.Exception"></exception>
		/// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
		public bool MoveTo(string moveToFullName, FileOption option)
		{
			if (moveToFullName == FullName)
				return true;
			if (Exist != true)
				return false;

			switch (option)
			{
				case FileOption.Append:
				case FileOption.IncrementFileNameIfExist:  // Child Record Picks Up name
				case FileOption.IncrementFileExtensionIfExist:
					CopyTo(moveToFullName, option);
					break;
				case FileOption.DoNothingIfExist:
					if (File.Exists(moveToFullName))
						return false;
					CopyTo(moveToFullName, option);
					break;
				case FileOption.Overwrite:
					if (File.Exists(moveToFullName))
						File.Delete(moveToFullName);
					File.Move(FullName, moveToFullName); // move the file
					return true;
				case FileOption.ReadOnly:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
			DeleteFile(false);
			return true;
		}


		/// <summary>
		/// Copies the file and deletes the original 
		/// </summary>
		/// <param name="moveToFullName"></param>
		/// <param name="option"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="bufferSize"></param>
		/// <exception cref="T:System.Exception"></exception>
		/// <exception cref="T:System.UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
		public async Task<bool> MoveToAsync(string moveToFullName, FileOption option, CancellationToken cancellationToken, int bufferSize = 4096)
		{
			if (moveToFullName == FullName)
				return true;
			if (Exist != true)
				return false;
			if (option == FileOption.Overwrite)
			{
				File.Move(FullName, moveToFullName); // move the file
				return true;
			}

			switch (option)
			{
				case FileOption.Append:
				case FileOption.IncrementFileNameIfExist:  // Child Record Picks Up name
				case FileOption.IncrementFileExtensionIfExist:
					await CopyToAsync(moveToFullName, option, cancellationToken, bufferSize);
					break;
				case FileOption.DoNothingIfExist:
					if (File.Exists(moveToFullName))
						return false;
					await CopyToAsync(moveToFullName, option, cancellationToken, bufferSize);
					break;
				case FileOption.Overwrite:
				case FileOption.ReadOnly:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
			DeleteFile(false);
			return true;
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
			newExtension.IsNullThrow();
			return MoveTo(Path.ChangeExtension(FullName, newExtension), option);
		}


		/// <summary>
		/// Changes the extension of the current file. Does nothing if file doesn't exist return boolean on whether or not the file extension actually got change
		/// 
		/// </summary>
		/// <param name="newExtension"></param>
		/// <param name="option"></param>
		/// <param name="cancellationToken"></param>
		/// <param name="bufferSize"></param>
		/// <exception cref="Exception"></exception>
		/// <exception cref="UnauthorizedAccessException"> throws if the application doesn't have the required permission </exception>
		public async Task<bool> ChangeExtensionAsync(string newExtension, FileOption option, CancellationToken cancellationToken = default, int bufferSize = 4096)
		{
			if (newExtension == null)
				throw new NullReferenceException($"Could not change the extension of file {FullName} Because Developer Provided A Null Value");
			return await MoveToAsync(Path.ChangeExtension(FullName, newExtension), option, cancellationToken, bufferSize);
		}




		/// <summary>
		/// Deletes the file.
		/// </summary>
		public void DeleteFile(bool disposeObject)
		{
			if (Exist != true)
				return;

			SetFileAttribute(AddOrRemoveEnum.Remove, new List<FileAttributes>() { FileAttributes.Hidden, FileAttributes.ReadOnly });
			File.Delete(FullName);
			if (!disposeObject)
				return;
			Dispose();
			return;

		}





		/// <summary>
		/// Creates a empty file if it doesn't exist otherwise truncates it if set to <c>true</c> [overwrite existing files].
		/// </summary>
		/// <param name="truncate">if set to <c>true</c> [truncate].</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public bool CreateOrTruncate(bool truncate = true)
		{

			if (!truncate && Exist)
			{
				return false;
			}
			if (!Directory.Exists(FilePathOnly))
				Directory.CreateDirectory(FilePathOnly);
			File.Create(FullName).Dispose();
			return true;

		}


		///// <summary>
		///// Increment file name by 1 only if the current file already exist
		///// </summary>
		public string GetIncrementFileName(string seperator = "")
		{

			var fileNameReverse = ReverseString(FileNameOnlyNoExtension);
			var fileNumber = fileNameReverse.TakeWhile(char.IsDigit).ToList();
			if (fileNumber.IsNullOrEmpty())
			{
				var fileVersionNumber = 0;
				var counter = 1;
				var temp = $"{FullName}";
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

		///// <summary>
		/////  Increment file extension by 1 only if the current file already exist
		///// </summary>
		public string GetIncrementExtension(string seperator = "")
		{

			var fileNameReverse = ReverseString(Extension);

			var fileNumber = fileNameReverse.TakeWhile(char.IsDigit).ToList();
			if (fileNumber.IsNullOrEmpty())
			{
				var fileVersionNumber = 0;
				var counter = 1;
				var temp = $"{FullName}";
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


		#region  Reading File

		/// <summary>
		/// Reads the file to list. You can start enumerating the collection of strings before the whole collection is returned.
		/// </summary>
		/// <returns>List&lt;System.String&gt;.</returns>
		public string[] ReadAllLines()
		{
			return File.ReadAllLines(FullName);
		}

#if NETSTANDARD21
        /// <summary>
        /// Reads the file to list. You can start enumerating the collection of strings before the whole collection is returned.
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public async Task<string[]> ReadAllLinesAsync(Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            return await File.ReadAllLinesAsync(FullName, encoding ?? DefaultEncoding,cancellationToken);
        }
#endif


		/// <summary>
		/// Reads the entire content of a file all at once
		/// </summary>
		/// <returns>List&lt;System.String&gt;.</returns>
		public string ReadAllText(Encoding encoding = null)
		{
			return File.ReadAllText(FullName, encoding ?? DefaultEncoding);
		}

#if NETSTANDARD21
        /// <summary>
        /// Reads the entire content of a file all at once 
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public async Task<string> ReadAllTextAsync(Encoding encoding = null, CancellationToken cancellationToken = default)
        {
            return await File.ReadAllTextAsync(FullName, encoding ?? DefaultEncoding, cancellationToken);
        }
#endif


#if NETSTANDARD21
        /// <summary>
        /// Reads the file in chunks instead of all at once 
        /// </summary>
        /// <returns>List&lt;System.String&gt;.</returns>
        public async Task<byte[]> ReadToBytesAsync(CancellationToken cancellationToken = default)
        {
            return await File.ReadAllBytesAsync(FullName, cancellationToken);
        }
#endif

		/// <summary>
		/// Reads the file to a byte[] but in chunks
		/// </summary>
		/// <returns>System.String.</returns>
		public async Task<byte[]> ReadToBytesInChunksAsync(int offset = 0, CancellationToken cancellationToken = default)
		{
			byte[] result;
			using (FileStream SourceStream = File.Open(FullName, FileMode.Open))
			{
				result = new byte[SourceStream.Length];
				await SourceStream.ReadAsync(result, offset, (int)SourceStream.Length, cancellationToken);
			}
			return result;
		}



		/// <summary>
		/// Reads to filestream
		/// </summary>
		/// <returns>System.String.</returns>
		public FileStream ReadToStream(int bufferSize = 4096)
		{
			return GetFileStream(FileOption.ReadOnly, bufferSize).fileStream;

		}

		/// <summary>
		/// Reads to async file stream
		/// </summary>
		/// <returns>System.String.</returns>
		public FileStream ReadToStringInChunksAsync(int bufferSize = 4096)
		{
			return GetFileStream(FileOption.ReadOnly, bufferSize, true).fileStream;

		}







		#endregion

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
				case FileOption.IncrementFileExtensionIfExist:

					break;
				case FileOption.ReadOnly:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
		}


		/// <summary>
		/// Gets the file stream.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <param name="access">The access.</param>
		/// <param name="share"></param>
		/// <param name="bufferSize"></param>
		/// <param name="fileOptions"></param>
		/// <param name="useIncrementFileName"></param>
		/// <param name="useIncrementExtension"></param>
		/// <returns>FileStream.</returns>
		/// default fileshare is read see https://referencesource.microsoft.com/#mscorlib/system/io/filestream.cs
		private (FileStream fileStream, string FullName) GetFileStream(FileMode mode, FileAccess access, FileShare share, int bufferSize = 4096, FileOptions fileOptions = FileOptions.None, bool useIncrementFileName = false, bool useIncrementExtension = false)
		{
			var file = FullName;
			if (useIncrementExtension)
			{
				file = GetIncrementExtension();
			}
			else if (useIncrementFileName)
			{
				file = GetIncrementFileName();
			}

			FileStream stream;
			if (Exist)
			{
				stream = new FileStream(file, mode, access, share, bufferSize, fileOptions) { };
				return (stream, file);
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
			stream = new FileStream(FullName, mode, access, share, bufferSize, fileOptions) { };
			return (stream, file);
		}



		/// <summary>
		/// Gets the file stream.
		/// </summary>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		/// <param name="useAsync"></param>
		/// <returns>FileStream.</returns>
		/// <exception cref="ArgumentOutOfRangeException">option - null</exception>
		/// <exception cref="FileNotFoundException"></exception>
		public (FileStream fileStream, string fullName) GetFileStream(FileOption option, int bufferSize = 4096, bool useAsync = false)
		{
			var fileOptions = useAsync ? (FileOptions.Asynchronous | FileOptions.SequentialScan) : FileOptions.None;
			(FileStream fileStream, string fullName) stream;
			switch (option)
			{
				case FileOption.ReadOnly:
					stream = (new FileStream(FullName, FileMode.Open, FileAccess.Read, FileShare.Read), FullName);
					break;
				case FileOption.Append:
					PrepareForStreamUse(option);
					stream = GetFileStream(FileMode.Append, FileAccess.Write, FileShare.Read, bufferSize, fileOptions);
					if (stream.fileStream.CanSeek)
					{
						stream.fileStream.Seek(0, SeekOrigin.End);
					}
					break;
				case FileOption.Overwrite:
					PrepareForStreamUse(option);
					stream = GetFileStream(FileMode.Truncate, FileAccess.Write, FileShare.Read, bufferSize, fileOptions);
					stream.fileStream.Seek(0, SeekOrigin.Begin);
					break;
				case FileOption.IncrementFileNameIfExist:
					stream = GetFileStream(FileMode.CreateNew, FileAccess.Write, FileShare.Read, bufferSize, fileOptions, true);
					stream.fileStream.Seek(0, SeekOrigin.Begin);
					break;
				case FileOption.IncrementFileExtensionIfExist:
					stream = GetFileStream(FileMode.CreateNew, FileAccess.Write, FileShare.Read, bufferSize, fileOptions, false, true);
					stream.fileStream.Seek(0, SeekOrigin.Begin);
					break;
				case FileOption.DoNothingIfExist:
					if (Exist)
					{
						stream = GetFileStream(FileMode.Open, FileAccess.ReadWrite, FileShare.Read, bufferSize, fileOptions);
						stream.fileStream.Seek(0, SeekOrigin.Begin);
						break;
					}
					else
					{
						stream = GetFileStream(FileMode.CreateNew, FileAccess.Write, FileShare.Read, bufferSize, fileOptions);
						if (stream.fileStream.CanSeek)
						{
							stream.fileStream.Seek(0, SeekOrigin.End);
						}
						break;
					}
				default:
					throw new ArgumentOutOfRangeException(nameof(option), option, null);
			}
			return stream;

		}



		#region  WRITING

		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="encoding">default to UTF-8</param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		public string Write(string content, FileOption option = FileOption.Overwrite, Encoding encoding = null, int bufferSize = 4096)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using var sw = new StreamWriter(fileStream, encoding ?? DefaultEncoding, bufferSize, false);
			sw.Write(content);
			return fullName;
		}

		/// <summary>
		/// Writes the bytes to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="option">The option.</param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public string Write(byte[] bytes, FileOption option = FileOption.Overwrite, int offset = 0, int? count = null)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using (fileStream)
			{
				fileStream.Write(bytes, offset, count.GetValueOrDefault(bytes.Length));
			}
			return fullName;
		}


		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		public string Write(Stream stream, FileOption option = FileOption.Overwrite, int bufferSize = 4096)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using (fileStream)
				stream.CopyTo(fileStream, bufferSize);
			return fullName;
		}

		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="progress">Report progress in stream bytes not percentages. </param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		public string Write(Stream stream, IProgress<long> progress, FileOption option = FileOption.Overwrite, int bufferSize = 4096)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using (fileStream)
				stream.CopyTo(fileStream, progress, bufferSize);
			return fullName;
		}



		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. 
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="encoding"></param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		public async Task<string> WriteAsync(string content, FileOption option = FileOption.Overwrite, Encoding encoding = null, int bufferSize = 4096)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using var sw = new StreamWriter(fileStream, encoding ?? DefaultEncoding, bufferSize, false);
			await sw.WriteAsync(content);
			return fullName;
		}


		/// <summary>
		/// Writes the bytes to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="bytes">The bytes.</param>
		/// <param name="option">The option.</param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <param name="cancellationToken"></param>
		public async Task<string> WriteAsync(byte[] bytes, FileOption option = FileOption.Overwrite, int offset = 0, int? count = null, CancellationToken cancellationToken = default)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
			using (fileStream)
			{
				await fileStream.WriteAsync(bytes, offset, count.GetValueOrDefault(bytes.Length), cancellationToken);
			}
			return fullName;
		}


		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		/// <param name="cancellationToken"></param>
		public async Task<string> WriteAsync(Stream stream, FileOption option = FileOption.Overwrite, int bufferSize = 4096, CancellationToken cancellationToken = default)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option, bufferSize, true);
#if NETSTANDARD21
            await using (fileStream)
                await stream.CopyToAsync(fileStream, null, cancellationToken, bufferSize);
            // await fileStream.CopyToAsync(stream, bufferSize, cancellationToken);
#else
			using (fileStream)
				await stream.CopyToAsync(fileStream, null, cancellationToken, bufferSize);
#endif
			return fullName;
		}


		/// <summary>
		/// Writes the content to file. Returns the full file name content was written to. This method is not thread safe
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="progress"></param>
		/// <param name="option">The option.</param>
		/// <param name="bufferSize"></param>
		/// <param name="cancellationToken"></param>
		public async Task<string> WriteAsync(Stream stream, IProgress<long> progress, FileOption option = FileOption.Overwrite, int bufferSize = 4096, CancellationToken cancellationToken = default)
		{
			if (option == FileOption.DoNothingIfExist && Exist)
				return FullName;
			var (fileStream, fullName) = GetFileStream(option);
#if NETSTANDARD21
            await using (fileStream)
                await stream.CopyToAsync(fileStream,  progress, cancellationToken, bufferSize);
#else
			using (fileStream)
				await stream.CopyToAsync(fileStream, progress, cancellationToken, bufferSize);
#endif
			return fullName;
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
			using (var z = File.OpenRead(FullName))
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
		/// Starts the watching.
		/// </summary>
		/// <param name="changeTypes">The change types.</param>
		/// <param name="onNewThread">if set to <c>true</c> [on new thread].</param>
		/// <exception cref="Exception"></exception>
		public void StartWatching(WatcherChangeTypes changeTypes = WatcherChangeTypes.All, bool onNewThread = true, NotifyFilters? filters = null)
		{
			Watcher = new FileSystemWatcher(FilePathOnly, "file")
			{
				IncludeSubdirectories = false,
				NotifyFilter = filters.GetValueOrDefault(NotifyFilters),
				EnableRaisingEvents = true
			};
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



		#region HelperMethods



		/// <summary>
		/// Sets the file attribute.
		/// </summary>
		/// <param name="option">The option.</param>
		/// <param name="list">The list.</param>
		public void SetFileAttribute(AddOrRemoveEnum option, List<FileAttributes> list)
		{
			if (Exist != true)
				return;
			if (list == null || !list.Any())
				return;
			try
			{
				if (option == AddOrRemoveEnum.Add)
				{
					foreach (var attr in list)
					{
						File.SetAttributes(FullName, File.GetAttributes(FullName) | attr);
					}

				}
				else if (option == AddOrRemoveEnum.Remove)
				{
					foreach (var attr in list)
					{
						File.SetAttributes(FullName, File.GetAttributes(FullName) & ~attr);
					}
				}
			}
			catch (Exception)
			{

				// ignored because this require the user to have full control permission set and we don't problems over that 
				// if developer is doing something that require full conrol permission let the application throw the error
			}
		}

		private static string ReverseString(string str)
		{
			var charArray = str.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);

		}
		#endregion






		private bool _isDisposed;
		// Dispose() calls Dispose(true)
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		// The bulk of the clean-up code is implemented in Dispose(bool)
		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			if (disposing)
			{
				// free managed resources
				// Managed resources are those that are pure .NET code and managed by the runtime and are under its direct control.
				FileInfo = null;
				if (Watcher != null)
				{

					Watcher?.EndInit();
					Watcher?.Dispose();
				}
			}

			//// free native resources if there are any.
			//if (nativeResource != IntPtr.Zero)
			//{
			//	// Unmanaged resources are those that are not. File handles, pinned memory, COM objects, database connections etc
			//	Marshal.FreeHGlobal(nativeResource);
			//	nativeResource = IntPtr.Zero;
			//}

			_isDisposed = true;
		}

	}


}