using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CityParser2000;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
namespace CityParser2000
{
	[TestClass()]
	public class CityMapTests
	{
		City testCity;

		[TestInitialize]
		public void CMTInit()
		{
			testCity = CityParser.ParseCityFile("blank.sc2");
			Trace.WriteLine("City " + testCity.CityName + " loaded.");
		}

		/// <summary>
		/// Check the first four tiles of the Map object for properly parsed ALTM data (altitude and water coverage).
		/// </summary>
		[TestMethod()]
		public void CheckCityMapData()
		{
			Tile tempTile;
			for (int i = 0; i < 4; i++ )
			{
				tempTile = testCity.Map.Tiles[i, 0];
				Trace.WriteLine("[" + i + ", 0] " + tempTile);
			}
		}
	}
}
