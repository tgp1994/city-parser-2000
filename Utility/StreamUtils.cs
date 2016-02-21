using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CityParser2000
{
	public static class StreamUtils
	{
		/// <summary>
		/// An extension method to read in 4 bytes as a 32 bit integer.
		/// </summary>
		/// <param name="stream">Active stream ready to be read.</param>
		/// <returns>Integer derived from the 4 bytes.</returns>
		public static int Read4ByteInt(this Stream stream)
		{
			byte[] buf = new byte[4];
			stream.Read(buf, 0, 4);
			//SC2-IFF format files are stored B.E, yet most (Intel-architecture-based) processors run L.E.
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buf);
			}

			return BitConverter.ToInt32(buf, 0);
		}
	}
}
