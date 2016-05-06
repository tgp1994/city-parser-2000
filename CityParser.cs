using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CityParser2000
{
	/// <summary>
	/// The <c>CityParser</c> type converts binary Sim City 2000 files to <see cref="City"/> <c>Objects</c>.
	/// </summary>
    public class CityParser
    {
        #region local constants

		// A few constant header bytes.
		const string HEADERCHUNK = "FORM";
		const string FILETYPE = "SCDH";
        
        // Binary segments describing city maps which are solely integer values.
        private static readonly HashSet<string> integerMaps = new HashSet<string> { "XPLC", "XFIR", "XPOP", "XROG", "XTRF", "XPLT", "XVAL", "XCRM" };

        // Binary segments describing city maps in which the byte data is uniqure to each segment.
        private static readonly HashSet<string> complexMaps = new HashSet<string> { "XTER", "XBLD", "XZON", "XUND", "XTXT", "XBIT", "ALTM" };

        // Binary codes that indicate what is underground in a tile. Multiples distinguish slope and direction.
        // Used when decoding XUND segment.
        private enum undergroundCode { 
            nothing = 0x00,
            subway1 = 0x01,
            subway2 = 0x02,
            subway3 = 0x03,
            subway4 = 0x04,
            subway5 = 0x05,
            subway6 = 0x06,
            subway7 = 0x07,
            subway8 = 0x08,
            subway9 = 0x09,
            subwayA = 0x0A,
            subwayB = 0x0B,
            subwayC = 0x0C,
            subwayD = 0x0D,
            subwayE = 0x0E,
            subwayF = 0x0F,
            pipe1 = 0x10,
            pipe2 = 0x11,
            pipe3 = 0x12,
            pipe4 = 0x13,
            pipe5 = 0x14,
            pipe6 = 0x15,
            pipe7 = 0x16,
            pipe8 = 0x17,
            pipe9 = 0x18,
            pipeA = 0x19,
            pipeB = 0x1A,
            pipeC = 0x1B,
            pipeD = 0x1C,
            pipeE = 0x1D,
            pipeF = 0x1E,
            pipeAndSubway1 = 0x1F,
            pipeAndSubway2 = 0x20,
            tunnel1 = 0x21,
            tunnel2 = 0x22,
            subwayStationOrSubRail = 0x23
        };

        // Zones. Order is important as this is used in decoding binary data.
        private enum zoneCode { none, lightResidential, denseResidential, lightCommercial, denseCommercial, lightIndustrial, denseIndustrial, military, airport, seaport };

        #endregion

        #region constructors

        public CityParser () 
        {
            //tileIterator = new Utility.CityTileIterator(City.TilesPerSide);
        }

        #endregion

        #region local variables

        private Utility.CityTileIterator tileIterator;

        #endregion

        #region parsing and storage

		/// <summary>
		/// Parses binary data from <paramref name="binaryFilename"/> and stores it in a <see cref="City"/> object.
		/// </summary>
		/// <param name="binaryFilename">Filepath to a .SC2 file.</param>
		/// <returns>A <see cref="City"/> instance reflecting data from <paramref name="binaryFilename"/></returns>
		public static City ParseCityFile(string binaryFilename)
		{
			City city;

			using (FileStream reader = File.Open(binaryFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				city = new City(ParseHeader(reader));
				Segments.DataSegment segment;

				// Begin walking through the file, handing off segment parsing to the appropriate parser.
				while (reader.Position < reader.Length)
				{
					segment = Segments.SegmentFactory.ParseSegment(reader);
					segment.PopulateCity(ref city);
					city.Segments.Add(segment);
					/*
					else if ("XTER".Equals(segmentName))
					{
						// Terrain slope map. 
						// Ignore for now. 
						reader.ReadBytes(segmentLength);   
					}
					else if ("XBLD".Equals(segmentName))
					{
						// Buildings map.
						city = parseBuildingMap(city, getDecompressedReader(reader, segmentLength));
					}
					else if ("XZON".Equals(segmentName))
					{
						// Zoning map (also specifies building corners).
						city = parseZoningMap(city, getDecompressedReader(reader, segmentLength));
					}
					else if ("XUND".Equals(segmentName))
					{
						// Underground structures map.
						city = parseUndergroundMap(city, getDecompressedReader(reader, segmentLength));  
					}
					else if ("XTXT".Equals(segmentName))
					{
						// Sign information, of some sort. 
						// Ignore for now. 
						reader.ReadBytes(segmentLength);
					}
					else if ("XLAB".Equals(segmentName)) 
					{
						// 256 Labels. Mayor's name, then sign text.
						city = parse256Labels(city, getDecompressedReader(reader, segmentLength));
					}
					else if ("XMIC".Equals(segmentName))
					{
						// Microcontroller info.
						// Ignore for now. 
						reader.ReadBytes(segmentLength);
					}
					else if ("XTHG".Equals(segmentName))
					{
						// Segment contents unknown.
						// Ignore for now. 
						reader.ReadBytes(segmentLength);
					}
					else if ("XBIT".Equals(segmentName))
					{
						// One byte of flags for each city tile.
						city = parseBinaryFlagMap(city, getDecompressedReader(reader, segmentLength));
					}
					else if (integerMaps.Contains(segmentName)) 
					{
						// Data in these segments are represented by integer values ONLY.
						city = parseIntegerMap(city, segmentName, getDecompressedReader(reader, segmentLength));
					}
					else
					{
						throw new Exception("Reached end of parse loop, unknown data block case should have been handled.")
					}*/
				}
			}
			return city;
		}

		/// <summary>
		/// Read in and validate the header of a city file.
		/// </summary>
		/// <param name="stream">File stream waiting at the header for instructions.</param>
		/// <returns>The length (in bytes) of the data portion of this file.</returns>
		private static int ParseHeader(FileStream reader)
		{
			// Case for too small of a file
			if (reader.Length < 12)
				throw new Exception("Unexpected file length.");

			// Read 12-byte header.
			string headChunk = reader.ReadString();
			int dataLength = reader.Read4ByteInt();
			string fileType = reader.ReadString();

			// Make sure the header represents a valid city file.
			if (!headChunk.Equals(HEADERCHUNK) || !fileType.Equals(FILETYPE))
				throw new Exception("Invalid SC2000 file or corrupted header.");

			return dataLength;
		}

        private City parseIntegerMap(City city, string segmentName, BinaryReader segmentReader)
        {
            List<int> mapData = new List<int>();

            while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
            {
                mapData.Add((int)segmentReader.ReadByte());
            }

            city = storeIntegerMapData(city, mapData, segmentName);
            return city;
        }

        private City storeIntegerMapData(City city, List<int> mapData, string segmentName)
        {
            if ("XTRF".Equals(segmentName))
            {
                city.SetTrafficMap(mapData);
            }
            else if ("XPLT".Equals(segmentName))
            {
                city.SetPollutionMap(mapData);
            }
            else if ("XVAL".Equals(segmentName))
            {
                city.SetPropertyValueMap(mapData);
            }
            else if ("XCRM".Equals(segmentName))
            {
                city.SetCrimeMap(mapData);
            }
            else if ("XPLC".Equals(segmentName))
            {
                city.SetPoliceMap(mapData);
            }
            else if ("XFIR".Equals(segmentName))
            {
                city.SetFirefighterMap(mapData);
            }
            else if ("XPOP".Equals(segmentName))
            {
                city.SetPopulationMap(mapData);
            }
            else if ("XROG".Equals(segmentName))
            {
                city.SetPopulationGrowthMap(mapData);
            }        

            return city;
        }

        /*private City parse256Labels(City city, BinaryReader segmentReader)
        {
            // This segment describes 256 strings. String 0 is the mayor's name, the remaining are text from user-generated signs in the city.
 
            int labelLength;
            string label;
            const int maxLabelLength = 24;

            // Parse mayor's name.
            labelLength = segmentReader.ReadByte();
            label = readString(segmentReader, labelLength);
            if (maxLabelLength - labelLength > 0)
            {
                segmentReader.ReadBytes(maxLabelLength - labelLength);
            }
            city.MayorName = label;

            while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
            {
                // Parse sign-text strings.
 
                // Each string is 24 bytes long, and is preceded by a 1-byte count. 
                labelLength = segmentReader.ReadByte();
                label = readString(segmentReader, labelLength);
                city.AddSignText(label);

                // Advance past any padding to next label.
                if (maxLabelLength - labelLength > 0)
                {
                    segmentReader.ReadBytes(maxLabelLength - labelLength);
                }
            }

            segmentReader.Dispose();
            return city;
        }*/

        #endregion

		#region complex city map parsers

		private City parseBinaryFlagMap(City city, BinaryReader segmentReader)
		{
			// Parse XBIT segment. 
			// XBIT contains one byte of binary flags for each city tile.
			//
			// The flags for each bit are:
			// 0: Salt water. (If true and this tile has water it will be salt water)
			// 1: (unknown)
			// 2: Water covered.
			// 3: (unknown)
			// 4: Supplied with water from city water-system.
			// 5: Conveys water-system water. (Building and pipes convey water)
			// 6: Has electricty.
			// 7: Conducts electricity.

			bool saltyFlag;
			bool waterCoveredFlag;
			bool waterSuppliedFlag;
			bool pipedFlag;
			bool poweredFlag;
			bool conductiveFlag;

			// These will be used to set the bool flags.
			const byte saltyMask = 1;
			// Unknown flag in 1 << 1 position.
			const byte waterCoveredMask = 1 << 2;
			// Unknown flag in 1 << 3 position.
			const byte waterSuppliedMask = 1 << 4;
			const byte pipedMask = 1 << 5;
			const byte poweredMask = 1 << 6;
			const byte conductiveMask = 1 << 7;
			byte tileByte;

			tileIterator.Reset();
			while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
			{
				// TODO: Possible bug. Test data "new city.sc2" does not seem to be decompressing this segment correctly.
				tileByte = segmentReader.ReadByte();

				saltyFlag = (tileByte & saltyMask) != 0;
				waterCoveredFlag = (tileByte & waterCoveredMask) != 0;
				waterSuppliedFlag = (tileByte & waterSuppliedMask) != 0;
				pipedFlag = (tileByte & pipedMask) != 0;
				poweredFlag = (tileByte & poweredMask) != 0;
				conductiveFlag = (tileByte & conductiveMask) != 0;

				city.SetTileFlags(tileIterator.X, tileIterator.Y, saltyFlag, waterCoveredFlag, waterSuppliedFlag, pipedFlag, poweredFlag, conductiveFlag);

				tileIterator.IncrementCurrentTile();
			}

			segmentReader.Dispose();
			return city;
		}

		private City parseUndergroundMap(City city, BinaryReader segmentReader)
		{
			// Parse XUND segment.
			// This segment indicates what exists underground in each tile, given by a one-byte integer code.

			undergroundCode tileCode;
			tileIterator.Reset();

			while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
			{
				tileCode = (undergroundCode)segmentReader.ReadByte();

				switch (tileCode)
				{
					case undergroundCode.nothing:
						// This tile doesn't have anything under the ground.
						break;
					case undergroundCode.pipeAndSubway1:
					case undergroundCode.pipeAndSubway2:
						city.SetUndergroundItem(tileIterator.X, tileIterator.Y, City.UndergroundItem.SubwayAndPipe);
						break;
					case undergroundCode.subwayStationOrSubRail:
						city.SetUndergroundItem(tileIterator.X, tileIterator.Y, City.UndergroundItem.SubwayStation);
						break;
					case undergroundCode.tunnel1:
					case undergroundCode.tunnel2:
						// NOTE: These codes appear to have not been used... nor does there appear to be any underground code at all for tunnels. 
						//  Perhaps these codes were meant to be tunnels but were never implemented as such, or possibly these codes indicate some other non-tunnel underground object.
						// TODO: Log if we ever get here? 
						city.SetUndergroundItem(tileIterator.X, tileIterator.Y, City.UndergroundItem.Tunnel);
						break;
					case undergroundCode.subway1:
					case undergroundCode.subway2:
					case undergroundCode.subway3:
					case undergroundCode.subway4:
					case undergroundCode.subway5:
					case undergroundCode.subway6:
					case undergroundCode.subway7:
					case undergroundCode.subway8:
					case undergroundCode.subway9:
					case undergroundCode.subwayA:
					case undergroundCode.subwayB:
					case undergroundCode.subwayC:
					case undergroundCode.subwayD:
					case undergroundCode.subwayE:
					case undergroundCode.subwayF:
						city.SetUndergroundItem(tileIterator.X, tileIterator.Y, City.UndergroundItem.Subway);
						break;
					case undergroundCode.pipe1:
					case undergroundCode.pipe2:
					case undergroundCode.pipe3:
					case undergroundCode.pipe4:
					case undergroundCode.pipe5:
					case undergroundCode.pipe6:
					case undergroundCode.pipe7:
					case undergroundCode.pipe8:
					case undergroundCode.pipe9:
					case undergroundCode.pipeA:
					case undergroundCode.pipeB:
					case undergroundCode.pipeC:
					case undergroundCode.pipeD:
					case undergroundCode.pipeE:
					case undergroundCode.pipeF:
						city.SetUndergroundItem(tileIterator.X, tileIterator.Y, City.UndergroundItem.Pipe);
						break;
					default:
						// Note: Hex codes over 0x23 are likely unused, but if they are used we would end up here.
						break;
				}

				tileIterator.IncrementCurrentTile();
			}

			segmentReader.Dispose();
			return city;
		}

		private City parseZoningMap(City city, BinaryReader segmentReader)
		{
			// Parse zoning and "building corner" information.

			// b00001111. The zone information is encoded in bits 0-3
			byte zoneMask = 15;
			// b0001000. Set if building has a corner in the 'top right'.
			byte cornerMask1 = 16;
			// b00100000. Set if building has a corner in the 'bottom right'.
			byte cornerMask2 = 32;
			// b01000000. Set if building has a corner in the 'bottom left'.
			byte cornerMask3 = 64;
			// b10000000. Set if building has a corner in the 'top left'.
			byte cornerMask4 = 128;
			zoneCode tileZoneCode;

			tileIterator.Reset();
			byte rawByte;

			while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
			{
				rawByte = segmentReader.ReadByte();

				// A little bit-wise arithmetic to extract our 4-bit zone code.
				tileZoneCode = (zoneCode)(rawByte & zoneMask);

				switch (tileZoneCode)
				{
					case zoneCode.lightResidential:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.LightResidential);
						break;
					case zoneCode.denseResidential:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.DenseResidential);
						break;
					case zoneCode.lightCommercial:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.LightCommercial);
						break;
					case zoneCode.denseCommercial:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.DenseCommercial);
						break;
					case zoneCode.lightIndustrial:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.LightIndustrial);
						break;
					case zoneCode.denseIndustrial:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.DenseIndustrial);
						break;
					case zoneCode.military:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.MilitaryBase);
						break;
					case zoneCode.airport:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.Airport);
						break;
					case zoneCode.seaport:
						city.SetZone(tileIterator.X, tileIterator.Y, City.Zone.Seaport);
						break;
				}

				if (hasCorner(rawByte, cornerMask1))
				{
					city.SetBuildingCorner(tileIterator.X, tileIterator.Y, Building.CornerCode.TopRight);
				}
				if (hasCorner(rawByte, cornerMask2))
				{
					city.SetBuildingCorner(tileIterator.X, tileIterator.Y, Building.CornerCode.BottomRight);
				}
				if (hasCorner(rawByte, cornerMask3))
				{
					city.SetBuildingCorner(tileIterator.X, tileIterator.Y, Building.CornerCode.BottomLeft);
				}
				if (hasCorner(rawByte, cornerMask4))
				{
					city.SetBuildingCorner(tileIterator.X, tileIterator.Y, Building.CornerCode.TopLeft);
				}

				tileIterator.IncrementCurrentTile();
			}

			segmentReader.Dispose();
			return city;
		}

		private bool hasCorner(byte b, byte cornerMask)
		{
			return (b & cornerMask) == (byte)1;
		}

		private City parseBuildingMap(City city, BinaryReader segmentReader)
		{
			// This segment indicates what is above ground in each square.

			// TODO: Shouldn't be relying on "Building.BuildingCode" order like this. BAD.
			tileIterator.Reset();
			byte rawByte;
			Building.BuildingCode buildingCode;

			while (segmentReader.BaseStream.Position < segmentReader.BaseStream.Length)
			{
				// This map contains on 'building code' for each square. 
				// The building code is a one-byte integer value.
				rawByte = segmentReader.ReadByte();
				buildingCode = (Building.BuildingCode)rawByte;
				city.SetBuilding(tileIterator.X, tileIterator.Y, buildingCode);

				tileIterator.IncrementCurrentTile();
			}

			segmentReader.Dispose();
			return city;
		}

		#endregion
    }
}

