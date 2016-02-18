using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CityParser2000.Utility
{
	static class StreamUtils
	{
		//TODO: Needs unit testing.
		/// <summary>
		/// Read in 4 bytes as a 32 bit integer.
		/// </summary>
		/// <param name="stream">Active stream ready to be read.</param>
		/// <returns>Integer derived from the 4 bytes.</returns>
		public static int Read4ByteInt(this Stream stream)
		{
			byte[] buf = new byte[4];
			stream.Read(buf, 0, 4);

			return BitConverter.ToInt32(buf, 0);
		}
	}
}
