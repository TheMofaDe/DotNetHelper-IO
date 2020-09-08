using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Archives;

namespace DotNetHelper_IO.Extension
{
	public static class SharpCompressExtension
	{
		/// <summary>
		/// I couldn't find a way to modify the existing archive using ZipArchive.Open so were just going to create a new instance and copy the existing content to it
		/// </summary>
		/// <param name="zipFileObject"></param>
		/// <returns></returns>
		internal static IWritableArchive CopyContentToNewWritableArchive(this ZipFileObject zipFileObject)
		{
			var newArchive = zipFileObject.GetWritableArchive();
			using (var currentArchive = zipFileObject.GetReadableArchive())
			{
				using (currentArchive)
				{
					foreach (var entry in currentArchive.Entries)
					{
						var memoryStream = new MemoryStream();
						entry.WriteTo(memoryStream);
						newArchive.AddEntry(entry.Key, memoryStream, true, entry.Size, entry.LastModifiedTime);
					}
				}
			}
			return newArchive;
		}


	}
}