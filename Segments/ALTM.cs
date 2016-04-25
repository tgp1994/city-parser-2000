using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CityParser2000.Segments
{
	class ALTM : DataSegment
	{
		public AltitudeDescriptor[,] AltitudeData { get; set; }

		internal ALTM() : base("ALTM")
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

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(base.ToString());

			for (int i = 0; i < 128; i++)
			{
				for (int j = 0; j < 128; j++)
				{
					AltitudeDescriptor ad = AltitudeData[i, j];
					sb.AppendFormat("[{0}, {1}]\t{2}", i, j, ad.ToString());
				}
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
		/// Number 0-31 representing altitude in feet, (x * 100) + 50. Lowest possible altitude is 50 ft.
		/// </summary>
		public int Altitude { get; private set; }

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
				System.Diagnostics.Debug.WriteLine(String.Format("Myster bits were set in altitude data, value: {0:X}", rawValue));
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("Height: {D:0}\tWater: {1}", Altitude, WaterCovered.ToString());
			return sb.ToString();
		}
	}
}
