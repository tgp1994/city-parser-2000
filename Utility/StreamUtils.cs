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
		/// SC2-IFF format files are stored Big Endian, yet most (Intel-architecture-based) processors run L.E. Thus
		/// it is necessary to have a method for converting the save file's values to the host processor's endianness.
		/// </summary>
		/// <param name="buffer">A buffer of bytes that may need its endianness changed.</param>
		private static void CorrectEndianness(ref byte[] buffer)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(buffer);
			}
		}

		public static int Read2ByteInt(this Stream stream)
		{
			byte[] buf = new byte[2];
			stream.Read(buf, 0, 2);
			CorrectEndianness(ref buf);

			return BitConverter.ToInt16(buf, 0);
		}

		/// <summary>
		/// An extension method to read in 4 bytes as a 32 bit integer.
		/// </summary>
		/// <param name="stream">Active stream ready to be read.</param>
		/// <returns>Integer derived from the 4 bytes.</returns>
		public static int Read4ByteInt(this Stream stream)
		{
			byte[] buf = new byte[4];
			stream.Read(buf, 0, 4);
			CorrectEndianness(ref buf);

			return BitConverter.ToInt32(buf, 0);
		}

		/// <summary>
		/// An easy shortcut method to read 4 bytes of a stream as a string.
		/// </summary>
		/// <param name="stream">A stream ready for reading.</param>
		/// <returns>An ASCII-encoded string from the bytes.</returns>
		public static string ReadString(this Stream stream)
		{
			byte[] buf = new byte[4];
			stream.Read(buf, 0, 4);
			return Encoding.ASCII.GetString(buf);
		}
	}
}
