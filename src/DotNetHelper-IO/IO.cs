
// ReSharper disable once InconsistentNaming
using System;
using System.IO;
using System.Text.RegularExpressions;
using DotNetHelper_DeviceInformation;
using DotNetHelper_IO.Enum;

namespace DotNetHelper_IO
{
    internal static class IO

    {

      

        public static string GetFullPathOnly(string file)
        {
            try
            {
                if (string.IsNullOrEmpty(file))
                    throw new Exception("Couldn't Retrieve Full Path Of A Empty Or Null String");
                var path = Path.GetDirectoryName(file);
                if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {

                }
                else
                {
                    path = path + Path.DirectorySeparatorChar;
                }
                return path;
            }
            catch (PathTooLongException) // https://support.microsoft.com/en-us/help/177665/path-too-long-error-message-when-exceeding-max-path
            {
                return (file.Substring(0, 200)); 

            }
        }

        public static string GetFileNameonly(string file)
        {
            return Path.GetFileName(file);
        }
        public static IOType IsStringAFileOrPath(string str)
        {
            // NEED TO TEST 
            var canBeFile = IsValidFilePathSyntax(str).Item1;
            var canBeFolder = IsValidFolderSyntax(str).Item1;

            if (canBeFile && !canBeFolder)
            {
                return IOType.File;
            }
            if (!canBeFile && canBeFolder)
            {
                return IOType.Folder;
            }
            if (canBeFile  && canBeFolder)
            {
                if (File.Exists(str))
                {
                    return IOType.File;
                }
                if (Directory.Exists(str))
                {
                    return IOType.Folder;
                }

                return IOType.Both;

            }

            throw new Exception("Invaild Path Or File");
        }




        public static Tuple<bool, Exception> IsValidFilePathSyntax(string fileFullPath)
        {
           
            var folder = GetFullPathOnly(fileFullPath);
            var file = GetFileNameonly(fileFullPath);
            var bruh = IsValidFolderSyntax(folder);
            if (!bruh.Item1) return new Tuple<bool, Exception>(bruh.Item1, bruh.Item2);

            var chars = Path.GetInvalidFileNameChars();
            var valid = file.IndexOfAny(chars) < 0;
            var error = valid ? null : new Exception($"{fileFullPath} Contains Invalid Characters");


            return new Tuple<bool, Exception>(valid, error);


        }

        public static Tuple<bool, Exception> IsValidFolderSyntax(string path)
        {
            Exception error;
            if (string.IsNullOrEmpty(path) || path.IndexOfAny(Path.GetInvalidPathChars()) != -1 )
            {
                error = new Exception($"The Following Path {path} Is Not Valid For A Folder");
                return new Tuple<bool, Exception>(false, error);
            }

            if (DeviceInformation.DeviceOS == DeviceInformation.DeviceOs.Windows && path.Contains(":")) 
            {
                var driveCheck = new Regex(@"^[a-zA-Z]:\\$");
                if (!driveCheck.IsMatch(path.Substring(0, 3)))
                    return new Tuple<bool, Exception>(false,
                        new Exception($"The Following Path {path} Is Not Valid For A Folder"));
                var strTheseAreInvalidFileNameChars = new string(Path.GetInvalidPathChars());
                strTheseAreInvalidFileNameChars += @":/?*" + "\"";
                var containsABadCharacter = new Regex("[" + Regex.Escape(strTheseAreInvalidFileNameChars) + "]");
                if (containsABadCharacter.IsMatch(path.Substring(3, path.Length - 3)))
                {
                    error = new Exception($"The Following Path {path} Is Not Valid For A Folder");
                    return new Tuple<bool, Exception>(false, error);
                }
            }

            return new Tuple<bool, Exception>(true, null);
        }

    }
}



