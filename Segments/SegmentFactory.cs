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
				case "CNAM":
					newSeg = new CNAM();
					break;
				case "MISC":
					newSeg = new MISC();
					break;
				default:
					Debug.WriteLine("WARNING: Parsing segment as generic:");
					newSeg = new DataSegment(segmentName);
					break;
			}

			newSeg.ParseSegment(stream);
			Debug.WriteLine("Parsed " + segmentName + " segment.");
			return newSeg;
		}
	}
}
