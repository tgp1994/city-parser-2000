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
	class MISC : CompressedDataSegment
	{
		List<int> Data = new List<int>();

		public MISC() : base("MISC") { }

		#region Properties
		public int Rotation { get {	return Data[2]; } }
		public int FoundingYear { get { return Data[3]; } }
		public int ElapsedDays { get { return Data[4]; } } //Note: All months are 25 days
		public int Money { get { return Data[5]; } }
		public int SimNationPopulation { get { return Data[20]; } }
		public int SeaLevel { get { return Data[912]; } }
		#endregion

		internal override void ParseSection(FileStream file)
		{
			base.ParseSection(file);

			// RawData now has the uncompressed bytes, lets get them.
			using (MemoryStream ms = new MemoryStream(RawData))
			{
				while (ms.Position < ms.Length)
				{
					Data.Add(ms.Read4ByteInt());
				}
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(base.ToString());

			for (int i = 0; i < Data.Count; ++i)
			{
				sb.AppendLine(string.Format("{0}\t{1}", i, Data[i]));
			}

			return sb.ToString();
		}
	}
}
