using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	/// <summary>
	/// The MISC segment contains miscellaneous information pertaining to the city. It is 4800 bytes compressed, and
	/// is typically the second segment after CNAM. Indexed from 0 to 1199.
	/// </summary>
	public class MISC : CompressedDataSegment
	{
		#region Properties and Fields
		public int Rotation { get {	return DataList[2]; } }
		public int FoundingYear { get { return DataList[3]; } }
		public int ElapsedDays { get { return DataList[4]; } } //Note: All months are 25 days
		public int Money { get { return DataList[5]; } }
		public int SimNationPopulation { get { return DataList[20]; } }
		public int SeaLevel { get { return DataList[912]; } }
		#endregion

		#region Variables
		List<int> DataList = new List<int>();
		#endregion

		public MISC() : base("MISC") { }

		internal override void ParseSegment(FileStream file)
		{
			base.ParseSegment(file);

			// RawData now has the uncompressed bytes, lets get them.
			using (MemoryStream ms = new MemoryStream(Data))
			{
				while (ms.Position < ms.Length)
				{
					DataList.Add(ms.Read4ByteInt());
				}
			}
		}


		public string ToString(bool nonZeroOnly)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(base.ToString());

			for (int i = 0; i < DataList.Count; ++i)
			{
				if (nonZeroOnly && DataList[i] != 0)
					sb.AppendLine(string.Format("{0}\t{1}", i, DataList[i]));
			}

			return sb.ToString();
		}
	}
}
