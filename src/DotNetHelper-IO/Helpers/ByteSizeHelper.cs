using System;
using DotNetHelper_IO.Enum;

namespace DotNetHelper_IO.Helpers
{
	public static class ByteSizeHelper
	{


        public static string GetSize(long bytes)
        {
            if (bytes < 1024)
            {
                return $"{bytes}B";
            }
            string[] unit = { "KB", "MB", "GB", "TB", "PB" };
            const int filter = 1024;
            long unitsize = 1;
            var flag = true;
            decimal? size = bytes;
            var index = -1;
            while (flag)
            {
                size /= filter;
                unitsize *= filter;
                flag = size > filter;
                index++;
                if (index >= unit.Length - 1)
                    flag = false;
            }
            return $"{size:f2}{unit[index]}";
        }


        /// <summary>
        /// Gets the file size display.
        /// </summary>
        /// <returns>System.String.</returns>
        public static long? GetSize(long bytes, SizeUnits sizeUnits)
        {  

            if (bytes == 0)
                return 0;
            const int filter = 1024;
            if (sizeUnits == SizeUnits.Byte)
                return bytes;

            var limit = (int)sizeUnits;
            var value = bytes;
            while (limit > 0)
            {
                limit--;
                value /= filter;
            }
            return value;
        }

    }
}
