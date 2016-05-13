using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CityParser2000.Segments
{
	public class XTER : CompressedDataSegment
	{
		public TerrainDescriptor[,] Terrain { get; private set; }

		public XTER() : base ("XTER")
		{
			Terrain = new TerrainDescriptor[CityMap.TILES_PER_SIDE, CityMap.TILES_PER_SIDE];
		}

		internal override void ParseSegment(FileStream file)
		{
			base.ParseSegment(file);

			int counter = 0;

			using (MemoryStream ms = new MemoryStream(Data))
			{
				TerrainDescriptor td;
				while (ms.Position < ms.Length)
				{
					td = new TerrainDescriptor((byte)ms.ReadByte());
					Terrain[counter % CityMap.TILES_PER_SIDE, counter / (CityMap.TILES_PER_SIDE)] = td;
					counter++;
				}
			}
		}

		/// <summary>
		/// Override default method to print a 4 by 4 square by default.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(4, 4);
		}

		public string ToString(int CountX = 0, int CountY = 0)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(base.ToString());
			sb.AppendFormat("Printing {0} XTER descriptors...\n", (CountX * CountY));

			for (int y = 0; y < CountY; y++)
			{
				for (int x = 0; x < CountX; x++)
				{
					TerrainDescriptor td = Terrain[x, y];
					sb.AppendFormat("[{0}, {1}] {3}\n", x, y, td);
				}
			}

			return sb.ToString();
		}
	}

	public class TerrainDescriptor
	{
		public TerrainType Type { get; private set; }
		public TerrainSlope Slope { get; private set; }
		public byte Data;

		public TerrainDescriptor(byte data)
		{
			byte slopeData = 0;
			Data = data;

			if (data <= 0x0D)
			{
				Type = TerrainType.Dry;
				slopeData = data;
			}
			else if (data >= 0x10 & data <= 0x1D)
			{
				Type = TerrainType.Submerged;
				slopeData = (byte)(data & 0xF);
			}
			else if (data >= 0x20 & data <= 0x2D)
			{
				Type = TerrainType.PartialSubmerged;
				slopeData = (byte)(data & 0xF);
			}
			else if (data >= 0x30 & data <= 0x3D)
			{
				Type = TerrainType.SurfaceWater;
				slopeData = (byte)(data & 0xF);
			}
			else if (data == 0x3E)
				Type = TerrainType.Waterfall;
			else if (data == 0x40)
				Type = TerrainType.CanalNS;
			else if (data == 0x41)
				Type = TerrainType.CanalEW;
			else if (data == 0x42)
				Type = TerrainType.BayE;
			else if (data == 0x43)
				Type = TerrainType.BayS;
			else if (data == 0x44)
				Type = TerrainType.BayW;
			else if (data == 0x45)
				Type = TerrainType.BayN;
			else if (data >= 0x46 & data <= 0x54)
				Type = TerrainType.Hole;

			switch (slopeData)
			{
				case 0:
					Slope = TerrainSlope.Flat;
					break;
				case 1:
					Slope = TerrainSlope.WestEdge;
					break;
				case 2:
					Slope = TerrainSlope.NorthEdge;
					break;
				case 3:
					Slope = TerrainSlope.EastEdge;
					break;
				case 4:
					Slope = TerrainSlope.SouthEdge;
					break;
				case 5:
					Slope = TerrainSlope.NWHalf;
					break;
				case 6:
					Slope = TerrainSlope.NEHalf;
					break;
				case 7:
					Slope = TerrainSlope.SEHalf;
					break;
				case 8:
					Slope = TerrainSlope.SWHalf;
					break;
				case 9:
					Slope = TerrainSlope.NWCorner;
					break;
				case 10:
					Slope = TerrainSlope.NECorner;
					break;
				case 11:
					Slope = TerrainSlope.SECorner;
					break;
				case 12:
					Slope = TerrainSlope.SWCorner;
					break;
				case 13:
					Slope = TerrainSlope.Block;
					break;
			}
		}

		public override string ToString()
		{
			return string.Format("<{0:X}>\tType: {1}\tSlope: {2}\n", Data, Type, Slope);
		}
	}

	/// <summary>
	/// Description of a tile slope. Directions indicate which area of the tile is raised, with north being to the
	/// upper-right.
	/// </summary>
	public enum TerrainSlope : byte
	{
		Flat,
		WestEdge,
		NorthEdge,
		EastEdge,
		SouthEdge,
		NWHalf,
		NEHalf,
		SEHalf,
		SWHalf,
		NWCorner,
		NECorner,
		SECorner,
		SWCorner,
		Block
	}

	public enum TerrainType
	{
		Dry,
		Submerged,
		PartialSubmerged,
		SurfaceWater,
		Waterfall,
		CanalNS,
		CanalEW,
		BayE,
		BayS,
		BayW,
		BayN,
		Hole
	}
}
