using System;
using DotNetHelper_IO;

namespace DotNetHelper_IO_Console
{
    class Program
    {
        static void Main(string[] args)
        {
       
            var folderObject = new FolderObject("./",false,true);
            var folderObject2 = new FolderObject(Environment.CurrentDirectory,false,true);


            Console.WriteLine($"Test Result {folderObject2.Files.Count == folderObject.Files.Count}");
            Console.WriteLine("Hello World!");

            Console.ReadKey();
        }
    }
}
