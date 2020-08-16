using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DotNetHelper.IO.Tests.Extensions
{
	public static class StringExtensions
	{
		public static byte[] ToByteArray(this string s) => Encoding.Default.GetBytes(s); //.ToByteSpan().ToArray()); //  heap allocation, use only when you cannot operate on spans
		public static ReadOnlySpan<byte> ToByteSpan(this string s) => MemoryMarshal.Cast<char, byte>(s);


		public static Stream ToStream(this string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return stream;
		}
	}
}