using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityParser2000;

namespace CP2000UnitTester
{
	[TestClass]
	public class UnitTest1
	{
		/// <summary>
		/// Consider this the "Main" unit test. It's the first one, and is meant to do initial functionality testing.
		/// </summary>
		[TestMethod]
		public void LetsDoThisThing()
		{
			CityParser parser = new CityParser();

			City ourCity = parser.ParseBinaryFile("E:\\Users\\Glen\\Desktop\\share\\blank.sc2");
		}
	}
}
