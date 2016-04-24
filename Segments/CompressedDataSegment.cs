using System;
using System.Collections.Generic;

namespace CityParser2000.Segments
{
	/// <summary>
	/// When a segment's data is compressed, it follows the following run-length algorithm:
	/// If a byte of data has a value of (int)1 thru (int) 127, then the following x bytes are uncompressed.
	/// If a byte of data has a value of (int)129 thru (int) 255, then the following byte is repeated 127 less times.
	/// </summary>
	public class CompressedDataSegment : DataSegment
	{
		protected CompressedDataSegment(string s) : base(s) { }
	
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

			RawData = bytes.ToArray();
		}
	}
}
