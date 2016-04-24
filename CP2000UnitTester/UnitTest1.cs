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
			city = City.ParseCityFile("E:\\Users\\Glen\\Desktop\\share\\blank.sc2");
			Trace.WriteLine("Data length came out to be " + city.DataLength + " bytes.");
		}
		
		[TestMethod]
		public void CheckCityName()
		{
			Trace.WriteLine("City name from the City object is: " + city.CityName);
		}
	}
}
