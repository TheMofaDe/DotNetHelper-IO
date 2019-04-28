﻿using System;
using System.Text;
using DotNetHelper_Contracts.Enum.Encryption;
using DotNetHelper_Contracts.Enum.IO;
using DotNetHelper_IO;

namespace DotNetHelper_IO_Console
{
    class Program
    {
        static void Main(string[] args)
        {   
            var folderObject = new FolderObject("./",false,true);

            var content = $"PasswordPasswordPasswordPassword";
            var newFile = new FileObject($"{folderObject.FullFolderPath}Encrypt");
            newFile.WriteContentToFile(content, Encoding.UTF8, FileOption.Overwrite);
            newFile.EncryptFile(SymmetricProvider.AES, Encoding.UTF8.GetBytes(content));
            var readValue = newFile.ReadFile();
            newFile.DecryptFile(SymmetricProvider.AES, Encoding.UTF8.GetBytes(content));
            Console.ReadKey();
        }
    }
}