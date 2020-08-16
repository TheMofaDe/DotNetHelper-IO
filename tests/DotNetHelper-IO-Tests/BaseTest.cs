using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using DotNetHelper_IO;

namespace DotNetHelper_IO_Tests
{
	public class BaseTest
	{


		public static string BaseFolder { get; } = $"{Path.Combine(AppContext.BaseDirectory, "UnitTests")}";

#if NETCOREAPP
        public string WorkingDirectory { get; } = Path.Combine(BaseFolder, $"NETCORE_31");
#elif NET452
        public string WorkingDirectory { get; } = Path.Combine(BaseFolder, $"NET_452");
#endif


		public static string BaseFolderRelative { get; } = $"{Path.Combine("./", "UnitTests")}";

#if NETCOREAPP
        public string WorkingDirectoryRelative { get; } = Path.Combine(BaseFolderRelative, $"NETCORE_31");
#elif NET452
        public string WorkingDirectoryRelative { get; } = Path.Combine(BaseFolderRelative, $"NET_452");
#endif

		public FileObject RandomTestFile
		{
			get
			{
				Thread.Sleep(1);
				return new FileObject(Path.Combine(WorkingDirectory, new Randomizer().String(10, 'A', 'Z')));
			}
		}

		public FileObject RandomTestFileWithExtension
		{
			get
			{
				Thread.Sleep(1);
				return new FileObject(Path.Combine(WorkingDirectory, new Randomizer().String(10, 'A', 'Z') + ".txt"));
			}
		}

		public BaseTest()
		{

		}

	}

}