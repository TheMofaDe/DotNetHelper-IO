using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Common;
using SharpCompress.Readers;
using SharpCompress.Writers;

namespace DotNetHelper_IO.Helpers
{
	public static class CompressExtensionHelper
	{
		public static Dictionary<ArchiveType, string> ZipExtensionLookup { get; } = new Dictionary<ArchiveType, string>()
		{
			{ArchiveType.GZip, ".gz"},
			{ArchiveType.Rar, ".rar"},
			{ArchiveType.SevenZip, ".7z"},
			{ArchiveType.Zip, ".zip"},
			{ArchiveType.Tar, ".tar"},
		};


		public static Dictionary<ArchiveType, CompressionType> DefaultCompressionTypeLookup { get; } = new Dictionary<ArchiveType, CompressionType>()
		{
			{ArchiveType.GZip, CompressionType.GZip},
			{ArchiveType.Rar, CompressionType.Rar},
			{ArchiveType.SevenZip, CompressionType.Xz},
			{ArchiveType.Zip,CompressionType.Deflate},
			{ArchiveType.Tar, CompressionType.GZip},
		};


		public static Dictionary<ArchiveType, ReaderOptions> DefaultReaderOptionsLookup { get; } = new Dictionary<ArchiveType, ReaderOptions>()
		{
			{ArchiveType.GZip,new ReaderOptions()},
			{ArchiveType.Rar, new ReaderOptions()},
			{ArchiveType.SevenZip, new ReaderOptions()},
			{ArchiveType.Zip,new ReaderOptions()},
			{ArchiveType.Tar, new ReaderOptions()},
		};

		public static Dictionary<ArchiveType, WriterOptions> DefaultWriterOptionsLookup { get; } = new Dictionary<ArchiveType, WriterOptions>()
		{
			{ArchiveType.GZip,new WriterOptions(DefaultCompressionTypeLookup[ArchiveType.GZip])},
			{ArchiveType.Rar,new WriterOptions(DefaultCompressionTypeLookup[ArchiveType.Rar])},
			{ArchiveType.SevenZip, new WriterOptions(DefaultCompressionTypeLookup[ArchiveType.SevenZip])},
			{ArchiveType.Zip,new WriterOptions(DefaultCompressionTypeLookup[ArchiveType.Zip])},
			{ArchiveType.Tar,new WriterOptions(DefaultCompressionTypeLookup[ArchiveType.Tar])},
		};
	}
}