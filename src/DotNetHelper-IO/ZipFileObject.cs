using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Extension;
using DotNetHelper_IO.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Common.GZip;
using SharpCompress.Common.Zip;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace DotNetHelper_IO
{

	public class ZipFileObject : FileObject
	{
		public string Password { get; }
		public ArchiveType Type { get; }

		public ZipFileObject(string fullFilePath, ArchiveType type, string password = null) : base(fullFilePath)
		{
			Password = password;
			Type = type;
		}

		public ZipFileObject(FolderObject folderObject, ArchiveType type, string password = null) : base(folderObject.GetParentFolder().FullName + $"{folderObject.Name}{CompressExtensionHelper.ExtensionLookup[type]}")
		{
			Password = password;
			Type = type;
		}


	
		private IWritableArchive GetNewWritableArchive()
		{
			switch (Type)
			{
				case ArchiveType.Zip:
					return ZipArchive.Create();
				case ArchiveType.Tar:
					return TarArchive.Create();
				case ArchiveType.GZip:
					return GZipArchive.Create();
				case ArchiveType.Rar:
				case ArchiveType.SevenZip:
					throw new NotImplementedException("Writing operation for Rar & SevenZip is not support yet. ");
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}
		private IWritableArchive GetWritableArchive()
		{
			IWritableArchive MapToNewArchive(IWritableArchive writableArchive)
			{
				IWritableArchive newWritableArchive = GetNewWritableArchive();
				using (writableArchive)
				{
					foreach (var entry in writableArchive.Entries)
					{
						var memoryStream = new MemoryStream();
						entry.WriteTo(memoryStream);
						newWritableArchive.AddEntry(entry.Key, memoryStream, true, entry.Size, entry.LastModifiedTime);
					}
				}
				return newWritableArchive;
			}

			switch (Type)
			{
				case ArchiveType.Zip:
					return (Exist ? MapToNewArchive(ZipArchive.Open(FullName)) : GetNewWritableArchive());
				case ArchiveType.Tar:
					return (Exist ? MapToNewArchive(TarArchive.Open(FullName)) : GetNewWritableArchive());
				case ArchiveType.GZip:
					return (Exist ? MapToNewArchive(GZipArchive.Open(FullName)) : GetNewWritableArchive());
				case ArchiveType.Rar:
				case ArchiveType.SevenZip:
					throw new NotImplementedException("Writing operation for Rar & SevenZip is not support yet. ");
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}

		public IArchive GetReadableArchive()
		{
			switch (Type)
			{
				case ArchiveType.Zip:
					return (Exist ? ZipArchive.Open(FullName) : ZipArchive.Create());
				case ArchiveType.Tar:
					return (Exist ? TarArchive.Open(FullName) : TarArchive.Create());
				case ArchiveType.GZip:
					return (Exist ? GZipArchive.Open(FullName) : GZipArchive.Create());
				case ArchiveType.Rar:
					return (Exist
						? RarArchive.Open(FullName)
						: throw new NotImplementedException("Rar files only have read only support"));
				case ArchiveType.SevenZip:
					return (Exist
						? SevenZipArchive.Open(FullName)
						: throw new NotImplementedException("7ZIP files only have read only support"));
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}

		private WriterOptions GetDefaultWriterOptions()
		{
			switch (Type)
			{
				case ArchiveType.Rar:
					return new WriterOptions(CompressionType.Rar){LeaveStreamOpen = false};
				case ArchiveType.Zip:
					return new WriterOptions(CompressionType.Deflate) { LeaveStreamOpen = false };
				case ArchiveType.Tar:
					return new WriterOptions(CompressionType.GZip) { LeaveStreamOpen = true };
				case ArchiveType.SevenZip:
					return new WriterOptions(CompressionType.Xz) { LeaveStreamOpen = false };
				case ArchiveType.GZip:
					return new WriterOptions(CompressionType.GZip) { LeaveStreamOpen = false };
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		//public IEnumerable<IArchiveEntry> GetArchiveEntries()
		//{
		//	return GetReadableArchive()?.Entries;
		//}

		

		/// <summary>
		/// Creates a empty zip file if it doesn't exist otherwise truncates it if set to <c>true</c> [overwrite existing files].
		/// </summary>
		/// <param name="truncate">if set to <c>true</c> [truncate].</param>
		/// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
		public new bool CreateOrTruncate(bool truncate = true)
		{
			if (truncate)
			{
				DeleteFile(false);
			}
			else
			{
				if (Exist == true)
					return true;
			}
			// HAVE TO CHECK IF DIRECTORY EXIST FIRST BEFORE THIS
			if (!Directory.Exists(FilePathOnly))
				Directory.CreateDirectory(FilePathOnly);
			using (var archive = GetWritableArchive())
			{
				var defaultWriteOptions = GetDefaultWriterOptions();
				defaultWriteOptions.LeaveStreamOpen = false;
				using var fs = GetFileStream(FileOption.Overwrite).fileStream;
				archive.SaveTo(fs, defaultWriteOptions);
			}
			return true;
		}



		public void ExtractToDirectory(string fullFolderPath, ExtractionOptions extractionOptions = null)
		{
			GetReadableArchive().WriteToDirectory(fullFolderPath,extractionOptions ?? new ExtractionOptions());
		}

		public void Compress(FolderObject folderObject, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			Compress(folderObject.FullName,searchPattern,searchOption);
		}
		public void Compress(string fullFolderPathToCompress,string searchPattern = "*",SearchOption searchOption = SearchOption.AllDirectories)
		{
			GetWritableArchive().AddAllFromDirectory(fullFolderPathToCompress,searchPattern,searchOption);
		}

		public int GetFileCount()
		{
			var readerOptions = new ReaderOptions() {Password = Password, LeaveStreamOpen = false,LookForHeader = false};
			switch (Type)
			{
				case ArchiveType.Rar:
					using (var archive = RarArchive.Open(FullName, readerOptions))
						return archive.Entries.Count;
				case ArchiveType.Zip:
					using (var archive = ZipArchive.Open(FullName, readerOptions))
						return archive.Entries.Count;
				case ArchiveType.Tar:
					using (var archive = TarArchive.Open(FullName, readerOptions))
						return archive.Entries.Count;
				case ArchiveType.SevenZip:
					using (var archive = SevenZipArchive.Open(FullName, readerOptions))
						return archive.Entries.Count;
				case ArchiveType.GZip:
					using (var archive = GZipArchive.Open(FullName, readerOptions))
						return archive.Entries.Count;
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}


		#region Helper Methods


		public bool ContainsFile(string key)
		{
			if (!Exist)
			{
				return false;
			}

			using (var archive = GetReadableArchive())
			{
				var entries = archive.Entries;
				foreach (var entry in entries)
				{
					var path = entry.Key;
					var p = path.Replace('/', '\\');
					if (p.StartsWith("\\"))
					{
						p = p.Substring(1);
					}
					var exist = string.Equals(p, key, StringComparison.OrdinalIgnoreCase);
					if (exist)
					{
						return true;
					}
				}
			}

			return false;
		}

		internal (bool exist, IArchive archive, IArchiveEntry file) TryGetFile(string key)
		{
			if (!Exist)
			{
				return (false,null,null);
			}

			var archive = GetReadableArchive();
			var entries = archive.Entries;
			foreach (var entry in entries)
			{
				var path = entry.Key;
				var p = path.Replace('/', '\\');
				if (p.StartsWith("\\"))
				{
					p = p.Substring(1);
				}
				var exist = string.Equals(p, key, StringComparison.OrdinalIgnoreCase);
				if (exist)
				{
					return (true,archive,entry);
				}
			}
			return (false,archive,null);
		}


		//internal bool EntryExist(IEnumerable<IArchiveEntry> entries, string key, out IArchiveEntry file)
		//{
		//	foreach (var entry in entries)
		//	{
		//		var path = entry.Key;
		//		var p = path.Replace('/', '\\');
		//		if (p.StartsWith("\\"))
		//		{
		//			p = p.Substring(1);
		//		}
		//		var exist = string.Equals(p, key, StringComparison.OrdinalIgnoreCase);
		//		if (exist)
		//		{
		//			file = entry;
		//			return true;
		//		}
		//	}
		//	file = null;
		//	return false;
		//}

		#endregion

		#region Remove from zip
		public void RemoveFilesToZip(Predicate<IArchiveEntry> whereClause)
		{
			using (var writeableArchive = GetWritableArchive())
			{
				var matches = writeableArchive.Entries.Where(whereClause.Invoke);
				foreach (var file in writeableArchive.Entries.Where(whereClause.Invoke))
				{
					writeableArchive.RemoveEntry(file);
				}
				using var fileStream = GetFileStream(FileOption.Overwrite).fileStream;
				writeableArchive.SaveTo(fileStream,GetDefaultWriterOptions());
			}
		}
#endregion

		#region Add To Zip

		public void Add(string fileFullPath, FileOption option = FileOption.Overwrite, WriterOptions writerOptions = null)
		{
			Add(new List<string>() { fileFullPath }, option, writerOptions);
		}

		public void Add(FileObject fileObject, FileOption option = FileOption.Overwrite, WriterOptions writerOptions = null)
		{
			Add(new List<FileObject>() { fileObject }, option, writerOptions);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileFullPaths">new files to be added to the zip file </param>
		/// <param name="option">how to handle files that will be added to the zip</param>
		/// <param name="writerOptions"></param>
		public void Add(IEnumerable<string> fileFullPaths, FileOption option = FileOption.Overwrite, WriterOptions writerOptions = null)
		{
			Add(fileFullPaths.Select(s => new FileObject(s)), option, writerOptions);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="files">new files to be added to the zip file </param>
		/// <param name="option">how to handle files that will be added to the zip</param>
		/// <param name="writerOptions"></param>
		public void Add(IEnumerable<FileObject> files, FileOption option = FileOption.Overwrite, WriterOptions writerOptions = null)
		{
			var existingFiles = files.Where(f => f.Exist).AsList();
			if (existingFiles.Count <= 0)
				return;

			using (var writableArchive = GetWritableArchive())
			{

				foreach (var file in files)
				{
					var fileStream = file.GetFileStream(FileOption.ReadOnly).fileStream;
					var (entryExist, archive, archiveEntry) = TryGetFile(file.Name);
					using (archive)
					{
						switch (option)
						{
							case FileOption.Append:
								if (entryExist)
								{
									var existingStream = archiveEntry.OpenEntryStream();
									fileStream.CopyTo(existingStream);
								}
								else
								{
									writableArchive.AddEntry(file.Name, fileStream, true, file.SizeInBytes.Value,
										file.LastWriteTime);
								}

								break;
							case FileOption.Overwrite:
								if (entryExist)
								{
									writableArchive.RemoveEntry(archiveEntry);
								}
								else
								{

								

									writableArchive.AddEntry(file.Name, fileStream, true, file.SizeInBytes.Value,
										file.LastWriteTime);
								}

								break;
							case FileOption.DoNothingIfExist:
								if (!ContainsFile(file.Name))
									writableArchive.AddEntry(file.Name, fileStream, true);
								break;
							case FileOption.IncrementFileNameIfExist:
							case FileOption.IncrementFileExtensionIfExist:
								throw new NotImplementedException(
									"Increment Support will implemented in future release");
							default:
								throw new ArgumentOutOfRangeException(nameof(option), option, null);
						}
					}
				}
				var options = writerOptions ?? GetDefaultWriterOptions();
				options.LeaveStreamOpen = false;

				if (Exist)
					DeleteFile(false);

				writableArchive.SaveTo(FullName, options);
			}


			
			
		}




		#endregion



	}




}