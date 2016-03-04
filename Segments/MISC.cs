using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	/// <summary>
	/// The MISC segment contains miscellaneous information pertaining to the city. It is 4800 bytes compressed, and
	/// is typically the second segment after CNAM.
	/// </summary>
	class MISC : CompressedDataSegment
	{
		List<int> Data = new List<int>();

		public MISC() : base("MISC") { }


	}
}
