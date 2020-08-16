using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using DotNetHelper_IO.Enum;
using DotNetHelper_IO.Extension;
using DotNetHelper_IO.Helpers;
using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace DotNetHelper_IO
{

	/// <summary>
	/// Class FolderObject.
	/// </summary>
	public abstract class PathObject
	{

		/// <summary>
		/// if object is file then get the name of the file with extension
		/// if object is folder then get the folder name only
		/// </summary>
		/// <value></value>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the full path.
		/// </summary>
		/// <value></value>
		public string FullName { get; } = null;

		public PathType PathType { get; }

		/// <summary>
		/// return the size of the object in bytes
		/// </summary>
		public long? Size => GetSize(SizeUnits.Byte);

		/// <summary>
		/// return whether path exist.
		/// </summary>
		/// <value><c>null</c> if [exist] contains no value, <c>true</c> if [exist]; otherwise, <c>false</c>.</value>
		public bool Exist => Exists();

		protected PathObject(PathType pathType, string fullName)
		{
			PathType = pathType;
			FullName = fullName;
		}

		public abstract string GetSize();
		public abstract long? GetSize(SizeUnits sizeUnits);
		public abstract FolderObject GetParentFolder();
		internal abstract bool Exists();


	}



}