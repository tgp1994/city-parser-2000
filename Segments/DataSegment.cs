﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CityParser2000.Utility;

namespace CityParser2000.Segments
{
	/// <summary>
	/// Provide a generic guideline for Interchange File Format segments. Every segment has a type and positive length.
	/// </summary>
	public class DataSegment
	{
		#region Properties

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
		internal byte[] Data { get; set; }

		/// <summary>
		/// Where in the city file this data was found.
		/// </summary>
		public int RawDataFileOffset { get; internal set; }

		#endregion

		/// <summary>
		/// Create the DataSegment object based off of the 8 byte segment header. Additional work should be done in
		/// separate methods.
		/// </summary>
		/// <param name="type"><see cref="DataSegment.Type"/></param>
		internal DataSegment(string type)
		{
			Type = type;
		}

		#region Methods

		/// <summary>
		/// Perform segment-specific processing on the raw data. At this point the four byte type has already been read
		/// in, so the position is at the four byte length int.
		/// </summary>
		/// <param name="file">The city file that is being read in.</param>
		internal virtual void ParseSegment(FileStream file)
		{
			Length = file.Read4ByteInt();
			RawDataFileOffset = (int)file.Position;
			Data = new byte[Length];
			file.Read(Data, 0, Length);
		}

		/// <summary>
		/// Fills in the appropriate properties of the <see cref="City"/> object after parsing has completed.
		/// </summary>
		/// <param name="city"></param>
		internal virtual void PopulateCity(ref City city)
		{
			// Nothing to do here.
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine("Type:\t" + Type);
			sb.AppendLine("Length:\t" + Length + " bytes");

			return sb.ToString();
		}

		#endregion
	}
}
