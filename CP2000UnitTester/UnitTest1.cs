using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityParser2000;
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
			City city = City.ParseCityFile("E:\\Users\\Glen\\Desktop\\share\\blank.sc2");
			Trace.Write("Data length came out to be " + city.DataLength);
		}
		
		[TestMethod]
		public void CheckCityName()
		{
			Trace.Write("City name from the city object is: " + city.CityName);
		}
	}
}
