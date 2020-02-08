using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DotNetHelper_IO;

namespace DotNetHelper_IO_Tests
{
    public class BaseTest
    {


        public static FolderObject BaseFolder { get; } = new FolderObject($"{Path.Combine(Environment.CurrentDirectory, "UnitTests")}");

#if NETCORE31
        public string WorkingDirectory { get; }  = Path.Combine(BaseFolder.FullFolderPath,$"NETCORE_31"); 
#elif NET452
        public string WorkingDirectory { get; } = Path.Combine(BaseFolder.FullFolderPath, $"NET_452");
#endif


        public BaseTest()
        {

        }

    }

}
