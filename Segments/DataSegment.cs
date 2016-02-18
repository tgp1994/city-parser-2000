using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	/// <summary>
	/// Provide a generic guideline for Interchange File Format segments. Every segment has a type and positive length.
	/// </summary>
	abstract class DataSegment
	{
		/// <summary>
		/// An identifier of the segment, always four bytes (characters) long.
		/// </summary>
		public string Type { get; internal set; }

		/// <summary>
		/// The number of bytes of segment data. Note that it may compressed or uncompressed, and does not include the
		/// 8 byte header.
		/// </summary>
		public int Length { get; internal set; }

		/// <summary>
		/// The segment bytes as they're read in from the city file.
		/// </summary>
		public byte[] RawData { get; internal set; }

		/// <summary>
		/// Where in the city file this data was found.
		/// </summary>
		public int RawDataFileOffset { get; internal set; }

		/// <summary>
		/// Create the DataSegment object based off of the 8 byte segment header.
		/// </summary>
		/// <param name="type"><see cref="DataSegment.Type"/></param>
		/// <param name="length"><see cref="DataSegment.Length"/></param>
		internal DataSegment(string type, int length)
		{
			Type = type;
			Length = length;
		}
	}
}
