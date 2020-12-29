using System;
using System.IO;
using System.Threading;
using Bogus;
using DotNetHelper_IO;

namespace DotNetHelper.IO.Tests
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

		public string RandomAlphaString => new Randomizer().String(10, 'A', 'Z');

		public FileObject RandomTestFileNoExtension
		{
			get
			{
				Thread.Sleep(1);
				return new FileObject(Path.Combine(WorkingDirectory, RandomAlphaString));
			}
		}

		public FileObject RandomTestFileWithExtension
		{
			get
			{
				Thread.Sleep(1);
				return new FileObject(Path.Combine(WorkingDirectory, RandomAlphaString + ".txt"));
			}
		}

		public BaseTest()
		{

		}

	}

}