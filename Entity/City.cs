﻿using System;
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
			get { return ((Segments.CNAM)GetSegment("CNAM")).cityName; }
			set { ((Segments.CNAM)GetSegment("CNAM")).cityName = value; } 
		}

        /// <summary>
        /// The mayor of the city.
        /// </summary>
        public string MayorName { get; private set; }

		/// <summary>
		/// The length of the save file as determined from the 4 byte value in the header.
		/// </summary>
		public int DataLength { get; private set; }

		/// <summary>
		/// Maintain all segments specific to the data from which this city was generated.
		/// </summary>
		public List<Segments.DataSegment> Segments { get; private set; }

		public CityMap Map { get; private set; }

        #endregion

        #region public constants

        /// <summary>
        /// Enumerates underground structures.
        /// </summary>
        public enum UndergroundItem { SubwayAndPipe, Tunnel, SubwayStation, Subway, Pipe };

        /// <summary>
        /// Enumerates city zones.
        /// </summary>
        public enum Zone { LightResidential, DenseResidential, LightCommercial, DenseCommercial, LightIndustrial, DenseIndustrial, MilitaryBase, Airport, Seaport };

        #endregion

        #region local variables

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

        public City(int dataLength = -1)
        {
			Map = new CityMap();
			Segments = new List<Segments.DataSegment>();
			DataLength = dataLength;
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
			foreach (Segments.DataSegment dseg in Segments)
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
            /*tiles[x, y].IsSalty = isSalty;
            tiles[x, y].IsWaterCovered = isWaterCovered;
            tiles[x, y].IsWaterSupplied = isWaterSupplied;
            tiles[x, y].IsPiped = isPiped;
            tiles[x, y].IsPowered = isPowered;
            tiles[x, y].IsConductive = isConductive;*/
        }

        /// <summary>
        /// Set what is underground of city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="undergroundItem">Underground structure code (pipe, subway, etc).</param>
        public void SetUndergroundItem(int x, int y, UndergroundItem undergroundItem)
        {
            /*switch (undergroundItem) 
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
            }*/
        }

        /// <summary>
        /// Set the zone for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="zone">Zone code (residential, commercial, etc).</param>
        public void SetZone(int x, int y, Zone zone)
        {
            /*switch (zone)
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
            }*/
        }

        /// <summary>
        /// Set the above-ground structure (<see cref="Building"/>) for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="buildingCode"></param>
        public void SetBuilding(int x, int y, Building.BuildingCode buildingCode)
        {
            //tiles[x, y].SetBuilding(buildingCode);
        }


        /// <summary>
        /// Mark a building corner for the city tile at (<paramref name="x"/>, <paramref name="y"/>).
        /// </summary>
        /// <param name="x">Tile x-coordinate.</param>
        /// <param name="y">Tile y-coordinate.</param>
        /// <param name="cornerCode">Indicates which corner of the tile is the building corner.</param>
        public void SetBuildingCorner(int x, int y, Building.CornerCode cornerCode)
        {
            /*switch (cornerCode)
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
            }*/
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

        #endregion
    }
}
