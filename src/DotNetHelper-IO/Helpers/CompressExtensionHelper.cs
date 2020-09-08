using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Common;

namespace DotNetHelper_IO.Helpers
{
	public static class CompressExtensionHelper
	{
		public static Dictionary<ArchiveType,string> ExtensionLookup { get; } = new Dictionary<ArchiveType, string>()
		{
			{ArchiveType.GZip, ".gz"},
			{ArchiveType.Rar, ".rar"},
			{ArchiveType.SevenZip, ".7z"},
			{ArchiveType.Zip, ".zip"},
			{ArchiveType.Tar, ".tar"},
		};
	}
}
