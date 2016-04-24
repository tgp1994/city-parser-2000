using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	/// <summary>
	/// This is just a 32 byte string representing the name of the city. This segment is also optional.
	/// </summary>
	class CNAM : DataSegment
	{
		public string cityName;

		public CNAM() : base("CNAM") { }

		internal override void ParseSegment(FileStream file)
		{
			base.ParseSegment(file);

			cityName = Encoding.ASCII.GetString(RawData);
		}
	}
}
