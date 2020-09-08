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
		public ReaderOptions ReaderOptions { get; set; }
		public WriterOptions WriterOptions { get; set; }
		public ArchiveType Type { get; }


		public ZipFileObject(string fullFilePath, ArchiveType type, string password = null) : base(fullFilePath)
		{
			ReaderOptions = CompressExtensionHelper.DefaultReaderOptionsLookup[type];
			ReaderOptions.Password = password;
			WriterOptions = CompressExtensionHelper.DefaultWriterOptionsLookup[type];

			Type = type;
		}

		public ZipFileObject(FolderObject folderObject, ArchiveType type, string password = null) : base(folderObject.GetParentFolder().FullName + $"{folderObject.Name}{CompressExtensionHelper.ZipExtensionLookup[type]}")
		{
			ReaderOptions = CompressExtensionHelper.DefaultReaderOptionsLookup[type];
			ReaderOptions.Password = password;
			WriterOptions = CompressExtensionHelper.DefaultWriterOptionsLookup[type];
			Type = type;
		}

		public ZipFileObject(string fullFilePath, ArchiveType type, ReaderOptions readerOptions, WriterOptions writerOptions) : base(fullFilePath)
		{
			ReaderOptions = readerOptions ?? CompressExtensionHelper.DefaultReaderOptionsLookup[type];
			WriterOptions = writerOptions ?? CompressExtensionHelper.DefaultWriterOptionsLookup[type];
			Type = type;
		}


		public IWritableArchive GetWritableArchive()
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
		public IArchive GetReadableArchive()
		{
			switch (Type)
			{
				case ArchiveType.Zip:
					return ZipArchive.Open(FileInfo);
				case ArchiveType.Tar:
					return TarArchive.Open(FileInfo);
				case ArchiveType.GZip:
					return GZipArchive.Open(FileInfo);
				case ArchiveType.Rar:
					return RarArchive.Open(FileInfo);
				case ArchiveType.SevenZip:
					return SevenZipArchive.Open(FileInfo);
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}

		public int GetEntriesCount()
		{

			switch (Type)
			{
				case ArchiveType.Rar:
					using (var archive = RarArchive.Open(FullName, ReaderOptions))
						return archive.Entries.Count;
				case ArchiveType.Zip:
					using (var archive = ZipArchive.Open(FullName, ReaderOptions))
						return archive.Entries.Count;
				case ArchiveType.Tar:
					using (var archive = TarArchive.Open(FullName, ReaderOptions))
						return archive.Entries.Count;
				case ArchiveType.SevenZip:
					using (var archive = SevenZipArchive.Open(FullName, ReaderOptions))
						return archive.Entries.Count;
				case ArchiveType.GZip:
					using (var archive = GZipArchive.Open(FullName, ReaderOptions))
						return archive.Entries.Count;
				default:
					throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
			}
		}





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
				using var fs = GetFileStream(FileOption.Overwrite).fileStream;
				archive.SaveTo(fs, WriterOptions);
			}
			return true;
		}



		public void ExtractToDirectory(string fullFolderPath, ExtractionOptions extractionOptions = null)
		{
			GetReadableArchive().WriteToDirectory(fullFolderPath, extractionOptions ?? new ExtractionOptions());
		}

		public void Compress(FolderObject folderObject, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			Compress(folderObject.FullName, searchPattern, searchOption);
		}
		public void Compress(string fullFolderPathToCompress, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			GetWritableArchive().AddAllFromDirectory(fullFolderPathToCompress, searchPattern, searchOption);
		}



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
				writeableArchive.SaveTo(fileStream, WriterOptions);
			}
		}
		#endregion

		#region Add To Zip


		public void AddFromDirectory(string folderFullPath, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			using (var archive = GetWritableArchive())
			{
				archive.AddAllFromDirectory(folderFullPath, searchPattern, searchOption);
				archive.SaveTo(FullName, WriterOptions);
			}

		}

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
			(bool entryExist, IArchiveEntry entry) EntryExist(IArchive archive, string key)
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
						return (true, entry);
					}
				}

				return (false, null);
			};

			var existingFiles = files.Where(f => f.Exist).AsList();
			if (existingFiles.Count <= 0)
				return;


			using (var writableArchive = Exist ? this.CopyContentToNewWritableArchive() : GetWritableArchive())
			{
				foreach (var file in files)
				{
					var fileStream = file.GetFileStream(FileOption.ReadOnly).fileStream;
					var (entryExist, existingEntry) = EntryExist(writableArchive, file.Name);
					switch (option)
					{
						case FileOption.Append:
							if (entryExist)
							{
								var existingStream = existingEntry.OpenEntryStream();
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
								writableArchive.RemoveEntry(existingEntry);
							}
							else
							{
								writableArchive.AddEntry(file.Name, fileStream, true, file.SizeInBytes.Value, file.LastWriteTime);
							}

							break;
						case FileOption.DoNothingIfExist:
							if (!entryExist)
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
				if (Exist)
					DeleteFile(false);

				writableArchive.SaveTo(FullName, WriterOptions);
			}
		}




		#endregion



	}




}