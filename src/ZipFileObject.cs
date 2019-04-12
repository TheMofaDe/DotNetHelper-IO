using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotNetHelper_Contracts.Enum.IO;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace DotNetHelper_IO
{

    public class ZipFileObject : FileObject
    {
        public ArchiveType Type { get; }

        public ZipFileObject(string file, ArchiveType type = ArchiveType.Zip) : base(file)
        {
            Type = type;
        }


        /// <summary>
        /// Creates a empty file if it doesn't exist otherwise truncates it if set to <c>true</c> [overwrite existing files].
        /// </summary>
        /// <param name="compressionType"></param>
        /// <param name="truncate">if set to <c>true</c> [truncate].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public new bool CreateOrTruncate(bool truncate = true)
        {
            RefreshObject();

            if (truncate)
            {
                DeleteFile(e => throw e);
            }
            else
            {
                if (Exist == true) return true;
            }
            // HAVE TO CHECK IF DIRECTORY EXIST FIRST BEFORE THIS
            if (!Directory.Exists(FilePathOnly))
                Directory.CreateDirectory(FilePathOnly);
            using (var archive = ZipArchive.Create())
            {
                using (var fs = GetFileStream(FileOption.Overwrite))
                {
                    archive.SaveTo(fs);
                }
            }
            RefreshObject();
            return true;

        }


        public void UnZipFile(FolderObject folder, FileOption option, bool extractFullPath = false)
        {
            bool overWrite = option != FileOption.DoNothingIfExist;
            if(option == FileOption.Append) throw new NotSupportedException("Append Option For Unzip files has not been implemented yet");
            if (option == FileOption.IncrementFileExtensionIfExist) throw new NotSupportedException("IncrementFileExtensionIfExist Option For Unzip files has not been implemented yet");
            if (option == FileOption.IncrementFileNameIfExist) throw new NotSupportedException("IncrementFileNameIfExist Option For Unzip files has not been implemented yet");
            folder.Create(e => throw e ); 
            using (var stream = File.OpenRead(FullFilePath))
            using (var reader = ReaderFactory.Open(stream))
            {
                while (reader.MoveToNextEntry())
                {
                    // if (!reader.Entry.IsDirectory)
                    // {
                    //    Console.WriteLine(reader.Entry.Key);
                    reader.WriteEntryToDirectory(folder.FullFolderPath, new ExtractionOptions()
                    {
                        ExtractFullPath = extractFullPath,
                        Overwrite = overWrite
                    });
                    //  }
                }
            }
        }


        public void AddFileToZip(string fullFilePath)
        {
            AddFilesToZip(new List<string>() { fullFilePath });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="files">new files to be added to the zip file </param>
        /// <param name="option">how to handle files that will be added to the zip</param>
        public void AddFilesToZip(List<string> files, FileOption option = FileOption.Overwrite)
        {
            var safeFiles = files.Select(s => new FileObject(s, false)).Where(f => f.Exist == true).ToList();
            if (safeFiles.Count <= 0) return;
            if (Exist != true) CreateOrTruncate(false);
            switch (Type)
            {
                case ArchiveType.Zip:
                    AddAllToZip(ZipArchive.Create(), safeFiles, option);
                    break;
                case ArchiveType.Tar:
                    AddAllToZip(TarArchive.Create(), safeFiles, option);
                    break;
                case ArchiveType.GZip:
                    AddAllToZip(GZipArchive.Open(FullFilePath), safeFiles, option);
                    break;
                case ArchiveType.Rar:
                case ArchiveType.SevenZip:
                    throw new NotImplementedException("This Functionality For Rar & 7ZIP files hasn't been implemented yet so feel free to do it yourself if you need it ");
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }
        }


        public void RemoveFilesToZip(Predicate<IArchiveEntry> whereClause)
        {

            switch (Type)
            {
                case ArchiveType.Rar:
                    throw new NotImplementedException("This Feature Hasn't Be Implemented Yet For Rar Files");
                case ArchiveType.Zip:
                    RemoveAllFilesFromZip(ZipArchive.Open(FullFilePath), whereClause);
                    break;
                case ArchiveType.Tar:
                    RemoveAllFilesFromZip(TarArchive.Open(FullFilePath), whereClause);
                    break;
                case ArchiveType.SevenZip:
                    throw new NotImplementedException("This Feature Hasn't Be Implemented Yet For 7Zip Files");
                case ArchiveType.GZip:
                    RemoveAllFilesFromZip(GZipArchive.Open(FullFilePath), whereClause);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }


        }

        /// <summary>
        /// Return
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        //public IEnumerable<CompressedFile> GetFilesInZip()
        //{
        //    RefreshObject();
        //    var list = new List<CompressedFile>() { };
        //    if (Exist != true) return list;

        //    switch (Type)
        //    {
        //        case ArchiveType.Rar:
        //            using (var archive = RarArchive.Open(FullFilePath))
        //            {
        //                list.AddRange(archive.Entries.Select(file => ObjectMapper.MapProperties(file, new CompressedFile(), true, StringComparison.OrdinalIgnoreCase)));
        //            }
        //            break;
        //        case ArchiveType.Zip:
        //            using (var archive = ZipArchive.Open(FullFilePath))
        //            {
        //                list.AddRange(archive.Entries.Select(file => ObjectMapper.MapProperties(file, new CompressedFile(), true, StringComparison.OrdinalIgnoreCase)));
        //            }
        //            break;
        //        case ArchiveType.Tar:
        //            using (var archive = TarArchive.Open(FullFilePath))
        //            {
        //                list.AddRange(archive.Entries.Select(file => ObjectMapper.MapProperties(file, new CompressedFile(), true, StringComparison.OrdinalIgnoreCase)));
        //            }
        //            break;
        //        case ArchiveType.SevenZip:
        //            using (var archive = SevenZipArchive.Open(FullFilePath))
        //            {
        //                list.AddRange(archive.Entries.Select(file => ObjectMapper.MapProperties(file, new CompressedFile(), true, StringComparison.OrdinalIgnoreCase)));
        //            }
        //            break;
        //        case ArchiveType.GZip:
        //            using (var archive = GZipArchive.Open(FullFilePath))
        //            {
        //                list.AddRange(archive.Entries.Select(file => ObjectMapper.MapProperties(file, new CompressedFile(), true, StringComparison.OrdinalIgnoreCase)));
        //            }
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
        //    }
        //    return list;
        //}



        private void RemoveAllFilesFromZip<T1, T2>(AbstractWritableArchive<T1, T2> writableArchive, Predicate<T1> whereClause) where T2 : IVolume where T1 : IArchiveEntry
        {
            using (writableArchive)
            {
                var filesToRemove = writableArchive.Entries.Where(whereClause.Invoke).ToList();
                if (filesToRemove.Count > 0)
                {
                    foreach (var file in filesToRemove)
                    {
                        writableArchive.RemoveEntry(file);
                    }
                    writableArchive.SaveTo(FullFilePath, CompressionType.Deflate);
                }
            }
        }

        private void AddAllToZip<T1, T2>(AbstractWritableArchive<T1, T2> overwriteZipFile, List<FileObject> safeFiles, FileOption option) where T2 : IVolume where T1 : IArchiveEntry
        {
            using (overwriteZipFile)
            {
                using (var stream = File.OpenRead(FullFilePath))
                {
                    using (var reader = ReaderFactory.Open(stream))
                    {
                        while (reader.MoveToNextEntry())
                        {
                            var temp = new MemoryStream();
                            reader.OpenEntryStream().CopyTo(temp);
                            overwriteZipFile.AddEntry(reader.Entry.Key, temp);
                        }
                    }
                }
                safeFiles.ForEach(delegate (FileObject o)
                {
                    var currentEntries = overwriteZipFile.Entries.ToList(); // THIS IS HERE BECAUSE DEVELOPERS MAY SEND THE SAME FILE TWICE 
                    var matchedEntries = currentEntries.Where(a => a.Key == o.FileNameOnly).ToList();
                    switch (option)
                    {
                        case FileOption.Append:
                            if (matchedEntries.Count > 0)
                            {
                                var temp = new MemoryStream();
                                matchedEntries.First().OpenEntryStream().CopyTo(temp);
                                temp.Position = temp.Length;
                                using (var tempFileStream = o.ReadFileToStream())
                                {
                                    tempFileStream.CopyTo(temp);
                                }
                                overwriteZipFile.RemoveEntry(matchedEntries.First());
                                overwriteZipFile.AddEntry(o.FileNameOnly, temp);
                                break;
                            }
                            overwriteZipFile.AddEntry(o.FileNameOnly, o.FullFilePath);
                            break;
                        case FileOption.Overwrite:
                            if (matchedEntries.Count > 0)
                                overwriteZipFile.RemoveEntry(matchedEntries.First());
                            overwriteZipFile.AddEntry(o.FileNameOnly, o.FullFilePath);
                            break;
                        case FileOption.DoNothingIfExist:
                            if (matchedEntries.Count > 0)
                            {
                                break;
                            }
                            overwriteZipFile.AddEntry(o.FileNameOnly, o.FullFilePath);
                            break;
                        case FileOption.IncrementFileNameIfExist:
                            if (matchedEntries.Count > 0)
                            {
                                overwriteZipFile.AddEntry(new FileObject(o.GetIncrementFileName()).FileNameOnly, o.FullFilePath);
                            }
                            break;
                        case FileOption.IncrementFileExtensionIfExist:
                            if (matchedEntries.Count > 0)
                            {
                                overwriteZipFile.AddEntry(new FileObject(o.GetIncrementExtension()).FileNameOnly, o.FullFilePath);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(option), option, null);
                    }

                });
                overwriteZipFile.SaveTo(FullFilePath, new WriterOptions(CompressionType.Deflate));
            }
        }


    }


    public class CompressedFile
    {
        private DateTime? ArchivedTime { get; set; }
        long CompressedSize { get; set; }
        long Crc { get; set; }
        DateTime? CreatedTime { get; set; }
        string Key { get; set; }
        bool IsDirectory { get; set; }
        bool IsEncrypted { get; set; }
        bool IsSplit { get; set; }
        DateTime? LastAccessedTime { get; set; }
        DateTime? LastModifiedTime { get; set; }
        long Size { get; set; }
        int? Attrib { get; set; }
    }

}
