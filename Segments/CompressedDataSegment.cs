using System;
using System.Collections.Generic;
using System.Text;

namespace CityParser2000.Segments
{
	/// <summary>
	/// Represents a generic <seealso cref="DataSegment"/> that contains data compressed using a run-length algorithm.
	/// </summary>
	public class CompressedDataSegment : DataSegment
	{
		/// <summary>
		/// The size of data (in bytes) after going through the uncompression algorithm.
		/// </summary>
		public int UncompressedLength { get; private set; }

		protected CompressedDataSegment(string s) : base(s) { }

		/// <summary>
		/// When a segment's data is compressed, it follows the following run-length algorithm:
		/// If a byte of data has a value of (int)1 thru (int) 127, then the following x bytes are uncompressed.
		/// If a byte of data has a value of (int)129 thru (int) 255, then the following byte is repeated 127 less times.
		/// </summary>
		internal override void ParseSegment(System.IO.FileStream file)
		{
			Length = file.Read4ByteInt();
			RawDataFileOffset = (int)file.Position;

			List<byte> bytes = new List<byte>();
			while (file.Position < RawDataFileOffset + Length)
			{
				byte b = (byte)file.ReadByte();

				if (b > 0 && b < 128)
				{
					for (int i = 0; i < b; ++i)
					{
						bytes.Add((byte)file.ReadByte());
					}
				}
				else if (b > 128)
				{
					byte nextByte = (byte)file.ReadByte();
					for (int i = 0; i < b - 127; ++i)
					{
						bytes.Add(nextByte);
					}
				}
				else
				{
					throw new Exception("Bad compressed data byte encountered at file offset " + file.Position);
				}
			}

			UncompressedLength = bytes.Count;
			Data = bytes.ToArray();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(base.ToString());
			sb.AppendFormat("Size: {0} bytes, Uncompressed size: {1} bytes\n", Length, UncompressedLength);
			return sb.ToString();
		}
	}
}
