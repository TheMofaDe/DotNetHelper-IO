using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using DotNetHelper_IO;

namespace DotNetHelper.IO.Performance
{

    [SimpleJob(RuntimeMoniker.Net461)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class FileObjectPerformance
    {
     
        FileObject TestFile = new FileObject(Path.Combine(Environment.CurrentDirectory,"WriteTest"));

        public FileObjectPerformance()
        {
            TestFile.CreateOrTruncate();
        }



        [Benchmark]
        public void WriteStringToFile()
        {
            //TestFile.WriteContentToFile();
        }
    }

}
