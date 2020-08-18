using System;
using System.Dynamic;
using System.IO;
using System.Text;

using DotNetHelper_IO;
using DotNetHelper_IO.Enum;

namespace DotNetHelper_IO_Console
{
	class Program
	{
		static void Main(string[] args)
		{
			///var sampleFile = @"C:\Temp\dotnet-hosting-2.2.1-win.exe";


			var file = new FileObject($@"C:\Temp\MyTestFile.txt");
			// Copies & Append file content to specified path
			file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.Append);

			// Copies file content and paste it to specified path and will overwrite if other file already exist
			file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.Overwrite);

			// Copies file content to specified path only if it doesn't exist otherwise do nothing
			file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.DoNothingIfExist);

			// Copy file content to specified path. If path already exist then create a new file with the file extension increment.
			var newFileName = file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.IncrementFileExtensionIfExist);

			// Copy file content to specified path. If path already exist then create a new file with the file name increment.
			var newFileName2 = file.CopyTo("D:\\Temp\\MyTestFile.txt", FileOption.IncrementFileNameIfExist);



			//var file = new FileObject($@"C:\Temp\MyTestFile.txt"); // special 
			//file.WriteAsync("Will create MyTestFile.txt with this content", FileOption.Append); // overload for stream & bytes exist as well
			//file.WriteAsync("will ovewrite the file MyTestFile.txt with this text", FileOption.Overwrite); // overload for stream & bytes exist as well
			//file.WriteAsync("nothing will happen since file already exist", FileOption.DoNothingIfExist); // overload for stream & bytes exist as well
			//var newFileName = file.WriteAsync("will create file MyTestFile.txt1 and this method returns file name", FileOption.IncrementFileExtensionIfExist); // overload for stream & bytes exist as well
			//var newFileName2 = file.WriteAsync("will create file MyTestFile1.txt and this method returns file name ", FileOption.IncrementFileNameIfExist); // overload for stream & bytes exist as well









		}


	}
}