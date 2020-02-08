using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetHelper_IO.Extension
{
    internal static class StreamExtensions
    {
        public static async Task CopyAndFlushAsync(this Stream source, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default, int bufferSize = 4096)
        {
            await source.CopyToAsync(destination, progress, cancellationToken, bufferSize);
            await destination.FlushAsync(cancellationToken);
        }

        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default, int bufferSize = 4096)
        {
            var buffer = new byte[bufferSize];
            var bytesRead = 0;
            long bytesTotal = 0;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                bytesTotal += bytesRead;
                progress?.Report(bytesTotal);
            }
            await destination.FlushAsync(cancellationToken);
        }
    }
}
