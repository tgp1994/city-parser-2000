using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityParser2000;
using CityParser2000.Segments;
using System.Diagnostics;

namespace CP2000UnitTester
{
	[TestClass]
	public class UnitTest1
	{
		City city;

		/// <summary>
		/// Prepare the City object as part of a unit test and for future tests.
		/// </summary>
		[TestInitialize]
		public void Initialize()
		{
			city = CityParser.ParseCityFile("blank.sc2");
			Trace.WriteLine("Data length came out to be " + city.DataLength + " bytes.");
		}
		
		[TestMethod]
		public void CheckCityName()
		{
			string cityName = city.CityName; // First try to get the city name
			Trace.WriteLine("City name from the City object is: " + cityName);
			city.CityName = "UnitTest"; // Now attempt to write to the city name
			// Verify
			CNAM cnamSeg = (CityParser2000.Segments.CNAM)city.GetSegment("CNAM");
			Assert.AreEqual("UnitTest", cnamSeg.cityName);
			// Put the old city name back
			city.CityName = cityName;
		}

		[TestMethod]
		public void CheckMISC()
		{
			Trace.WriteLine("\n*MISC Segment*");
			MISC miscSeg = (MISC)city.GetSegment("MISC");
			Trace.WriteLine(miscSeg.ToString(true));
		}

		[TestMethod]
		public void CheckALTM()
		{
			Trace.WriteLine("\n*ALTM Segment*");
			ALTM altmSeg = (ALTM)city.GetSegment("ALTM");
			Trace.WriteLine(altmSeg.ToString(0, 4, 0, 10));
		}

		[TestMethod]
		public void TestAltitudeDescriptor()
		{
			int[] testTiles = new int[] { 0x0, 0x4, 0x84, 0xB400 };

			for (int i = 0; i < testTiles.Length; i++)
			{
				AltitudeDescriptor ad = new AltitudeDescriptor(testTiles[i]);
				Trace.Write("Tile " + i + ": ");
				Trace.WriteLine(ad);
			}
		}

		[TestMethod]
		public void TestTerrainDescriptor()
		{
			// Expected results are: Dry flat, dry North edge, dry NWcorner, dry block, dry undefined, partially-
			// submerged and south edge slope, waterfall, hole and east edge slope, undefined undefined
			byte[] descriptorData = new byte[] { 0x0, 0x02, 0x09, 0x0E, 0x0F, 0x24, 0x3E, 0x49, 0xDE };

			for (int i = 0; i < descriptorData.Length; i++)
			{
				TerrainDescriptor td = new TerrainDescriptor(descriptorData[i]);
				Trace.Write("TD " + i + ": ");
				Trace.WriteLine(td);
			}
		}

		[TestMethod]
		public void TestXTER()
		{

		}
	}
}
