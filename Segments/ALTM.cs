using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	public class ALTM : DataSegment
	{
		public AltitudeDescriptor[,] AltitudeData { get; set; }

		public ALTM() : base("ALTM")
		{
			AltitudeData = new AltitudeDescriptor[CityMap.TILES_PER_SIDE, CityMap.TILES_PER_SIDE];
		}

		internal override void ParseSegment(FileStream file)
		{
			base.ParseSegment(file);
			if (RawData.Length != Math.Pow(CityMap.TILES_PER_SIDE, 2) * 2)
				throw new Exception("ALTM data length incorrect.");

			using (MemoryStream ms = new MemoryStream(RawData))
			{
				int i = 0;
				while (ms.Position < ms.Length)
				{
					AltitudeData[i / 128, i % 127] = new AltitudeDescriptor(ms.Read2ByteInt());
				}
			}
		}

		/// <summary>
		/// Default method that prints every tile contained in the AltitudeData array.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(0, 127, 0, 127);
		}

		/// <summary>
		/// Prints out information about the specified range of tiles.
		/// </summary>
		/// <returns></returns>
		public string ToString(int x_min, int x_max, int y_min, int y_max)
		{
			StringBuilder sb = new StringBuilder();
			AltitudeDescriptor ad;
			sb.Append(base.ToString());

			for (int i = x_min; i <= x_max; i++)
			{
				for (int j = y_min; j <= y_max; j++)
				{
					ad = AltitudeData[i, j];
					if (j % 5 == 0)
						sb.AppendLine();
					// Catch the case where the data is not filled yet.
					sb.AppendFormat("[{0}, {1}]\t{2}\t", i, j, (ad != null ? ad.ToString() : "Null\t\t\t"));
				}
				sb.AppendLine();
			}

			return sb.ToString();
		}
	}

	/// <summary>
	/// Describes a single title in the context of an altitude map.
	/// </summary>
	public class AltitudeDescriptor
	{
		/// <summary>
		/// Number 0-31 representing altitude of the tile.
		/// </summary>
		public int Altitude { get; private set; }

		/// <summary>
		/// Returns the Altitude value converted into feet. Lowest value is 50 feet.
		/// </summary>
		public int AltInFeet { get { return (Altitude * 100) + 50; } }

		/// <summary>
		/// Represents bit 7 which theoretically indicates whether or not a tile is covered in water. This is
		/// disputed and may change.
		/// </summary>
		public bool WaterCovered { get; private set; }
		public int RawValue { get; private set; }

		/// <summary>
		/// Extracts the tile data from the two byte (16 bit) integer.
		/// </summary>
		/// <param name="rawValue"></param>
		public AltitudeDescriptor(int rawValue)
		{
			RawValue = rawValue;

			Altitude = rawValue & 0x1F; // Look at bits 4-0
			WaterCovered = (rawValue & 0x80) != 0; // Is bit 7 set?

			// Some debugging for unknown bits
			if ((rawValue & 0xFF60) != 0)
				System.Diagnostics.Debug.WriteLine(String.Format("Myster bits set in altitude data: {0:X}", rawValue));
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<0x{0:X}> Height: {1:D}ft. Water: " + WaterCovered.ToString(), RawValue, AltInFeet);
			return sb.ToString();
		}
	}
}
