using System;
using System.Collections.Generic;
using System.IO;

namespace CityParser2000
{
    /// <summary>
    /// The <c>City</c> type is a representation of a simulated city.
    /// </summary>
    public class City
    {
        #region properties and fields

        /// <summary>
        /// The city's name.
        /// </summary>
		public string CityName 
		{
			// TODO: Test both getting and setting the city variable.
			get { return ((Segments.CNAM)GetSegment("CNAM")).cityName; }
			set { ((Segments.CNAM)GetSegment("CNAM")).cityName = value; } 
		}

        /// <summary>
        /// The mayor of the city.
        /// </summary>
        public string MayorName { get; set; }

		/// <summary>
		/// The length of the save file as determined from the 4 byte value in the header.
		/// </summary>
		public int DataLength { get { return dataLength; } }

        #endregion

        #region public constants
		// A few constant header bytes.
		static const string HEADERCHUNK = "FORM";
		static const string FILETYPE = "SCDH";

        /// <summary>
        /// Enumerates underground structures.
        /// </summary>
        public enum UndergroundItem { SubwayAndPipe, Tunnel, SubwayStation, Subway, Pipe };

        /// <summary>
        /// Enumerates city zones.
        /// </summary>
        public enum Zone { LightResidential, DenseResidential, LightCommercial, DenseCommercial, LightIndustrial, DenseIndustrial, MilitaryBase, Airport, Seaport };

        /// <summary>
        /// <para>Indicates the number of city tiles along each edge of the city.</para>
        /// <para>Cities are always square.</para>
        /// </summary>
        public const int TilesPerSide = 128;

        #endregion

        #region local variables
		List<Segments.DataSegment> segments = new List<Segments.DataSegment>();
		int dataLength;

        private Tile[,] tiles = new Tile[TilesPerSide, TilesPerSide];

        private Dictionary<string, int> miscStats = new Dictionary<string, int>();

        private List<int> policeMap;
        private List<int> firefigherMap;
        private List<int> crimeMap;
        private List<int> pollutionMap;
        private List<int> populationMap;
        private List<int> populationGrowthMap;
        private List<int> trafficMap;
        private List<int> propertyValueMap;

        private List<string> signs = new List<string>();

        #endregion

        #region constructors and initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public City()
        {
            initializeTiles();
        }

        private void initializeTiles()
        {
            for (int i = 0; i < TilesPerSide; i++)
            {
                for (int j = 0; j < TilesPerSide; j++)
                {
                    tiles[i, j] = new Tile();
                }
            }
        }

        #endregion

		#region Parsing
		/// <summary>
		/// Parses binary data from <paramref name="binaryFilename"/> and stores it in a <see cref="City"/> object.
		/// </summary>
		/// <param name="binaryFilename">Filepath to a .SC2 file.</param>
		/// <returns>A <see cref="City"/> instance reflecting data from <paramref name="binaryFilename"/></returns>
		public static City ParseCityFile(string binaryFilename)
        {
            var city = new City();

            using (FileStream reader = File.Open(binaryFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
				city.ParseHeader(reader); // Used for validation and sanity-checking before anything else is done.

				// Begin walking through the file, handing off segment parsing to the appropriate parser.
                string segmentName;
                Int32 segmentLength;
                while (reader.Position < reader.Length)
                {
					// TODO: Complete migration to new parser.
					city.segments.Add(Segments.SegmentFactory.ParseSegment(reader));
                    // Parse segment data and store it in a City object. 
                    segmentName = reader.ReadString();
                    segmentLength = reader.Read4ByteInt();

                    /*if ("CNAM".Equals(segmentName))
                    {
                        // City name (uncompressed).
                        city = parseCityName(city, reader, segmentLength);
                    }
                    else if ("ALTM".Equals(segmentName))
                    {
                        // Altitude map. (Not compressed)
                        city = parseAltitudeMap(city, reader, segmentLength);
                    }
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
		private void ParseHeader(FileStream reader)
		{
			// Case for too small of a file
			if (reader.Length < 12)
				throw new Exception("Unexpected file length.");

			// Read 12-byte header.
			string headChunk = reader.ReadString();
			dataLength = reader.Read4ByteInt();
			string fileType = reader.ReadString();

			// Make sure the header represents a valid city file.
			if (!headChunk.Equals(HEADERCHUNK) || !fileType.Equals(FILETYPE))
				throw new Exception("Invalid SC2000 file or corrupted header.");
		}
		#endregion

		#region Getters and Setters
		/// <summary>
		/// Find a DataSegment object from within the segments List.
		/// </summary>
		/// <param name="type">The four letter segment type to be found.</param>
		/// <returns></returns>
		public Segments.DataSegment GetSegment(string type)
		{
			foreach (Segments.DataSegment dseg in segments)
			{
				if (type.Equals(dseg.Type))
					return dseg;
			}

			throw new Exception("Segment " + type + " was not found.");
		}

		/// <summary>
        /// Set a series of boolean flags for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</x></param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="isSalty">True if this tile would be salt water.</param>
        /// <param name="isWaterCovered">True if this tile is covered in water.</param>
        /// <param name="isWaterSupplied">True if this tile is connected to the city's water system</param>
        /// <param name="isPiped">True if this tile can convey water.</param>
        /// <param name="isPowered">True if this tile has access to the electric grid.</param>
        /// <param name="isConductive">True if this tile can conduct electricity.</param>
        public void SetTileFlags(int x, int y, bool isSalty, bool isWaterCovered, bool isWaterSupplied, bool isPiped, bool isPowered, bool isConductive) 
        {
            tiles[x, y].IsSalty = isSalty;
            tiles[x, y].IsWaterCovered = isWaterCovered;
            tiles[x, y].IsWaterSupplied = isWaterSupplied;
            tiles[x, y].IsPiped = isPiped;
            tiles[x, y].IsPowered = isPowered;
            tiles[x, y].IsConductive = isConductive;
        }

        /// <summary>
        /// Set what is underground of city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="undergroundItem">Underground structure code (pipe, subway, etc).</param>
        public void SetUndergroundItem(int x, int y, UndergroundItem undergroundItem)
        {
            switch (undergroundItem) 
            {
                case UndergroundItem.Pipe:
                    tiles[x, y].HasPipe = true;
                    return;
                case UndergroundItem.Subway:
                    tiles[x, y].HasSubway = true;
                    return;
                case UndergroundItem.SubwayAndPipe:
                    tiles[x, y].HasPipe = true;
                    tiles[x, y].HasSubway = true;
                    return;
                case UndergroundItem.SubwayStation:
                    tiles[x, y].HasSubwayStation = true;
                    return;
                case UndergroundItem.Tunnel:
                    tiles[x, y].HasTunnel = true;
                    return;
            }
        }

        /// <summary>
        /// Set the zone for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="zone">Zone code (residential, commercial, etc).</param>
        public void SetZone(int x, int y, Zone zone)
        {
            switch (zone)
            {
                case Zone.LightResidential:
                    tiles[x, y].IsLightResidential = true;
                    return;
                case Zone.DenseResidential:
                    tiles[x, y].IsDenseResidential = true;
                    return;
                case Zone.LightCommercial:
                    tiles[x, y].IsLightCommercial = true;
                    return;
                case Zone.DenseCommercial:
                    tiles[x, y].IsDenseCommerical = true;
                    return;
                case Zone.LightIndustrial:
                    tiles[x, y].IsLightIndustrial = true;
                    return;
                case Zone.DenseIndustrial:
                    tiles[x, y].IsDenseIndustrial = true;
                    return;
                case Zone.Airport:
                    tiles[x, y].IsAirport = true;
                    return;
                case Zone.Seaport:
                    tiles[x, y].IsSeaport = true;
                    return;
                case Zone.MilitaryBase:
                    tiles[x, y].IsMilitaryBase = true;
                    return;
            }
        }

        /// <summary>
        /// Set the above-ground structure (<see cref="Building"/>) for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="buildingCode"></param>
        public void SetBuilding(int x, int y, Building.BuildingCode buildingCode)
        {
            tiles[x, y].SetBuilding(buildingCode);
        }


        /// <summary>
        /// Mark a building corner for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="cornerCode">Indicates which corner of the tile is the building corner.</param>
        public void SetBuildingCorner(int x, int y, Building.CornerCode cornerCode)
        {
            switch (cornerCode)
            {
                case Building.CornerCode.BottomRight:
                    tiles[x, y].HasBuildingCornerBottomRight = true;
                    return;
                case Building.CornerCode.BottomLeft:
                    tiles[x, y].HasBuildingCornerBottomLeft = true;
                    return;
                case Building.CornerCode.TopLeft:
                    tiles[x, y].HasBuildingCornerTopLeft = true;
                    return;
                case Building.CornerCode.TopRight:
                    tiles[x, y].HasBuildingCornerTopRight = true;
                    return;
            }
        }

        /// <summary>
        /// Set altitude for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="altitude">Altitude in meters.</param>
        public void SetAltitude(int x, int y, int altitude)
        {
            tiles[x, y].Altitude = altitude;
        }

        /// <summary>
        /// Set police strength map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetPoliceMap(List<int> mapData)
        {
            policeMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set crime leves map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetCrimeMap(List<int> mapData)
        {
            crimeMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set firefighter power map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetFirefighterMap(List<int> mapData)
        {
            firefigherMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set pollution level map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetPollutionMap(List<int> mapData)
        {
            pollutionMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set population map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetPopulationMap(List<int> mapData)
        {
            populationMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set population growth rate map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetPopulationGrowthMap(List<int> mapData)
        {
            populationGrowthMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set traffic congestion map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetTrafficMap(List<int> mapData)
        {
            trafficMap = new List<int>(mapData);
        }

        /// <summary>
        /// Set property value map for the city.
        /// </summary>
        /// <param name="mapData"></param>
        public void SetPropertyValueMap(List<int> mapData)
        {
            propertyValueMap = new List<int>(mapData);
        }

        /// <summary>
        /// Record user-generated sign text.
        /// </summary>
        /// <param name="signText"></param>
        public void AddSignText(string signText)
        {
            signs.Add(signText);
        }

        #endregion // setters

    }
}
