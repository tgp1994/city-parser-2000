using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	class SegmentFactory
	{
		public static DataSegment ParseSegment(System.IO.FileStream stream)
		{
			DataSegment newSeg;
			string segmentName = stream.ReadString();

			switch (segmentName)
			{
				case "MISC":
					newSeg = new MISC();
					break;
				default:
					Debug.WriteLine("WARNING: Parsing unrecognized segment '" + segmentName + "'.");
					newSeg = new DataSegment(segmentName);
					break;
			}

			newSeg.ParseSegment(stream);
			return newSeg;
		}
	}
}
