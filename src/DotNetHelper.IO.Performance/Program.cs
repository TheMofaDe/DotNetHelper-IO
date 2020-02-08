using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace DotNetHelper.IO.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //   var summary = BenchmarkRunner.Run<Md5VsSha256>();
            Console.ReadLine();
        }
    }



}
