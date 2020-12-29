using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Extension;
using DotNetHelper_IO.Helpers;
using SharpCompress.Archives;

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
	public class FolderObject : PathObject, IDisposable
	{
		public DirectoryInfo DirectoryInfo { get; private set; }


		FileSystemWatcher _watcher;
		/// <summary>
		/// Gets the watcher.
		/// </summary>
		/// <value>The watcher.</value>
		public FileSystemWatcher Watcher
		{
			get
			{
				if (_watcher == null)
				{
					try
					{
						_watcher = new FileSystemWatcher(FullName, "*");
					}
					catch (Exception) // TODO :: File watcher is not supported on every os platform so I need to find the exact exception that gets thrown and ignore 
					{

					}
				}
				return _watcher;
			}
			set
			{
				_watcher = value;
			}
		}


		/// <summary>
		/// Gets or sets the watch timeout.
		/// </summary>
		/// <value>The watch timeout.</value>
		public int WatchTimeout { get; set; } = int.MaxValue;

		/// <summary>
		/// Gets or sets the notify filters.
		/// </summary>
		/// <value>The notify filters.</value>
		public NotifyFilters NotifyFilters { get; set; } = NotifyFilters.LastAccess | NotifyFilters.LastWrite |
														   NotifyFilters.FileName | NotifyFilters.CreationTime;


		public override string Name => DirectoryInfo?.Name;


		public override DateTime? LastWriteTime => DirectoryInfo?.LastWriteTime;
		public override DateTime? LastWriteTimeUtc => DirectoryInfo?.LastWriteTimeUtc;
		public override DateTime? CreationTimeUtc => DirectoryInfo?.CreationTimeUtc;
		public override DateTime? CreationTime => DirectoryInfo?.CreationTime;
		public override DateTime? LastAccessTimeUtc => DirectoryInfo?.LastAccessTimeUtc;
		public override DateTime? LastAccessTime => DirectoryInfo?.LastAccessTime;



		/// <summary>
		/// Initializes a new instance of the <see cref="FolderObject"/> class.
		/// </summary>
		/// <param name="path">The path.</param>
		public FolderObject(string path) : base(PathType.Folder, FormatPath(path))
		{
			DirectoryInfo = new DirectoryInfo(path);
			RefreshObject();
		}



		public FolderObject(DirectoryInfo directoryInfo) : base(PathType.Folder, FormatPath(directoryInfo.FullName))
		{
			DirectoryInfo = directoryInfo;
		}



		/// <summary>
		/// Formats the path.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>System.String.</returns>
		internal static string FormatPath(string path)
		{
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
		public void RefreshObject()
		{
			if (DirectoryInfo == null)
			{
				DirectoryInfo = new DirectoryInfo(FullName);
			}
			else
			{
				DirectoryInfo.Refresh();
			}
		}

		public IEnumerable<FolderObject> GetDirectories(string searchPattern, SearchOption searchOption)
		{
			if (Exist)
				return DirectoryInfo.GetDirectories(searchPattern, searchOption).Select(info => new FolderObject(info));
			return new List<FolderObject>();
		}

		public IEnumerable<FileObject> GetFiles(string searchPattern, SearchOption searchOption)
		{
			if (Exist)
				return DirectoryInfo.GetFiles(searchPattern, searchOption).Select(info => new FileObject(info));
			return new List<FileObject>();
		}




		/// <summary>Creates all directories and subdirectories in the path for this FolderObject unless they already exist.
		/// if Folder.Option
		/// </summary>
		/// <returns>An object that represents the directory at the specified path. This object is returned regardless of whether a directory at the specified path already exists.</returns>
		/// <exception cref="T:System.IO.IOException">The directory specified is a file.-or-The network name is not known.</exception>
		/// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
		/// <exception cref="T:System.ArgumentException"> </exception>
		/// <exception cref="T:System.ArgumentNullException"> </exception>
		/// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
		/// <exception cref="T:System.IO.DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
		/// <exception cref="T:System.NotSupportedException"></exception>
		public void Create(FolderOption folderOption)
		{
			var exist = Exist;
			switch (folderOption)
			{
				case FolderOption.OverwriteFilesIfExist:
					DirectoryInfo = Directory.CreateDirectory(FullName);
					break;
				case FolderOption.DoNothingIfExist:
					DirectoryInfo = Directory.CreateDirectory(FullName);
					break;
				case FolderOption.DoNothingIfFileExist:
					DirectoryInfo = Directory.CreateDirectory(FullName);
					break;
				case FolderOption.DeleteThenWrite:
					if (exist)
					{
						DirectoryInfo.Delete(true);
						DirectoryInfo = Directory.CreateDirectory(FullName);
					}
					else
					{
						DirectoryInfo = Directory.CreateDirectory(FullName);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(folderOption), folderOption, null);
			}

		}

		public void Delete(bool recursive, bool dispose = true)
		{
			if (Exist)
				DirectoryInfo.Delete(recursive);
			if (dispose)
				Dispose();
		}



		private void CopyLogic(DirectoryInfo source, DirectoryInfo target, FolderOption folderOption)
		{
			//if (string.Equals(source.FullName, target.FullName, StringComparison.CurrentCultureIgnoreCase))
			//{
			//	return;
			//}

			// Check if the target directory exists, if not, create it.
			if (Directory.Exists(target.FullName) == false)
			{
				Directory.CreateDirectory(target.FullName);
			}

			// Copy each file into it's new directory.
			foreach (var fi in source.GetFiles())
			{
				//Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
				var destinationFile = new FileObject(Path.Combine(target.ToString(), fi.Name));
				var sourceFile = new FileObject(fi);
				switch (folderOption)
				{
					case FolderOption.OverwriteFilesIfExist:
						sourceFile.CopyTo(destinationFile.FullName, FileOption.Overwrite);
						break;
					case FolderOption.DoNothingIfExist: // WE SHOULD NEVER GET THIS FAR TO BEGIN WITH
						sourceFile.CopyTo(destinationFile.FullName, FileOption.Overwrite);
						break;
					case FolderOption.DeleteThenWrite:
						if (destinationFile.Exist)
							destinationFile.DeleteFile(false);
						sourceFile.CopyTo(destinationFile.FullName, FileOption.Overwrite);
						break;
					case FolderOption.DoNothingIfFileExist:
						sourceFile.CopyTo(destinationFile.FullName, FileOption.DoNothingIfExist);
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(folderOption), folderOption, null);
				}
			}

			// Copy each subDirectory using recursion.
			foreach (var diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
				CopyLogic(diSourceSubDir, nextTargetSubDir, folderOption);
			}
		}


		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="folderOption"></param>
		/// <exception cref="Exception">
		/// </exception>
		public void CopyContentsTo(string location, FolderOption folderOption)
		{
			var target = new DirectoryInfo(location);
			if (folderOption == FolderOption.DoNothingIfExist && target.Exists)
				return;
			CopyLogic(DirectoryInfo, target, folderOption);
		}


		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="folderOption"></param>
		/// <exception cref="Exception">
		/// </exception>
		public void CopyTo(string location, FolderOption folderOption)
		{
			var destination = new DirectoryInfo(Path.Combine(location, Name));
			if (folderOption == FolderOption.DoNothingIfExist && destination.Exists)
				return;
			CopyLogic(DirectoryInfo, destination, folderOption);
		}





		private void MoveLogic(string location, FolderOption folderOption, bool includeFolder)
		{

			if (!Exist)
				return;

			switch (folderOption)
			{
				case FolderOption.OverwriteFilesIfExist:
					CopyLogic(DirectoryInfo, new DirectoryInfo(location), folderOption);
					Delete(true, false);
					break;
				case FolderOption.DoNothingIfExist: // WE SHOULD NEVER GET THIS FAR TO BEGIN WITH
					if (Directory.Exists(location) != true)
					{
						Directory.CreateDirectory(location);
						Directory.Delete(location);
						Directory.Move(FullName, location);
					}
					break;
				case FolderOption.DeleteThenWrite:
					new FolderObject(location).Delete(true, true);
					Directory.CreateDirectory(location);
					Directory.Delete(location);
					Directory.Move(FullName, location);
					break;
				case FolderOption.DoNothingIfFileExist:
					if (includeFolder)
					{
						CopyLogic(DirectoryInfo, new DirectoryInfo(location), folderOption);
					}
					else
					{
						CopyContentsTo(location, folderOption);
					}
					Delete(true, false);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(folderOption), folderOption, null);
			}
		}



		/// <summary>
		/// Moves to.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="folderOption"></param>
		/// <exception cref="Exception">
		/// </exception>
		public void MoveTo(string location, FolderOption folderOption)
		{
			MoveLogic(Path.Combine(location, Name), folderOption, true);
		}


		/// <summary>
		/// Moves to.
		/// </summary>
		/// <param name="location">The location.</param>
		/// <param name="folderOption"></param>
		/// <exception cref="Exception">
		/// </exception>
		public void MoveContentsTo(string location, FolderOption folderOption)
		{
			MoveLogic(location, folderOption, false);
		}


		/// <summary>
		/// Zips the folder to file system in this parent folder.
		/// </summary>
		/// <param name="archiveType">Type of the archive.</param>
		public ZipFileObject ZipFolderToFileSystem(ArchiveType archiveType)
		{
			var zipFileObject = new ZipFileObject(this, archiveType);
			zipFileObject.AddFromDirectory(FullName);
			return zipFileObject;
		}


		/// <summary>
		/// Zips the folder to file system.
		/// </summary>
		/// <param name="fullPathToZipFile"></param>
		/// <param name="archiveType">Type of the archive.</param>
		/// <param name="writerOptions"></param>
		/// <param name="searchPattern"></param>
		/// <param name="searchOption"></param>
		public ZipFileObject ZipFolderToFileSystem(string fullPathToZipFile, ArchiveType archiveType,
			WriterOptions writerOptions = null, string searchPattern = "*",
			SearchOption searchOption = SearchOption.AllDirectories)
		{
			var zipFileObject = new ZipFileObject(fullPathToZipFile, archiveType, null, writerOptions);
			zipFileObject.AddFromDirectory(FullName, searchPattern, searchOption);
			return zipFileObject;
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
				archive.AddAllFromDirectory(FullName);
				// TODO :: Make This Work For .Net Standard
				archive.SaveTo(memoryStream, new WriterOptions(CompressionType.Deflate) { LeaveStreamOpen = true });
			}

			//reset memoryStream to be usable now
			memoryStream.Position = 0;
			return memoryStream;
		}


		///// <summary>
		///// Sets the file attribute.
		///// </summary>
		///// <param name="option">The option.</param>
		///// <param name="list">The list.</param>
		//public void SetFolderAttribute(AddOrRemoveEnum option, List<FileAttributes> list)
		//{

		//	if (Exist != true)
		//		return;
		//	try
		//	{
		//		if (option == AddOrRemoveEnum.Add)
		//		{
		//			foreach (var attr in list)
		//			{
		//				var di = new DirectoryInfo(FullName);
		//				di.Attributes &= ~attr;

		//			}

		//		}
		//		else if (option == AddOrRemoveEnum.Remove)
		//		{
		//			foreach (var attr in list)
		//			{
		//				var di = new DirectoryInfo(FullName);
		//				di.Attributes &= attr;
		//			}
		//		}
		//	}
		//	catch (Exception)
		//	{

		//		// ignored because this require the user to have full control permission set and we don't problems over that 
		//		// if developer is doing something that require full conrol permission let the application throw the error
		//	}
		//}





		public FileObject AddFile(string relativeFileName, string fileContent, FileOption fileOption,
			Encoding encoding = null, int bufferSize = 4096)
		{
			var file = new FileObject(FullName + relativeFileName);
			file.Write(fileContent, fileOption, encoding, bufferSize);
			return file;
		}

		public FileObject AddFile(string relativeFileName, byte[] fileContent, FileOption fileOption, int offset = 0,
			int? count = null)
		{
			var file = new FileObject(FullName + relativeFileName);
			file.Write(fileContent, fileOption, offset, count);
			return file;
		}

		public FileObject AddFile(string relativeFileName, Stream fileContent, FileOption fileOption, int bufferSize)
		{
			var file = new FileObject(FullName + relativeFileName);
			file.Write(fileContent, fileOption, bufferSize);
			return file;
		}

		public FolderObject AddFolder(string relativeFolderPath)
		{
			var folder = new FolderObject(FullName + relativeFolderPath);
			folder.Create(FolderOption.DoNothingIfExist);
			return folder;
		}


		public async Task<FileObject> AddFileAsync(string relativeFileName, string fileContent, FileOption fileOption,
			Encoding encoding = null, int bufferSize = 4096)
		{
			var file = new FileObject(FullName + relativeFileName);
			await file.WriteAsync(fileContent, fileOption, encoding, bufferSize);
			return file;
		}

		public async Task<FileObject> AddFileAsync(string relativeFileName, byte[] fileContent, FileOption fileOption, int offset = 0,
			int? count = null)
		{
			var file = new FileObject(FullName + relativeFileName);
			await file.WriteAsync(fileContent, fileOption, offset, count);
			return file;
		}

		public async Task<FileObject> AddFileAsync(string relativeFileName, Stream fileContent, FileOption fileOption, int bufferSize)
		{
			var file = new FileObject(FullName + relativeFileName);
			await file.WriteAsync(fileContent, fileOption, bufferSize);
			return file;
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
				throw new Exception($"The Following Folder {FullName} Is Not A Valid Folder Or Doesn't Exist Therefore FileSystemWatcher Can't Be Started");
			}
			else
			{
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
				DirectoryInfo = null;
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



		public override string GetSize()
		{
			return ByteSizeHelper.GetSize(DirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Select(f => new FileObject(f).SizeInBytes.GetValueOrDefault(0)).Sum());
		}

		public override long? GetSize(SizeUnits sizeUnits)
		{
			return DirectoryInfo.GetFiles("*", SearchOption.AllDirectories).Select(f => new FileObject(f).GetSize(sizeUnits).GetValueOrDefault(0)).Sum();
		}

		public override FolderObject GetParentFolder()
		{
			return new FolderObject(DirectoryInfo.Parent);
		}

		internal override bool Exists()
		{
			var exist = Directory.Exists(FullName);
			if (exist && DirectoryInfo == null)
				RefreshObject(); // DATA OUT OF SYNC
			return exist;

		}




		//internal static string ReverseString(string str)
		//{
		//	var charArray = str.ToCharArray();
		//	Array.Reverse(charArray);
		//	return new string(charArray);

		//}

		/////// <summary>
		/////// Increment file name by 1 only if the current file already exist
		/////// </summary>
		//public string GetIncrementFolderName(string seperator = "")
		//{

		//	var fileNameReverse = ReverseString(FullName);
		//	var fileNumber = fileNameReverse.TakeWhile(char.IsDigit).ToList();
		//	if (fileNumber.IsNullOrEmpty())
		//	{
		//		var fileVersionNumber = 0;
		//		var counter = 1;
		//		var temp = $"{FullName}";
		//		while (Directory.Exists(temp))
		//		{
		//			var number = fileVersionNumber + counter;
		//			temp = $"{FullName}{seperator}{number}";
		//			counter++;
		//		}

		//		return temp;
		//	}
		//	else
		//	{
		//		var sb = new StringBuilder();
		//		foreach (var c in fileNumber)
		//		{
		//			sb.Append(c);
		//		}

		//		var fileVersionNumber = ReverseString(sb.ToString()).ToInt() + 1;
		//		var counter = fileVersionNumber;
		//		var len = (counter - 1).ToString().Length;
		//		var temp = $"{DirectoryInfo.Parent.FullName}{seperator}{counter}";
		//		while (Directory.Exists(temp))
		//		{
		//			var number = fileVersionNumber + counter;
		//			temp =
		//				$"{FullName}{seperator}{number}";
		//			counter++;
		//		}

		//		return temp;
		//	}
		//}

	}
}