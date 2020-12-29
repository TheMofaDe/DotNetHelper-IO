using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetHelper_IO.Enum
{


	/// <summary>
	/// Enum FileOption
	/// </summary>
	public enum FolderOption
	{
		// IncrementFolderNameIfExist = 1,
		/// <summary>
		/// Will not delete any files or subfolders but when copying or moving if a file already exist with the same name it will get overwritten
		/// </summary>
		OverwriteFilesIfExist = 2,
		/// <summary>
		/// Do absolutely nothing if the specified folder already exist. Nothing will get copy or moved even if subdirectories doesn't exist 
		/// </summary>
		DoNothingIfExist = 3,
		/// <summary>
		/// Do absolutely nothing if the folder already exist
		/// </summary>
		DoNothingIfFileExist = 4,
		/// <summary>
		/// Delete the specified path recursively and then perform the copy or move operation
		/// </summary>
		DeleteThenWrite = 5

	}


	/// <summary>
	/// Enum FileOption
	/// </summary>
	public enum FileOption
	{
		/// <summary>
		/// The append
		/// </summary>
		Append = 1,
		/// <summary>
		/// The overwrite
		/// </summary>
		Overwrite = 2,
		/// <summary>
		/// The do nothing if exist
		/// </summary>
		DoNothingIfExist = 3,
		/// <summary>
		/// Increment the file name +1
		/// </summary>
		IncrementFileNameIfExist = 4,
		/// <summary>
		/// Increments the file extension +1
		/// </summary>
		IncrementFileExtensionIfExist = 5,
		/// <summary>
		/// Read Only Access
		/// </summary>
		ReadOnly = 6
	}

	/// <summary>
	/// Enum AddOrRemoveEnum
	/// </summary>
	public enum AddOrRemoveEnum
	{
		/// <summary>
		/// The add
		/// </summary>
		Add = 1,
		/// <summary>
		/// The remove
		/// </summary>
		Remove = 2
	}

	public enum IOType
	{
		File = 1,
		Folder = 2,
		Both = 3,
	}


}