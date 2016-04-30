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
			//For the sake of modularity, create the instance from the string.
			newSeg = (DataSegment)System.Reflection.Assembly.GetExecutingAssembly().CreateInstance("CityParser2000.Segments." + segmentName);

			if (newSeg == null)
			{
				Debug.WriteLine("WARNING: Parsing segment as generic:");
				newSeg = new DataSegment(segmentName);
			}

			newSeg.ParseSegment(stream);
			Debug.WriteLine("Parsed " + segmentName + " segment.");
			return newSeg;
		}
	}
}
