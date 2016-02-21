using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CityParser2000;
using System.Diagnostics;

namespace CP2000UnitTester
{
	[TestClass]
	public class Utility
	{
		/// <summary>
		/// Testing the StreamUtils Read4ByteInt method with a standard stream of four bytes to make sure it works.
		/// </summary>
		[TestMethod]
		public void StandardParseStreamToInt()
		{
			//Lets make sure four bytes, 02 D7 9D E2 work out to ‭47685090‬.
			byte[] theByte = { 0x02, 0xD7, 0x9D, 0xE2 };
			//Note that that's Big Endian, however Intel uses *little* endian. Be sure it's converted.
			int expectedInt = 47685090;
			Trace.Write("Sanity check: the byte array = int(32): " + BitConverter.ToInt32(theByte, 0));

			System.IO.Stream stream = new System.IO.MemoryStream(theByte);
			int returnedInt = stream.Read4ByteInt();

			Assert.AreEqual(expectedInt, returnedInt);
		}
	}
}
